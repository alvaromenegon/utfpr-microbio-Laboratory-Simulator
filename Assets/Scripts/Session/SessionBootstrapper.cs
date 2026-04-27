using UnityEngine;

[DefaultExecutionOrder(-200)]
public class SessionBootstrapper : MonoBehaviour {
    public static bool IsSessionReady { get; private set; }

    private void Awake() {
        Debug.Log("[SessionBootstrapper] Iniciando sessão...");
        IsSessionReady = SessionData.Initialize();
        if (!IsSessionReady) {
            Debug.LogError("[SessionBootstrapper] Falha ao iniciar sessão.");
            return;
        }

        Debug.Log($"Sessão iniciada → Practice: {SessionData.Current.PracticeId} " +
                  $"| User: {SessionData.Current.UserId} " +
                  $"| Course: {SessionData.Current.CourseId}");
    }
}
