using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ReversePhoneLookup.IntegrationTests.Shared
{
    public class Helpers
    {
        public static StringContent ToStringContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
