using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

namespace ElevenLabsMod
{
    public class ElevenManagerBuilder : ModScript, ICreatedOnLoad
    {
        private GameObject _audioReader;
        
        public GameObject GetPrefab()
        {
            _audioReader = ModAssets.Load<GameObject>("ElevenDispatcher");
            return _audioReader;
        }
    }
}