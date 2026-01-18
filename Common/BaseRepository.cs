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
    public class BaseRepository<TDataEntity> : IBaseRepository<TDataEntity>, IDisposable
        where TDataEntity : class
    {
        private bool disposedValue;
        private readonly IMemoryCache memoryCache;
        bool IBaseRepository<TDataEntity>.IsTransactionScope { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected readonly DbContext _DbContext;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   
                }

                
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<ResponseResult>  Update(TDataEntity entity)
        {
            if (entity == null)
                return new ResponseResult { Message = "Entity cannot be null" };

            _DbContext.Set<TDataEntity>().Attach(entity);
            _DbContext.Entry(entity).State = EntityState.Modified;

            int affected = await _DbContext.SaveChanges();

            return affected > 0
                ? new ResponseResult { StatusCode = 200 }
                : new ResponseResult { StatusCode = 400 };
        }

       
        public Task<TDataEntity> Get(dynamic parameters, TDataEntity entity)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            var properties = (PropertyInfo[])parameters.GetType().GetProperties();
            var property = properties.FirstOrDefault();
            if (property == null)
                throw new ArgumentException("No property found in parameters");
            var id = Convert.ToInt32(property.GetValue(parameters));

            return await _DbContext.Set<TDataEntity>().Find(id);
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
     
        public BaseRepository(DbContext dbContext, IMemoryCache cache)
        {
            _DbContext = dbContext;
            memoryCache = cache ?? throw new ArgumentNullException("not cached");
        }
    }
}
