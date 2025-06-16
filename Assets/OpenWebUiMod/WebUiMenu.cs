using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace OpenWebUiMod
{
    public class WebUiMenu : ModScript, IModPanelCreator
    {
        private GameObject _bar;
        private GameObject _panel;
    
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("WebUIBar");
            _panel = ModAssets.Load<GameObject>("WebUIPanel");
        }

        public GameObject GetSettingsButton()
        {
            return _bar;
        }

        public GameObject GetSettingsPanel()
        {
            return _panel;
        }
    }
}
