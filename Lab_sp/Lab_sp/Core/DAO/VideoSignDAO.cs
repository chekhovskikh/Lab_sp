using Lab_sp.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Core.DAO
{
    class VideoSignDAO : IEntityDTO<VideoSign>
    {
        private SQLiteConnection connection = null;

        public VideoSignDAO(SQLiteConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();
            this.connection = connection;
        }

        public VideoSign Get(int id)
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT * FROM VideoSign WHERE Id=" + id + ";", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            VideoSign videoSign = new VideoSign();
            foreach (DbDataRecord record in reader)
                videoSign.Load(record);
            return videoSign;
        }

        public List<VideoSign> GetAll()
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT * FROM VideoSign;", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<VideoSign> videoSigns = new List<VideoSign>();
            foreach (DbDataRecord record in reader)
                videoSigns.Add(new VideoSign(record));
            return videoSigns;
        }

        public void Add(VideoSign videoSign)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO VideoSign ('SignId', 'Time', 'Image') " +
               "VALUES (" + videoSign.SignId + ", '" + videoSign.Time + "', @0);", connection);
            SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.Binary);
            param.Value = ImageExtention.BitmapToBytes(videoSign.Image);
            command.Parameters.Add(param);
            command.ExecuteNonQuery();
        }

        public void AddAll(List<VideoSign> videoSigns)
        {
            string query = "INSERT INTO VideoSign ('SignId', 'Time', 'Image') VALUES ";
            SQLiteParameter[] parameters = new SQLiteParameter[videoSigns.Count];
            for (int i = 0, count = videoSigns.Count; i < count; i++)
            {
                VideoSign videoSign = videoSigns[i];
                query += "(" + videoSign.SignId + ", '" + videoSign.Time + "', @" + i + ")";
                parameters[i] = new SQLiteParameter("@" + i, System.Data.DbType.Binary);
                parameters[i].Value = ImageExtention.BitmapToBytes(videoSign.Image);
                if (i + 1 == count)
                    query += ";";
                else query += ", ";
            }
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }

        public void Remove(int id)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM VideoSign WHERE Id=" + id + ";", connection);
            command.ExecuteNonQuery();
        }

        public void Remove(VideoSign videoSign)
        {
            Remove(videoSign.Id);
        }

        public void Update(VideoSign updatedVideoSign)
        {
            SQLiteCommand command = new SQLiteCommand("UPDATE VideoSign SET " +
                "SignId=" + updatedVideoSign.SignId +
                ", Time='" + updatedVideoSign.Time +
                "', Image=@0" +
                " WHERE Id=" + updatedVideoSign.Id + ";", connection);

            SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.Binary);
            param.Value = ImageExtention.BitmapToBytes(updatedVideoSign.Image);
            command.Parameters.Add(param);
            command.ExecuteNonQuery();
        }
    }
}
