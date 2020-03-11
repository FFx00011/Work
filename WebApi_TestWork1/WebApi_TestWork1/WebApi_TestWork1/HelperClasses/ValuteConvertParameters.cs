using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_TestWork1.HelperClasses
{
    // parameters for POST body ValuteConvert
    public class ValuteConvertParameters
    {
        public string InValute { get; set; }
        public string ToValute { get; set; }
        public decimal Amount { get; set; }
    }
}
