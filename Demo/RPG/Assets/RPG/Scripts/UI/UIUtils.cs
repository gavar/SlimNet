using UnityEngine;
using System.Collections;

public static class UIUtils
{
    public static Rect Center(float width, float height, float widthOffset, float heightOffset)
    {
        return new Rect(
            (GameSettings.UIHalfWidth - (width / 2f)) + widthOffset,
            (GameSettings.UIHalfHeight - (height / 2f)) + heightOffset,
            width,
            height
        );
    }

    public static GUIStyle ErrorLabel
    {
        get
        {
            GUIStyle style = new GUIStyle("Label");
            style.normal.textColor = Color.red;
            return style;
        }
    }
}