using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* @description:平台下落
* @author: dazzi
* @time: 2023.07
*/
public class PlatformScript : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacle;
    private bool startTimer;
    private float fallTime;
    private Rigidbody2D my_Body;

    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
    }

    public void Init(Sprite sprite,float fallTime,int obstacleDir)
    {
        my_Body.bodyType = RigidbodyType2D.Static;
        startTimer = true;
        this.fallTime = fallTime;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;
        }

        if (obstacleDir == 0)
        {
            if (obstacle != null)
            {
                obstacle.transform.localPosition = new Vector3(
                    -obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y,
                    obstacle.transform.localPosition.z);
            }
        }
    }

    private void Update()
    {
        if(GameManager.Instance.IsGameStarted == false||GameManager.Instance.PlayerIsMove == false) return;
        if (startTimer)
        {
            fallTime -= Time.deltaTime;
            if (fallTime < 0)
            {
                startTimer = false;
                if (my_Body.bodyType != RigidbodyType2D.Dynamic)
                {
                    //平台掉落
                    my_Body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DealyHide());
                }
            }
        }

        if (transform.position.y - Camera.main.transform.position.y < -6)
        {
            StartCoroutine(DealyHide());
        }
    }

    private IEnumerator DealyHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

}
