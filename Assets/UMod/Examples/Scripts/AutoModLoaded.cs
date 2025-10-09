using System;
using UnityEngine;

namespace UMod.Example
{
    public class AutoModLoaded : MonoBehaviour
    {
        // Private
        private float loadTimeCountdown = 0f;

        // Public
        public float loadDelayTime = 0f;
        public string[] loadModNames;

        // Methods
        public void Start()
        {
            if(loadDelayTime == 0)
            {
                LoadAllMods();
            }
            else
            {
                loadTimeCountdown = loadDelayTime;
                Invoke("LoadAllMods", loadDelayTime);
            }
        }

        public void Update()
        {
            if(loadTimeCountdown > 0)
                loadTimeCountdown -= Time.deltaTime;
        }

        public void OnGUI()
        {
            if (loadTimeCountdown > 0)
                GUILayout.Label("Mods will be loaded in: " + loadTimeCountdown.ToString("f2"));
        }

        public void LoadAllMods()
        {
            // Create the mod directory from the examples folder
            ModDirectory directory = new ModDirectory(Application.dataPath + "/UMod/Examples/ExampleMods");

            // Try to load all specifie dmods
            foreach (string modName in loadModNames)
            {
                // Try to get mod path
                Uri modPath = directory.GetModPath(modName);

                // Check for error path
                if (modPath == null)
                {
                    Debug.LogWarningFormat("Failed to find mod '{0}' in the default mod directory", modName);
                    continue;
                }

                // Load the mod
                Mod.LoadAsync(modPath);
            }
        }
    }
}
