using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class LaboratoryObject : MonoBehaviour {
        [SerializeField] private string objectName;
        [SerializeField] private string canonicalIdentifier;

        private Dictionary<string, object> properties = new();

        public T Get<T>(string key) {
            return properties.ContainsKey(key) ? (T)properties[key] : default;
        }

        public void Set(string key, object value) {
            properties[key] = value;
        }

        public bool Has(string key) {
            return properties.ContainsKey(key);
        }

        public virtual string GetObjectName() => objectName;

        public virtual string GetIdentifier() => canonicalIdentifier;

        public virtual void OnHover() {
            // Implementar comportamento de hover, como destacar o objeto
        }

        public virtual void OnExitHover() {
            // Implementar comportamento quando o hover termina, como remover destaque
        }
    }
}
