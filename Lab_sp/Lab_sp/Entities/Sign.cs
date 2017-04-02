using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Entities
{
    public class Sign : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gost { get; set; }
        public string Type { get; set; }
        public Bitmap Image { get; set; }
        public Sign() { }
        public Sign(int id)
        {
            this.Id = id;
        }
        public Sign(string name, string gost, string type, Bitmap image)
        {
            this.Name = name;
            this.Gost = gost;
            this.Type = type;
            this.Image = image;
        }

        public Sign(int id, string name, string gost, string type, Bitmap image)
        {
            this.Name = name;
            this.Id = id;
            this.Gost = gost;
            this.Type = type;
            this.Image = image;
        }

        public Sign(DbDataRecord record)
        {
            Load(record);
        }

        public void Load(DbDataRecord record)
        {
            Id = int.Parse(record["Id"].ToString());
            Name = record["Name"].ToString();
            Gost = record["Gost"].ToString();
            Type = record["Type"].ToString();
            Image = ImageExtention.BytesToBitmap((byte[])record["Image"]);
        }
    }
}
