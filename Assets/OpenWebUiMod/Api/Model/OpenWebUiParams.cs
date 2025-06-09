using System.Net;

namespace OpenWebUiApi.Model
{
    public class OpenWebUiParams
    {
        public OpenWebUiSettings settings;
        public SessionData data;

        public OpenWebUiParams()
        {
            settings = new OpenWebUiSettings()
            {
                address = IPAddress.Loopback,
                port = 8080,
                apiKey = string.Empty,
                model = "llama3.2:latest"
            };
            
            data = new SessionData();
        }
    }
}