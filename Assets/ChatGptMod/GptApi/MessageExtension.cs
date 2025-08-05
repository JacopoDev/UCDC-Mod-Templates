using System.Collections.Generic;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.TextGen;

namespace ChatGptMod.GptApi
{
    public static class MessageExtension
    {
        public static MessageGpt ToGptMessage(this Message message, bool isIncludeImages = true)
        {
            MessageGpt newMessage = new MessageGpt();
            newMessage.role = message.role;
            
            newMessage.content = new List<ContentGpt>();
            newMessage.content.Add(new ContentGpt(EGptContentType.Text, message.content));
            
            if (message.imageBase64 != null &&
                isIncludeImages)
            {
                newMessage.content.Add(new ContentGpt(EGptContentType.Image, message.imageBase64));
            }

            return newMessage;
        }
    }
}