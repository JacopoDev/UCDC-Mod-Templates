using System;
using System.Threading.Tasks;
using OpenAiTtsMod.Api;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models.VoiceGen;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace OpenAiTtsMod
{
    public class OpenAiTtsVoiceGenerator : ModScript, IVoiceAiAccessor
    {
        public static IVoiceAiAccessor MainModule;
        public static IAiApiProvider AIDatabase;
        
        private OpenAiTtsApi _api;
        
        public void SetProvider(IAiApiProvider provider)
        {
            AIDatabase = provider;
            MainModule = this;
            _api = new OpenAiTtsApi();
        }

        public int GenerateMessage(string text, Action<VoiceResult> finishedAction)
        {
            int result = SendGoogleRequest(text, finishedAction).Result;
            return result;
        }

        private async Task<int> SendGoogleRequest(string text, Action<VoiceResult> finishedAction)
        {
            int result = await _api.GenerateMessage(text, finishedAction);
            return result;
        }
    }
}