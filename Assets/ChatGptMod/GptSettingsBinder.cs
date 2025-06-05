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
        [SerializeField] private Button restoreDefaultButton;

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
            restoreDefaultButton.onClick.AddListener(ResetSettings);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            RefreshTextData();
        }

        // Save All settings after closing settings window
        private void OnDisable()
        {
            GptSettings.SaveAllSettings();
        }

        private void ResetSettings()
        {
            GptSettings.ResetDefaults();
            RefreshTextData();
        }

        
        // Refreshing UI values in case those were changed from other sources
        private void RefreshTextData()
        {
            modToggle.SetIsOnWithoutNotify(GptMessageSender.AIDatabase.GetActive() == GptMessageSender.MainModule);
            api.SetTextWithoutNotify(GptSettings.GetApiDecoded());
            model.SetTextWithoutNotify(GptSettings.GetString(EGptSettings.Model, "gpt-4o-mini"));
            temperature.SetTextWithoutNotify(GptSettings.GetFloat(EGptSettings.Temperature, 0.9f).ToString("0.##"));
            maxTokens.SetTextWithoutNotify(GptSettings.GetInt(EGptSettings.MaxTokens, 1000).ToString());
            topP.SetTextWithoutNotify(GptSettings.GetFloat(EGptSettings.TopP, 1.0f).ToString("0.##"));
            frequencyPenalty.SetTextWithoutNotify(GptSettings.GetFloat(EGptSettings.FrequencyPenalty, 2.0f).ToString("0.##"));
            presencePenalty.SetTextWithoutNotify(GptSettings.GetFloat(EGptSettings.PresencePenalty, 2.0f).ToString("0.##"));
            stopSequences.SetTextWithoutNotify(string.Join("|", GptSettings.GetStopStrings())); 
        }

        private void SwitchMod(bool value)
        {
            GptMessageSender.AIDatabase.SetApiActive(GptMessageSender.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(GptMessageSender.AIDatabase.GetActive() == GptMessageSender.MainModule);
        }

        private void SetApi(string value)
        {
            GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.Api]] = value;
        }

        private void SetModel(string value)
        {
            GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.Model]] = value;
        }

        private void SetTemperature(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.Temperature]] = parsed;
            }
        }

        private void SetMaxTokens(string value)
        {
            if (int.TryParse(value, out int parsed))
            {
                GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.MaxTokens]] = parsed;
            }
        }

        private void SetTopP(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.TopP]] = parsed;
            }
        }

        private void SetFrequencyPenalty(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.FrequencyPenalty]] = parsed;
            }
        }

        private void SetPresencePenalty(string value)
        {
            if (float.TryParse(value, out float parsed))
            {
                GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.PresencePenalty]] = parsed;
            }
        }

        private void SetStopSequences(string value)
        {
            var stops = value
                .Split('|')
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            GptSettings.GetAllSettings()[GptSettings.SettingsKeys[EGptSettings.Stop]] = stops;
        }
    }
}
