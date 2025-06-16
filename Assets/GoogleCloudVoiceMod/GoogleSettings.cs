using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GoogleCloudVoiceMod.Utility;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace GoogleCloudVoiceMod
{
    public class GoogleSettings : ModScript, ISettingsAccessor 
    {
        public static GoogleSettings Instance;
        
        private readonly Dictionary<EGoogleSettings, string> _settingsKeys = new Dictionary<EGoogleSettings, string>()
        {
            {EGoogleSettings.Api, "GoogleCloud.Api"},
            {EGoogleSettings.LanguageCode, "GoogleCloud.LangCode"},
            {EGoogleSettings.VoiceName, "GoogleCloud.VoiceName"},
            {EGoogleSettings.Speed, "GoogleCloud.Speed"},
            {EGoogleSettings.Pitch, "GoogleCloud.Pitch"},
        };
        
        private readonly Dictionary<EGoogleSettings, object> _settingDefaults = new Dictionary<EGoogleSettings, object>()
        {
            {EGoogleSettings.Api, string.Empty},
            {EGoogleSettings.LanguageCode, "en-GB"},
            {EGoogleSettings.VoiceName, "en-GB-Wavenet-A"},
            {EGoogleSettings.Speed, 1f},
            {EGoogleSettings.Pitch, 0f},
        };

        private ISettingsProvider _database;
        private Dictionary<string, object> _loadedSettings;

        public void SetProvider(ISettingsProvider database)
        {
            _database = database;
            Instance = this;
            if (IsNeedToInitDefaults()) return;
            
            _loadedSettings = _database.LoadGroupData(_settingsKeys.Values.ToArray());
            _loadedSettings[_settingsKeys[EGoogleSettings.Api]] = GetApiDecoded();
        }

        public void SaveAllData()
        {
            var filtered = _loadedSettings
                .Where(kv => kv.Key != _settingsKeys[EGoogleSettings.Api])
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            _database.SaveGroupData(filtered);
            SetApi((string)_loadedSettings[_settingsKeys[EGoogleSettings.Api]]);
        }

        public void SetLoaded(EGoogleSettings setting, object data)
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

        public object GetDefaultValue(EGoogleSettings key)
        {
            return _settingDefaults[key];
        }


        public void RestoreDefaultSettings()
        {
            string savedApi = GetApiDecoded();

            SetLoaded(EGoogleSettings.Api, savedApi);
            SetLoaded(EGoogleSettings.LanguageCode, _settingDefaults[EGoogleSettings.LanguageCode]);
            SetLoaded(EGoogleSettings.VoiceName, _settingDefaults[EGoogleSettings.VoiceName]);
            SetLoaded(EGoogleSettings.Speed, _settingDefaults[EGoogleSettings.Speed]);
            SetLoaded(EGoogleSettings.Pitch, _settingDefaults[EGoogleSettings.Pitch]);
            
            SaveAllData();
        }

        private bool IsNeedToInitDefaults()
        {
            if (_database.Exists(_settingsKeys[EGoogleSettings.VoiceName])) return false;
            _loadedSettings = new Dictionary<string, object>();
            SetLoaded(EGoogleSettings.Api, string.Empty);
            SetLoaded(EGoogleSettings.LanguageCode, _settingDefaults[EGoogleSettings.LanguageCode]);
            SetLoaded(EGoogleSettings.VoiceName, _settingDefaults[EGoogleSettings.VoiceName]);
            SetLoaded(EGoogleSettings.Speed, _settingDefaults[EGoogleSettings.Speed]);
            SetLoaded(EGoogleSettings.Pitch, _settingDefaults[EGoogleSettings.Pitch]);
            
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
            _database.SaveString(_settingsKeys[EGoogleSettings.Api], encoded);
        }

        public void SetInt(EGoogleSettings setting, int value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveInt(key, value);
        }

        public void SetFloat(EGoogleSettings setting, float value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveFloat(key, value);
        }

        public void SetBool(EGoogleSettings setting, bool value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveBool(key, value);
        }

        public void SetString(EGoogleSettings setting, string value)
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
            string encodedValue = _database.LoadString(_settingsKeys[EGoogleSettings.Api], string.Empty);
            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public int GetInt(EGoogleSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public float GetFloat(EGoogleSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public bool GetBool(EGoogleSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public string GetString(EGoogleSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        #endregion

        private void ValidateSetting(EGoogleSettings setting)
        {
            if (setting == EGoogleSettings.Api)
            {
                throw new ArgumentException(
                    "Don't get/set Api via regular method - Use SetApi(string decodedValue) or GetApiDecoded instead!");
            }
        }
    }
}
