using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace GoogleCloudVoiceMod
{
    public class GoogleManagerBuilder : ModScript, ICreatedOnLoad
    {
        private GameObject _audioReader;
        
        public GameObject GetPrefab()
        {
            _audioReader = ModAssets.Load<GameObject>("GoogleTextToSpeech");
            return _audioReader;
        }
    }
}
