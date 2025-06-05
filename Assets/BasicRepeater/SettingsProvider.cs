using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace Repeater
{
    public class SettingsProvider : ModScript, IModPanel
    {
        private GameObject _bar;
        private GameObject _panel;
    
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("RepeaterBar");
            _panel = ModAssets.Load<GameObject>("RepeaterPanel");
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
