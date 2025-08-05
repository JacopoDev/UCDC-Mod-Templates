using System.Collections.Generic;
using Newtonsoft.Json;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.TextGen;

namespace OpenWebUiMod.Api
{
    public class MessageWebUi
    {
        [JsonProperty("role")] public string Role;
        [JsonProperty("content")] public string Content;
    };
    
    public class Response
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("created")]
        public long Created { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("choices")]
        public List<ChatChoice> Choices { get; set; }
    }

    public class ChatChoice
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("message")]
        public Message Message { get; set; }
        [JsonProperty("finishReason")]
        public string FinishReason { get; set; }
    }
}