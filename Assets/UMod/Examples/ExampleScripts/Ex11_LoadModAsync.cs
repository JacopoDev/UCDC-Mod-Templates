using UnityEngine;
using System.Collections;

using UMod;
using System;

namespace UMod.Example
{
    // An example script that shows how to load a mod from file using coroutines in a similar way to WWW.
    // To use this script simply attach it to a game object and ensure that the path variable points to a valid mod.
    public class Ex11_LoadModAsync : MonoBehaviour
    {
        // The path to the mod
        public string modPath = "C:/Mods/Test Mod";

        private IEnumerator Start()
        {
            // We need to specify the location of the mod using the 'ModPath' class.
            Uri path = new Uri(modPath);

            // Create a mod load request
            ModAsyncOperation<ModHost> request = Mod.LoadAsync(path);

            // Wait for the load to complete
            yield return request;

            // Check for success
            if (request.IsSuccessful == true)
            {
                // The mod is now loaded
                ExampleUtil.Log(this, "Mod Loaded!");

                // Get the mod host that loaded the mod
                ModHost loadedHost = request.Result;

                Debug.Log("Mod Async Load Complete: " + loadedHost.CurrentMod.NameInfo);
            }
            else
            {
                ExampleUtil.LogError(this, "Failed to load the mod");

                // Print the error code to the console
                ExampleUtil.LogError(this, request.Status);
            }
        }
    }
}
