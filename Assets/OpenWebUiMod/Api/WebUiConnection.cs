using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenWebUiApi.Model;
using UnityEngine;

namespace OpenWebUiMod.Api
{
    public class WebUiConnection
    { 
        private OpenWebUiParams _parameters;
        public string LastMessageString;

        public WebUiConnection()
        {
            _parameters = new OpenWebUiParams();
        }

        public OpenWebUiParams GetParameters()
        {
            return _parameters;
        }

        public async Task<Response> SendRequestAsync(List<MessageWebUi> messages, OpenWebUiParams settings)
        {
            _parameters = settings;
            
            if (messages == null)
            {
                throw new ArgumentException("no messages!");
            }

            if (_parameters.settings.apiKey == String.Empty)
            {
                throw new ArgumentException("no api key set!");
            }

            if (_parameters.data.model == String.Empty)
            {
                throw new ArgumentException("model not set!");
            }
            
            _parameters.data.SetMessages(messages);
            _parameters.data.model = _parameters.settings.model;
            
            string url = $"http://{_parameters.settings.address}:{_parameters.settings.port}/api/chat/completions";
            var requestData = new { model = _parameters.data.model, messages = _parameters.data.messages };
        
            string json = JsonConvert.SerializeObject(requestData);
        
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.settings.apiKey}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                
                string responseString = await response.Content.ReadAsStringAsync();
                LastMessageString = responseString;
                
                Response chatResponse = JsonConvert.DeserializeObject<Response>(responseString);
                return chatResponse;
            }
        }
    }
}