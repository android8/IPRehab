using Microsoft.EntityFrameworkCore;

namespace UserModel
{
  /// <summary>
  /// custom partial with DbSet for stored procedure return type so that the partial does't get wiped out when the EF CORE Power Tools is rerun
  /// </summary>
  public partial class MasterreportsContext
  {
    public virtual DbSet<uspVSSCMain_SelectAccessInformationFromNSSDResult> uspVSSCMain_SelectAccessInformationFromNSSDResult { get; set; }
  }
}
