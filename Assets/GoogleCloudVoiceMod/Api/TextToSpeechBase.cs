using System;
using GoogleCloudVoiceMod.Api.Data;
using UnityEngine;

namespace GoogleCloudVoiceMod.Api
{
    public abstract class TextToSpeechBase: MonoBehaviour
    {
        protected static AudioConverter _audioConverter;
        public abstract void GetSpeech(string textToConvert, Action<AudioClip> audioClipReceived,
            Action<BadRequestData> errorReceived);

        protected void RequestReceived(string requestData, Action<AudioClip> audioClipReceived)
        {
            var audioData = JsonUtility.FromJson<AudioData>(requestData);
            byte[] wavBytes = Convert.FromBase64String(audioData.audioContent);

            _audioConverter.ConvertWavBufferToClip(wavBytes, audioClipReceived);
        }
    }
}