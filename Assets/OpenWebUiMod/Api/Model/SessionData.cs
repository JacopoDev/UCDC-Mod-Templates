using System.Collections.Generic;
using OpenWebUiMod.Api;

namespace OpenWebUiApi.Model
{
    public class SessionData
    {
        public string model;
        public MessageWebUi[] messages;

        public SessionData()
        {
            model = "llama3.2:latest";
        }

        public void SetMessages(List<MessageWebUi> msgs)
        {
            messages = msgs.ToArray();
        }
    }
}