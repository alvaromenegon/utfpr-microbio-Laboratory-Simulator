// como a API retorna um json do tipo {"error": ..., "data": {...}} para as requisições, essa classe é um modelo genérico para representar esse formato de resposta.
// // O tipo T representa o tipo específico de dados que a resposta pode conter, permitindo flexibilidade na manipulação dos dados retornados pela API.
class APIResult<T> {
    public string Error { get; set; }
    public T Data { get; set; }
    public APIResult(string error = null, T data = default)
    {
        Error = error;
        Data = data;
    }

    public bool Ok()
    {
        return Error == null;
    }
}