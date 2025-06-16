using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ElevenLabsMod.Utility;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace ElevenLabsMod
{
    public class ElevenSettings : ModScript, ISettingsAccessor
    {
        public static ElevenSettings Instance;
        
        private readonly Dictionary<EElevenSettings, string> _settingsKeys = new Dictionary<EElevenSettings, string>()
        {
            {EElevenSettings.Api, "ElevenLabs.Api"},
            {EElevenSettings.VoiceID, "ElevenLabs.VoiceId"},
            {EElevenSettings.Stability, "ElevenLabs.Stability"},
            {EElevenSettings.SimilarityBoost, "ElevenLabs.SimilarityBoost"},
            {EElevenSettings.Model, "ElevenLabs.Model"},
            {EElevenSettings.Format, "ElevenLabs.Format"},
        };
        
        private readonly Dictionary<EElevenSettings, object> _settingDefaults = new Dictionary<EElevenSettings, object>()
        {
            {EElevenSettings.Api, string.Empty},
            {EElevenSettings.VoiceID, string.Empty},
            {EElevenSettings.Stability, 0.5f},
            {EElevenSettings.SimilarityBoost, 0.5f},
            {EElevenSettings.Model, "eleven_flash_v2_5"},
            {EElevenSettings.Format, "pcm_22050"},
        };

        private ISettingsProvider _database;
        private Dictionary<string, object> _loadedSettings;

        public void SetProvider(ISettingsProvider database)
        {
            _database = database;
            Instance = this;
            if (IsNeedToInitDefaults()) return;
            
            _loadedSettings = _database.LoadGroupData(_settingsKeys.Values.ToArray());
            _loadedSettings[_settingsKeys[EElevenSettings.Api]] = GetApiDecoded();
        }

        public void SaveAllData()
        {
            var filtered = _loadedSettings
                .Where(kv => kv.Key != _settingsKeys[EElevenSettings.Api])
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            _database.SaveGroupData(filtered);
            SetApi((string)_loadedSettings[_settingsKeys[EElevenSettings.Api]]);
        }

        public void SetLoaded(EElevenSettings setting, object data)
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

        public object GetDefaultValue(EElevenSettings key)
        {
            return _settingDefaults[key];
        }


        public void RestoreDefaultSettings()
        {
            string savedApi = GetApiDecoded();

            SetLoaded(EElevenSettings.Api, savedApi);
            SetLoaded(EElevenSettings.VoiceID, _settingDefaults[EElevenSettings.VoiceID]);
            SetLoaded(EElevenSettings.Stability, _settingDefaults[EElevenSettings.Stability]);
            SetLoaded(EElevenSettings.SimilarityBoost, _settingDefaults[EElevenSettings.SimilarityBoost]);
            
            
            SaveAllData();
        }

        private bool IsNeedToInitDefaults()
        {
            if (_database.Exists(_settingsKeys[EElevenSettings.Stability])) return false;
            _loadedSettings = new Dictionary<string, object>();
            SetLoaded(EElevenSettings.Api, string.Empty);
            SetLoaded(EElevenSettings.VoiceID, _settingDefaults[EElevenSettings.VoiceID]);
            SetLoaded(EElevenSettings.Stability, _settingDefaults[EElevenSettings.Stability]);
            SetLoaded(EElevenSettings.SimilarityBoost, _settingDefaults[EElevenSettings.SimilarityBoost]);
            
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
            _database.SaveString(_settingsKeys[EElevenSettings.Api], encoded);
        }

        public void SetInt(EElevenSettings setting, int value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveInt(key, value);
        }

        public void SetFloat(EElevenSettings setting, float value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveFloat(key, value);
        }

        public void SetBool(EElevenSettings setting, bool value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveBool(key, value);
        }

        public void SetString(EElevenSettings setting, string value)
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
            string encodedValue = _database.LoadString(_settingsKeys[EElevenSettings.Api], string.Empty);
            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public int GetInt(EElevenSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public float GetFloat(EElevenSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public bool GetBool(EElevenSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public string GetString(EElevenSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        #endregion

        private void ValidateSetting(EElevenSettings setting)
        {
            if (setting == EElevenSettings.Api)
            {
                throw new ArgumentException(
                    "Don't get/set Api via regular method - Use SetApi(string decodedValue) or GetApiDecoded instead!");
            }
        }
    }
}