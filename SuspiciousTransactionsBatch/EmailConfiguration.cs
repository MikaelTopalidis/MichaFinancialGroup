using System;
using System.Collections.Generic;
using System.Text;

namespace SuspiciousTransactionsBatch
{

   
    public class EmailConfiguration 
    {
        // public string From { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }

}
