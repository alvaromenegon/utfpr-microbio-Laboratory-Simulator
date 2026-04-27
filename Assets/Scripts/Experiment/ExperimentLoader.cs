using Newtonsoft.Json;
using System.IO;

public class ExperimentLoader {
    public ExperimentData Load(string path) {
        string json = File.ReadAllText(path);
        var dataDto = JsonConvert.DeserializeObject<ExperimentDataDto>(json);
        return ExperimentMapper.ToModel(dataDto);
    }
}
