namespace IPRehabWebAPI2.Models
{
   public class CodeSetDTO
   {
      public int CodeSetID { get; set; }
      public int? CodeSetParent { get; set; }
      public string CodeValue { get; set; }
      public string CodeDescription { get; set; }
      public string Comment { get; set; }
   }
}