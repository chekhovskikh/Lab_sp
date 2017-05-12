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
    class UserDAO : IEntityDAO<User>
    {
        private SQLiteConnection connection = null;

        public UserDAO(SQLiteConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();
            this.connection = connection;
        }

        public User Get(int id)
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT * FROM User WHERE Id=" + id + ";", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            User user = new User();
            foreach (DbDataRecord record in reader)
                user.Load(record);
            return user;
        }

        public List<User> GetAll()
        {
            SQLiteCommand command =
            new SQLiteCommand("SELECT * FROM User;", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            foreach (DbDataRecord record in reader)
                users.Add(new User(record));
            return users;
        }

        public void Add(User user)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO User ('Login', 'Pass', 'Role') " +
                "VALUES ('" + user.Login + "', '" + user.Pass + "', '" + user.Role + "');", connection);
            command.ExecuteNonQuery();
        }

        public void AddAll(List<User> users)
        {
            string query = "INSERT INTO User ('Login', 'Pass', 'Role') VALUES ";
            for (int i = 0, count = users.Count; i < count; i++)
            {
                User user = users[i];
                query += "('" + user.Login + "', '" + user.Pass + "', '" + user.Role + "')";
                if (i + 1 == count)
                    query += ";";
                else query += ", ";
            }
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }

        public void Remove(int id)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM User WHERE Id=" + id + ";", connection);
            command.ExecuteNonQuery();
        }

        public void Remove(User user)
        {
            Remove(user.Id);
        }

        public void Update(User updatedUser)
        {
            SQLiteCommand command = new SQLiteCommand("UPDATE User SET " +
                "Login='" + updatedUser.Login +
                "', Pass='" + updatedUser.Pass +
                "', Role='" + updatedUser.Role +
                "' WHERE Id=" + updatedUser.Id + ";", connection);
            command.ExecuteNonQuery();
        }
    }
}
