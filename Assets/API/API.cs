public class API {
    private const string BaseUrl = "http://localhost:8000/"; // Substitua pela URL real da API
    public static string GetEndpoint(string path) {
        return $"{BaseUrl}/{path}";
    }


}