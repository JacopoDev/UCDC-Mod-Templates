using System;
using System.Threading.Tasks;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace ChatGptMod
{
    public class GptMessageSender : ModScript, ITextAiAccessor
    {
        public static ITextAiAccessor MainModule;
        public static IAiApiDatabase AIDatabase;

        private ChatGptApi _api;
        
        
        public void SetDatabase(IAiApiDatabase database)
        {
            AIDatabase = database;
            MainModule = this;
            _api = new ChatGptApi(this);
        }

        public int GenerateMessage(ITextAiProcessor aiProcessor, Action<Result> finishedAction)
        {
            int result = SendGptRequest(aiProcessor, finishedAction).Result;
            return result;
        }
    
        private async Task<int> SendGptRequest(ITextAiProcessor aiProcessor, Action<Result> finishedAction)
        {
            Result result = await _api.SendPrompt(aiProcessor.GetChat().Messages);
            finishedAction.Invoke(result);
            return result.Code;
        }
    }
}
