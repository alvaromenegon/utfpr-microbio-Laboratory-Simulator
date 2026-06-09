using UnityEngine;
using System.Runtime.InteropServices;


public class CloseApplicationHandler {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void QuitWebGLInstance();
#endif

    public static void CloseApplication() {
#if UNITY_WEBGL && !UNITY_EDITOR
        QuitWebGLInstance();
#else
        Application.Quit();
#endif
    }
}