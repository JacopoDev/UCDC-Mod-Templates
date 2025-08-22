using System.Collections;
using System.Collections.Generic;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models.Scenes;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;
using UnityEngine.UI;

public class SceneAdder : ModScript, ISceneDatabaseAccessor
{
    public void SetProvider(ISceneDatabaseProvider provider)
    {
        Texture2D tex = ModAssets.Load<Texture2D>("testImage");

        SceneData sd = new SceneData()
        {
            LocationName = "Empty Test Scene",
            LocationDescription = "Template Description",
            AiContext =
                "Unity-chan and {0} are on an empty scenery, probably a place for testing purposes. It feels a bit boring and plain here.",
            Preview = tex,
            Scene = ModScenes.Find("TemplateScene")
        };
        
        provider.AddOutdoorScene(sd);
    }
}
