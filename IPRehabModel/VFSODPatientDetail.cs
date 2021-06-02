using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPRehabModel
{
   /// <summary>
   /// model of external patient view from bi25.DMHealthFactors
   /// </summary>
   public partial class VFSODPatientDetail
   {
      public string VISN { get; set; }
      public string Facility { get; set; }
      public string District { get; set; }
      public string Division { get; set; }
      public int ADMParent_Key { get; set; }
      public int Sta6aKey { get; set; }
      public int bedsecn { get; set; }
      public string Name { get; set; }
      public string PTFSSN { get; set; }
      public string FSODSSN { get; set; }
      public string FiscalPeriod { get; set; }
      public int FiscalPeriodInt { get; set; }
   }
}
