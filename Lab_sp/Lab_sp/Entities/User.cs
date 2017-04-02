using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public Core.Settings.UserRole Role { get; set; }
        public User() { }
        public User(int id)
        {
            this.Id = id;
        }
        public User(string login, string pass, Core.Settings.UserRole role)
        {
            this.Login = login;
            this.Pass = pass;
            this.Role = role;
        }
        public User(int id, string login, string pass, Core.Settings.UserRole role)
        {
            this.Id = id;
            this.Login = login;
            this.Pass = pass;
            this.Role = role;
        }

        public User(DbDataRecord record)
        {
            Load(record);
        }

        public void Load(DbDataRecord record)
        {
            Id = int.Parse(record["Id"].ToString());
            Login = (string)record["Login"];
            Pass = (string)record["Pass"];
            Role = (record["Role"].ToString() == "User") ? 
                Core.Settings.UserRole.User : Core.Settings.UserRole.Admin;
        }
    }
}
