using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GoogleCloudVoiceMod.Api.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace GoogleCloudVoiceMod.Api
{
    struct PostData
    {
        public string url;
        public string bodyJsonString;
        public Action<string> requestReceived;
        public Action<BadRequestData> errorReceived;
        public Dictionary<string, string> headers;
    }
    
    public class RequestService
    {
        public async Task SendDataToGoogle(string url, DataToSend dataToSend, string apiKey, Action<string> requestReceived,
            Action<BadRequestData> errorReceived)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                BadRequestData badRequestData = new BadRequestData()
                {
                    error = new Error()
                    {
                        code = 400,
                        details = null,
                        message = "GoogleApi: api key is not empty or not correct",
                        status = "error",
                    }
                };
                errorReceived?.Invoke(badRequestData);
                return;
            }
            
            var headers = new Dictionary<string, string>();
            headers.Add("X-Goog-Api-Key", apiKey);
            PostData pd = new PostData()
            {
                url = url,
                bodyJsonString = JsonConvert.SerializeObject(dataToSend),
                requestReceived = requestReceived,
                errorReceived = errorReceived,
                headers = headers
            };

            await Post(pd);
        }
        
        private async Task Post(PostData pd)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var header in pd.headers)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }

                var content = new StringContent(pd.bodyJsonString, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(pd.url, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (HasError(response, responseText, out var badRequest))
                {
                    pd.errorReceived?.Invoke(badRequest);
                    return;
                }
                
                pd.requestReceived?.Invoke(responseText);
            }
        }

        private static bool HasError(HttpResponseMessage response, string responseText, out BadRequestData badRequestData)
        {
            if ((int)response.StatusCode == 200 || (int)response.StatusCode == 201)
            {
                badRequestData = null;
                return false;
            }

            try
            {
                badRequestData = JsonConvert.DeserializeObject<BadRequestData>(responseText);

                return true;
            }
            catch (Exception)
            {
                // Fallback: Create a safe BadRequestData with raw text
                badRequestData = new BadRequestData
                {
                    error = new Error
                    {
                        code = (int)response.StatusCode,
                        message = response.ReasonPhrase
                    }
                };

                return true;
            }
        }
    }
}