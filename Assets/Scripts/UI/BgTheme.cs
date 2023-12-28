using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/**
* @description:¸ü»»ÓÎÏ·Ò³Ãæ±³¾°
* @author: dazzi
* @time: 2023.07
*/
public class BgTheme : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    private ManagerVars vars;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        int ranValue = Random.Range(0, vars.bgThemeSpriteList.Count);
        m_SpriteRenderer.sprite = vars.bgThemeSpriteList[ranValue];
    }

}
