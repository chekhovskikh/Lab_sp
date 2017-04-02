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
        private IEntityDTO<User> userDTO = null;
        private IEntityDTO<Sign> signDTO = null;
        private IEntityDTO<VideoSign> videoSignDTO = null;

        private SQLiteConnection connection = null;

        public DAL()
        {
            connection = new SQLiteConnection(
                string.Format("Data Source={0};", Properties.Resources.db_name));
            userDTO = new UserDAO(connection);
            signDTO = new SignDAO(connection);
            videoSignDTO = new VideoSignDAO(connection);
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        #region User
        public User GetUser(int id)
        {
            return userDTO.Get(id);
        }

        public List<User> AllUsers()
        {
            return userDTO.GetAll();
        }

        public void AddUser(User user)
        {
            userDTO.Add(user);
        }

        public void AddAllUsers(List<User> users)
        {
            userDTO.AddAll(users);
        }

        public void RemoveUser(User user)
        {
            userDTO.Remove(user);
        }

        public void RemoveUser(int id)
        {
            userDTO.Remove(id);
        }

        public void UpdateUser(User updatedUser)
        {
            userDTO.Update(updatedUser);
        }
        #endregion User

        #region Sign
        public Sign GetSign(int id)
        {
            return signDTO.Get(id);
        }

        public int GetIdForGost(string Gost)
        {
            return (signDTO as SignDAO).GetIdForGost(Gost);
        }

        public List<Sign> AllSigns()
        {
            return signDTO.GetAll();
        }

        public void AddSign(Sign sign)
        {
            signDTO.Add(sign);
        }

        public void AddAllSigns(List<Sign> signs)
        {
            signDTO.AddAll(signs);
        }

        public void RemoveSign(Sign sign)
        {
            signDTO.Remove(sign);
        }

        public void RemoveSign(int id)
        {
            signDTO.Remove(id);
        }

        public void UpdateSign(Sign updatedSign)
        {
            signDTO.Update(updatedSign);
        }
        #endregion Sign

        #region VideoSign
        public VideoSign GetVideoSign(int id)
        {
            return videoSignDTO.Get(id);
        }

        public List<VideoSign> AllVideoSigns()
        {
            return videoSignDTO.GetAll();
        }

        public void AddVideoSign(VideoSign videoSign)
        {
            videoSignDTO.Add(videoSign);
        }

        public void AddAllVideoSigns(List<VideoSign> videoSigns)
        {
            videoSignDTO.AddAll(videoSigns);
        }

        public void RemoveVideoSign(VideoSign videoSign)
        {
            videoSignDTO.Remove(videoSign);
        }

        public void RemoveVideoSign(int id)
        {
            videoSignDTO.Remove(id);
        }

        public void UpdateVideoSign(VideoSign updatedVideoSign)
        {
            videoSignDTO.Update(updatedVideoSign);
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
