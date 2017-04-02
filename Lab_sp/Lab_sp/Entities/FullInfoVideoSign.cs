using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Entities
{
    public class FullInfoVideoSign : VideoSign
    {
        public string Name { get; set; }
        public string Gost { get; set; }
        public string Type { get; set; }
        public Bitmap SignImage { get; set; }

        public FullInfoVideoSign() { }

        public FullInfoVideoSign(DbDataRecord record)
        {
            Load(record);
        }

        public override void Load(DbDataRecord record)
        {
            Id = int.Parse(record["Id"].ToString());
            SignId = int.Parse(record["SignId"].ToString());
            Time = (string)record["Time"];
            Image = ImageExtention.BytesToBitmap((byte[])record["Image"]);

            Name = record["Name"].ToString();
            Gost = record["Gost"].ToString();
            Type = record["Type"].ToString();
            SignImage = ImageExtention.BytesToBitmap((byte[])record["SignImage"]);
        }
    }
}
