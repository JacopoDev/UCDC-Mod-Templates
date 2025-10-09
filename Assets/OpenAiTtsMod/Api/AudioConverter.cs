using System;
using System.Threading.Tasks;
using OpenAiTtsMod.Utility;
using UnityEngine;

namespace OpenAiTtsMod.Api
{
    public class AudioConverter
    {
         public async Task<AudioClip> ConvertWavBufferToClip(byte[] wavBytes, Action<AudioClip> onClipReady)
        {
            try
            {
                var clip = await ParseWav(wavBytes);
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

         public static async Task<AudioClip> ParseWav(byte[] data, string clipName = "wav_clip")
         {
             int channels;
             int sampleRate;
             float[] samples = ToFloatArray(data, out channels, out sampleRate);
             
             int sampleCount = samples.Length / channels;
             AudioClip clip = await MainThreadDispatcher.EnqueueAsync(() =>
             {
                 AudioClip clip = AudioClip.Create(clipName, sampleCount, channels, sampleRate, false);
                 clip.SetData(samples, 0);
                 return clip;
             });
             
             return clip;
         }

         public static float[] ToFloatArray(byte[] wavData, out int channels, out int sampleRate)
        {
            // WAV header parsing
            // [0-3] "RIFF"
            // [22-23] channels (ushort)
            // [24-27] sample rate (uint)
            // [34-35] bits per sample (ushort)
            // [44...] data
            channels = BitConverter.ToUInt16(wavData, 22);
            sampleRate = BitConverter.ToInt32(wavData, 24);
            int bitsPerSample = BitConverter.ToUInt16(wavData, 34);
            int dataStart = 44; // assuming PCM header without extra chunks

            if (bitsPerSample != 16)
                throw new Exception($"Unsupported WAV bit depth: {bitsPerSample}");

            int sampleCount = (wavData.Length - dataStart) / 2;
            float[] samples = new float[sampleCount];

            int offset = 0;
            for (int i = dataStart; i < wavData.Length; i += 2)
            {
                short sample16 = BitConverter.ToInt16(wavData, i);
                samples[offset++] = sample16 / 32768f; // normalize to [-1, 1]
            }

            return samples;
        }

        private static float[] ConvertPCMToFloats(byte[] pcm, int bitsPerSample)
        {
            int bytesPerSample = bitsPerSample / 8;
            int sampleCount = pcm.Length / bytesPerSample;
            float[] result = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                int index = i * bytesPerSample;
                short sample = (short)(pcm[index] | (pcm[index + 1] << 8));
                result[i] = sample / 32768f;
            }

            return result;
        }
    }
}