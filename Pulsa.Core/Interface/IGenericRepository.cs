using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Core.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(object Id);
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
        void Save();
        void UpdateNotExcludedColumns(T entity, string[] ExcludedColumns);
        void UpdateIncludedColumns(T entity, string[] IncludedColumns);
    }
}
