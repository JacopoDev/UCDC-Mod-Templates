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
        public static readonly Dictionary<EGptSettings, string> SettingsKeys = new()
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
            Dictionary<string, object> excludedSpecial = _storedSettings
                .Where(kv => kv.Key != "GptMod.Api" && kv.Key != "GptMod.Model")
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            _database.SaveGroupData(excludedSpecial);
            SetApi((string)_storedSettings["GptMod.Api"]);
            SetStopStrings((string[])_storedSettings["GptMod.Stop"]);
        }
        
        public void SetDatabase(ISettingsDatabase database)
        {
            _database = database;
            if (!CheckSetDefault())
            {
                _database.SaveGroupData(_storedSettings);
                LoadAllSettings();
            }
        }

        private bool CheckSetDefault()
        {
            if (_database.Exists(SettingsKeys[EGptSettings.Model])) return false;

            _storedSettings = new Dictionary<string, object>
            {
                { SettingsKeys[EGptSettings.Api], string.Empty },
                { SettingsKeys[EGptSettings.Model], string.Empty },
                { SettingsKeys[EGptSettings.Temperature], string.Empty },
                { SettingsKeys[EGptSettings.MaxTokens], string.Empty },
                { SettingsKeys[EGptSettings.TopP], string.Empty },
                { SettingsKeys[EGptSettings.FrequencyPenalty], string.Empty },
                { SettingsKeys[EGptSettings.PresencePenalty], 2.0f },
                { SettingsKeys[EGptSettings.Stop], new string[]{"Assistant:", " Unity-chan: "} }
            };
            return true;
        }

        public static void ResetDefaults()
        {
            string storedApi = (string)_storedSettings[SettingsKeys[EGptSettings.Api]];
            
            _storedSettings = new Dictionary<string, object>
            {
                { SettingsKeys[EGptSettings.Api], storedApi },
                { SettingsKeys[EGptSettings.Model], string.Empty },
                { SettingsKeys[EGptSettings.Temperature], string.Empty },
                { SettingsKeys[EGptSettings.MaxTokens], string.Empty },
                { SettingsKeys[EGptSettings.TopP], string.Empty },
                { SettingsKeys[EGptSettings.FrequencyPenalty], string.Empty },
                { SettingsKeys[EGptSettings.PresencePenalty], 2.0f },
                { SettingsKeys[EGptSettings.Stop], new string[]{"Assistant:", " Unity-chan: "} }
            };
            _database.SaveGroupData(_storedSettings);
        }

        private void LoadAllSettings()
        {
            _storedSettings = _database.LoadGroupData(SettingsKeys.Values.ToArray());
            _storedSettings[SettingsKeys[EGptSettings.Api]] = GetApiDecoded();
            _storedSettings[SettingsKeys[EGptSettings.Stop]] = GetStopStrings();
        }

        #region setters
        // setting API key to settings file, encoding it for safety
        public static void SetApi(string decodedValue)
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            
            string encoded = XorEncoder.Encode(decodedValue, key);
            _database.SaveString(SettingsKeys[EGptSettings.Api], encoded);
        }
        
        public static void SetStopStrings(string[] values)
        {
            string encoded = string.Join("|", values);
            _database.SaveString(SettingsKeys[EGptSettings.Stop], encoded);
        }

        #endregion

        #region getters

        // setting API key to settings file, needs to be decoded before used
        public static string GetApiDecoded()
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            string encodedValue = _database.LoadString(SettingsKeys[EGptSettings.Api], string.Empty);

            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public static int GetInt(EGptSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public static float GetFloat(EGptSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public static bool GetBool(EGptSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public static string GetString(EGptSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = SettingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        public static string[] GetStopStrings()
        {
            string raw = _database.LoadString(SettingsKeys[EGptSettings.Stop], string.Empty);
            return string.IsNullOrEmpty(raw)
                ? Array.Empty<string>()
                : raw.Split('|');
        }

        #endregion

        private static void ValidateSetting(EGptSettings setting)
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
