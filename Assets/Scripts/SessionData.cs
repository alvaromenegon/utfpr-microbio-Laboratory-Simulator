public class SessionData {
    public int PracticeId { get; private set; }
    public int UserId { get; private set; }
    public int CourseId { get; private set; }
    public string Username { get; set; }
    public string UserPhotoURL { get; set; } = string.Empty;

    public static SessionData Current { get; private set; }

    public static bool Initialize() {
        var p = UrlParamsReader.GetParams();

        if (!int.TryParse(p["practiceId"], out int practiceId) ||
            !int.TryParse(p["userId"], out int userId) ||
            !int.TryParse(p["courseId"], out int courseId)) {
            UnityEngine.Debug.LogError("[SessionData] Parâmetros inválidos ou ausentes na URL.");
            return false;
        }

        Current = new SessionData {
            PracticeId = practiceId,
            UserId = userId,
            CourseId = courseId
        };

        return true;
    }
}