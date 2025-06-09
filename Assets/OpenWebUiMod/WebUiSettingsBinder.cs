using System;
using System.Net;
using TMPro;
using UMod;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWebUiMod
{
    public class WebUiSettingsBinder : ModScriptBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle modToggle;
        [SerializeField] private TMP_InputField api;
        [SerializeField] private TMP_InputField model;
        [SerializeField] private TMP_InputField ipAddress;
        [Tooltip("Ip object displayed on wrong validation")] [SerializeField]
        private GameObject ipValidation;
        [SerializeField] private TMP_InputField port;
        [Tooltip("Ip object displayed on wrong validation")] [SerializeField]
        private GameObject portValidation;
        [SerializeField] private Button resetDefaultsBtn;

        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            modToggle.onValueChanged.AddListener(SwitchMod);
            api.onValueChanged.AddListener(SetApi);
            model.onValueChanged.AddListener(SetModel);
            ipAddress.onValueChanged.AddListener(SetIp);
            port.onValueChanged.AddListener(SetPort);
            resetDefaultsBtn.onClick.AddListener(RestoreDefaults);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            ipValidation.SetActive(false);
            portValidation.SetActive(false);
            RefreshData();
        }

        private void OnDisable()
        {
            WebUiSettings.Instance.SaveAllData();
        }

        private void RestoreDefaults()
        {
            WebUiSettings.Instance.RestoreDefaultSettings();
            RefreshData();
        }

        private void RefreshData()
        {
            // Refreshing UI values in case those were changed from other sources
            modToggle.SetIsOnWithoutNotify(WebUiMessageSender.AIDatabase.GetActive() == WebUiMessageSender.MainModule);
            api.SetTextWithoutNotify(WebUiSettings.Instance.GetApiDecoded());
            model.SetTextWithoutNotify(WebUiSettings.Instance.GetString(
                EWebUiSettings.Model, 
                WebUiSettings.Instance.GetDefaultValue(EWebUiSettings.Model).ToString()));
            ipAddress.SetTextWithoutNotify(WebUiSettings.Instance.GetString(
                EWebUiSettings.Ip, 
                WebUiSettings.Instance.GetDefaultValue(EWebUiSettings.Ip).ToString()));
            port.SetTextWithoutNotify(WebUiSettings.Instance.GetInt(
                EWebUiSettings.Port, 
                (int)WebUiSettings.Instance.GetDefaultValue(EWebUiSettings.Port)).ToString());
        }

        public void SwitchMod(bool value)
        {
            WebUiMessageSender.AIDatabase.SetApiActive(WebUiMessageSender.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(WebUiMessageSender.AIDatabase.GetActive() == WebUiMessageSender.MainModule);
        }

        public void SetApi(string value)
        {
            WebUiSettings.Instance.SetLoaded(EWebUiSettings.Api, value);
        }

        public void SetModel(string value)
        {
            WebUiSettings.Instance.SetLoaded(EWebUiSettings.Model, value);
        }

        public void SetIp(string value)
        {
            string filtered = System.Text.RegularExpressions.Regex.Replace(value, @"[^0-9.]", "");
            ipAddress.SetTextWithoutNotify(filtered);
            
            if (!IPAddress.TryParse(filtered, out IPAddress address) && address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipValidation.SetActive(true);
            }
            else
            {
                ipValidation.SetActive(false);
                WebUiSettings.Instance.SetLoaded(EWebUiSettings.Ip, address.ToString());
            }
        }

        public void SetPort(string value)
        {
            if (int.TryParse(value, out int parsed))
            {
                if (parsed > 0 && parsed < 65535)
                {
                    portValidation.SetActive(false);
                    WebUiSettings.Instance.SetLoaded(EWebUiSettings.Port, parsed);
                }
                else
                {
                    portValidation.SetActive(true);
                }
            }
            else
            {
                portValidation.SetActive(true);
            }
        }
    }
}
