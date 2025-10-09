using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;

public class SpawningSphere : ModScript
{
    // Private
    private Transform sphereTransform = null;
    private float startingScale = 1f;

    // Public
    public float spawnSpeed = 1.3f;

    // Methods
    public override void OnModLoaded()
    {
        // Load and create an instance of the sphere prefab
        sphereTransform = ModAssets.Instantiate<GameObject>("Sphere").transform;

        // Store the initial scale
        startingScale = sphereTransform.localScale.x;

        // Scale to nothing
        sphereTransform.localScale = Vector3.zero;
    }

    public override void OnModUpdate()
    {
        if(sphereTransform.localScale.magnitude <= startingScale)
        { 
            // Scale up over time
            sphereTransform.localScale += Vector3.one * spawnSpeed * Time.deltaTime;
        }
    }
}
