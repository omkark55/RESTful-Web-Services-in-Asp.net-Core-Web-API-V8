using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApi_Angular.Models;


namespace WebApi_Angular.Common
{
        public interface IBaseRepository<TDataEntity>
            where TDataEntity : class
        {
            bool IsTransactionScope { get; set; }
            TDataEntity DTO(object entity);
           
            ResponseResult Update(TDataEntity entity);
            
            ResponseResult Delete(TDataEntity entity);
           
            TDataEntity Get(dynamic parameters, TDataEntity entity);

            IEnumerable<TDataEntity> GetAll(Expression<Func<TDataEntity, bool>> predicate = null);

            bool SoftDelete<T>(T id);
            int Commit();

            void CommitAsync();

        }

    }

