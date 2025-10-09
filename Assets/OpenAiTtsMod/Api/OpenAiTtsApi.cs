using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UCDC_Mod_Api.Models.VoiceGen;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenAiTtsMod.Api
{
    public class OpenAiTtsApi
    {
        // === CONFIGURATION ===
        private const string TTS_URL = "https://api.openai.com/v1/audio/speech";

        private VoiceResult _result;

        // === ENTRY POINT ===
        public async Task<int> GenerateMessage(string text, Action<VoiceResult> finishedAction)
        {
            OpenAiTtsSettings settings = OpenAiTtsSettings.Instance;

            _result = new VoiceResult();

            string modelSettings = settings.GetString(EOpenAiTtsSettings.Model, (string)settings.GetDefaultValue(EOpenAiTtsSettings.Model));
            string inputSettings = text;
            string voiceSettings = settings.GetString(EOpenAiTtsSettings.Voice, (string)settings.GetDefaultValue(EOpenAiTtsSettings.Voice));

            try
            {
                TtsData payload = new TtsData()
                {
                    model = modelSettings,
                    input = inputSettings,
                    voice = voiceSettings,
                    response_format = "wav"
                };
                
                string jsonData = JsonUtility.ToJson(payload);
                
                var request = new HttpRequestMessage(HttpMethod.Post, TTS_URL)
                {
                    Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
                };
                

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.GetApiDecoded());

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.SendAsync(request);

                _result.Code = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    byte[] wavData = await response.Content.ReadAsByteArrayAsync();
                    _result.Voice = TextToSpeech.Instance.ConvertSound(wavData, SetVoiceResult);
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    _result.ErrorMessage = $"HTTP {(int)response.StatusCode}: {err}";
                }
            }
            catch (Exception ex)
            {
                _result.Code = -1;
                _result.ErrorMessage = ex.Message;
            }

            finishedAction?.Invoke(_result);
            return _result.Code;
        }

        public void SetVoiceResult(AudioClip audio)
        {
            if (audio != null)
            {
                _result.Voice = audio;
            }
        }
    }
}