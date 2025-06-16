namespace ElevenLabsMod.Api.Model
{
    public struct ElevenLabsResult
    {
        public int Code;
        public BadRequestData Error;
        public byte[] AudioFile;
    }
}