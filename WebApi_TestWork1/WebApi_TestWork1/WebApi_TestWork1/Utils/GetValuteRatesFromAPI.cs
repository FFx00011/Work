using System.Collections.Generic;
using System.Linq;
using WebApi_TestWork1.HelperClasses;

namespace WebApi_TestWork1.Utils
{
    // api utils
    public static class GetValuteRatesFromAPI
    {
        private static readonly string RateAPIUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        // Get serialized values into web api service
        public static List<ValuteRate> GetActualValuteRate()
        {
            var XMLString = HTTPUtils.GetValuteRate(RateAPIUrl);
            if (XMLString != null & XMLString != "")
            {
                var ValuteRate = XMLUtils.ParseXMLRateToClass(XMLString);
                if (ValuteRate != null)
                {
                    if (ValuteRate.Count() > 0)
                    {
                        return ValuteRate;
                    }
                }
            }
            return null;
        }
    }
}
