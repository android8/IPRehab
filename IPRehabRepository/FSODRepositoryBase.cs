using IPRehabModel;
using IPRehabRepository.Contracts;
using Microsoft.EntityFrameworkCore;
using PatientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IPRehabRepository
{
   /// <summary>
   /// https://code-maze.com/net-core-web-development-part4/
   /// Repository project references RepositoryGenericInterface and PCC_Fit_Model_CoreLibrary, and inside the Repository project create the abstract class RepositoryBase which implements the interface IRepositoryBase. Reference this project to the main project too. This abstract class, as well as IRepositoryBase interface, uses generic type T to work with. This type T gives even more reusability to the RepositoryBase class. That means we don’t have to specify the exact model (class) right now for the RepositoryBase to work with.
   /// </summary>
   /// <typeparam name="T">Generic class to be solidified by the inheriting class</typeparam>
   public abstract class FSODRepositoryBase<T> : IRepositoryBase<T> where T : class
   {
      protected DmhealthfactorsContext RepositoryContext { get; set; }

      public FSODRepositoryBase(DmhealthfactorsContext repositoryContext)
      {
         this.RepositoryContext = repositoryContext;
      }

      ///// <summary>
      ///// pass a list of records to be inserted
      ///// </summary>
      ///// <param name="entityList"></param>
      ///// <returns></returns>
      //public async Task BulkInsertAsync(IList<T> entityList)
      //{
      //  await RepositoryContext.BulkInsertAsync(entityList);
      //}

      ///// <summary>
      ///// Delete all records matching the entityList
      ///// </summary>
      ///// <param name="entityList"></param>
      ///// <returns></returns>
      //public async Task BulkDeleteAsync(IList<T> entityList)
      //{
      //  await RepositoryContext.BulkDeleteAsync(entityList);
      //}

      ///// <summary>
      ///// bulk update columns in matching records with new value contained in entityList
      ///// </summary>
      ///// <param name="entityList"></param>
      ///// <returns></returns>
      //public async Task BulkUpdateAsync(IList<T> entityList)
      //{
      //  await RepositoryContext.BulkUpdateAsync(entityList);
      //}

      //public async Task BulkInsertOrUpdateAsync(IList<T> entityList)
      //{
      //  await RepositoryContext.BulkInsertOrUpdateAsync(entityList);
      //}

      public int BatchInsert(IList<T> entityList)
      {
         int inserted = 0;
         foreach (T entity in entityList)
         {
            this.RepositoryContext.Set<T>().Add(entity);
            inserted++;
         }
         return inserted;
      }

      public int BatchDelete(IList<T> entityList)
      {
         int deleted = 0;
         foreach (T entity in entityList)
         {
            this.RepositoryContext.Set<T>().Remove(entity);
            deleted++;
         }
         return deleted;
      }

      public object TransactionalDeleteAndInsert2(IList<T> deleteEntityList, IList<T> insertEntityList)
      {
         using (var transaction = this.RepositoryContext.Database.BeginTransaction())
         {
            int totalDeleted = this.BatchDelete(deleteEntityList);
            this.RepositoryContext.SaveChanges();

            int totalInserted = this.BatchInsert(insertEntityList);

            //this.RepositoryContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT app.TblUserAnswer ON");
            this.RepositoryContext.SaveChanges();
            //this.RepositoryContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT app.TblUserAnswer OFF");

            transaction.Commit();

            var objResult = new
            {
               TotalDeleted = totalDeleted,
               TotalInserted = totalInserted,
               InsertedEntities = insertEntityList //the newly inserted entities have new record IDs
            };
            return objResult;
         }
      }

      public List<int> TransactionalDeleteAndInsert(IList<T> deleteEntityList, IList<T> insertEntityList)
      {
         using (var transaction = this.RepositoryContext.Database.BeginTransaction())
         {
            int totalDeleted = this.BatchDelete(deleteEntityList);
            this.RepositoryContext.SaveChanges();

            int totalInserted = this.BatchInsert(insertEntityList);

            //this.RepositoryContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT app.TblUserAnswer ON");
            this.RepositoryContext.SaveChanges();
            //this.RepositoryContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT app.TblUserAnswer OFF");

            transaction.Commit();

            List<int> result = new List<int>();
            result.Add(totalDeleted);
            result.Add(totalInserted);
            return result;
         }
      }

      public int spDelete(IList<T> entityList)
      {
         throw new NotImplementedException();
      }

      public int spInsert(IList<T> entityList)
      {
         throw new NotImplementedException();
      }

      public List<int> spTransactionalDeleteAndInsert(IList<T> deleteEntityList, IList<T> insertEntityList)
      {
         throw new NotImplementedException();
      }

      IQueryable<T> IRepositoryBase<T>.FindAll() =>
         //return this.RepositoryContext.Set<T>().AsNoTracking();
         RepositoryContext.Set<T>().AsQueryable<T>();

      IQueryable<T> IRepositoryBase<T>.FindByCondition(Expression<Func<T, bool>> expression)
      {
         var questions = RepositoryContext.Set<T>().Where(expression).AsQueryable();
         //return this.RepositoryContext.Set<T>().Where(expression).AsNoTracking();
         return questions;
      }

      public async Task<int> CreateAsync(T entity)
      {
         RepositoryContext.Set<T>().Add(entity);
         return await RepositoryContext.SaveChangesAsync();
      }

      public async Task<int> UpdateAsync(T entity)
      {
         RepositoryContext.Set<T>().Update(entity);
         return await RepositoryContext.SaveChangesAsync();
      }

      public async Task<int> DeleteAsync(T entity)
      {
         RepositoryContext.Set<T>().Remove(entity);
         return await RepositoryContext.SaveChangesAsync();
      }
   }
}