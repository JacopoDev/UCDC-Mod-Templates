using System.Net;
using System.Threading.Tasks;
using GoogleCloudVoiceMod.Api.Data;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.VoiceGen;

namespace GoogleCloudVoiceMod.Api
{
    public class GoogleApi
    {
        private int _countDown;

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
