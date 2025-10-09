using System;
using System.Threading.Tasks;
using UnityEngine;
using AudioConverter = OpenAiTtsMod.Api.AudioConverter;

namespace OpenAiTtsMod
{
    public class TextToSpeech : MonoBehaviour
    {
        private static AudioConverter _audioConverter;
        public static TextToSpeech Instance;
        
        private Action<string> _actionRequestReceived;
        private Action<AudioClip> _audioClipReceived;

        private void Awake()
        {
            Instance = this;
            _audioConverter = new AudioConverter();
            DontDestroyOnLoad(gameObject);
        }

        public AudioClip ConvertSound(byte[] wavBytes, Action<AudioClip> audioClipReceived)
        {
            AudioClip result = _audioConverter.ConvertWavBufferToClip(wavBytes, audioClipReceived).Result;
            return result;
        }
    }
}