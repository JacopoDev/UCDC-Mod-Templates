using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElevenLabsMod
{
    public class ElevenSettingsBinder : MonoBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle modToggle;
        [SerializeField] private TMP_InputField api;
        [SerializeField] private TMP_InputField voiceId;
        [SerializeField] private Slider similarity;
        [SerializeField] private Slider stability;
        [SerializeField] private TMP_InputField model;
        [SerializeField] private TMP_InputField format;
        
        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            modToggle.onValueChanged.AddListener(SwitchMod);
            api.onValueChanged.AddListener(SetApi);
            voiceId.onValueChanged.AddListener(SetVoiceId);
            stability.onValueChanged.AddListener(SetStability);
            similarity.onValueChanged.AddListener(SetSimilarity);
            model.onValueChanged.AddListener(SetModel);
            format.onValueChanged.AddListener(SetFormat);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            RefreshData();
        }

        private void OnDisable()
        {
            ElevenSettings.Instance.SaveAllData();
        }

        private void RestoreDefaults()
        {
            ElevenSettings.Instance.RestoreDefaultSettings();
            RefreshData();
        }

        private void RefreshData()
        {
            // Refreshing UI values in case those were changed from other sources
            modToggle.SetIsOnWithoutNotify(ElevenVoiceGenerator.AIDatabase.GetActiveVoiceAccessor() == ElevenVoiceGenerator.MainModule);
            api.SetTextWithoutNotify(ElevenSettings.Instance.GetApiDecoded());
            voiceId.SetTextWithoutNotify(ElevenSettings.Instance.GetString(
                EElevenSettings.VoiceID, 
                (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.VoiceID)));
            stability.SetValueWithoutNotify(ElevenSettings.Instance.GetFloat(
                EElevenSettings.Stability, 
                (float)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Stability)));
            similarity.SetValueWithoutNotify(ElevenSettings.Instance.GetFloat(
                EElevenSettings.SimilarityBoost, 
                (float)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.SimilarityBoost)));
            model.SetTextWithoutNotify(ElevenSettings.Instance.GetString(
                EElevenSettings.Model, 
                (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Model)));
            format.SetTextWithoutNotify(ElevenSettings.Instance.GetString(
                EElevenSettings.Format, 
                (string)ElevenSettings.Instance.GetDefaultValue(EElevenSettings.Format)));
        }
        
        public void SwitchMod(bool value)
        {
            ElevenVoiceGenerator.AIDatabase.SetActiveVoiceAccessor(ElevenVoiceGenerator.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(ElevenVoiceGenerator.AIDatabase.GetActiveVoiceAccessor() == ElevenVoiceGenerator.MainModule);
        }

        public void SetApi(string value)
        {
            ElevenSettings.Instance.SetLoaded(EElevenSettings.Api, value);
        }

        public void SetVoiceId(string value)
        {
            ElevenSettings.Instance.SetLoaded(EElevenSettings.VoiceID, value);
        }

        public void SetStability(float value)
        { 
            ElevenSettings.Instance.SetLoaded(EElevenSettings.Stability, value);
        }

        public void SetSimilarity(float value)
        { 
            ElevenSettings.Instance.SetLoaded(EElevenSettings.SimilarityBoost, value);
        }

        public void SetModel(string value)
        {
            ElevenSettings.Instance.SetLoaded(EElevenSettings.Model, value);
        }

        public void SetFormat(string value)
        {
            ElevenSettings.Instance.SetLoaded(EElevenSettings.Format, value);
        }
    }
}