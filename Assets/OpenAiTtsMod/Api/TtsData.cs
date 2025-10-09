using System;

namespace OpenAiTtsMod.Api
{
    [Serializable]
    public class TtsData
    {
        public string model;
        public string voice;
        public string input;
        public string response_format;
    }
}