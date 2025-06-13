using System;
using System.Threading.Tasks;
using GoogleCloudVoiceMod.Api.Data;
using UnityEngine;
using Input = GoogleCloudVoiceMod.Api.Data.Input;

namespace GoogleCloudVoiceMod.Api
{
    public class TextToSpeech : MonoBehaviour
    {
        private static AudioConverter _audioConverter;
        public static TextToSpeech Instance;
        
        private Action<string> _actionRequestReceived;
        private Action<BadRequestData> _errorReceived;
        private Action<AudioClip> _audioClipReceived;

        private RequestService _requestService;
        public bool isBusy;

        private void Awake()
        {
            Instance = this;
            _audioConverter = new AudioConverter();
            _requestService = new RequestService();
            DontDestroyOnLoad(gameObject);
        }

        public async Task<int> GetSpeech(DataToSend data, Action<AudioClip> audioClipReceived,  Action<BadRequestData> errorReceived)
        {
            int code;
            _actionRequestReceived += (requestData => RequestReceived(requestData, audioClipReceived));
            errorReceived += (requestData =>
            {
                Debug.LogError(requestData.error.message);
            });

            code = await _requestService.SendDataToGoogle("https://texttospeech.googleapis.com/v1/text:synthesize", data,
                GoogleSettings.Instance.GetApiDecoded(),
                _actionRequestReceived, errorReceived);

            _actionRequestReceived = null;

            while (isBusy)
            {
                await Task.Delay(250);
            }
            errorReceived = null;
                
            return code;
        }

        protected async Task RequestReceived(string requestData, Action<AudioClip> audioClipReceived)
        {
            isBusy = true;
            var audioData = JsonUtility.FromJson<AudioData>(requestData);
            
            byte[] wavBytes = Convert.FromBase64String(audioData.audioContent);

            await _audioConverter.ConvertWavBufferToClip(wavBytes, audioClipReceived);
            isBusy = false;
        }

    }
}