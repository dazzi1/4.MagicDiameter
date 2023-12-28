using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/**
* @description:Íæ¼Ò¿ØÖÆ
* @author: dazzi
* @time: 2023.07
*/
public class PlayerController : MonoBehaviour
{
    public Transform rayDown, rayLeft, rayRight;
    public LayerMask platformLayer, obstacleLayer;
    //ÊÇ·ñÏò×óÒÆ¶¯
    private bool isMoveLeft = false;
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_Body;
    //ÊÇ·ñÕýÔÚÌøÔ¾
    private bool IsJumping = false;
    private SpriteRenderer spriteRenderer;
    private bool isMove = false;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_Body = GetComponent<Rigidbody2D>();
        m_AudioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());

    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void IsMusicOn(bool value)
    {
        m_AudioSource.mute = !value;
    }
    private void ChangeSkin(int skinIndex) {
        spriteRenderer.sprite = vars.characterSkinSpriteList[skinIndex];
    }
    private bool IsPointerOverGameObject(Vector2 mousePositon) {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePositon;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }

    void Update()
    {
        /*if (Application.platform == RuntimePlatform.Android)
        {
            int fingerId = Input.GetTouch(0).fingerId;
            if (EventSystem.current.IsPointerOverGameObject(fingerId)) return;
        }
        else {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }*/
        if (IsPointerOverGameObject(Input.mousePosition)) return;
        
        if (GameManager.Instance.IsGameStarted == false||GameManager.Instance.IsGameOver
                                                       ||GameManager.Instance.IsPause)
            return;
        if (Input.GetMouseButtonDown(0)&&IsJumping == false&&nextPlatformLeft!=Vector3.zero)
        {

            if (isMove == false)
            {
                EventCenter.Broadcast(EventDefine.PlayerMove);
                isMove = true;
            }
            m_AudioSource.PlayOneShot(vars.jumpClip);
            EventCenter.Broadcast(EventDefine.DecidePath);
            IsJumping = true;
            Vector3 mousePos = Input.mousePosition;
            //µã»÷×ó±ßÆÁÄ»
            if (mousePos.x <= Screen.width / 2)
            {
                isMoveLeft = true;

            }
            else if (mousePos.x > Screen.width / 2) //µã»÷ÓÒ±ßÆÁÄ»
            {
                isMoveLeft = false;
            }

            Jump();
        }
        //ÓÎÏ·½áÊø
        if (my_Body.velocity.y < 0&&IsRayPlatform()==false&&GameManager.Instance.IsGameOver == false)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            StartCoroutine(DealyShowGameOverPanel());
        }

        if (IsJumping && IsRayObstacle() && GameManager.Instance.IsGameOver == false)
        {
            m_AudioSource.PlayOneShot(vars.hitClip);
            GameObject go = ObjectPool.Instance.GetDeathEffect();
            go.SetActive(true);
            go.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;
            StartCoroutine(DealyShowGameOverPanel());

        }

        if (transform.position.y - Camera.main.transform.position.y < -6&&GameManager.Instance.IsGameOver==false)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            GameManager.Instance.IsGameOver = true;
            StartCoroutine(DealyShowGameOverPanel());
        }
    }

    IEnumerator DealyShowGameOverPanel() {
        yield return new WaitForSeconds(1.5f);
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
    }
    private GameObject lastHitGo = null;
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                if (lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }

                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }

                
                return true;
            }

            
        }
        return false;
    }

    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
            {
                return true;
            }

        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ÌøÔ¾
    /// </summary>
    private void Jump()
    {
        if (isMoveLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);
            transform.DOMoveY(nextPlatformLeft.y+0.8f, 0.15f);
        }
        else
        {
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
            transform.localScale = Vector3.one;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            IsJumping = false;
            Vector3 currentPlatformPos = collision.gameObject.transform.position;
            nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nextXPos, currentPlatformPos.y + vars.nextYPos);
            nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nextXPos, currentPlatformPos.y + vars.nextYPos);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "PickUp") {
            m_AudioSource.PlayOneShot(vars.diamondClip);
            EventCenter.Broadcast(EventDefine.AddDiamond);
            collision.gameObject.SetActive(false);
        }
    }
}
