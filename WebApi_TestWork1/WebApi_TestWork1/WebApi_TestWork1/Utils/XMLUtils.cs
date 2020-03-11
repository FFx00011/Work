using System;
using System.Collections.Generic;
using System.Xml;
using WebApi_TestWork1.HelperClasses;

namespace WebApi_TestWork1.Utils
{
    // xml utils
    public static class XMLUtils
    {
        // parse xml result from string and serialize to class
        public static List<ValuteRate> ParseXMLRateToClass(string XMLString)
        {
            try
            {
                List<ValuteRate> _ValuteRates = new List<ValuteRate>();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(XMLString);
                XmlNodeList name = xDoc.GetElementsByTagName("Cube");
                for (int i = 2; i < name.Count; i++)
                {
                    try
                    {
                        string ValuteName = name[i].Attributes[0].Value.ToString();
                        decimal ValuteRate = Convert.ToDecimal(name[i].Attributes[1].Value.Replace(".", ","));
                        if (ValuteName != null & ValuteName != "" & ValuteRate > 0)
                        {
                            _ValuteRates.Add(new ValuteRate() { Name = ValuteName, Rate = ValuteRate });
                        }
                    }
                    catch { }
                }
                return _ValuteRates;
            }
            catch { }
            return null;
        }
    }
}
