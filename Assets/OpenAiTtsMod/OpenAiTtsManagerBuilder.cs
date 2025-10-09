using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace OpenAiTtsMod
{
    public class OpenAiTtsManagerBuilder : ModScript, ICreatedOnLoad
    {
        private GameObject _audioReader;
        
        public GameObject GetPrefab()
        {
            _audioReader = ModAssets.Load<GameObject>("OpenAiTts");
            return _audioReader;
        }
    }
}