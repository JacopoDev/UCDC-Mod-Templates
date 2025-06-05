using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace ChatGptMod
{
    public class GptSettings : ModScript, ISettingsAccessor 
    {
        public static GptSettings Instance;
        
        public static readonly Dictionary<EGptSettings, string> SettingsKeys = new Dictionary<EGptSettings, string>()
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

        private static Dictionary<string, object> _storedSettings;
        private static ISettingsDatabase _database;

        public static Dictionary<string, object> GetAllSettings()
        {
            return _storedSettings;
        }

        public static void SaveAllSettings()
        {
            Dictionary<string, object> excludedSpecial = new Dictionary<string, object>();

            foreach (var pair in _storedSettings)
            {
                if (pair.Key is "GptMod.Api" or "GptMod.Stop")
                {
                    continue;
                }
                excludedSpecial.Add(pair.Key, pair.Value);
            }
            
            _database.SaveGroupData(excludedSpecial);
            Instance.SetApi((string)_storedSettings["GptMod.Api"]);
            Instance.SetStopStrings((string[])_storedSettings["GptMod.Stop"]);
        }
        
        public void SetDatabase(ISettingsDatabase database)
        {
            _database = database;
            Instance = this;
            if (!CheckSetDefault())
            {
                LoadAllSettings();
            }
        }

        public void RestoreDefaultSettings()
        {
            string savedApi = GetApiDecoded();
            
            _storedSettings = new Dictionary<string, object>();
            _storedSettings.Add(SettingsKeys[EGptSettings.Api], savedApi);
            _storedSettings.Add(SettingsKeys[EGptSettings.Model], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.Temperature], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.MaxTokens], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.TopP], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.FrequencyPenalty], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.PresencePenalty], 2.0f);
            _storedSettings.Add(SettingsKeys[EGptSettings.Stop], new string[]{"Assistant:", " Unity-chan: "});
            _database.SaveGroupData(_storedSettings);
        }

        private bool CheckSetDefault()
        {
            if (_database.Exists(SettingsKeys[EGptSettings.Model])) return false;
            
            _storedSettings = new Dictionary<string, object>();
            _storedSettings.Add(SettingsKeys[EGptSettings.Api], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.Model], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.Temperature], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.MaxTokens], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.TopP], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.FrequencyPenalty], string.Empty);
            _storedSettings.Add(SettingsKeys[EGptSettings.PresencePenalty], 2.0f);
            _storedSettings.Add(SettingsKeys[EGptSettings.Stop], new string[]{"Assistant:", " Unity-chan: "});
            _database.SaveGroupData(_storedSettings);
            return true;
        }

        private void LoadAllSettings()
        {
            _storedSettings = _database.LoadGroupData(SettingsKeys.Values.ToArray());
            _storedSettings[SettingsKeys[EGptSettings.Api]] = GetApiDecoded();
            _storedSettings[SettingsKeys[EGptSettings.Stop]] = GetStopStrings();
        }

        #region setters
        // setting API key to settings file, encoding it for safety
        public void SetApi(string decodedValue)
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            
            string encoded = XorEncoder.Encode(decodedValue, key);
            _database.SaveString(SettingsKeys[EGptSettings.Api], encoded);
        }

        public void SetInt(EGptSettings setting, int value)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            _database.SaveInt(key, value);
        }

        public void SetFloat(EGptSettings setting, float value)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            _database.SaveFloat(key, value);
        }

        public void SetBool(EGptSettings setting, bool value)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            _database.SaveBool(key, value);
        }

        public void SetString(EGptSettings setting, string value)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            _database.SaveString(key, value);
        }
        
        public void SetStopStrings(string[] values)
        {
            string encoded = string.Join("|", values);
            _database.SaveString(SettingsKeys[EGptSettings.Stop], encoded);
        }

        #endregion

        #region getters

        // setting API key to settings file, needs to be decoded before used
        public string GetApiDecoded()
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            string encodedValue = _database.LoadString(SettingsKeys[EGptSettings.Api], string.Empty);

            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public int GetInt(EGptSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public float GetFloat(EGptSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public bool GetBool(EGptSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public string GetString(EGptSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        public string[] GetStopStrings()
        {
            string raw = _database.LoadString(SettingsKeys[EGptSettings.Stop], string.Empty);
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
