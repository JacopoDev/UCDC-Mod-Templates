using System;
using UnityEngine;

namespace ElevenLabsMod.Api.Model
{
    public class ElevenLabsData
    {
        public string prompt;
        public string voice;
        public string apikey;
        public float stability;
        public float similarityBoost;
        public string model;
        public string format;
        public Action<AudioClip> audioClipReceived;
        public Action<BadRequestData> errorReceived;
    }
}