using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OpenWebUiApi.Model;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.TextGen;
using UnityEngine;

namespace OpenWebUiMod.Api
{
    public class WebUiApi
    {
        private WebUiMessageSender _sender;
        private OpenWebUiParams _settings;
        private readonly WebUiConnection _connection;

        public WebUiApi(WebUiMessageSender sender)
        {
            _sender = sender;
            _connection = new WebUiConnection();
            _settings = new OpenWebUiParams();
        }

        public async Task<TextResult> SendPrompt(List<Message> messages)
        {
            TextResult returnStatus = new TextResult();
            returnStatus.Code = (int)HttpStatusCode.NotFound;
            
            _settings.settings.apiKey = WebUiSettings.Instance.GetApiDecoded();

            if (_settings.settings.apiKey == string.Empty)
            {
                returnStatus.Code = (int)HttpStatusCode.BadRequest;
                returnStatus.ErrorMessage = "OpenWebUI api key is not set.";
                return returnStatus;
            }

            List<MessageWebUi> openWebMessages = new();
            foreach (var mes in messages)
            {
                openWebMessages.Add(new MessageWebUi()
                {
                    Content = mes.content,
                    Role = mes.role
                });
            }

            if (!IPAddress.TryParse(WebUiSettings.Instance.GetString(EWebUiSettings.Ip, IPAddress.Loopback.ToString()),
                    out _settings.settings.address))
            {
                _settings.settings.address = IPAddress.Loopback;
            }

            _settings.settings.model = WebUiSettings.Instance.GetString(EWebUiSettings.Model, "llama3.2:latest");
            _settings.settings.port = WebUiSettings.Instance.GetInt(EWebUiSettings.Port, 8080);

            Response result = await _connection.SendRequestAsync(openWebMessages, _settings);
            
            if (result == null)
            {
                returnStatus.Code = (int)HttpStatusCode.BadRequest;
                returnStatus.ErrorMessage = $"Error: {_connection.LastMessageString}";
                return returnStatus;
            }

            if (result.Choices == null)
            {
                returnStatus.Code = (int)HttpStatusCode.BadRequest;
                returnStatus.ErrorMessage = $"Error: {_connection.LastMessageString}";
                return returnStatus;
            }
                
            returnStatus.Code = (int)HttpStatusCode.OK;
            returnStatus.Message = result.Choices[0].Message;
                
            return returnStatus;
        }
    }
}