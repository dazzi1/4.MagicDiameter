using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/**
* @description:商店页面
* @author: dazzi
* @time: 2023.07
*/
public class ShopPanel : MonoBehaviour
{
    private ManagerVars vars;
    private Transform parent;
    private Text txt_Name;
    private Text txt_Diamond;
    private Button btn_Back;
    private Button btn_Select;
    private Button btn_Buy;
    private int selectIndex;
    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
        parent = transform.Find("ScroolRect/Parent");
        txt_Name = transform.Find("txt_Name").GetComponent<Text>();
        txt_Diamond = transform.Find("Diamond/Text").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        
        btn_Back.onClick.AddListener(OnBackButtonClick);
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Select.onClick.AddListener(OnSelectButtonClick);

        btn_Buy = transform.Find("btn_Buy").GetComponent<Button>();
        btn_Buy.onClick.AddListener(OnBuyButtonClick);

       vars = ManagerVars.GetManagerVars();
        
        
    }
    private void Start()
    {
        Init();
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void OnBackButtonClick() {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }
    private void OnBuyButtonClick() {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        int price = int.Parse(btn_Buy.GetComponentInChildren<Text>().text);
        if (price > GameManager.Instance.GetAllDiamond()) {
            EventCenter.Broadcast(EventDefine.Hint, "钻石不足");
            return;
        }
        GameManager.Instance.UpdateAllDiamond(-price);
        GameManager.Instance.SetSkinUnlocked(selectIndex);
        parent.GetChild(selectIndex).GetChild(0).GetComponent<Image>().color = Color.white;
    }
    private void OnSelectButtonClick() {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ChangeSkin, selectIndex);
        GameManager.Instance.SetSelectedSkin(selectIndex);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }
    private void Init()
    {
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(160 * (vars.skinSpriteList.Count + 2), 302);
        for (int i = 0; i < vars.skinSpriteList.Count; i++) {
            GameObject go = Instantiate(vars.skinChooseItemPre, parent);
            if (GameManager.Instance.GetSkinUnlocked(i) == false)
            {
                go.GetComponentInChildren<Image>().color = Color.gray;
            }
            else {
                go.GetComponentInChildren<Image>().color = Color.white;
            }
            go.GetComponentInChildren<Image>().sprite= vars.skinSpriteList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        parent.transform.localPosition = 
            new Vector3(GameManager.Instance.GetCurrentSelectedSkin() * -160, 0);

    }
    private void Update()
    {
        selectIndex = (int)Mathf.Round(parent.transform.localPosition.x / -160.0f);
        if (Input.GetMouseButtonUp(0)){
            parent.transform.DOLocalMoveX(selectIndex * -160, 0.2f);
            //parent.transform.localPosition = new Vector3(currentIndex * -160, 0); 
        }
        SetItemSize(selectIndex);
        ReFreshUI(selectIndex);

    }
    private void SetItemSize(int index)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (index == i)
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160);
            }
            else
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
        }
    }
    private void ReFreshUI(int selectIndex) {
        txt_Name.text = vars.skinNameList[selectIndex];
        txt_Diamond.text = GameManager.Instance.GetAllDiamond().ToString();
        if (GameManager.Instance.GetSkinUnlocked(selectIndex) == false)
        {
            btn_Select.gameObject.SetActive(false);
            btn_Buy.gameObject.SetActive(true);
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPrice[selectIndex].ToString();
        }
        else {
            btn_Select.gameObject.SetActive(true);
            btn_Buy.gameObject.SetActive(false);
        }
    }
}
