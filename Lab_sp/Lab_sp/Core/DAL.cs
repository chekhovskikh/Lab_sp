using Lab_sp.Core.DAO;
using Lab_sp.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Core
{
    public class DAL
    {
        private IEntityDAO<User> userDAO = null;
        private IEntityDAO<Sign> signDAO = null;
        private IEntityDAO<VideoSign> videoSignDAO = null;

        private SQLiteConnection connection = null;

        public DAL()
        {
            connection = new SQLiteConnection(
                string.Format("Data Source={0};", Properties.Resources.db_name));
            userDAO = new UserDAO(connection);
            signDAO = new SignDAO(connection);
            videoSignDAO = new VideoSignDAO(connection);
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        #region User
        public User GetUser(int id)
        {
            return userDAO.Get(id);
        }

        public List<User> AllUsers()
        {
            return userDAO.GetAll();
        }

        public void AddUser(User user)
        {
            userDAO.Add(user);
        }

        public void AddAllUsers(List<User> users)
        {
            userDAO.AddAll(users);
        }

        public void RemoveUser(User user)
        {
            userDAO.Remove(user);
        }

        public void RemoveUser(int id)
        {
            userDAO.Remove(id);
        }

        public void UpdateUser(User updatedUser)
        {
            userDAO.Update(updatedUser);
        }
        #endregion User

        #region Sign
        public Sign GetSign(int id)
        {
            return signDAO.Get(id);
        }

        public int GetIdForGost(string Gost)
        {
            return (signDAO as SignDAO).GetIdForGost(Gost);
        }

        public List<Sign> AllSigns()
        {
            return signDAO.GetAll();
        }

        public void AddSign(Sign sign)
        {
            signDAO.Add(sign);
        }

        public void AddAllSigns(List<Sign> signs)
        {
            signDAO.AddAll(signs);
        }

        public void RemoveSign(Sign sign)
        {
            signDAO.Remove(sign);
        }

        public void RemoveSign(int id)
        {
            signDAO.Remove(id);
        }

        public void UpdateSign(Sign updatedSign)
        {
            signDAO.Update(updatedSign);
        }
        #endregion Sign

        #region VideoSign
        public VideoSign GetVideoSign(int id)
        {
            return videoSignDAO.Get(id);
        }

        public List<VideoSign> AllVideoSigns()
        {
            return videoSignDAO.GetAll();
        }

        public void AddVideoSign(VideoSign videoSign)
        {
            videoSignDAO.Add(videoSign);
        }

        public void AddAllVideoSigns(List<VideoSign> videoSigns)
        {
            videoSignDAO.AddAll(videoSigns);
        }

        public void RemoveVideoSign(VideoSign videoSign)
        {
            videoSignDAO.Remove(videoSign);
        }

        public void RemoveVideoSign(int id)
        {
            videoSignDAO.Remove(id);
        }

        public void RemoveAllVideoSigns()
        {
            (videoSignDAO as VideoSignDAO).RemoveAll();
        }

        public void UpdateVideoSign(VideoSign updatedVideoSign)
        {
            videoSignDAO.Update(updatedVideoSign);
        }
        #endregion VideoSign

        public FullInfoVideoSign GetFullInfoVideoSign(int IdVideoSign)
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT" +
                                " VideoSign.Id AS Id," +
                                " VideoSign.SignId AS SignId," +
                                " VideoSign.Time AS Time," +
                                " VideoSign.Image AS Image," +
                                " Sign.Name AS Name," +
                                " Sign.Gost AS Gost," +
                                " Sign.Type AS Type," +
                                " Sign.Image AS SignImage" +
                                " FROM VideoSign JOIN Sign ON VideoSign.SignId=Sign.Id" +
                                " WHERE VideoSign.SignId =" + IdVideoSign + ";", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            FullInfoVideoSign infoSign = new FullInfoVideoSign();
            foreach (DbDataRecord record in reader)
                infoSign.Load(record);
            return infoSign;
        }

        public List<FullInfoVideoSign> AllFullInfoVideoSigns()
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT" +
                                " VideoSign.Id AS Id," +
                                " VideoSign.SignId AS SignId," +
                                " VideoSign.Time AS Time," +
                                " VideoSign.Image AS Image," +
                                " Sign.Name AS Name," +
                                " Sign.Gost AS Gost," +
                                " Sign.Type AS Type," +
                                " Sign.Image AS SignImage" +
                                " FROM VideoSign JOIN Sign ON VideoSign.SignId=Sign.Id;", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<FullInfoVideoSign> infoSigns = new List<FullInfoVideoSign>();
            foreach (DbDataRecord record in reader)
                infoSigns.Add(new FullInfoVideoSign(record));
            return infoSigns;
        }
    }
}
