using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using OpenAiTtsMod.Utility;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace OpenAiTtsMod
{
    public enum EOpenAiTtsSettings
    {
        Api,
        Model,
        Voice,
    };

    public class OpenAiTtsSettings : ModScript, ISettingsAccessor
    {
        public static OpenAiTtsSettings Instance;
        
        private readonly Dictionary<EOpenAiTtsSettings, string> _settingsKeys = new Dictionary<EOpenAiTtsSettings, string>()
        {
            {EOpenAiTtsSettings.Api, "OpenAiTts.Api"},
            {EOpenAiTtsSettings.Model, "OpenAiTts.Model"},
            {EOpenAiTtsSettings.Voice, "OpenAiTts.Voice"},
        };
        
        private readonly Dictionary<EOpenAiTtsSettings, object> _settingDefaults = new Dictionary<EOpenAiTtsSettings, object>()
        {
            {EOpenAiTtsSettings.Api, string.Empty},
            {EOpenAiTtsSettings.Model, "gpt-4o-mini-tts"},
            {EOpenAiTtsSettings.Voice, "nova"},
        };

        private ISettingsProvider _database;
        private Dictionary<string, object> _loadedSettings;
        public void SetProvider(ISettingsProvider provider)
        {
            _database = provider;
            Instance = this;
            if (IsNeedToInitDefaults()) return;
            
            _loadedSettings = _database.LoadGroupData(_settingsKeys.Values.ToArray());
            _loadedSettings[_settingsKeys[EOpenAiTtsSettings.Api]] = GetApiDecoded();
        }

        public void SaveAllData()
        {
            var filtered = _loadedSettings
                .Where(kv => kv.Key != _settingsKeys[EOpenAiTtsSettings.Api])
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            _database.SaveGroupData(filtered);
            SetApi((string)_loadedSettings[_settingsKeys[EOpenAiTtsSettings.Api]]);
        }

        public void SetLoaded(EOpenAiTtsSettings setting, object data)
        {
            if (!_loadedSettings.ContainsKey(_settingsKeys[setting]))
            {
                _loadedSettings.Add(_settingsKeys[setting], data);
            }
            else
            {
                _loadedSettings[_settingsKeys[setting]] = data;
            }
        }

        public object GetDefaultValue(EOpenAiTtsSettings key)
        {
            return _settingDefaults[key];
        }


        public void RestoreDefaultSettings()
        {
            string savedApi = GetApiDecoded();

            SetLoaded(EOpenAiTtsSettings.Api, savedApi);
            SetLoaded(EOpenAiTtsSettings.Model, _settingDefaults[EOpenAiTtsSettings.Model]);
            SetLoaded(EOpenAiTtsSettings.Voice, _settingDefaults[EOpenAiTtsSettings.Voice]);
            
            SaveAllData();
        }

        private bool IsNeedToInitDefaults()
        {
            if (_database.Exists(_settingsKeys[EOpenAiTtsSettings.Voice])) return false;
            _loadedSettings = new Dictionary<string, object>();
            SetLoaded(EOpenAiTtsSettings.Api, string.Empty);
            SetLoaded(EOpenAiTtsSettings.Model, _settingDefaults[EOpenAiTtsSettings.Model]);
            
            SaveAllData();
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
            _database.SaveString(_settingsKeys[EOpenAiTtsSettings.Api], encoded);
        }

        public void SetInt(EOpenAiTtsSettings setting, int value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveInt(key, value);
        }

        public void SetFloat(EOpenAiTtsSettings setting, float value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveFloat(key, value);
        }

        public void SetBool(EOpenAiTtsSettings setting, bool value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveBool(key, value);
        }

        public void SetString(EOpenAiTtsSettings setting, string value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveString(key, value);
        }

        #endregion
        
        #region getters

        // setting API key to settings file, needs to be decoded before used
        public string GetApiDecoded()
        {
            string baseSalt = Environment.MachineName + "_" + Environment.UserName;
            string key = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseSalt)));
            string encodedValue = _database.LoadString(_settingsKeys[EOpenAiTtsSettings.Api], string.Empty);
            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public int GetInt(EOpenAiTtsSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public float GetFloat(EOpenAiTtsSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public bool GetBool(EOpenAiTtsSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public string GetString(EOpenAiTtsSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        #endregion

        private void ValidateSetting(EOpenAiTtsSettings setting)
        {
            if (setting == EOpenAiTtsSettings.Api)
            {
                throw new ArgumentException(
                    "Don't get/set Api via regular method - Use SetApi(string decodedValue) or GetApiDecoded instead!");
            }
        }
    }
}