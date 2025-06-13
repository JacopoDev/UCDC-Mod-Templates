using System.Net;
using System.Threading.Tasks;
using GoogleCloudVoiceMod.Api.Data;
using UCDC_Mod_Api.Models;

namespace GoogleCloudVoiceMod.Api
{
    public class GoogleApi
    {
        private GoogleVoiceGenerator _voiceGen;
        private const int Timeout = 60;
        private int _countDown;
    
        public GoogleApi(GoogleVoiceGenerator mainModule)
        {
            _voiceGen = mainModule;
        }

        public async Task<VoiceResult> SendPrompt(DataToSend data)
        {
            VoiceResult result = new VoiceResult()
            {
                Code = (int)HttpStatusCode.BadRequest,
                ErrorMessage = "Script Error",
                Voice = null
            };
            
            await TextToSpeech.Instance.GetSpeech(data, clip =>
                {
                    result.Code = (int)HttpStatusCode.OK;
                    result.ErrorMessage = string.Empty;
                    result.Voice = clip;

                },
                error =>
                {
                    result.Code = error.error.code;
                    result.ErrorMessage = error.error.message;
                    result.Voice = null;
                });
        
            return result;
        }
    }
}
