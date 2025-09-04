using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.TextGen;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace ChatGptMod
{
    public class GptMessageSender : ModScript, ITextAiAccessor, ITextAiExtraSettings
    {
        public static ITextAiAccessor MainModule;
        public static IAiApiProvider AIDatabase;

        private ChatGptApi _api;
        
        private readonly TextAiInfo _textAiInfo = new TextAiInfo()
        {
            IsQuick = true,
            IsStreaming = false,
            IsStructured = true,
            IsImageReading = true,
            IsMultiLanguage = true
        };
        
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
            TextResult result = await _api.SendPrompt(aiProcessor.GetChat().Messages, aiProcessor.GetResponseFormat());
            finishedAction.Invoke(result);
            return result.Code;
        }
    
        public TextAiInfo GetSettings()
        {
            return _textAiInfo;
        }
    }
}
