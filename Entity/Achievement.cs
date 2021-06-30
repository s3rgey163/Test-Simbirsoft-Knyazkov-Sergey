using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabbiticcaLogic.Entity
{
    public class Achievement
    {
        private readonly DBConnection _dBConnection;
        public Achievement(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            Description = new AchievementDescription(_dBConnection);
            Image = new AchievementImage(_dBConnection);
            Name = new AchievementName(_dBConnection);
        }
        /// <summary>
        /// Методы по обработке атрибута Description сущности Achievement
        /// </summary>
        public AchievementDescription Description { get; set; }
        /// <summary>
        /// Методы по обработке атрибута Image сущности Achievement
        /// </summary>
        public AchievementImage Image { get; set; }
        /// <summary>
        /// Методы по обработке атрибута Name сущности Achievement
        /// </summary>
        public AchievementName Name { get; set; }
        /// <summary>
        /// Занести достижение в базу данных
        /// </summary>
        /// <param name="achievName">Имя достижения (Максимум 300 символов)</param>
        /// <param name="achievDesc">Описание достижения (Максимум 300 символов)</param>
        /// <param name="achievImageUrl">Ссылка на картинку достижения</param>
        /// <returns></returns>
        public bool Push(string achievName, string achievDesc, string achievImageUrl)
        {
            bool res = true;
            MySqlConnection connect = _dBConnection.Connection;
            try
            {
                connect.Open();
                string sqlCommandText = $"INSERT INTO achievement (achiev_name, achiev_desc, achiev_img) " +
                    $"VALUES ('{achievName}','{achievDesc}', '{achievImageUrl}')";
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


        public class AchievementName
        {
            private readonly DBConnection _dBConnection;
            public AchievementName(DBConnection dBConnection)
            {
                _dBConnection = dBConnection;
            }

            /// <summary>
            /// Обновить название достижения.
            /// </summary>
            /// <param name="currentAchievName">Текущее название достижения.</param>
            /// <param name="newAchievName">Новое название достижения.</param>
            /// <returns>true - запрос выполнен успешно. false - ошибка в запросе </returns>
            public bool Update(string currentAchievName, string newAchievName)
            {
                MySqlConnection connect = _dBConnection.Connection;
                bool result = true;
                try
                {
                    connect.Open();
                    string query = $"UPDATE achievement " +
                        $"SET achiev_name = '{newAchievName}'" +
                        $" WHERE achiev_name = '{currentAchievName}'";
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
    }





    public class AchievementDescription
    {
        private readonly DBConnection _dBConnection;
        public AchievementDescription(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
        }
        /// <summary>
        /// Обновить описание достижения
        /// </summary>
        /// <param name="achievName">Текущее описание достижения.</param>
        /// <param name="newAchievDesc">Новое описание достижения.</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Update(string achievName, string newAchievDesc)
        {
            MySqlConnection connect = _dBConnection.Connection;
            bool result = true;
            try
            {
                connect.Open();
                string query = $"UPDATE achievement " +
                    $"SET achiev_desc = '{newAchievDesc}'" +
                    $" WHERE achiev_name = '{achievName}'";
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
    public class AchievementImage
    {
        private readonly DBConnection _dBConnection;
        public AchievementImage(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
        }
        /// <summary>
        /// Обновить ссылку на изображения достижения
        /// </summary>
        /// <param name="achievName">Название достижения, у которого необходимо обновить изображение.</param>
        /// <param name="newAchievImageUrl">Новая ссылка на изображение достижения</param>
        /// <returns>true - запрос выполнен успешно. false - ошибка в запросе</returns>
        public bool Update(string achievName, string newAchievImageUrl)
        {
            MySqlConnection connect = _dBConnection.Connection;
            bool result = true;
            try
            {
                connect.Open();
                string query = $"UPDATE achievement " +
                    $"SET achiev_img = '{newAchievImageUrl}'" +
                    $" WHERE achiev_name = '{achievName}'";
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

}
