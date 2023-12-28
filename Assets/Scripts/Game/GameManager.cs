using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Linq;

/**
* @description:游戏管理
* @author: dazzi
* @time: 2023.07
*/
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsGameStarted { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsPause { get; set; }
    private int gameScore;
    private int gameDiamond;
    private GameData data;
    private ManagerVars vars;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinUnLocked;
    private int diamondCount;

    public bool PlayerIsMove { get; set; }

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        Instance = this;
        EventCenter.AddListener(EventDefine.AddScore,AddGameScore);
        EventCenter.AddListener(EventDefine.PlayerMove,PlayerMove);
        EventCenter.AddListener(EventDefine.AddDiamond, AddGameDiamond);
        if (GameData.IsAgainGame) {
            IsGameStarted = true;
        }
        InitGameData();
    }

    /// <summary>
    /// 保存分数
    /// </summary>
    /// <param name="score"></param>
    public void SaveScore(int score) {
        List<int> list = bestScoreArr.ToList();
        //从大到小排序
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();

        int index = -1;
        for (int i = 0; i < bestScoreArr.Length; i++) {
            if (score > bestScoreArr[i]) {
                index = i;
            }
        }
        if (index == -1) {
            return;
        }
        for (int i = bestScoreArr.Length - 1; i > index; i--) {
            bestScoreArr[i] = bestScoreArr[i - 1];

        }
        bestScoreArr[index] = score;
        Save();
    }

    /// <summary>
    /// 获取最高分
    /// </summary>
    /// <returns></returns>
    public int GetBestScore() {
        return bestScoreArr.Max();
    }

    /// <summary>
    /// 获取排名数组
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr() {
        List<int> list = bestScoreArr.ToList();
        //从大到小排序
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();
        return bestScoreArr;
    }



    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
        EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddGameDiamond);
    }

    private void PlayerMove()
    {
        PlayerIsMove = true;
    }

    private void AddGameScore()
    {
        if (IsGameStarted == false||IsGameOver||IsPause) return;
        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdateScoreText,gameScore);
    }

    public int GetGameScore()
    {
        return gameScore;
    }

    private void AddGameDiamond() {
        gameDiamond++;
        EventCenter.Broadcast(EventDefine.UpdateDiamondText, gameDiamond);
    }
    public int GetGameDiamond() { return gameDiamond; }

    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    private void InitGameData() {
        Read();
        if (data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else {
            isFirstGame = true;
        }
        if (isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            skinUnLocked = new bool[vars.skinSpriteList.Count];
            skinUnLocked[0] = true;
            diamondCount = 10;

            data = new GameData();
            Save();
        }
        else {
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectSkin = data.GetSelectSkin();
            skinUnLocked = data.GetSkinUnlocked();
            diamondCount = data.GetDiamondCount();
        }
    }

    public bool GetSkinUnlocked(int index) {
        return skinUnLocked[index];
    }
    public void SetSkinUnlocked(int index) {
        skinUnLocked[index] = true;
        Save();
    }
    public int GetAllDiamond() {
        return diamondCount;
    }
    public void UpdateAllDiamond(int value) {
        diamondCount += value;
        Save();
    }
    public void SetSelectedSkin(int index) {
        selectSkin = index;
        Save();
    }
    public int GetCurrentSelectedSkin() {
        return selectSkin;

    }
    public void SetIsMusicOn(bool value) {
        isMusicOn = value;
        Save();
    }

    public bool GetIsMusicOn() {
        return isMusicOn;
    }
    /// <summary>
    /// 保存游戏数据
    /// </summary>
    private void Save() {
        try { 
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Create(Application.persistentDataPath+"/GameData.data")) {
                data.SetBestScoreArr(bestScoreArr);
                data.SetDiamondCount(diamondCount);
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetSelectSkin(selectSkin);
                data.SetSkinUnlocked(skinUnLocked);
                bf.Serialize(fs, data);
            }
        } catch (System.Exception e) { Debug.Log(e.Message); }
    }
    /// <summary>
    /// 读取游戏数据
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open))
            {
                data = (GameData)bf.Deserialize(fs);
            }
        }
        catch (System.Exception e) { Debug.Log(e.Message); }
    }
    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetData() {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        skinUnLocked = new bool[vars.skinSpriteList.Count];
        skinUnLocked[0] = true;
        diamondCount = 10;

        Save();
    }
}
