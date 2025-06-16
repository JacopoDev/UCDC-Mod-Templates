using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWebUiMod.Api;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace OpenWebUiMod
{
    public class WebUiMessageSender : ModScript, ITextAiAccessor
    {
        public static ITextAiAccessor MainModule;
        public static IAiApiProvider AIDatabase;

        private WebUiApi _api;


        public void SetProvider(IAiApiProvider database)
        {
            AIDatabase = database;
            MainModule = this;
            _api = new WebUiApi(this);
        }

        public int GenerateMessage(IChatProvider aiProcessor, Action<TextResult> finishedAction)
        {
            int result = SendWebUiRequest(aiProcessor, finishedAction).Result;
            return result;
        }

        private async Task<int> SendWebUiRequest(IChatProvider aiProcessor, Action<TextResult> finishedAction)
        {
            TextResult result = await _api.SendPrompt(aiProcessor.GetChat().Messages);
            finishedAction.Invoke(result);
            return result.Code;
        }
    }
}
