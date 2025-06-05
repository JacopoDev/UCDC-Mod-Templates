using System;
using System.Net;
using System.Threading.Tasks;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace Repeater
{
    public class Repeater : ModScript, ITextAiAccessor
    {
        public static IAiApiDatabase AIApiDatabase;
        public static ITextAiAccessor MainModule;
    
        // game will call it to provide its API database to the mod
        public void SetDatabase(IAiApiDatabase database)
        {
            AIApiDatabase = database;
            MainModule = this;
        }
        
        public int GenerateMessage(ITextAiProcessor aiProcessor, Action<Result> finishedAction)
        {
            int result = SendAfterTime(finishedAction).Result;
            return result;
        }
    
        private async Task<int> SendAfterTime(Action<Result> finishedAction)
        {
            await Task.Delay(500); // Waits for 0.5 seconds - emulate thinking
        
            Result result = new Result()
            {
                Code = (int)HttpStatusCode.OK, 
                Message = new Message()
                {
                    role = "assistant", // possible values - system, assistant, user
                    content = RepeaterSettings.Phrase
                }
            };
        
            finishedAction.Invoke(result);
            return result.Code;
        }
    }
}
