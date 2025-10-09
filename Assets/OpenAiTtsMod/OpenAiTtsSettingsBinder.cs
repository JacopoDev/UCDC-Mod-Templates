using TMPro;
using UMod;
using UnityEngine;
using UnityEngine.UI;

namespace OpenAiTtsMod
{
    public class OpenAiTtsSettingsBinder : ModScriptBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle modToggle;
        [SerializeField] private TMP_InputField api;
        [SerializeField] private TMP_InputField model;
        [SerializeField] private TMP_InputField voice;
        [SerializeField] private Button resetDefaultsBtn;

        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            modToggle.onValueChanged.AddListener(SwitchMod);
            
            api.onValueChanged.AddListener(SetApi);
            model.onValueChanged.AddListener(SetModel);
            voice.onValueChanged.AddListener(SetVoice);
            
            resetDefaultsBtn.onClick.AddListener(RestoreDefaults);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            RefreshData();
        }

        private void OnDisable()
        {
            OpenAiTtsSettings.Instance.SaveAllData();
        }

        private void RestoreDefaults()
        {
            OpenAiTtsSettings.Instance.RestoreDefaultSettings();
            RefreshData();
        }

        private void RefreshData()
        {
            // Refreshing UI values in case those were changed from other sources
            modToggle.SetIsOnWithoutNotify(OpenAiTtsVoiceGenerator.AIDatabase.GetActiveVoiceAccessor() == OpenAiTtsVoiceGenerator.MainModule);
            api.SetTextWithoutNotify(OpenAiTtsSettings.Instance.GetApiDecoded());
            model.SetTextWithoutNotify(OpenAiTtsSettings.Instance.GetString(
                EOpenAiTtsSettings.Model, 
                (string)OpenAiTtsSettings.Instance.GetDefaultValue(EOpenAiTtsSettings.Model)));
            voice.SetTextWithoutNotify(OpenAiTtsSettings.Instance.GetString(
                EOpenAiTtsSettings.Voice, 
                (string)OpenAiTtsSettings.Instance.GetDefaultValue(EOpenAiTtsSettings.Voice)));
        }

        public void SwitchMod(bool value)
        {
            OpenAiTtsVoiceGenerator.AIDatabase.SetActiveVoiceAccessor(OpenAiTtsVoiceGenerator.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(OpenAiTtsVoiceGenerator.AIDatabase.GetActiveVoiceAccessor() == OpenAiTtsVoiceGenerator.MainModule);
        }

        public void SetApi(string value)
        {
            OpenAiTtsSettings.Instance.SetLoaded(EOpenAiTtsSettings.Api, value);
        }

        public void SetModel(string value)
        {
            OpenAiTtsSettings.Instance.SetLoaded(EOpenAiTtsSettings.Model, value);
        }

        public void SetVoice(string value)
        {
            OpenAiTtsSettings.Instance.SetLoaded(EOpenAiTtsSettings.Voice, value);
        }
    }
}