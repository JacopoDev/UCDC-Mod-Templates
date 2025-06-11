using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace GoogleCloudVoiceMod
{
    public class GoogleMenu : ModScript, IModPanel
    {
        private GameObject _bar;
        private GameObject _panel;
    
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("GoogleBar");
            _panel = ModAssets.Load<GameObject>("GooglePanel");
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
