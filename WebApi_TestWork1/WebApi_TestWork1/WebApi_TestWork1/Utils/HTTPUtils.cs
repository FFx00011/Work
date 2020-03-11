using System.Net.Http;

namespace WebApi_TestWork1.Utils
{
    // http utils
    public static class HTTPUtils
    {
        private static HttpClient _HttpClient = new HttpClient();
        // Http request for ewb service to return actual valute rate
        public static string GetValuteRate(string path)
        {
            try
            {
                var response = _HttpClient.GetAsync(path).Result;
                var ResponseXMLString = response.Content.ReadAsStringAsync().Result;
                return ResponseXMLString;
            }
            catch { }
            return null;
        }
    }
}
