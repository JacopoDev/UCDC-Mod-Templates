using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatGptMod.GptApi;
using Newtonsoft.Json;
using UCDC_Mod_Api.Models;
using UnityEngine;

namespace ChatGptMod
{
    public class ChatGptApi
    {
        private GptMessageSender _sender;
        
        public string openAIUrl = "https://api.openai.com/v1/chat/completions";
        public string contentType = "application/json";

        
        public ChatGptApi(GptMessageSender messageSender)
        {
            _sender = messageSender;
        }
        
        public async Task<TextResult> SendPrompt(List<Message> messages)
        {
            OpenAiRequestData data = LoadRequestData(messages);

            var returnStatus = await SendPrompt(data, messages);
            return returnStatus;
        }

        private OpenAiRequestData LoadRequestData(List<Message> messages)
        {
            List<MessageGpt> gptMessages = new List<MessageGpt>();
            messages.ForEach(x => gptMessages.Add(x.ToGptMessage()));

            OpenAiRequestData requestData = new OpenAiRequestData()
            {
                model = GptSettings.Instance.GetString(EGptSettings.Model, "gpt-4o-mini"),
                messages = gptMessages,
                temperature = GptSettings.Instance.GetFloat(EGptSettings.Temperature, 0.9f),
                max_tokens = GptSettings.Instance.GetInt(EGptSettings.MaxTokens, 1000),
                top_p = GptSettings.Instance.GetFloat(EGptSettings.TopP, 1.0f),
                frequency_penalty = GptSettings.Instance.GetFloat(EGptSettings.FrequencyPenalty, 2.0f),
                presence_penalty = GptSettings.Instance.GetFloat(EGptSettings.PresencePenalty, 2.0f),
                stop = GptSettings.Instance.GetStopStrings()
            };

            return requestData;
        }
        
        public virtual async Task<TextResult> SendPrompt(OpenAiRequestData requestData, List<Message> messages)
        {
            TextResult apiResult = new TextResult();
            apiResult.Code = (int)HttpStatusCode.NotFound;

            string api = GptSettings.Instance.GetApiDecoded();
            if (api == string.Empty)
            {
                apiResult.Code = (int)HttpStatusCode.BadRequest;
                apiResult.ErrorMessage = "No API key was set";
                return apiResult;
            }

            var requestJson = JsonConvert.SerializeObject(requestData);
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {api}");

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync(openAIUrl, content);
            }
            catch (Exception ex)
            {
                apiResult.Code = (int)HttpStatusCode.BadRequest;
                apiResult.ErrorMessage = ex.Message;
                return apiResult;
            }
            
            string result = await response.Content.ReadAsStringAsync();
            apiResult.Code = ((int)response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                OpenAIError error = JsonConvert.DeserializeObject<OpenAIError>(result);
                if (error?.error.code == KnownCodes.TextExceeded)
                {
                    messages.RemoveAt(1);
                    messages.RemoveAt(1);
    
                    apiResult.Code = (int)HttpStatusCode.BadRequest;
                    apiResult = await SendPrompt(requestData, messages);
                    return apiResult;
                }
                    
                apiResult.ErrorMessage = response.ReasonPhrase;
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    error.error.message = "Too many requests or no available credits on account.";
                    response.ReasonPhrase = "Too many requests or no available credits on account.";
                    apiResult.ErrorMessage = response.ReasonPhrase;
                }
                
                return apiResult;
            }
            
            var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(result);

            if (responseObject == null)
            {
                apiResult.Code = (int)HttpStatusCode.BadRequest;
                apiResult.ErrorMessage = "Error downloading a message";
                return apiResult;
            }
                
            apiResult.Code = (int)HttpStatusCode.OK;
            apiResult.Message = responseObject.choices[0].message;
                
            return apiResult;
        }
    }
}
