using System.Collections;
using System.Collections.Generic;
using UCDC_Mod_Api.GameInterfaces;
using UCDC_Mod_Api.Models;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

public class MitaSkinAdder : ModScript, ISkinAccessor
{
    public Avatar animationAvatar;
    public GameObject skinPrefab;
    public Sprite skinPreview;
    
    public void GetDatabase(ISkinDatabaseProvider databaseProvider)
    {
        skinPrefab = ModAssets.Load<GameObject>("Mita_Prefab");
        GameObject avatarObj = ModAssets.Load<GameObject>("Mita_Data");
        animationAvatar = avatarObj.GetComponent<Animator>().avatar;
        skinPreview = avatarObj.GetComponent<SpriteRenderer>().sprite;
        
        
        databaseProvider.Add(new SkinInfo()
        {
            AnimationAvatar = animationAvatar,
            SkinName = "Mita",
            Description = "Mita from the game MiSide",
            Prefab = skinPrefab,
            Preview = skinPreview
        });
    }
}
