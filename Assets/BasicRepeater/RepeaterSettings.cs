using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace Repeater
{
    public class RepeaterSettings : ModScript, ISettingsAccessor
    {
        private static ISettingsDatabase _settings;

        private const string PhraseSettingKey = "Repeater.Phrase";
        public static string Phrase;
        
        public void SetDatabase(ISettingsDatabase database)
        {
            _settings = database;

            if (_settings.Exists(PhraseSettingKey))
            {
                Phrase = _settings.LoadString(PhraseSettingKey, "I am the smortest companion!");
            }
            else
            {
                Phrase = "I am the smortest companion!";
                _settings.SaveString(PhraseSettingKey, Phrase);
            }
        }

        public static void SaveSettings()
        {
            _settings.SaveString(PhraseSettingKey, Phrase);
        }
    }
}