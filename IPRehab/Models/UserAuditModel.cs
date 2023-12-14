using System;
using System.ComponentModel.DataAnnotations;

namespace IPRehab.Models
{
    public class UserAuditModel
    {
        public long ID { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public string Application { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateAccessed { get; set; }
        public long ProductID { get; set; }
    }
}
