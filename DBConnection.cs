using MySql.Data.MySqlClient;

namespace HabbiticcaLogic
{
    /// <summary>
    /// Подключение к базе данных
    /// </summary>
    public class DBConnection
    {
        public MySqlConnection Connection { get; }

        public DBConnection(string server, string port, string dataBaseName, string userId, string password)
        {
            string connectionText = $"Server={server};Port={port};Database={dataBaseName};User Id={userId};Password={password};Allow User Variables=True;";
            Connection = new MySqlConnection(connectionText);
        }
    }
}