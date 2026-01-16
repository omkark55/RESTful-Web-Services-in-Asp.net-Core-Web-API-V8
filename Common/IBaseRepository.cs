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


            /// <summary>
            /// Retrieve a single item using it's primary key, exception if not found
            /// </summary>
            /// <param name="primaryKey">The primary key of the record</param>
            /// <param name="userInfo">Information of the user</param>
            /// <returns>T</returns>
            TDataEntity Single(object primaryKey);
            //TDataEntity Single(object primaryKey);

            /// <summary>
            /// Retrieve a single item by it's primary key or return null if not found
            /// </summary>
            /// <param name="primaryKey">Prmary key to find</param>
            /// <param name="userInfo">Information of the user</param>
            /// <returns>T</returns>
            TDataEntity SingleOrDefault(object primaryKey);
            //TDataEntity SingleOrDefault(object primaryKey);

            /// <summary>
            /// Returns all the rows for type T
            /// </summary>
            /// <returns></returns>
            //IEnumerable<T> GetAll();

            /// <summary>
            /// Does this item exist by it's primary key
            /// </summary>
            /// <param name="primaryKey"></param>
            /// <returns></returns>
            bool Exists(object primaryKey);

            /// <summary>
            /// Inserts the data into the table
            /// </summary>
            /// <param name="entity">The entity to insert</param>
            /// <param name="userInfo">Information of the user</param>
            /// <returns></returns>
            //void Insert(T entity);
            //RichtechIT.CRM.Common.ResponseResult Insert(TDataEntity entity, IUserInfo userInfo);

            /// <summary>
            /// Updates this entity in the database using it's primary key
            /// </summary>
            /// <param name="entity">The entity to update</param>
            /// <param name="userInfo">Information of the user</param>
            //void Update(T entity);
            ResponseResult Update(TDataEntity entity);

            //repo.Update(entity, e => e.Name, e => e.Description);
            //RichtechIT.CRM.Common.ResponseResult Update(TDataEntity entity, IUserInfo userInfo, params Expression<Func<TDataEntity, object>>[] properties);


            /// <summary>
            /// Deletes this entry fro the database
            /// ** WARNING - Most items should be marked inactive and Updated, not deleted
            /// </summary>
            /// <param name="entity">The entity to delete</param>
            /// <param name="userInfo">Information of the user</param>
            /// <returns></returns>
            //void Delete(T entity);
            ResponseResult Delete(TDataEntity entity);

           
            TDataEntity Get(dynamic parameters, TDataEntity entity);

            TDataEntity Get(Expression<Func<TDataEntity, bool>> predicate);
            //TDataEntity Get(IUserInfo userInfo, Expression<Func<TDataEntity, bool>> predicate);

            IEnumerable<TDataEntity> GetAll(Expression<Func<TDataEntity, bool>> predicate = null);

        //RichtechIT.CRM.Common.ResponseResult Save(TDataEntity entity);

        bool SoftDelete<T>(T id);
            int Commit();


            void CommitAsync();




        }

    }

