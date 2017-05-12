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
    class SignDAO : IEntityDAO<Sign>
    {
        private SQLiteConnection connection = null;

        public SignDAO(SQLiteConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();
            this.connection = connection;
        }
        public Sign Get(int id)
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT * FROM Sign WHERE Id=" + id + ";", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            Sign sign = new Sign();
            foreach (DbDataRecord record in reader)
                sign.Load(record);
            return sign;
        }

        public List<Sign> GetAll()
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT * FROM Sign;", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Sign> signs = new List<Sign>();
            foreach (DbDataRecord record in reader)
                signs.Add(new Sign(record));
            return signs;
        }

        public int GetIdForGost(string gost)
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT Id FROM Sign WHERE Gost='" + gost + "';", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            int id = 0;
            foreach (DbDataRecord record in reader)
                id = int.Parse(record["Id"].ToString());
            return id;
        }

        public void Add(Sign sign)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Sign ('Name', 'Gost', 'Type', 'Image') " +
                "VALUES ('" + sign.Name + "', '" + sign.Gost + "', '" + sign.Type + "', @0);", connection);
            SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.Binary);
            param.Value = ImageExtention.BitmapToBytes(sign.Image);
            command.Parameters.Add(param);
            command.ExecuteNonQuery();
        }

        public void AddAll(List<Sign> signs)
        {
            string query = "INSERT INTO Sign ('Name', 'Gost', 'Type', 'Image') VALUES ";
            SQLiteParameter[] parameters = new SQLiteParameter[signs.Count];
            for (int i = 0, count = signs.Count; i < count; i++)
            {
                Sign sign = signs[i];
                query += "('" + sign.Name + "', '" + sign.Gost + "', '" + sign.Type + "', @" + i + ")";
                parameters[i] = new SQLiteParameter("@" + i, System.Data.DbType.Binary);
                parameters[i].Value = ImageExtention.BitmapToBytes(sign.Image);
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
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Sign WHERE Id=" + id + ";", connection);
            command.ExecuteNonQuery();
        }

        public void Remove(Sign sign)
        {
            Remove(sign.Id);
        }

        public void Update(Sign updatedSign)
        {
            SQLiteCommand command = new SQLiteCommand("UPDATE Sign SET " +
                "Name='" + updatedSign.Name +
                "', Gost='" + updatedSign.Gost +
                "', Type='" + updatedSign.Type +
                "', Image=@0" +
                " WHERE Id=" + updatedSign.Id + ";", connection);

            SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.Binary);
            param.Value = ImageExtention.BitmapToBytes(updatedSign.Image);
            command.Parameters.Add(param);
            command.ExecuteNonQuery();
        }
    }
}
