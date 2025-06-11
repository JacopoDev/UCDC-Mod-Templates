using System;
using GoogleCloudVoiceMod.Api.Data;
using UnityEngine;
using Input = GoogleCloudVoiceMod.Api.Data.Input;

namespace GoogleCloudVoiceMod.Api
{
    public class TextToSpeech: TextToSpeechBase
    {
        public static TextToSpeech Instance;
        
        private Action<string> _actionRequestReceived;
        private Action<BadRequestData> _errorReceived;
        private Action<AudioClip> _audioClipReceived;

        private RequestService _requestService;
        public bool isBusy;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void GetSpeech(string textToConvert, Action<AudioClip> audioClipReceived,  Action<BadRequestData> errorReceived)
        {
            isBusy = true;
            _actionRequestReceived += (requestData => RequestReceived(requestData, audioClipReceived));

            if (_requestService == null)
                _requestService = gameObject.AddComponent<RequestService>();

            if (_audioConverter == null)
                _audioConverter = gameObject.AddComponent<AudioConverter>();

            var dataToSend = new DataToSend
            {
                input =
                    new Input()
                    {
                        text = textToConvert
                    },
                voice =
                    new Voice()
                    {
                        languageCode = GoogleSettings.Instance.GetString(EGoogleSettings.LanguageCode,
                            (string)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.LanguageCode)),
                        name = GoogleSettings.Instance.GetString(EGoogleSettings.LanguageCode,
                            (string)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.LanguageCode))
                    },
                audioConfig =
                    new AudioConfig()
                    {
                        audioEncoding = "LINEAR16",
                        pitch = GoogleSettings.Instance.GetFloat(EGoogleSettings.Pitch,
                            (float)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.Pitch)),
                        speakingRate = GoogleSettings.Instance.GetFloat(EGoogleSettings.Speed,
                            (float)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.Speed))
                    }
            };

            RequestService.instance.SendDataToGoogle("https://texttospeech.googleapis.com/v1/text:synthesize", dataToSend,
                GoogleSettings.Instance.GetApiDecoded(),
                _actionRequestReceived, errorReceived);

            _actionRequestReceived = null;
            isBusy = false;
        }

    }
}