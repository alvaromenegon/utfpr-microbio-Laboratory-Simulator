using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web;
using UnityEngine;

public static class UrlParamsReader {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetQueryString();
#endif

    public static NameValueCollection GetParams() {
#if UNITY_WEBGL && !UNITY_EDITOR
        string qs = GetQueryString(); // ex: "?practiceId=1&userId=2&courseId=3"
#else
        // Valor de fallback para testar dentro do Editor
        Debug.Log("[UrlParamsReader] Usando parâmetros de teste no Editor.");
        string qs = "?practiceId=6&userId=1&courseId=1";
        Debug.Log($"[UrlParamsReader] Parâmetros de teste: {qs}");
#endif
        return HttpUtility.ParseQueryString(qs);
    }
}