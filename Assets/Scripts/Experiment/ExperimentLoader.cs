using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class ExperimentLoader {
    public ExperimentData Load(string path) {
        string json = File.ReadAllText(path);
        ExperimentData data = JsonConvert.DeserializeObject<ExperimentData>(json);
        
        return data;

    }
}