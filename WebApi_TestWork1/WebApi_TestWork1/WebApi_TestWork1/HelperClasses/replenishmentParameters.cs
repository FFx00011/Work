using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_TestWork1.HelperClasses
{
    // parameters for POST body replenishment
    public class replenishmentParameters
    {
        public string Valute { get; set; }
        public decimal Amount { get; set; }
    }
}
