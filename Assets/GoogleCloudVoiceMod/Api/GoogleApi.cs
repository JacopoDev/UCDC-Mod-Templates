using System.Net;
using System.Threading.Tasks;
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

        public async Task<VoiceResult> SendPrompt(string text)
        {
            VoiceResult result = new VoiceResult()
            {
                Code = (int)HttpStatusCode.BadRequest,
                ErrorMessage = "Script Error",
                Voice = null
            };
            
            TextToSpeech.Instance.GetSpeech(text, clip =>
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

            _countDown = Timeout;
            while (_countDown > 0 && TextToSpeech.Instance.isBusy)
            {
                _countDown--;
                await Task.Delay(1000);
            }
        
            return result;
        }
    }
}
