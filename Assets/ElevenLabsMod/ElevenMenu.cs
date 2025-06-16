using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace ElevenLabsMod
{
    public class ElevenMenu : ModScript, IModPanelCreator
    {
        private GameObject _bar;
        private GameObject _panel;
    
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("ElevenBar");
            _panel = ModAssets.Load<GameObject>("ElevenPanel");
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