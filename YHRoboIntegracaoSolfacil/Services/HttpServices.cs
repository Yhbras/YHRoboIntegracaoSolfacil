using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace YHRoboIntegracaoSolfacil.Services
{
    public class HttpServices
    {
        private static HttpClient client = new HttpClient();

        public static HttpResponseMessage Send(HttpMethod metodo, string endPoint, string body, string apiKeyName = "", string apiKeyValue = "", string token = "", string autenticacao = "")
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                string jsonString = string.Empty;

                if (!string.IsNullOrEmpty(apiKeyName) && !string.IsNullOrEmpty(apiKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiKeyName, apiKeyValue);
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(autenticacao, token);
                }

                var request = new HttpRequestMessage
                {
                    Method = metodo,
                    RequestUri = new Uri(endPoint),
                    Content = (!string.IsNullOrEmpty(body) ? new StringContent(body, Encoding.UTF8, "application/json") : null),
                };

                response = client.SendAsync(request).Result;
            }
            catch (Exception ex)
            {
                string erro = ex.Message;
                response = null;
            }

            return response;
        }
    }
}
