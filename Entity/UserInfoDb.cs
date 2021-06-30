using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabbiticcaLogic.Entity
{
    /// <summary>
    /// Данный класс содержит функции по обработке пользовательской информации
    /// </summary>
    public class UserInfoDb
    {
        protected readonly DBConnection _dBConnection;
        public UserInfoDb(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            Days = new DaysGone(_dBConnection);
            LoginDay = new LastLoginDay(_dBConnection);
            Points = new UserPoints(dBConnection);
        }

        public DaysGone Days { get; set; }
        public LastLoginDay LoginDay { get; set; }
        public UserPoints Points { get; set; }
        /// <summary>
        /// Занести информацию о пользователе в базу данных
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="daysGone">Число дней, которые пользователь заходил подряд</param>
        /// <param name="lastLoginDay">Дата последней аутентификации пользователя. Формат "dd.mm.YYYY  H:m:s"</param>
        /// <param name="points">Общее количество очков пользователя</param>
        /// <param name="dailyPoints">Количество очков, полученных пользователем за день</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Push(string userLogin, uint daysGone, DateTime lastLoginDay, int points, int dailyPoints)
        {
            bool res = true;
            MySqlDataReader reader = null;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"SET @id = (SELECT user_id FROM user WHERE user_login = '{userLogin}');" +
                    $"INSERT INTO achievement_info (user_id, days_gone, last_login_day,points,daily_points) " +
                    $"VALUES (@id,{daysGone}," +
                    $"str_to_date(\"{lastLoginDay}\", \"%d.%m.%Y %H:%i:%s\"), {points},{dailyPoints})";
                MySqlCommand command = new(sqlCommandText, connect);
                reader = command.ExecuteReader();

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
        /// <summary>
        /// Удалить информацию о пользователе
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Remove(string userLogin)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"SET @id = (SELECT user_id FROM user WHERE user_login = '{userLogin}');" +
                    $" DELETE FROM achievement_info " +
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
        /// Очистить информацию о всех пользователях
        /// </summary>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Clear()
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"TRUNCATE TABLE achievement_info";
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
        /// Получить все значения атрибутов данного пользователя
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="userInfoAttributes">Массив для хранения значений атрибутов пользвателя</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Get(string userLogin, ref string[] userInfoAttributes)
        {
            userInfoAttributes = null;
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            MySqlDataReader reader = null;
            try
            {
                connect.Open();
                string sqlCommandText = $"SELECT ai.* FROM achievement_info ai JOIN user u ON ai.user_id = u.user_id " +
                    $" WHERE user_login = '{userLogin}'";
                MySqlCommand command = new(sqlCommandText, connect);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    userInfoAttributes = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        userInfoAttributes[i] = reader.GetValue(i).ToString();
                    }
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


        public class DaysGone
        {
            private readonly DBConnection _dBConnection;
            public DaysGone(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }
            /// <summary>
            /// Обновить число проведенных дней подряд в приложении у пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="newDays">Новое значение проведенных дней</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin, int newDays)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE achievement_info ui JOIN user u" +
                        $" ON ui.user_id = u.user_id " +
                        $"SET ui.days_gone = {newDays}" +
                        $" WHERE user_login = '{userLogin}'";
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
            /// Получить проведенных дней подряд в приложении у пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="daysGone">Целочисленный 32-х разрядный тип данных для хранения числа проведенных дней подряд пользователя</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, ref int daysGone)
            {
                daysGone = 0;
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;

                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT days_gone FROM achievement_info ai JOIN user u " +
                        $"ON ai.user_id = u.user_id " +
                        $"WHERE user_login = '{userLogin}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        daysGone = int.Parse(reader[0].ToString());
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

        public class LastLoginDay
        {
            private readonly DBConnection _dBConnection;
            public LastLoginDay(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }
            /// <summary>
            /// Обновить дату последней аутентификации пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="newLastLogin">Дата последнего входа. Формат записи "dd.mm.yyyy hh:m:s"</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin, DateTime newLastLogin)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE achievement_info ui JOIN user u" +
                        $" ON ui.user_id = u.user_id " +
                        $"SET ui.last_login_day = str_to_date(\"{newLastLogin}\", \"%d.%m.%Y %H:%i:%s\")" +
                        $" WHERE user_login = '{userLogin}'";
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
            /// Получить дату последней аутентификации пользвателя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="lastLoginDay">Тип данных для хранения даты последней аутентификации пользователя</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, ref DateTime lastLoginDay)
            {
                lastLoginDay = DateTime.Now;
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;

                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT last_login_day FROM achievement_info ai JOIN user u " +
                        $"ON ai.user_id = u.user_id " +
                        $"WHERE user_login = '{userLogin}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        lastLoginDay = DateTime.Parse(reader[0].ToString()); ;
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

        public class UserPoints
        {
            private readonly DBConnection _dBConnection;
            public UserPoints(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }
            /// <summary>
            /// Обновить количество очков у данного пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="newLastLogin">Новое количество очков</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin, int newPoints)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE achievement_info ui JOIN user u" +
                        $" ON ui.user_id = u.user_id " +
                        $"SET ui.points = {newPoints}" +
                        $" WHERE user_login = '{userLogin}'";
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
            /// Получить список очков пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификтор пользователя</param>
            /// <param name="points">Целочисленный 32-х разрядный тип данных для хранения очков пользователя</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, ref int points)
            {
                points = 0;
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;
                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT points FROM achievement_info ai JOIN user u " +
                        $"ON ai.user_id = u.user_id " +
                        $"WHERE user_login = '{userLogin}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        points = int.Parse(reader[0].ToString());
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
        }
    }
}
