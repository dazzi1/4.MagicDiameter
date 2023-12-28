using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlatformGroupType
{
    Grass,
    Winter
}
/**
* @description:平台角色生成
* @author: dazzi
* @time: 2023.07
*/
public class PlatformSpanner : MonoBehaviour
{
    public Vector3 startSpawnPos;
    public int milestoneCount = 10;
    public float fallTime;
    public float minFallTime;
    public float multiple;
    private ManagerVars vars;
    // 生成平台数量
    private int spawnPlatformCount;
    // 平台生成位置
    private Vector3 platformSpawnPosition;
    // 是否向左生成
    private bool isLeftSpawn = false;

    private Sprite selectPlatformSprite;
    private PlatformGroupType groupType;
    private bool spikeSpawnLeft = false;
    private Vector3 spikeDirPlatformPos;
    private int afterSpawnSpikeSpawnCount;
    private bool isSpawnSpike;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.DecidePath,DecidePath);
        vars = ManagerVars.GetManagerVars();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }

    private void Start()
    {
        RandomPlatformTheme();
        //生成平台
        platformSpawnPosition = startSpawnPos;
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }

        //生成人物
        GameObject go = Instantiate(vars.characterPre);
        go.transform.position = new Vector3(0, -1.8f, 8.5f);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameStarted && GameManager.Instance.IsGameOver == false)
        {
            UpdateFallTime();
        }

        UpdateFallTime();
    }

    private void UpdateFallTime()
    {
        if (GameManager.Instance.GetGameScore() > milestoneCount)
        {
            milestoneCount *= 2;
            fallTime *= multiple;
            if (fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    }
    /// <summary>
    /// 随机平台种类
    /// </summary>
    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.PlatformThemeSpriteList.Count);
        selectPlatformSprite = vars.PlatformThemeSpriteList[ran];

        if (ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }
    /// <summary>
    /// 确定路径
    /// </summary>
    private void DecidePath()
    {
        if (isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }

        //生成单个平台
        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            //反转方向
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1, 4);
            SpawnPlatform();
        }
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        int ranObstacleDir = Random.Range(0, 2);
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(ranObstacleDir);
        }else if(spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            if (ran == 0)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);
            }
            else if (ran==1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                int value = -1;
                if (isLeftSpawn)
                {
                    value = 0;
                }
                else
                {
                    value = 1;
                }

                SpawnSpikePlatform(value);
                isSpawnSpike = true;
                afterSpawnSpikeSpawnCount = 4;
                if (spikeSpawnLeft)
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPosition.x - 1.65f,
                        platformSpawnPosition.y + vars.nextYPos, platformSpawnPosition.z);
                }
                else
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPosition.x + 1.65f,
                        platformSpawnPosition.y + vars.nextYPos, platformSpawnPosition.z);
                }
            }
        }

        int ranSpawnDiamond = Random.Range(0, 10);
        if (ranSpawnDiamond == 6 && GameManager.Instance.PlayerIsMove)
        {
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position =
                new Vector3(platformSpawnPosition.x, platformSpawnPosition.y+0.5f, platformSpawnPosition.z);
            go.SetActive(true);

        }

        if (isLeftSpawn)//向左生成
        {
            platformSpawnPosition = new Vector3(platformSpawnPosition.x - vars.nextXPos,
                platformSpawnPosition.y + vars.nextYPos, platformSpawnPosition.z);
        }
        else//向右生成
        {
            platformSpawnPosition = new Vector3(platformSpawnPosition.x + vars.nextXPos,
                platformSpawnPosition.y + vars.nextYPos, platformSpawnPosition.z);
        }
    }
    /// <summary>
    /// 生成普通平台
    /// </summary>
    /// <param name="ranObstacleDir"></param>
    private void SpawnNormalPlatform(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite,fallTime,ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成公共平台
    /// </summary>
    /// <param name="ranObstacleDir"></param>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetCommonPlatformGroup();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成春天平台
    /// </summary>
    /// <param name="ranObstacleDir"></param>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetGrassPlatformGroup();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冬天平台
    /// </summary>
    /// <param name="ranObstacleDir"></param>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetWinterPlatformGroup();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }

    private void SpawnSpikePlatform(int dir)
    {
        GameObject temp = null;
        if (dir == 0)
        {
            spikeSpawnLeft = false;
            temp = ObjectPool.Instance.GetRightSpikePlatform();
        }
        else
        {
            spikeSpawnLeft = true;
            temp = ObjectPool.Instance.GetLeftSpikePlatform();
        }
        temp.transform.position = platformSpawnPosition;
        temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, dir);
        temp.SetActive(true);
    }

    private void AfterSpawnSpike()
    {
        if (afterSpawnSpikeSpawnCount > 0)
        {
            afterSpawnSpikeSpawnCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject temp = ObjectPool.Instance.GetNormalPlatform();
                if (i == 0)
                {
                    temp.transform.position = platformSpawnPosition;
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPosition = new Vector3(platformSpawnPosition.x + vars.nextXPos
                            , platformSpawnPosition.y + vars.nextYPos, platformSpawnPosition.z);

                    }
                    else
                    {
                        platformSpawnPosition = new Vector3(platformSpawnPosition.x - vars.nextXPos
                            , platformSpawnPosition.y + vars.nextYPos, platformSpawnPosition.z);
                    }
                }
                else
                {
                    temp.transform.position = spikeDirPlatformPos;
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXPos
                            , spikeDirPlatformPos.y + vars.nextYPos, spikeDirPlatformPos.z);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXPos
                            , spikeDirPlatformPos.y + vars.nextYPos, spikeDirPlatformPos.z);
                    }
                }
                temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, 1);
                temp.SetActive(true);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
    }
}

