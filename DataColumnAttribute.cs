using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DKSH.MailAgent.Attributes
{

    public class DataColumnAttribute : Attribute
    {
        public string Name { get; }
        public DataColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
