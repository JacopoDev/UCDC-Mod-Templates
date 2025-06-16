using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace ChatGptMod
{
    public class GptMenu : ModScript, IModPanelCreator
    {
        private GameObject _bar;
        private GameObject _panel;
    
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("GptBar");
            _panel = ModAssets.Load<GameObject>("GptPanel");
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
