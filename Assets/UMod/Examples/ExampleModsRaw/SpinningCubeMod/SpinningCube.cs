using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;

public class SpinningCube : ModScriptBehaviour
{
    // Public
    public float rotateSpeed = 2f;

    // Methods
    public override void OnModLoaded()
    {
        ModDebug.LogMessage("This is a message from spinning cube mod");
    }

    public override void OnModUpdate()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
