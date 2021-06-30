using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabbiticcaLogic.Entity
{
    /// <summary>
    /// Данный класс содержит функции по обработке заданий пользователя
    /// </summary>
    public class UserTaskDb
    {
        private readonly DBConnection _dBConnection;
        public TaskDesc Description { get; set; }
        public TaskPoints Points { get; set; }
        public CompleteCount TaskCompleteCount { get; set; }

        public UserTaskDb(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            Description = new TaskDesc(_dBConnection);
            Points = new TaskPoints(_dBConnection);
            TaskCompleteCount = new CompleteCount(_dBConnection);
        }

        /// <summary>
        /// Занести в базу данных новое задание пользователя
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="taskDesc">Описание задания</param>
        /// <param name="taskPoints">Очки за выполнение данного задания</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Push(string userLogin, string taskDesc, int taskPoints)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            if (!Exists(userLogin, taskDesc))
            {
                MySqlDataReader reader = null;
                try
                {
                    connect.Open();
                    string sqlCommandText = $"SET @id = (SELECT user_id FROM user WHERE user_login = '{userLogin}');" +
                        $"INSERT INTO user_task (user_id, task_desc, task_points,complete_count) " +
                        $"VALUES (@id,'{taskDesc}', {taskPoints}, 0)";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                }
                catch
                {
                    res = false;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    connect.Close();
                }
            }
            return res;
        }
        /// <summary>
        /// Удалить все записи с заданиями определенного пользователя
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool RemoveAllRecords(string userLogin)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"SET @id = (SELECT user_id FROM user WHERE user_login = '{userLogin}');" +
                    $" DELETE FROM user_task " +
                    $" WHERE user_id = @id";
                MySqlCommand command = new(sqlCommandText, connect);
                MySqlDataReader reader = command.ExecuteReader();
            }
            catch
            {
                res = false;
            }
            finally
            {
                connect.Close();
            }
            return res;
        }
        /// <summary>
        /// Удалить определенное задание у пользователя
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="taskDesc">Описание задания</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool RemoveRecord(string userLogin, string taskDesc)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"SET @id = (SELECT user_id FROM user WHERE user_login = '{userLogin}');" +
                    $" DELETE FROM user_task " +
                    $" WHERE user_id = @id AND task_desc = '{taskDesc}'";
                MySqlCommand command = new(sqlCommandText, connect);
                MySqlDataReader reader = command.ExecuteReader();
            }
            catch
            {
                res = false;
            }
            finally
            {
                connect.Close();
            }
            return res;
        }
        /// <summary>
        /// Узнать, существует ли данное задание у пользователя
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="taskDesc">Описание задания</param>
        /// <returns>true - задание существует. false - задание не существует или же SQL запрос вернул ошибку</returns>
        public bool Exists(string userLogin, string taskDesc)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            MySqlDataReader reader = null;
            try
            {
                connect.Open();
                string sqlCommandText = $"SELECT task_desc FROM user u JOIN user_task ui" +
                        $" ON u.user_id = ui.user_id WHERE task_desc = '{taskDesc}' AND user_login = '{userLogin}'";
                MySqlCommand command = new(sqlCommandText, connect);
                reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    res = false;
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                connect.Close();
            }
            return res;
        }
        /// <summary>
        /// Получить список заданий пользователя
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="userTasks">Массив для хранения пользовательских заданий</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Get(string userLogin, ref string[] userTasks)
        {
            userTasks = null;
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            MySqlDataReader reader = null;
            try
            {
                connect.Open();
                string getRowsSql = $"SELECT COUNT(*) FROM user_task " +
                    $"WHERE user_id IN (SELECT user_id FROM user " +
                    $"WHERE user_login = '{userLogin}')";
                string sqlCommandText = $"SELECT task_desc FROM user_task " +
                    $"WHERE user_id IN (SELECT user_id FROM user " +
                    $"WHERE user_login = '{userLogin}')";
                string getResultSqlCommand = getRowsSql + ";" + sqlCommandText;
                MySqlCommand command = new(getResultSqlCommand, connect);
                reader = command.ExecuteReader();
                int columns = 0;
                if (reader.Read())
                {
                    columns = int.Parse(reader[0].ToString());
                }
                userTasks = new string[columns];
                reader.NextResult();
                int i = 0;
                while (reader.Read())
                {
                    userTasks[i] = reader[0].ToString();
                    i++;
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                connect.Close();
                if (reader != null)
                    reader.Close();
            }
            return res;
        }

        public class TaskDesc
        {
            private readonly DBConnection _dBConnection;
            public TaskDesc(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;

            }
            /// <summary>
            /// Обновить описание задания
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="userTask">Описание задания, которое необходимо обновить</param>
            /// <param name="newTaskDesc">Новое описание задания</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin, string userTask, string newTaskDesc)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE user u JOIN user_task ui" +
                        $" ON u.user_id = ui.user_id" +
                        $" SET task_desc = '{newTaskDesc}'" +
                        $" WHERE user_login = '{userLogin}'" +
                        $" AND task_desc = '{userTask}'";
                    MySqlCommand command = new MySqlCommand(query, connect);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    connect.Close();
                }
                return result;
            }
        }

        public class TaskPoints
        {
            private readonly DBConnection _dBConnection;
            public TaskPoints(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;

            }
            /// <summary>
            /// Обновить количество очков у данного задания
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="userTask">Описание задания пользователя</param>
            /// <param name="newTaskPoints">Новое число очков</param>
            /// <returns></returns>
            public bool Update(string userLogin, int userTask,int newTaskPoints)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE user u JOIN user_task ui" +
                        $" ON u.user_id = ui.user_id" +
                        $" SET task_points = {newTaskPoints}" +
                        $" WHERE user_login = '{userLogin}'" +
                        $" AND task_desc = '{userTask}'";
                    MySqlCommand command = new MySqlCommand(query, connect);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    connect.Close();
                }
                return result;
            }
            /// <summary>
            /// Получить количество очков за данное пользовательское задание
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="userTask">Описание пользовательского задания</param>
            /// <param name="taskPoints">Целочисленный 32-х разрядный тип данных для хранения очков задания</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, string userTask,ref int taskPoints)
            {
                taskPoints = 0;
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;
                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT task_points FROM user_task ut JOIN user u " +
                        $"ON ut.user_id = u.user_id " +
                        $"WHERE user_login = '{userLogin}'" +
                        $" AND task_desc = '{userTask}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        taskPoints = int.Parse(reader[0].ToString());
                    }
                }
                catch
                {
                    res = false;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                return res;
            }
        }

        public class CompleteCount
        {
            private readonly DBConnection _dBConnection;
            public CompleteCount(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;

            }
            /// <summary>
            /// Обновить количество выполнений данного задания
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор</param>
            /// <param name="userTask">Описание пользовательского задания</param>
            /// <param name="newCompleteCount">Новое количество выполнений задания</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin,string userTask, int newCompleteCount)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE user u JOIN user_task ui" +
                        $" ON u.user_id = ui.user_id" +
                        $" SET complete_count = {newCompleteCount}" +
                        $" WHERE user_login = '{userLogin}'" +
                        $" AND task_desc = '{userTask}'";
                    MySqlCommand command = new MySqlCommand(query, connect);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    connect.Close();
                }
                return result;
            }
            /// <summary>
            /// Получить количество выполнений пользовательского задания
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="completeCount">Целочисленный 32-х разрядный тип данных для хранения количества выполнений</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, string userTask,ref int completeCount)
            {
                completeCount = 0;
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;
                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT complete_count FROM user_task ut JOIN user u " +
                        $"ON ut.user_id = u.user_id " +
                        $"WHERE user_login = '{userLogin}'" +
                        $" AND task_desc = '{userTask}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        completeCount = int.Parse(reader[0].ToString());
                    }
                }
                catch
                {
                    res = false;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                return res;
            }
        }

    }
}
