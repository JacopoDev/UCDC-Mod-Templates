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
        public static IAiApiDatabase AIDatabase;

        private WebUiApi _api;


        public void SetDatabase(IAiApiDatabase database)
        {
            AIDatabase = database;
            MainModule = this;
            _api = new WebUiApi(this);
        }

        public int GenerateMessage(ITextAiProcessor aiProcessor, Action<Result> finishedAction)
        {
            int result = SendWebUiRequest(aiProcessor, finishedAction).Result;
            return result;
        }

        private async Task<int> SendWebUiRequest(ITextAiProcessor aiProcessor, Action<Result> finishedAction)
        {
            Result result = await _api.SendPrompt(aiProcessor.GetChat().Messages);
            finishedAction.Invoke(result);
            return result.Code;
        }
    }
}
