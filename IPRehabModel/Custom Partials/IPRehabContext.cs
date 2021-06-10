using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPRehabModel
{
  /// <summary>
/// 
/// </summary>
  public partial class IPRehabContext {
    public virtual List<Sp_CustomerDetails02> Sp_CustomerDetails()
    {
      //return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_CustomerDetails02>("Sp_CustomerDetails");
      //  this.Database.SqlQuery<Sp_CustomerDetails02>("Sp_CustomerDetails");
      using (JobScheduleSmsEntities db = new JobScheduleSmsEntities())
      {
        return db.Database.SqlQuery<Sp_CustomerDetails02>("Sp_CustomerDetails").ToList();

      }
    }
}
