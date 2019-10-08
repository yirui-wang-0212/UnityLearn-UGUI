# UnityLearn-UGUI



## Unity 界面发展史

- OnGUI
- NGUI
- UGUI



## 基础控件

- Canvas 画布
- Rect Transform
- Image
- Text
- Button 按钮
- Toggle 复选框
- Slider 滑动条
- Scrollbar 滑动条
- Dropdown 下拉菜单
- InputField 输入框
- Panel 



## 分辨率调整

### 方法1：Game 窗口中添加

- Fixed Resolution
- Aspect Ration：比例

### 方法2：修改 Canvas - Inspector - Canvas Scaler(Script)

- Canvas Scaler(Script)

  - Scale With Screen Size

- Reference Match Mode：改为 Fixed Resolution

- Match：选择 Reference Match Mode 的较小边

  

## Component 组件

### Grid Layout Group (Script)

- Layout - Grid Layout Group 表格布局
- 自动排列，控制子元素的大小和位置，子元素不能更改。



## 事件注册

注意 EventSystem 是否可用，Scene 中 EventSystem 是共用的。

共 4 种方法：

### 方法1：通过编辑器方法

优点：方便、所见即所得

缺点：在代码中看不到是谁调用了这个方法，工作中（尤其团队开发）用得最少

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventDemo : MonoBehaviour
{
    // 事件注册的 4 种方法：
    // 1. 通过编辑器方法

    public void Fun1(string input){
       
       print("Fun1: " + input);
   }
}
```

**Button**

1. 将脚本（EventDemo）挂到 GameObject 上（例如 Canvas 上）
2. 在 Button GameObject 的 Inspector 中：Button(Script) - On Click，点击 +，将挂有脚本的 GameObject（例如 Canvas）拖入相应框框里，选择脚本组件（EventDemo）中要调用的方法（Fun1）

![1](Pictures/1.png)

可以为函数传入一个参数：在 Button(Script) - On Click 相应框框中输入参数即可，如下图：

当参数为数组时不可以。

![2](Pictures/2.png)

**InputField**

两种事件：

- On Value Changed (String)：当值改变时触发。

- On End Edit (String)：当输入完成时触发：按下回车键或点击除此 InputField 外的其他地方。

参数可以选择：

- Dynamic string：传入的值即是在 InputField 中输入的值，是动态的，不能自己输入。

- Static Parameters：和 Button 一样，在相应的框框中输入。

![3](Pictures/3.png)

![4](Pictures/4.png)

**其他 Canvas 元素**

### 方法2：AddListener

优点：在代码中能看到是谁调用了这个方法

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventDemo2 : MonoBehaviour
{
    // 事件注册的 4 种方法
    // 2. AddListener
    
    public void Fun2(){
    
        print("Fun2");
    }

    public void Fun3(string str){
    
        print("Fun3: " + str);
    }

    private void Start(){
        
        Button btn = this.transform.Find("AddListener/Button").GetComponent<Button>();
        // public delegate void UnityAction();
        // 需要传入一个 无返回值、无参数列表的方法
        btn.onClick.AddListener(Fun2);

        InputField input = this.transform.Find("AddListener/InputField").GetComponent<InputField>();
        // public delegatevoid UnityAction<T0>(T0 arg0);
        // 需要传入一个 无返回值，1个参数，参数类型为泛型，在这里是 string 的方法
        // input.onValueChange.AddListener(Fun3);
        input.onEndEdit.AddListener(Fun3);
    }
}
```

### 方法3：实现接口

所有的 UI、GameObject 都可以使用

不能挂在 Canvas 上，实现接口的脚本需要挂在有 Raycast Target 的物体上：如 Image、Text、Button 等。

依赖于：

EventSystem 上的 Event System (Script) ：负责分发。

和 Stamdalone Input Module  (Script) ：负责检测鼠标键盘（输入模块）。移动端应将其用 Touch Input Module (Script) 替换。如果是 VR 游戏，则需要重写Stamdalone Input Module 或者使用 VR TK。

以及 Canvas 上的 Graphic Raycaster (Script)：负责检测 Image 和 Text，Image 和 Text 接受事件。对于 Button 等 UI 元素来说也是 Button 上的 Image 和 Text 检测的，若将 Button 上的 Image (Script) Component 中的 Ray Target 和 Button 子 UI 元素 Text 上的 Text (Script) Component 中的 Ray Target 都禁用，Button 无法检测到被按下，事件也不会被触发。

- **鼠标指针类**（PC、移动端）
  - IPointerEnterHandler：移入
  - IPointerExitHandler：移出
  - IPointerDownHandler：按下
  - IPointerUpHandler：抬起
  - PointerClickHandler：按下后抬起
- **拖拽类**
  - IBeginDragHandler：开始拽
  - IDragHandler：正在拽
  - IEndDragHandler：结束拽
  - IDropHandler：释放
- **点选类**（InputField 有，Button 没有）
  - IUpdateSelectedHandler：点了选中后每帧执行
  - ISelectHandler：点了选中后那一帧执行
  - IDeselectHandler：不选了那一帧
- **输入类**（建立在选中的基础上）
  - IScrollHandler：选中 UI 后，滑动鼠标滚轮
  - IMoveHandler：选中 UI 后，移动（按键盘上的上下左右、wasd）
  - ISubmitHandler：选中 UI 后，按回车键
  - ICancelHandler：选中 UI 后，按 Esc 键

#### 实现：双击、拖拽 UI 元素（使之随光标移动，有不足，完整版在下面）

```c#
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
```

#### 实现拖拽 UI 元素（使之随光标移动，完整版）

```c#
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
```

### 方法4：自定义框架

