using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace OpenAiTtsMod
{
    public class OpenAiTtsPanel : ModScript, IModPanelCreator
    {
        private GameObject _bar;
        private GameObject _panel;
    
        public override void OnModLoaded()
        {
            _bar = ModAssets.Load<GameObject>("OpenAiTtsBar");
            _panel = ModAssets.Load<GameObject>("OpenAiTtsPanel");
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