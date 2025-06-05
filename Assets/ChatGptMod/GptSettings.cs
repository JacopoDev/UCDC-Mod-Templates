using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace ChatGptMod
{
    public class GptSettings : ModScript, ISettingsAccessor 
    {
        public static GptSettings Instance;
        
        private readonly Dictionary<EGptSettings, string> _settingsKeys = new Dictionary<EGptSettings, string>()
        {
            {EGptSettings.Api, "GptMod.Api"},
            {EGptSettings.Model, "GptMod.Model"},
            {EGptSettings.Temperature, "GptMod.Temperature"},
            {EGptSettings.MaxTokens, "GptMod.MaxTokens"},
            {EGptSettings.TopP, "GptMod.TopP"},
            {EGptSettings.FrequencyPenalty, "GptMod.FrequencyPenalty"},
            {EGptSettings.PresencePenalty, "GptMod.PresencePenalty"},
            {EGptSettings.Stop, "GptMod.Stop"},
        };

        private ISettingsDatabase _database;
        
        public void SetDatabase(ISettingsDatabase database)
        {
            _database = database;
            Instance = this;
            if (!CheckSetDefault());
        }

        public void RestoreDefaultSettings()
        {
            string savedApi = GetApiDecoded();
            
            SetApi(string.Empty);
            SetString(EGptSettings.Model, savedApi);
            SetFloat(EGptSettings.Temperature, 0.9f);
            SetInt(EGptSettings.MaxTokens, 1000);
            SetFloat(EGptSettings.TopP, 1.0f);
            SetFloat(EGptSettings.FrequencyPenalty, 2.0f);
            SetFloat(EGptSettings.PresencePenalty, 2.0f);
            SetStopStrings(new []{"Assistant:", " Unity-chan: "});
        }

        private bool CheckSetDefault()
        {
            if (_database.Exists(_settingsKeys[EGptSettings.Model])) return false;
            
            SetApi(string.Empty);
            SetString(EGptSettings.Model, "gpt-4o-mini");
            SetFloat(EGptSettings.Temperature, 0.9f);
            SetInt(EGptSettings.MaxTokens, 1000);
            SetFloat(EGptSettings.TopP, 1.0f);
            SetFloat(EGptSettings.FrequencyPenalty, 2.0f);
            SetFloat(EGptSettings.PresencePenalty, 2.0f);
            SetStopStrings(new []{"Assistant:", " Unity-chan: "});
            return true;
        }

        #region setters
        // setting API key to settings file, encoding it for safety
        public void SetApi(string decodedValue)
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            
            string encoded = XorEncoder.Encode(decodedValue, key);
            _database.SaveString(_settingsKeys[EGptSettings.Api], encoded);
        }

        public void SetInt(EGptSettings setting, int value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveInt(key, value);
        }

        public void SetFloat(EGptSettings setting, float value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveFloat(key, value);
        }

        public void SetBool(EGptSettings setting, bool value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveBool(key, value);
        }

        public void SetString(EGptSettings setting, string value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveString(key, value);
        }
        
        public void SetStopStrings(string[] values)
        {
            string encoded = string.Join("|", values);
            _database.SaveString(_settingsKeys[EGptSettings.Stop], encoded);
        }

        #endregion

        #region getters

        // setting API key to settings file, needs to be decoded before used
        public string GetApiDecoded()
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            string encodedValue = _database.LoadString(_settingsKeys[EGptSettings.Api], string.Empty);

            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public int GetInt(EGptSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public float GetFloat(EGptSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public bool GetBool(EGptSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public string GetString(EGptSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        public string[] GetStopStrings()
        {
            string raw = _database.LoadString(_settingsKeys[EGptSettings.Stop], string.Empty);
            return string.IsNullOrEmpty(raw)
                ? Array.Empty<string>()
                : raw.Split('|');
        }

        #endregion

        private void ValidateSetting(EGptSettings setting)
        {
            if (setting == EGptSettings.Api)
            {
                throw new ArgumentException(
                    "Don't get/set Api via regular method - Use SetApi(string decodedValue) or GetApiDecoded instead!");
            }

            if (setting == EGptSettings.Stop)
            {
                throw new ArgumentException(
                    "Stop parameter uses different get/set method - use SetStopStrings or GetStopStrings instead!");
            }
        }
    }
}
