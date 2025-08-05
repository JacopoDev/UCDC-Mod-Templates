using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.TextGen;

namespace ChatGptMod.GptApi
{
    public class OpenAIResponse
    {
        public Choice[] choices { get; set; }

        public class Choice
        {
            public Message message { get; set; }
            public string finish_reason { get; set; }
            public int index { get; set; }
        }
    }
    
    public enum EGptContentType
    {
        Text,
        Image
    }
    
    public struct MessageGpt
    {
        public string role;
        public List<ContentGpt> content;

        public Message ToRegularMessage()
        {
            Message msg = new Message();
            content = new List<ContentGpt>();
            msg.role = role;
            msg.content = content[0].text;
            if (content[0].image_url != null)
            {
                msg.imageBase64 = content[0].image_url.url;
            }
            
            return msg;
        }
    };

    public struct ContentGpt
    {
        public string type;
        [CanBeNull] [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string text;
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public ImageGpt image_url;

        public ContentGpt(EGptContentType gptType, string content)
        {
            switch (gptType)
            {
                case EGptContentType.Text:
                    type = "text";
                    text = content;
                    image_url = null;
                    break;
                case EGptContentType.Image:
                    type = "image_url";
                    text = null;
                    image_url = new ImageGpt(content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), gptType, null);
            }
        }
    };
    
    public class ImageGpt
    {
        public string url;
        public string detail;

        public ImageGpt(string imageBase64)
        {
            url = imageBase64;
            detail = "low";
        }
    };

    public struct OpenAiRequestData
    {
        public string model;
        public List<MessageGpt> messages;
        public float temperature;
        public int max_tokens;
        public float top_p;
        public float frequency_penalty;
        public float presence_penalty;
        [CanBeNull] public string[] stop;
    }
    
    public static class KnownCodes
    {
        public const string TextExceeded = "context_length_exceeded";
    }

    [Serializable]
    public class OpenAIError
    {
        public Error error;
    }
    
    [Serializable]
    public class Error
    {
        public string message;
        public string type;
        public string param;
        public string code;
    }
}