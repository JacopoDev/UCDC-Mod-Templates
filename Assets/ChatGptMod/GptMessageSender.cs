using System;
using System.Threading.Tasks;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.TextGen;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace ChatGptMod
{
    public class GptMessageSender : ModScript, ITextAiAccessor
    {
        public static ITextAiAccessor MainModule;
        public static IAiApiProvider AIDatabase;

        private ChatGptApi _api;
        
        
        public void SetProvider(IAiApiProvider provider)
        {
            AIDatabase = provider;
            MainModule = this;
            _api = new ChatGptApi(this);
        }

        public int GenerateMessage(IChatProvider aiProcessor, Action<TextResult> finishedAction)
        {
            int result = SendGptRequest(aiProcessor, finishedAction).Result;
            return result;
        }
    
        private async Task<int> SendGptRequest(IChatProvider aiProcessor, Action<TextResult> finishedAction)
        {
            TextResult result = await _api.SendPrompt(aiProcessor.GetChat().Messages);
            finishedAction.Invoke(result);
            return result.Code;
        }

    }
}
