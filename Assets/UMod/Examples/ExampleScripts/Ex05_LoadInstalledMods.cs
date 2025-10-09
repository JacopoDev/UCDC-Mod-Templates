using UnityEngine;
using System.Collections;

// Make sure we can access the uMod api for loading
using UMod;

namespace UMod.Example
{
    /// <summary>
    /// An example script that shows how to load all installed mods.
    /// This example makes use of the 'ModDirectory' but there are other methods.
    /// To use this script simlpe attatch it to a game object.
    /// </summary>
    [ExecuteInEditMode]
    public class Ex05_LoadInstalledMods : MonoBehaviour
    {
        private ModDirectory modDirectory = null;

        // The path to the mod
        public string modDirectoryPath = "";

        private void Start()
        {
            // Check if we are in-editor and not in play mode - if so the component has just been added
            if (Application.isEditor == true && Application.isPlaying == false)
            {
                // Make sure we dont alter the users suggestion
                if (string.IsNullOrEmpty(modDirectoryPath) == true)
                {
                    // Initialize to a default directory
                    modDirectoryPath = Application.persistentDataPath + "/Mods";
                }
                return;
            }

            // Create the mod directory at the path
            modDirectory = new ModDirectory(modDirectoryPath);

            // Setup the mod directory before we can use it
            //ModDirectory.DirectoryLocation = modDirectoryPath;

            // Check if there are any installed mods
            if (modDirectory.HasMods == true)// ModDirectory.HasAnyMods == true)
            {
                // This method will attempt to locate any mods installed in the 'modDirectory' location.
                ModHost[] hosts = Mod.LoadAll(true, modDirectory);

                // Check the load status of all hosts
                foreach (ModHost host in hosts)
                {
                    if (host.IsModLoaded == true)
                    {
                        // The mod is now loaded
                        ExampleUtil.Log(this, string.Format("Mod Loaded: {0}", host.CurrentModPath));
                    }
                    else
                    {
                        // The mod did not load correctly
                        ExampleUtil.LogError(this, string.Format("Failed to load mod: {0}, ({1})", host.CurrentModPath, host.LoadResult.Message));
                    }
                }
            }
            else
            {
                // There are no mods installed in 'modDirectory' so just print a message.
                ExampleUtil.LogError(this, "There are no mods installed in the mod directory");
            }
        }
    }
}
