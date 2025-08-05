using System;
using System.Net;
using System.Threading.Tasks;
using ElevenLabsMod.Api;
using ElevenLabsMod.Api.Model;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.VoiceGen;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace ElevenLabsMod
{
    public class ElevenVoiceGenerator : ModScript, IVoiceAiAccessor
    {
        public static IVoiceAiAccessor MainModule;
        public static IAiApiProvider AIDatabase;

        private ElevenLabsTextToSpeech _elevenTts;
        
        public void SetProvider(IAiApiProvider provider)
        {
            AIDatabase = provider;
            MainModule = this;
            _elevenTts = new();
        }

        public int GenerateMessage(string text, Action<VoiceResult> finishedAction)
        {
            return SendElevenLabsRequest(text, finishedAction).Result;
        }
    
        private async Task<int> SendElevenLabsRequest(string text, Action<VoiceResult> finishedAction)
        {
            VoiceResult result = new VoiceResult();
            int code = (int)HttpStatusCode.NotFound;
            
            try
            {
                code = await _elevenTts.GetSpeech(text, clip =>
                    {
                        result.Code = (int)HttpStatusCode.OK;
                        result.Voice = clip;
                    }, error =>
                    {
                        result.Code = error.error.code;
                        result.ErrorMessage = error.error.message;
                        result.Voice = null;
                    }
                    );
            }
            catch (Exception ex)
            {
                Debug.LogError($"ElevenLabsMod exception: {ex.Message}");
                Debug.LogError($"ElevenLabsMod faulted api call: code: {result.Code}: {result.ErrorMessage}");
            }
            finally
            {
                finishedAction.Invoke(result);
            }
            
            return code;
        }
    }
}