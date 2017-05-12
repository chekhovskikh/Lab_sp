using Lab_sp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp.Core.DAO
{
    interface IEntityDAO<T> where T : IEntity
    {
        T Get(int id);
        List<T> GetAll();
        void Add(T entity);
        void AddAll(List<T> entities);
        void Remove(T entity);
        void Remove(int id);
        void Update(T updatedEntity);
    }
}
