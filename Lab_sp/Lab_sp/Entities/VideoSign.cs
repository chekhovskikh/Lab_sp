using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Entities
{
    public class VideoSign : IEntity
    {
        public int Id { get; set; }
        public int SignId { get; set; }
        public string Time { get; set; }
        public Bitmap Image { get; set; }
        public VideoSign() { }
        public VideoSign(int id)
        {
            this.Id = id;
        }
        public VideoSign(int signId, string time, Bitmap image)
        {
            this.SignId = signId;
            this.Time = time;
            this.Image = image;
        }

        public VideoSign(int id, int signId, string time, Bitmap image)
        {
            this.Id = id;
            this.SignId = signId;
            this.Time = time;
            this.Image = image;
        }

        public VideoSign(DbDataRecord record)
        {
            Load(record);
        }

        public virtual void Load(DbDataRecord record)
        {
            Id = int.Parse(record["Id"].ToString());
            SignId = int.Parse(record["SignId"].ToString());
            Time = (string)record["Time"];
            Image = ImageExtention.BytesToBitmap((byte[])record["Image"]);
        }
    }
}
