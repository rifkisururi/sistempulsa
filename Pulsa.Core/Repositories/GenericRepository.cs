using Microsoft.EntityFrameworkCore;
using Pulsa.Core.Interface;
using Pulsa.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Pulsa.Core.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly PulsaDataContext _context;
        public GenericRepository(PulsaDataContext context)
        {
            _context = context;
        }
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }
        public IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }
        public T GetById(Object id)
        {
            return _context.Set<T>().Find(id);
        }
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            var entry = _context.Entry(entity);

            if (entry == null || entry.State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }
            entry.State = EntityState.Modified;
        }

        public void UpdateNotExcludedColumns(T entity, string[] ExcludedColumns)
        {
            if (typeof(T).GetProperty("Id") == null)
                throw new Exception("Object doesn't have an ID.");

            var Id = typeof(T).GetProperty("Id").GetValue(entity);
            var currentValues = this.GetById(Id);
            if (currentValues != null)
            {
                var entry = _context.Entry(currentValues);
                entry.CurrentValues.SetValues(entity);
                entry.State = EntityState.Modified;

                foreach (var pe in entry.Properties)
                {
                    if (ExcludedColumns.Contains(pe.Metadata.Name) || pe.Metadata.IsPrimaryKey())
                        pe.IsModified = false;
                }
            }
            else
            {
                throw new Exception("Record not found.");
            }
        }

        public void UpdateIncludedColumns(T entity, string[] IncludedColumns)
        {
            if (typeof(T).GetProperty("Id") == null)
                throw new Exception("Object doesn't have an ID.");

            var Id = typeof(T).GetProperty("Id").GetValue(entity);
            var currentValues = this.GetById(Id);
            if (currentValues != null)
            {
                var entry = _context.Entry(currentValues);
                entry.CurrentValues.SetValues(entity);
                entry.State = EntityState.Modified;

                foreach (var pe in entry.Properties)
                {
                    if (IncludedColumns.Contains(pe.Metadata.Name) && !pe.Metadata.IsPrimaryKey())
                        pe.IsModified = true;
                    else
                        pe.IsModified = false;
                }
            }
            else
            {
                throw new Exception("Record not found.");
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        private Func<T, bool> GetEqualsExp<T>(string nameOfParameter, object valueToCompare)
        {
            PropertyInfo _property = typeof(T).GetTypeInfo().GetDeclaredProperty(nameOfParameter);
            ParameterExpression lambdaArg = Expression.Parameter(typeof(T));
            Expression propertyAccess = Expression.MakeMemberAccess(lambdaArg, _property);
            Expression propertyEquals = Expression.Equal(propertyAccess, Expression.Constant(valueToCompare));
            //Expression<Func<T, bool>> expressionHere = Expression.Lambda<Func<T, bool>>(propertyEquals, lambdaArg);

            return Expression.Lambda<Func<T, bool>>(propertyEquals, lambdaArg).Compile();
        }
    }
}
