using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class LaboratoryObject : MonoBehaviour, IInteractable {
        [SerializeField] private string objectName;
        [SerializeField] private string canonicalIdentifier;

        private Dictionary<string, object> properties = new();
        public List<string> Tags;
        public string Identifier => canonicalIdentifier;
        public string DisplayName => objectName;

        public bool TryGet(string key, out object value) {
            return properties.TryGetValue(key, out value);
        }

        public T Get<T>(string key) {
            return properties.ContainsKey(key) ? (T)properties[key] : default;
        }

        [Obsolete("Use SetProperty method instead.")]
        public void Set(string key, object value) {
            if (properties.ContainsKey(key)) {
                //Debug.Log($"[{canonicalIdentifier}] - Property updated: {key} = {value} (was {properties[key]})");
            } else {
                Debug.Log($"[{canonicalIdentifier}] - Property added: {key} = {value}");
            }
            properties[key] = value;
            //Debug.Log($"[{canonicalIdentifier}] - Property set: {key} = {value}");
        }

        public void SetProperty(string key, object value) {
            if (properties.ContainsKey(key)) {
                //Debug.Log($"[{canonicalIdentifier}] - Property updated: {key} = {value} (was {properties[key]})");
            }
            else {
                Debug.Log($"[{canonicalIdentifier}] - Property added: {key} = {value}");
            }
            properties[key] = value;
            //Debug.Log($"[{canonicalIdentifier}] - Property set: {key} = {value}");
        }

        [System.Obsolete("Use HasProperty method instead.")]
        public bool Has(string key) {
            return properties.ContainsKey(key);
        }

        public bool HasProperty(string key) {
            return properties.ContainsKey(key);
        }

        public bool HasTag(string tag) {
            return Tags.Contains(tag);
        }

        // evitar usar os métodos abaixo
        // mantidos por compatibilidade
        [Obsolete("Use DisplayName instead.")]
        public virtual string GetObjectName() => objectName;

        [Obsolete("Use Identifier instead.")]
        public virtual string GetIdentifier() => canonicalIdentifier;

        public virtual void OnHover() {
            // Implementar comportamento de hover, como destacar o objeto
        }

        public virtual void OnExitHover() {
            // Implementar comportamento quando o hover termina, como remover destaque
        }

    }
}
