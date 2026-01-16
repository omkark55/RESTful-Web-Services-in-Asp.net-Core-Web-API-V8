using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Transactions;
using WebApi_Angular.Models;

namespace WebApi_Angular.Common
{
    // Remove duplicate explicit interface implementations for Get(dynamic) and Get(Expression<Func<TDataEntity, bool>>)
    // Implement missing Single(object) method

    // Remove these duplicate explicit interface implementations:
    // TDataEntity IBaseRepository<TDataEntity>.Get(dynamic parameters)
    // TDataEntity IBaseRepository<TDataEntity>.Get(Expression<Func<TDataEntity, bool>> predicate)
    // TDataEntity IBaseRepository<TDataEntity>.Get(Expression<Func<TDataEntity, bool>> predicate)

    // Add missing implementation for Single(object primaryKey)

    public class BaseRepository<TDataEntity> : IBaseRepository<TDataEntity>, IDisposable
        where TDataEntity : class
    {
        private bool disposedValue;
        private readonly IMemoryCache memoryCache;
        bool IBaseRepository<TDataEntity>.IsTransactionScope { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected readonly DbContext _DbContext;

        // ... existing code ...

        // Remove these duplicate explicit interface implementations:
        // TDataEntity IBaseRepository<TDataEntity>.Get(dynamic parameters)
        // TDataEntity IBaseRepository<TDataEntity>.Get(Expression<Func<TDataEntity, bool>> predicate)
        // TDataEntity IBaseRepository<TDataEntity>.Get(Expression<Func<TDataEntity, bool>> predicate)

        // Add missing implementation for Single(object primaryKey)
        public TDataEntity? Single(object primaryKey)
        {
            // Use Find<TDataEntity>(params object?[]? keyValues) and cast result to TDataEntity?
            // This addresses both CS0266 (explicit cast) and CS8603 (nullable return)
            return _DbContext.Find<TDataEntity>(primaryKey) as TDataEntity;
        }

        int IBaseRepository<TDataEntity>.Commit()
        {
            throw new NotImplementedException();
        }

        void IBaseRepository<TDataEntity>.CommitAsync()
        {
            throw new NotImplementedException();
        }

        TDataEntity IBaseRepository<TDataEntity>.DTO(object entity)
        {
            throw new NotImplementedException();
        }

        bool IBaseRepository<TDataEntity>.Exists(object primaryKey)
        {
            throw new NotImplementedException();
        }

        //public TDataEntity Get(dynamic parameters,Type type)
        //{
        //    if (parameters == null)
        //        throw new ArgumentNullException(nameof(parameters));
        //    //int id = Convert.ToInt32(parameters["Id"]);
        //    if (type == null)
        //        throw new ArgumentNullException(nameof(type));
        //    TDataEntity = type;
        //    return _DbContext.Set<TDataEntity>().Find(Convert.ToDecimal(parameters.Id));
        //}

        TDataEntity IBaseRepository<TDataEntity>.Get(Expression<Func<TDataEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<TDataEntity> IBaseRepository<TDataEntity>.GetAll(Expression<Func<TDataEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        TDataEntity IBaseRepository<TDataEntity>.SingleOrDefault(object primaryKey)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ResponseResult Update(TDataEntity entity)
        {
            if (entity == null)
                return new ResponseResult { Message = "Entity cannot be null" };

            _DbContext.Set<TDataEntity>().Attach(entity);
            _DbContext.Entry(entity).State = EntityState.Modified;

            int affected = _DbContext.SaveChanges();

            return affected > 0
                ? new ResponseResult { StatusCode = 200 }
                : new ResponseResult { StatusCode = 400 };
        }

       
        public TDataEntity Get(dynamic parameters, TDataEntity entity)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            //int id = Convert.ToInt32(parameters.Id);
            var properties = (PropertyInfo[])parameters.GetType().GetProperties();
            var property = properties.FirstOrDefault();
            if (property == null)
                throw new ArgumentException("No property found in parameters");
            var id = Convert.ToInt32(property.GetValue(parameters));

            return _DbContext.Set<TDataEntity>().Find(id);
        }

        ResponseResult Delete(TDataEntity entity)
        {
            throw new NotImplementedException();
        }

        ResponseResult IBaseRepository<TDataEntity>.Delete(TDataEntity entity)
        {
            return Delete(entity);
        }
        
        public bool SoftDelete<T>(T id)
        {
            var entity = _DbContext.Set<TDataEntity>().Find(id);
            if (entity == null) return false;

            var prop = typeof(TDataEntity).GetProperty("IsDeleted");
            if (prop == null)
                throw new InvalidOperationException("Entity does not support soft delete");

            prop.SetValue(entity, true);
            _DbContext.SaveChanges();
            return true;
        }


        // Add this constructor to BaseRepository<TDataEntity>
        public BaseRepository(DbContext dbContext, IMemoryCache cache)
        {
            _DbContext = dbContext;
            memoryCache = cache ?? throw new ArgumentNullException("not cached");
        }
        // ... existing code ...
    }
}
