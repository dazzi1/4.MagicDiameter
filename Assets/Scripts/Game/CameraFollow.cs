using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* @description:摄像机视角跟随
* @author: dazzi
* @time: 2023.07
*/
public class CameraFollow : MonoBehaviour
{
    //玩家位置
    private Transform target;
    //偏移量
    private Vector3 offset;
    private Vector2 velocity;

    private void Update()
    {
        if (target == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            
            target = GameObject.FindGameObjectWithTag("Player").transform;
            offset = target.position - transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x - offset.x,
                ref velocity.x, 0.05f);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y - offset.y,
                ref velocity.y, 0.05f);
            //解决抖动
            if (posY > transform.position.y)
            {
                //实现跟随
                transform.position = new Vector3(posX, posY, transform.position.z);
            }

        }

    }
}
