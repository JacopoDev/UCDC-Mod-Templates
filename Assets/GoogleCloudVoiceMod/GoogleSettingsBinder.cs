using System.Linq;
using TMPro;
using UMod;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleCloudVoiceMod
{
    public class GoogleSettingsBinder : ModScriptBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle modToggle;
        [SerializeField] private TMP_InputField api;
        [SerializeField] private TMP_InputField code;
        [SerializeField] private TMP_InputField voiceName;
        [SerializeField] private Slider speed;
        [SerializeField] private Slider pitch;
        [SerializeField] private Button resetDefaultsBtn;

        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            modToggle.onValueChanged.AddListener(SwitchMod);
            api.onValueChanged.AddListener(SetApi);
            code.onValueChanged.AddListener(SetLangCode);
            voiceName.onValueChanged.AddListener(SetVoiceName);
            speed.onValueChanged.AddListener(SetSpeed);
            pitch.onValueChanged.AddListener(SetPitch);
            resetDefaultsBtn.onClick.AddListener(RestoreDefaults);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            RefreshData();
        }

        private void OnDisable()
        {
            GoogleSettings.Instance.SaveAllData();
        }

        private void RestoreDefaults()
        {
            GoogleSettings.Instance.RestoreDefaultSettings();
            RefreshData();
        }

        private void RefreshData()
        {
            // Refreshing UI values in case those were changed from other sources
            modToggle.SetIsOnWithoutNotify(GoogleVoiceGenerator.AIDatabase.GetActiveVoiceAccessor() == GoogleVoiceGenerator.MainModule);
            api.SetTextWithoutNotify(GoogleSettings.Instance.GetApiDecoded());
            code.SetTextWithoutNotify(GoogleSettings.Instance.GetString(
                EGoogleSettings.LanguageCode, 
                (string)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.LanguageCode)));
            voiceName.SetTextWithoutNotify(GoogleSettings.Instance.GetString(
                EGoogleSettings.VoiceName, 
                (string)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.VoiceName)));
            speed.SetValueWithoutNotify(GoogleSettings.Instance.GetFloat(
                EGoogleSettings.Speed, 
                (float)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.Speed)));
            pitch.SetValueWithoutNotify(GoogleSettings.Instance.GetFloat(
                EGoogleSettings.Pitch, 
                (float)GoogleSettings.Instance.GetDefaultValue(EGoogleSettings.Pitch)));
        }

        public void SwitchMod(bool value)
        {
            GoogleVoiceGenerator.AIDatabase.SetActiveVoiceAccessor(GoogleVoiceGenerator.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(GoogleVoiceGenerator.AIDatabase.GetActiveVoiceAccessor() == GoogleVoiceGenerator.MainModule);
        }

        public void SetApi(string value)
        {
            GoogleSettings.Instance.SetLoaded(EGoogleSettings.Api, value);
        }

        public void SetLangCode(string value)
        {
            GoogleSettings.Instance.SetLoaded(EGoogleSettings.LanguageCode, value);
        }

        public void SetVoiceName(string value)
        {
            GoogleSettings.Instance.SetLoaded(EGoogleSettings.VoiceName, value);
        }

        public void SetSpeed(float value)
        { 
            GoogleSettings.Instance.SetLoaded(EGoogleSettings.Speed, value);
        }

        public void SetPitch(float value)
        { 
            GoogleSettings.Instance.SetLoaded(EGoogleSettings.Pitch, value);
        }
    }
}
