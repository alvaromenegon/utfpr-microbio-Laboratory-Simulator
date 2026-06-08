using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public class LoggerService {
        public string ClassName { get; private set; }

        public LoggerService(string className) {
            ClassName = className;
        }

        public void Log(string message) {
            Debug.Log($"[{ClassName} - {System.DateTime.Now}] {message}");
        }

        public void Error(string message) {
            Debug.LogError($"[Error at {ClassName} - {System.DateTime.Now}] {message}");
        }

        public void Warning(string message) {
            Debug.LogWarning($"[Warning at {ClassName} - {System.DateTime.Now}] {message}");
        }
    }
}