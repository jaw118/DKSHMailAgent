using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DKSH.MailAgent.Data.Models;
 

namespace DKSH.MailAgent
{

    public class Options
    {
        public JobState? JobState { get; set; }
        public bool Migration { get; set; }
        public DateTime? Date { get; set; }
        public int? ExportTextType { get; set; }
    }
}

