using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItweenDemo : MonoBehaviour
{
    public Transform imgTF, btnTF;

    // 移动速度
    public float moveSpeed = 100;

    public void DoMovement(){

        // 让图片移动到按钮位置

        // 若使用此方法，每帧都要执行，即需要在 Update 里调用 DoMovment
        // imgTF.position = Vector3.MoveTowards(imgTF.position, btnTF.position, Time.deltaTime * moveSpeed);

        // 使用 iTween 只需要调用 1 次
        // 参数1：需要移动的游戏对象
        // 参数2：需要移动到的位置
        // 参数3：移动的时间
        iTween.MoveTo(imgTF.gameObject, btnTF.position, 2);
    }
}
