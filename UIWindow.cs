using UnityEngine;
using UnityEngine.InputSystem;

namespace AlchAssV4;

public static class UIWindow
{
    #region 渲染主体

    /// <summary>
    /// 设置窗口大小
    /// </summary>
    public static void ResizeMenu()
    {
        var rect = new Rect(Variable.MenuRect.xMax - 25, Variable.MenuRect.yMax - 25, 25, 25);
        var mousePos = Mouse.current.position.ReadValue();
        mousePos.y = Screen.height - mousePos.y;
        if (Mouse.current.leftButton.wasPressedThisFrame && rect.Contains(mousePos))
        {
            Variable.IsResizing = true;
            Variable.LastMousePos = mousePos;
        }

        if (Variable.IsResizing)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                var delta = mousePos - Variable.LastMousePos;
                Variable.MenuRect.width = Mathf.Max(450, Variable.MenuRect.width + delta.x);
                Variable.MenuRect.height = Mathf.Max(600, Variable.MenuRect.height + delta.y);
                Variable.LastMousePos = mousePos;
            }
            else
                Variable.IsResizing = false;
        }
    }

    #endregion
}