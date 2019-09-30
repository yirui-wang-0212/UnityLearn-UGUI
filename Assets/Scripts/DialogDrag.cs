using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogDrag : MonoBehaviour, IPointerDownHandler, IDragHandler{

    // 父物体的变换组件
    private RectTransform parentRTF;
    // 按下点到 UI 元素中心点的偏移量（坐标）
    private Vector3 offset;

    private void  Start(){
        // 获取父物体的变换组件
        parentRTF = this.transform.parent as RectTransform;
    }

    // 1 当按下当前物体时执行
    public void OnPointerDown(PointerEventData eventData){

        // 1.1 先将光标的屏幕坐标 --> 世界坐标
        Vector3 worldPoint;
        // 参数1：父物体的变换组件，参数2:屏幕坐标，参数3：摄像机，参数4：out 世界坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRTF, eventData.position, eventData.pressEventCamera, out worldPoint);
        // 1.2 计算从按下点到 UI 元素中心点的偏移量（坐标）
        offset = this.transform.position - worldPoint;
    }

    // 2 当拖拽时执行
    public  void OnDrag(PointerEventData eventData){

        // 2.1 先将光标的屏幕坐标 --> 世界坐标
        Vector3 worldPoint;
        // 参数1：父物体的变换组件，参数2:屏幕坐标，参数3：摄像机，参数4：out 世界坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRTF, eventData.position, eventData.pressEventCamera, out worldPoint);
        // 2.2 再将光标的世界坐标赋值给 UI 元素的世界坐标
        this.transform.position = worldPoint + offset;
    }
}