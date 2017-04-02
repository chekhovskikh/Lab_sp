using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
        void Load(DbDataRecord record);
    }
}
