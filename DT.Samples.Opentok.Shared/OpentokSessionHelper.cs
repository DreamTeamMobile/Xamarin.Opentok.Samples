using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DT.Samples.Opentok.Shared
{
    public static class OpentokSessionHelper
    {
        /// <summary>
        /// The URI to request a new session ID from your custom API.
        /// Takes one parameter - room name/channel name
        /// </summary>
        public const string SessionRequestURI = "";
        /// <summary>
        /// The URI to request a new token for a specific session from your custom API.
        /// Takes one parameter - session id
        /// </summary>
        public const string TokenRequestURI = "";

        public static async Task<string> RequestDataFromApiAsync(string urlTemplate, params string[] parameters)
        {
            var url = string.Format(urlTemplate, parameters);
            try
            {
                var client = new HttpClient();
                var result = await client.GetStringAsync(url);
                var apiResponse = JsonConvert.DeserializeObject<CustomApiResponse>(result);
                return apiResponse.Result;
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }
    }
}
