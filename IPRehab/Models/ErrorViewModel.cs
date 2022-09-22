using System;

namespace IPRehab.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ExceptionCategory { get; set; }
        public string Message { get; set; }
        public string InnerExceptionMessage { get; set; }
        public ErrorPartialViewModel ErrorPartial { get;set; }
   }
}
