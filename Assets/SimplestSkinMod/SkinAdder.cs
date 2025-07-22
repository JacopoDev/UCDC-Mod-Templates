using System.Collections;
using System.Collections.Generic;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

public class SkinAdder : ModScript, ISkinAccessor
{
    public Avatar animationAvatar;
    public GameObject skinPrefab;
    public Sprite skinPreview;
    
    public void GetDatabase(ISkinDatabaseProvider databaseProvider)
    {
        skinPrefab = ModAssets.Load<GameObject>("template_prefab");
        GameObject avatarObj = ModAssets.Load<GameObject>("template_skindata");
        animationAvatar = avatarObj.GetComponent<Animator>().avatar;
        skinPreview = avatarObj.GetComponent<SpriteRenderer>().sprite;
        
        
        databaseProvider.Add(new SkinInfo()
        {
            AnimationAvatar = animationAvatar,
            SkinName = "Template Character",
            Description = "This is a test mod character",
            Prefab = skinPrefab,
            Preview = skinPreview
        });
    }
}
