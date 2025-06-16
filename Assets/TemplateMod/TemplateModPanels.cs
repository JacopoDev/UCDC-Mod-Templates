using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace TemplateMod
{
    public class TemplateModPanels : ModScript, IModPanelCreator  // game seeks for IModPanelCreator Scripts to build menu panel settings
    {
        private GameObject _bar;
        private GameObject _panel;
    
        // we search for mod assets to attach mod bar and settings panel
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("ModBarSelector"); // name has to be exact as the name of the object in the game folder
            _panel = ModAssets.Load<GameObject>("ModSettingsPanel");
        }

        // method that gives the game bar UI object
        public GameObject GetSettingsButton()
        {
            return _bar;
        }

        // method that gives the game mod settings panel UI object
        public GameObject GetSettingsPanel()
        {
            return _panel;
        }
    }
}
