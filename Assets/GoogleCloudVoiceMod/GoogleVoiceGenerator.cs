using System;
using System.Threading.Tasks;
using GoogleCloudVoiceMod.Api;
using GoogleCloudVoiceMod.Api.Data;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;
using Input = GoogleCloudVoiceMod.Api.Data.Input;

namespace GoogleCloudVoiceMod
{
    public class GoogleVoiceGenerator : ModScript, IVoiceAiAccessor
    {
    
        public static IVoiceAiAccessor MainModule;
        public static IAiApiProvider AIDatabase;

        private GoogleApi _api;
    
        public void SetProvider(IAiApiProvider database)
        {
            AIDatabase = database;
            MainModule = this;
            _api = new GoogleApi(this);
        }

        public int GenerateMessage(string text, Action<VoiceResult> finishedAction)
        {
            string langCode = GoogleSettings.Instance.GetString(EGoogleSettings.LanguageCode,
                (string)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.LanguageCode));

            string voiceName = GoogleSettings.Instance.GetString(EGoogleSettings.VoiceName,
                (string)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.VoiceName));

            float voicePitch = GoogleSettings.Instance.GetFloat(EGoogleSettings.Pitch,
                (float)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.Pitch));

            float voiceRate = GoogleSettings.Instance.GetFloat(EGoogleSettings.Speed,
                (float)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.Speed));

            DataToSend data = new DataToSend()
                {
                    input =
                        new Input()
                        {
                            text = text
                        },
                    voice =
                        new Voice()
                        {
                            languageCode = langCode,
                            name = voiceName
                        },
                    audioConfig =
                        new AudioConfig()
                        {
                            audioEncoding = "LINEAR16",
                            pitch = voicePitch,
                            speakingRate = voiceRate
                        }
                };
            
            int result = SendGoogleRequest(data, finishedAction).Result;
            return result;
        }
    
        private async Task<int> SendGoogleRequest(DataToSend data, Action<VoiceResult> finishedAction)
        {
            VoiceResult result = new VoiceResult();
            
            try
            {
                result = await _api.SendPrompt(data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"GoogleVoiceGenerator exception: {ex.Message}");
                Debug.LogError($"GoogleVoiceGenerator faulted api call: code: {result.Code}: {result.ErrorMessage}");
            }
            finally
            {
                finishedAction.Invoke(result);
            }
            
            return result.Code;
        }
    }
}
