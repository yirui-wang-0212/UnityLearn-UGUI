using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 方法 3：实现接口 所需引入的命名空间
using UnityEngine.EventSystems;

                                        // 方法 3：实现接口 所需继承的类：鼠标指针类别中的类、拖拽类别中的类等
public class EventDemo3 : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // 事件注册的 4 种方法：
    // 3. 实现接口
    
    // 实现 IPointerClickHandler 接口，去查看 IPointerClickHandler 知道实现接口的函数名，在 Visual Studio 中可以自动生成
    // 当光标单击时执行
    public void OnPointerClick(PointerEventData eventData){
        // eventData 时间参数类：提供了引发事件时的一些信息
        // 判断是否是双击(单击、3击、4击都可以判断)
        if(eventData.clickCount == 2)
            print("OnPointerClick: " + eventData.clickCount);
    }

    // 实现 IDragHandler 接口，去查看 IDragHandler 知道实现接口的函数名，在 Visual Studio 中可以自动生成
    // 当拖拽时执行
    public  void OnDrag(PointerEventData eventData){

        // eventData.position：光标位置（屏幕坐标）

        // 拖动 UI 元素
        // 方法 1
        // 将光标的屏幕坐标直接复赋值给 UI 元素的世界坐标
        // this.transform.position = eventData.position;
        // 遇到问题：
        // 仅仅适用于 Canvas Overlay 模式：Overlay 模式下世界坐标原点和屏幕坐标原点重合
        // 不适用于 Canvas Camera 模式：Camera 模式下世界坐标原点和屏幕坐标原点不重合

        // 拖动 UI 元素
        // 方法 2：通用方法
        // 1. 先将光标的屏幕坐标先转换为世界坐标
        RectTransform parentRTF = this.transform.parent as RectTransform;
        Vector3 worldPos;
        // 参数1：父物体的变换组件，参数2:屏幕坐标，参数3：摄像机，参数4：out 世界坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRTF, eventData.position, eventData.pressEventCamera, out worldPos);
        // 2. 再将光标的世界坐标赋值给 UI 元素的世界坐标
        this.transform.position = worldPos;
        // 遇到问题：当光标在点击在 UI 元素左下角时进行拖拽，拖完结束时光标在 UI 元素的中心
        // 于是调整：见 DialogDrag.cs   
    }

}