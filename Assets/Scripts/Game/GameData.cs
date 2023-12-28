using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* @description:ÓÎÏ·Êý¾Ý
* @author: dazzi
* @time: 2023.07
*/
[System.Serializable]
public class GameData
{
    public static bool IsAgainGame = false;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinUnlocked;
    private int diamondCount;

    public void SetIsFirstGame(bool isFirstGame) {
        this.isFirstGame = isFirstGame;
    }
    public bool GetIsFirstGame()
    {
        return isFirstGame;
    }

    public void SetIsMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;
    }
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }

    public void SetBestScoreArr(int[] bestScoreArr) {
        this.bestScoreArr = bestScoreArr;
    }
    public int[] GetBestScoreArr()
    {
        return bestScoreArr;
    }

    public void SetSelectSkin(int selectSkin) {
        this.selectSkin = selectSkin;
    }
    public int GetSelectSkin()
    {
        return selectSkin;
    }

    public void SetSkinUnlocked(bool[] skinUnlocked) {
        this.skinUnlocked = skinUnlocked;
    }
    public bool[] GetSkinUnlocked()
    {
        return skinUnlocked;
    }

    public void SetDiamondCount(int diamondCount) {
        this.diamondCount = diamondCount;
    }
    public int GetDiamondCount()
    {
        return diamondCount;
    }
}
