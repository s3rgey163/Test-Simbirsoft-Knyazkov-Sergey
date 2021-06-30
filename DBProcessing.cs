using HabbiticcaLogic.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabbiticcaLogic
{
    /// <summary>
    /// Методы по ведению данных в базе данных.
    /// </summary>
    public class DBProcessing
    {
        
        private readonly DBConnection _dBConnection;
        /// <summary>
        /// Методы по обработке сущности User
        /// </summary>
        public UserDb User { get; }
        /// <summary>
        /// Методы по обработке сущности UserInfo
        /// </summary>
        public UserInfoDb UserInfo { get; }
        /// <summary>
        /// Методы по обработке сущности Achiev
        /// </summary>
        public Achievement Achiev { get; }
        /// <summary>
        /// Методы по обработке сущности AchievDone
        /// </summary>
        public AchievemntDone AchievDone { get; }
        /// <summary>
        /// Методы по обработке сущности UserTask
        /// </summary>
        public UserTaskDb UserTask { get; }

        public DBProcessing(DBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            User = new UserDb(dBConnection);
            UserInfo = new UserInfoDb(dBConnection);
            Achiev = new(dBConnection);
            AchievDone = new AchievemntDone(dBConnection);
            UserTask = new UserTaskDb(dBConnection);
        }
    }

}
