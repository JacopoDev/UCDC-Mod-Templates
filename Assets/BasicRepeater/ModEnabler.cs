using System;
using TMPro;
using UMod;
using UnityEngine;
using UnityEngine.UI;

namespace Repeater
{
    public class ModEnabler : ModScriptBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle modToggle;
        [SerializeField] private TMP_InputField phrase;

        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            modToggle.onValueChanged.AddListener(SwitchMod);
            phrase.onValueChanged.AddListener(SetPhrase);
        }

        // Unity method that runs each time the object becomes enabled
        private void OnEnable()
        {
            // Refreshing UI values in case those were changed from other sources
            phrase.SetTextWithoutNotify(RepeaterSettings.Phrase);
            modToggle.SetIsOnWithoutNotify(Repeater.AIApiDatabase.GetActiveTextAccessor() == Repeater.MainModule);
        }

        // This method runs when the object becomes deactivated, in this case once we close the settings panel
        // Save mod settings to file once you close the mod settings view
        private void OnDisable()
        {
            RepeaterSettings.SaveSettings();
        }

        public void SwitchMod(bool value)
        {
            Repeater.AIApiDatabase.SetActiveTextAccessor(Repeater.MainModule); // set this mod module as currently active (or deactivate)
            modToggle.SetIsOnWithoutNotify(Repeater.AIApiDatabase.GetActiveVoiceAccessor() == Repeater.MainModule);
        }

        public void SetPhrase(string value)
        {
            RepeaterSettings.Phrase = value;
        }
    }
}
