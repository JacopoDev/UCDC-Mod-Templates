using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.ModInterfaces;
using UMod;

namespace OpenWebUiMod
{
    public class WebUiSettings : ModScript, ISettingsAccessor
    {
        public static WebUiSettings Instance;
        
        private readonly Dictionary<EWebUiSettings, string> _settingsKeys = new Dictionary<EWebUiSettings, string>()
        {
            {EWebUiSettings.Api, "OpenWebUiMod.Api"},
            {EWebUiSettings.Model, "OpenWebUiMod.Model"},
            {EWebUiSettings.Ip, "OpenWebUiMod.Ip"},
            {EWebUiSettings.Port, "OpenWebUiMod.Port"},
        };
        
        private readonly Dictionary<EWebUiSettings, object> _settingDefaults = new Dictionary<EWebUiSettings, object>()
        {
            {EWebUiSettings.Api, string.Empty},
            {EWebUiSettings.Model, "llama3.2:latest"},
            {EWebUiSettings.Ip, IPAddress.Loopback.ToString()},
            {EWebUiSettings.Port, 8080},
        };

        private ISettingsProvider _database;
        private Dictionary<string, object> _loadedSettings;

        public void SetProvider(ISettingsProvider database)
        {
            _database = database;
            Instance = this;
            if (CheckSetDefault()) return;
            
            _loadedSettings = _database.LoadGroupData(_settingsKeys.Values.ToArray());
            _loadedSettings[_settingsKeys[EWebUiSettings.Api]] = GetApiDecoded();
        }

        public void SaveAllData()
        {
            var filtered = _loadedSettings
                .Where(kv => kv.Key != _settingsKeys[EWebUiSettings.Api])
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            SetApi((string)_loadedSettings[_settingsKeys[EWebUiSettings.Api]]);
            _database.SaveGroupData(filtered);
        }

        public void SetLoaded(EWebUiSettings setting, object data)
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

        public object GetDefaultValue(EWebUiSettings key)
        {
            return _settingDefaults[key];
        }

        public void RestoreDefaultSettings()
        {
            string savedApi = GetApiDecoded();

            SetLoaded(EWebUiSettings.Api, savedApi);
            SetLoaded(EWebUiSettings.Model, _settingDefaults[EWebUiSettings.Model]);
            SetLoaded(EWebUiSettings.Ip, _settingDefaults[EWebUiSettings.Ip]);
            SetLoaded(EWebUiSettings.Port, _settingDefaults[EWebUiSettings.Port]);
            
            SaveAllData();
        }

        private bool CheckSetDefault()
        {
            if (_database.Exists(_settingsKeys[EWebUiSettings.Model])) return false;
            _loadedSettings = new Dictionary<string, object>();

            SetLoaded(EWebUiSettings.Api, string.Empty);
            SetLoaded(EWebUiSettings.Model, _settingDefaults[EWebUiSettings.Model]);
            SetLoaded(EWebUiSettings.Ip, _settingDefaults[EWebUiSettings.Ip]);
            SetLoaded(EWebUiSettings.Port, _settingDefaults[EWebUiSettings.Port]);
            
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
            _database.SaveString(_settingsKeys[EWebUiSettings.Api], encoded);
        }

        public void SetInt(EWebUiSettings setting, int value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveInt(key, value);
        }

        public void SetFloat(EWebUiSettings setting, float value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveFloat(key, value);
        }

        public void SetBool(EWebUiSettings setting, bool value)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            _database.SaveBool(key, value);
        }

        public void SetString(EWebUiSettings setting, string value)
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
            string encodedValue = _database.LoadString(_settingsKeys[EWebUiSettings.Api], string.Empty);

            if (encodedValue == string.Empty) return encodedValue;
            
            string decodedValue = XorEncoder.Decode(encodedValue, key);
            return decodedValue;
        }

        public int GetInt(EWebUiSettings setting, int defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadInt(key, defaultValue);
        }

        public float GetFloat(EWebUiSettings setting, float defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadFloat(key, defaultValue);
        }

        public bool GetBool(EWebUiSettings setting, bool defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadBool(key, defaultValue);
        }

        public string GetString(EWebUiSettings setting, string defaultValue)
        {
            ValidateSetting(setting);
            string key = _settingsKeys[setting];
            return _database.LoadString(key, defaultValue);
        }

        #endregion

        private void ValidateSetting(EWebUiSettings setting)
        {
            if (setting == EWebUiSettings.Api)
            {
                throw new ArgumentException(
                    "Don't get/set Api via regular method - Use SetApi(string decodedValue) or GetApiDecoded instead!");
            }
        }
    }
}