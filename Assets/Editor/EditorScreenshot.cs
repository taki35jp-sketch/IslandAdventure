using System;
using UnityEditor;
using UnityEngine;

public static class EditorScreenshot
{
    [MenuItem("Tools/Screenshot _F12")]
    private static void TakeScreenshot()
    {
        var now = DateTime.Now;
        var savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\Screenshot{now.ToFileTime()}.png";
        ScreenCapture.CaptureScreenshot(savePath);
    }
}