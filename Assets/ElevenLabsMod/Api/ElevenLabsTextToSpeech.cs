using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ElevenLabsMod.Api.Model;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace ElevenLabsMod.Api
{
    public class ElevenLabsTextToSpeech
    {
        const string baseURL = "https://api.elevenlabs.io/v1/text-to-speech/"; // Base URL of HTTP request
        
        private static AudioConverter _audioConverter;
        public static ElevenLabsTextToSpeech Instance;


        public ElevenLabsTextToSpeech()
        {
            Instance = this;
            _audioConverter = new AudioConverter();
        }

        public async Task<int> GetSpeech(string textToConvert, Action<AudioClip> audioClipReceived, Action<BadRequestData> errorReceived)
        {
            ElevenLabsData data = new ElevenLabsData
            {
                prompt = textToConvert,
                voice = ElevenSettings.Instance.GetString(EElevenSettings.VoiceID, (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.VoiceID)),
                apikey = ElevenSettings.Instance.GetApiDecoded(),
                stability = ElevenSettings.Instance.GetFloat(EElevenSettings.Stability, (float)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Stability)),
                similarityBoost = ElevenSettings.Instance.GetFloat(EElevenSettings.SimilarityBoost,
                    (float)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.SimilarityBoost)),
                model = ElevenSettings.Instance.GetString(EElevenSettings.Model,
                    (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Model)),
                format = ElevenSettings.Instance.GetString(EElevenSettings.Format,
                    (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Format)),
                audioClipReceived = audioClipReceived,
                errorReceived = errorReceived
            };
            
            ElevenLabsResult result = await GetAudioFile(data);
            return result.Code;
        }

        private async Task<ElevenLabsResult> GetAudioFile(ElevenLabsData data)
        {
            ElevenLabsResult result = await RequestAudio(data);

            if (result.Error != null)
            {
                data.errorReceived(result.Error);
                return result;
            }

            await _audioConverter.ConvertWavBufferToClip(result.AudioFile, data.audioClipReceived);
            return result;
        }

        
        public async Task<ElevenLabsResult> RequestAudio(ElevenLabsData requestData) 
        {
            ElevenLabsResult newResponse = new ElevenLabsResult();
            string url = baseURL + requestData.voice + "?output_format=" + requestData.format; // add Voice ID and format to end of URL
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("xi-api-key", requestData.apikey); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));

            var data = new {
                text = requestData.prompt,
                model_id = requestData.model,
                voice_settings = new {
                    stability = requestData.stability,
                    similarity_boost = requestData.similarityBoost
                }
            };
            
            string json = JsonConvert.SerializeObject(data);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.Default, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, httpContent);

            
            if (response == null)
            {
                BadRequestData brd = new BadRequestData()
                {
                    error = new Error()
                    {
                        status = "No connection",
                        message = $"(ElevenLabs): No connection",
                        code = 404,
                    }
                };
                
                newResponse.Code = 404;
                newResponse.Error = brd;
                return newResponse;
            }
            
            newResponse.Code = (int)response.StatusCode;
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                BadRequestData brd = new BadRequestData()
                {
                    error = new Error()
                    {
                        status = response.StatusCode.ToString(),
                        message = $"(ElevenLabs): {response.ReasonPhrase}",
                        code = (int)response.StatusCode,
                    }
                };
                
                newResponse.Error = brd;
                return newResponse;
            }
            
            newResponse.Code = (int)response.StatusCode;
            newResponse.AudioFile = await response.Content.ReadAsByteArrayAsync();

            return newResponse;
        }
    }
}