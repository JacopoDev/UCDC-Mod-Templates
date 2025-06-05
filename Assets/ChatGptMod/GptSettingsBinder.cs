using System;
using System.Linq;
using TMPro;
using UMod;
using UnityEngine;
using UnityEngine.UI;

namespace ChatGptMod
{
    public class GptSettingsBinder : ModScriptBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle modToggle;
        [SerializeField] private TMP_InputField api;
        [SerializeField] private TMP_InputField model;
        [SerializeField] private TMP_InputField temperature;
        [SerializeField] private TMP_InputField maxTokens;
        [SerializeField] private TMP_InputField topP;
        [SerializeField] private TMP_InputField frequencyPenalty;
        [SerializeField] private TMP_InputField presencePenalty;
        [SerializeField] private TMP_InputField stopSequences;
        [SerializeField] private Button resetDefaultsBtn;

        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            modToggle.onValueChanged.AddListener(SwitchMod);
            api.onValueChanged.AddListener(SetApi);
            model.onValueChanged.AddListener(SetModel);
            temperature.onValueChanged.AddListener(SetTemperature);
            maxTokens.onValueChanged.AddListener(SetMaxTokens);
            topP.onValueChanged.AddListener(SetTopP);
            frequencyPenalty.onValueChanged.AddListener(SetFrequencyPenalty);
            presencePenalty.onValueChanged.AddListener(SetPresencePenalty);
            stopSequences.onValueChanged.AddListener(SetStopSequences);
            resetDefaultsBtn.onClick.AddListener(RestoreDefaults);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            RefreshData();
        }

        private void RestoreDefaults()
        {
            GptSettings.Instance.RestoreDefaultSettings();
            RefreshData();
        }

        private void RefreshData()
        {
            // Refreshing UI values in case those were changed from other sources
            modToggle.SetIsOnWithoutNotify(GptMessageSender.AIDatabase.GetActive() == GptMessageSender.MainModule);
            api.SetTextWithoutNotify(GptSettings.Instance.GetApiDecoded());
            model.SetTextWithoutNotify(GptSettings.Instance.GetString(EGptSettings.Model, "gpt-4o-mini"));
            temperature.SetTextWithoutNotify(GptSettings.Instance.GetFloat(EGptSettings.Temperature, 0.9f).ToString("0.##"));
            maxTokens.SetTextWithoutNotify(GptSettings.Instance.GetInt(EGptSettings.MaxTokens, 1000).ToString());
            topP.SetTextWithoutNotify(GptSettings.Instance.GetFloat(EGptSettings.TopP, 1.0f).ToString("0.##"));
            frequencyPenalty.SetTextWithoutNotify(GptSettings.Instance.GetFloat(EGptSettings.FrequencyPenalty, 2.0f).ToString("0.##"));
            presencePenalty.SetTextWithoutNotify(GptSettings.Instance.GetFloat(EGptSettings.PresencePenalty, 2.0f).ToString("0.##"));
            stopSequences.SetTextWithoutNotify(string.Join("|", GptSettings.Instance.GetStopStrings())); 
        }

        public void SwitchMod(bool value)
        {
            GptMessageSender.AIDatabase.SetApiActive(GptMessageSender.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(GptMessageSender.AIDatabase.GetActive() == GptMessageSender.MainModule);
        }

        public void SetApi(string value)
        {
            GptSettings.Instance.SetApi(value);
        }

        public void SetModel(string value)
        {
            GptSettings.Instance.SetString(EGptSettings.Model, value);
        }

        public void SetTemperature(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.Instance.SetFloat(EGptSettings.Temperature, parsed);
            }
        }

        public void SetMaxTokens(string value)
        {
            if (int.TryParse(value, out int parsed))
            {
                GptSettings.Instance.SetInt(EGptSettings.MaxTokens, parsed);
            }
        }

        public void SetTopP(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.Instance.SetFloat(EGptSettings.TopP, parsed);
            }
        }

        public void SetFrequencyPenalty(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.Instance.SetFloat(EGptSettings.FrequencyPenalty, parsed);
            }
        }

        public void SetPresencePenalty(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.Instance.SetFloat(EGptSettings.PresencePenalty, parsed);
            }
        }

        public void SetStopSequences(string value)
        {
            // Split by comma, trim whitespace, filter out empty strings
            var stops = value
                .Split('|')
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            GptSettings.Instance.SetStopStrings(stops);
        }
    }
}
