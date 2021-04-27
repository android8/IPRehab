using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IPRehabRepository.Contracts
{
  public interface IRepositoryBase<T>
  {
    IQueryable<T> FindAll();

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);

    //Task BulkInsertAsync(IList<T> entityList);

    //Task BulkDeleteAsync(IList<T> entityList);

    //Task BulkUpdateAsync(IList<T> entityList);

    //Task BulkInsertOrUpdateAsync(IList<T> entityList);

    int BatchDelete(IList<T> entityList);

    int BatchInsert(IList<T> entityList);

    List<int> TransactionalDeleteAndInsert(IList<T> deleteEntityList, IList<T> insertEntityList);
  
    object TransactionalDeleteAndInsert2(IList<T> deleteEntityList, IList<T> insertEntityList);
    
    int spDelete(IList<T> entityList);

    int spInsert(IList<T> entityList);

    List<int> spTransactionalDeleteAndInsert(IList<T> deleteEntityList, IList<T> insertEntityList);
  }
}
