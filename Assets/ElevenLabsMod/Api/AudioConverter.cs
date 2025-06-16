using System;
using System.Threading.Tasks;
using ElevenLabsMod.Utility;
using UnityEngine;

namespace ElevenLabsMod.Api
{
    public class AudioConverter
    {
        public async Task<AudioClip> ConvertWavBufferToClip(byte[] wavBytes, Action<AudioClip> onClipReady)
        {
            try
            {
                string format = ElevenSettings.Instance.GetString(EElevenSettings.Format,
                    (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Format));
                
                int sampleRate = format switch
                {
                    "pcm_8000" => 8000,
                    "pcm_16000" => 16000,
                    "pcm_22050" => 22050,
                    "pcm_24000" => 24000,
                    "pcm_44100" => 44100,
                    "pcm_48000" => 48000,
                    _ => throw new ArgumentException($"Unknown format: {format}")
                };
                
                var clip = await ParseWav(wavBytes, sampleRate);
                onClipReady?.Invoke(clip);
                return clip;
            }
            catch (Exception ex)
            {
                Debug.LogError("WAV parsing failed: " + ex.Message);
                return null;
            }
            
            return null;
        }
        
        public static async Task<AudioClip> ParseWav(byte[] data, int sampleRate = 22050, string clipName = "wav_clip")
        {
            float[] samples = ConvertPCMToFloats(data);
            
            AudioClip clip = await MainThreadDispatcher.EnqueueAsync(() =>
            {
                AudioClip clip = AudioClip.Create(clipName, samples.Length, 1, sampleRate, false);
                clip.SetData(samples, 0);
                return clip;
            });
            
            return clip;
        }

        private static float[] ConvertPCMToFloats(byte[] pcm)
        {
            int sampleCount = pcm.Length / 2;
            float[] floatSamples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = (short)(pcm[i * 2] | (pcm[i * 2 + 1] << 8));
                floatSamples[i] = sample / 32768f; // normalize to -1.0f to 1.0f
            }

            return floatSamples;
        }
    }
}