using System;
using System.Threading.Tasks;
using GoogleCloudVoiceMod.Api;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace GoogleCloudVoiceMod
{
    public class GoogleVoiceGenerator : ModScript, IVoiceAiAccessor
    {
    
        public static IVoiceAiAccessor MainModule;
        public static IAiApiDatabase AIDatabase;

        private GoogleApi _api;
    
        public void SetDatabase(IAiApiDatabase database)
        {
            AIDatabase = database;
            MainModule = this;
            _api = new GoogleApi(this);
        }

        public int GenerateMessage(string text, Action<VoiceResult> finishedAction)
        {
            int result = SendGptRequest(text, finishedAction).Result;
            return result;
        }
    
        private async Task<int> SendGptRequest(string text, Action<VoiceResult> finishedAction)
        {
            VoiceResult result = await _api.SendPrompt(text);
            finishedAction.Invoke(result);
            return result.Code;
        }
    }
}
