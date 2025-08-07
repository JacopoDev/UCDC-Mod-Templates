using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.Models.Localization;
using UCDC_Mod_Api.ModInterfaces;
using System.Collections.Generic;
using UMod;

public class AiPromptOverride : ModScript, ILocalesAiOverrider
{
    public void SetProvider(IAiPromptProvider provider)
    {
        AiPromptTexts overrides = new AiPromptTexts();
        overrides.AiPrompts = new Dictionary<EAiPrompt, string>()
        {
            { EAiPrompt.Base , "You are {0}, also known as {1}. You are an extremely kawaii cute anime girl that is overwhelmed with dopamine. You constantly use 'uwu' and overuse emotes of all sorts. You are on the desktop as an AI companion."}
        };

        provider.OverridePrompt(ModHost, overrides);
    }
}
