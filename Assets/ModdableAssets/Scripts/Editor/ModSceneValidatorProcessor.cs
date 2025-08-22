using System.Collections;
using System.Collections.Generic;
using UMod.BuildEngine;
using UMod.BuildPipeline;
using UMod.BuildPipeline.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This processor will be invoked on every scene asset when building the mod.
/// We use it for validation that required objects exist.
/// </summary>
[UModBuildProcessor(".unity", -100)]
public class ModSceneValidatorProcessor : BuildEngineProcessor
{
    // Methods
    public override void ProcessAsset(BuildContext context, BuildPipelineAsset asset)
    {
        // Load the scene into the editor
        Scene scene = EditorSceneManager.OpenScene(asset.FullPath);

        bool validScene = true;

        // Check for spawn point 1
        if (FindSceneRootObject(scene, "UnityChanPos") == false)
        {
            context.Log.Error("Scene '{0}' must have a root game object named 'UnityChanPos'", asset.RelativePath);
            validScene = false;
        }

        // Check for spawn point 2
        if(FindSceneRootObject(scene, "MainCameraPos") == false)
        {
            context.Log.Error("Scene '{0}' must have a root game object named 'MainCameraPos'", asset.RelativePath);
            validScene = false;
        }

        // Check for valid scene
        if(validScene == false)
        {
            context.FailBuild("One or more required scene objects are missing");
        }
    }

    private bool FindSceneRootObject(Scene scene, string objectName)
    {
        foreach(GameObject obj in scene.GetRootGameObjects())
        {
            if (obj.name == objectName)
                return true;
        }
        return false;
    }
}
