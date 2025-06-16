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
            var reader = new ByteReader(data);

            if (reader.ReadString(4) != "RIFF") throw new Exception("Missing RIFF");
            reader.ReadInt32(); // Chunk size
            if (reader.ReadString(4) != "WAVE") throw new Exception("Missing WAVE");

            if (reader.ReadString(4) != "fmt ") throw new Exception("Missing fmt ");
            int subChunk1Size = reader.ReadInt32();
            short audioFormat = reader.ReadInt16();
            short numChannels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            reader.ReadInt32(); // byteRate
            reader.ReadInt16(); // blockAlign
            short bitsPerSample = reader.ReadInt16();

            if (subChunk1Size > 16)
                reader.Skip(subChunk1Size - 16); // Skip any extra data in fmt chunk

            // Skip until we find the "data" chunk
            while (reader.ReadString(4) != "data")
            {
                int chunkSize = reader.ReadInt32();
                reader.Skip(chunkSize);
            }

            int dataSize = reader.ReadInt32();
            byte[] pcmBytes = reader.ReadBytes(dataSize);
            float[] samples = ConvertPCMToFloats(pcmBytes, bitsPerSample);

            int sampleCount = samples.Length / numChannels;
            AudioClip clip = await MainThreadDispatcher.EnqueueAsync(() =>
            {
                AudioClip clip = AudioClip.Create(clipName, sampleCount, numChannels, sampleRate, false);
                clip.SetData(samples, 0);
                return clip;
            });
            
            return clip;
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