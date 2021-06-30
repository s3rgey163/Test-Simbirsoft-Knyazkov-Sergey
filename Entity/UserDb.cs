using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabbiticcaLogic.Entity
{
    /// <summary>
    /// Данный класс хранит функции по обработке пользователей
    /// </summary>
    public class UserDb
    {
        private readonly DBConnection _dBConnection;
        public UserDb(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            Login = new UserLogin(_dBConnection);
            Password = new UserPassword(_dBConnection);
            Image = new UserImage(_dBConnection);
        }

        public UserLogin Login { get; set; }
        public UserPassword Password { get; set; }
        public UserImage Image { get; }

        /// <summary>
        /// Занести пользователя в базу данных
        /// </summary>
        /// <param name="login">Уникальный идентификатор пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="img">Ссылка на изображение пользователя</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Push(string login, string password, string img)
        {
            if(img.Equals("") || !img.StartsWith("http://") || !img.StartsWith("https://"))
            {
                img = "NULL";
            }
            MySqlConnection connect = _dBConnection.Connection;
            bool result = true;
            try
            {
                connect.Open();
                string query = $"INSERT INTO user (user_login, user_password, user_img) VALUES ('{login}','{password}', '{img}')";
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
        /// Удалить пользователя из базы данных
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
                string sqlCommandText = $"DELETE FROM user " +
                    $" WHERE user_login = '{userLogin}'";
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
        /// Получить массив атрибутов пользователя 
        /// </summary>
        /// <param name="userLogin">Уникальный идентификатор пользователя</param>
        /// <param name="userAttributes">Массив для хранения атрибутов пользователя</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Get(string userLogin,ref string[] userAttributes)
        {
            userAttributes = null;
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            MySqlDataReader reader = null;

            try
            {
                connect.Open();
                string sqlCommandText = $"SELECT * FROM user WHERE user_login = '{userLogin}'";
                MySqlCommand command = new(sqlCommandText, connect);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    userAttributes = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        userAttributes[i] = reader.GetValue(i).ToString();
                    }
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
        /// <summary>
        /// Проверка аутентификации. Содержится ли данный пароль у данного пользователя
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="userPassword"></param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool CheckAuth(string userLogin, string userPassword)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            MySqlDataReader reader = null;
            try
            {
                connect.Open();
                string sqlCommandText = $"SELECT user_login FROM user WHERE user_login = '{userLogin}'" +
                    $" AND user_password = {userPassword}";
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
                    reader.Close();
            }
            return res;
        }
        /// <summary>
        /// Очистить список пользователей из базы данных (удалит информацию о всех пользователей из базы данных)
        /// </summary>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Clear()
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"TRUNCATE TABLE user";
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

        public class UserLogin
        {
            private readonly DBConnection _dBConnection;
            public UserLogin(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }
            /// <summary>
            /// Обновить уникальный идентификатор пользователя
            /// </summary>
            /// <param name="currentLogin">Текущий уникальный идентификатор</param>
            /// <param name="newLogin">Новый уникальный идентификатор</param>
            /// <returns></returns>
            public bool Update(string currentLogin, string newLogin)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE user " +
                        $"SET user_login = '{newLogin}'" +
                        $" WHERE user_login = '{currentLogin}'";
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
        public class UserPassword
        {
            private readonly DBConnection _dBConnection;
            public UserPassword(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }
            /// <summary>
            /// Обновить пароль у данного пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="newPassword">Новый пароль пользователя</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin, string newPassword)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE user " +
                        $"SET user_password = '{newPassword}'" +
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
            /// Получить пароль данного пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="password">Строка для хранения полученного пользовательского пароля</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, ref string password)
            {
                password = "";
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;

                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT user_password FROM user WHERE user_login = '{userLogin}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        password = (string)reader[0];
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

        public class UserImage
        {
            private readonly DBConnection _dBConnection;
            public UserImage(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }
            /// <summary>
            /// Обновить ссылку на изображения пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="newImgUrl">Новая ссылка на изображение</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Update(string userLogin, string newImgUrl)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE user " +
                        $"SET user_img = '{newImgUrl}'" +
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
            /// Получить ссылку на изображение данного пользователя
            /// </summary>
            /// <param name="userLogin">Уникальный идентификатор пользователя</param>
            /// <param name="imageUrl">Строка для хранения полученной ссылки на изображение пользователя</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
            public bool Get(string userLogin, ref string imageUrl)
            {
                imageUrl = "";
                bool res = true;
                MySqlConnection connect = _dBConnection.Connection;
                MySqlDataReader reader = null;

                try
                {
                    connect.Open();
                    string sqlCommandText = $"SELECT user_img FROM user WHERE user_login = '{userLogin}'";
                    MySqlCommand command = new(sqlCommandText, connect);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        imageUrl = (string)reader[0];
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

