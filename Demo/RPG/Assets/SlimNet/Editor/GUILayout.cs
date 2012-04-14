/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public static class SlimNetGUILayout
{
    public class Horizontal : IDisposable
    {
        public Horizontal()
        {
            EditorGUILayout.BeginHorizontal();
        }

        public Horizontal(GUIStyle style)
        {
            EditorGUILayout.BeginHorizontal(style);
        }

        public Horizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }

    public class Vertical : IDisposable
    {
        public Vertical()
        {
            EditorGUILayout.BeginVertical();
        }

        public Vertical(GUIStyle style)
        {
            EditorGUILayout.BeginVertical(style);
        }

        public Vertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }

    public static int IntField(string label, int value)
    {
        using (new Horizontal())
        {
            Label(label, false);
            return EditorGUILayout.IntField(value);
        }
    }

    public static float FloatField(string label, float value)
    {
        using (new Horizontal())
        {
            Label(label, false);
            return EditorGUILayout.FloatField(value);
        }
    }

    public static void Label(string label)
    {
        Label(label, true);
    }

    public static void Label(string label, bool printEmpty)
    {
        Label(label, printEmpty, EditorStyles.label);
    }

    public static void Label(string label, GUIStyle style)
    {
        Label(label, true, style);
    }

    public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
    {
        return GUILayout.Button(text, style, options);
    }

    public static bool Button(string text, params GUILayoutOption[] options)
    {
        return GUILayout.Button(text, options);
    }

    public static void Label(string label, bool printEmpty, GUIStyle style, params GUILayoutOption[] options)
    {
        if (String.IsNullOrEmpty(label) && !printEmpty)
        {
            return;
        }

        GUILayout.Label(label, style, options);
    }

    public static void DoubleLabel(string label1, object label2)
    {
        GUIStyle style1 = new GUIStyle(EditorStyles.label);
        GUIStyle style2 = new GUIStyle(EditorStyles.label);
        style2.fontStyle = FontStyle.Bold;

        using (new Horizontal())
        {
            Label(label1 + ": ", false, style1);
            Label(label2.ToString(), false, style2);
        }
    }

    public static string TextField(string label, string value, params GUILayoutOption[] options)
    {
        return TextField(label, value, EditorStyles.textField, options);
    }

    public static string TextField(string label, string value, GUIStyle style, params GUILayoutOption[] options)
    {
        using (new Horizontal())
        {
            Label(label, false);
            return GUILayout.TextField(value ?? "", style, options);
        }
    }

    public static float PercentSlider(string label, float value, params GUILayoutOption[] options)
    {
        return UnitSlider(label, value, 100, 0, 100, options);
    }

    public static float MillisecondSlider(string label, float value, params GUILayoutOption[] options)
    {
        return UnitSlider(label, value, 1000, 0, 1000, options);
    }

    public static int IntSlider(string label, int value, int min, int max, params GUILayoutOption[] options)
    {
        using (new Horizontal())
        {
            Label(label, false);
            return EditorGUILayout.IntSlider(value, min, max);
        }
    }

    public static string TickLabel(int ticks, int multiplier)
    {
        return String.Format("{1} ms", ticks, ticks * multiplier);
    }

    public static int TickSlider(string label, int value, int min, int max, int multiplier, params GUILayoutOption[] options)
    {
        using (new Horizontal())
        {
            Label(label, false);
            value = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)value, min, max));
            Label(TickLabel(value, multiplier), false, EditorStyles.label, GUILayout.Width(75));

            return value;
        }
    }

    public static float UnitSlider(string label, float value, int unit, int min, int max, params GUILayoutOption[] options)
    {
        using (new Horizontal())
        {
            Label(label, false);
            int intValue = Mathf.RoundToInt((value * (float)unit));
            return (float)EditorGUILayout.IntSlider(intValue, min, max) / (float)unit;
        }
    }

    public static bool Toggle(string label, bool value, params GUILayoutOption[] options)
    {
        return GUILayout.Toggle(value, label, EditorStyles.toggle, options);
    }

    public static bool Toggle(string label, bool value, GUIStyle style, params GUILayoutOption[] options)
    {
        return GUILayout.Toggle(value, label, style, options);
    }

    public static T EnumPopup<T>(string label, T value, params GUILayoutOption[] options)
        where T : struct
    {
        return EnumPopup(label, value, EditorStyles.popup, options);
    }

    public static T EnumPopup<T>(string label, T value, GUIStyle style, params GUILayoutOption[] options)
        where T : struct
    {
        using (new Horizontal())
        {
            Label(label, false);
            return (T)(object)EditorGUILayout.EnumPopup((Enum)(object)value, style, options);
        }
    }

    public static Enum EnumIntPopup(string label, int value, Type type, params GUILayoutOption[] options)
    {
        return EnumIntPopup(label, value, type, EditorStyles.popup, options);
    }

    public static Enum EnumIntPopup(string label, int value, Type type, GUIStyle style, params GUILayoutOption[] options)
    {
        using (new Horizontal())
        {
            Label(label, false);
            return EditorGUILayout.EnumPopup((Enum)Enum.ToObject(type, value), style, options);
        }
    }
}
