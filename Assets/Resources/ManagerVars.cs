using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "CreateManagerVarsContainer")]
/**
* @description:资源管理器
* @author: dazzi
* @time: 2023.07
*/
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }

    public List<Sprite> bgThemeSpriteList = new List<Sprite>();
    public List<Sprite> PlatformThemeSpriteList = new List<Sprite>();
    public List<Sprite> skinSpriteList = new List<Sprite>();
    public List<Sprite> characterSkinSpriteList = new List<Sprite>();
    public GameObject characterPre;
    public GameObject skinChooseItemPre;
    public GameObject normalPlatformPre;
    public List<string> skinNameList = new List<string>();
    public List<int> skinPrice = new List<int>();
    public List<GameObject> commonPlatformGroup = new List<GameObject>();
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();

    public GameObject spikePlatformLeft;
    public GameObject spikePlatformRight;

    public GameObject deathEffect;

    public GameObject diamondPre;

    public float nextXPos = 0.554f, nextYPos = 0.645f;

    public AudioClip jumpClip, fallClip, hitClip, diamondClip, buttonClip;
    public Sprite musicOn, musicOff;
}
