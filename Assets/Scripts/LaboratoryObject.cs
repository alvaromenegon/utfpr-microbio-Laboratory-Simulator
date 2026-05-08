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

        [Obsolete("Use TryGetProperty<T> instead for type safety.")]
        public bool TryGet(string key, out object value) {
            return properties.TryGetValue(key, out value);
        }

        public bool TryGetProperty<T>(string key, out T value) {
            if (properties.TryGetValue(key, out var objValue) && objValue is T typedValue) {
                value = typedValue;
                return true;
            }
            value = default;
            return false;
        }

        public List<string> GetProperties() {
            return new List<string>(properties.Keys);
        }

        [Obsolete("Use GetProperty<T> instead")]
        public T Get<T>(string key) {
            return properties.ContainsKey(key) ? (T)properties[key] : default;
        }

        public T GetProperty<T>(string key) {
            return properties.ContainsKey(key) ? (T)properties[key] : default;
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

        public override string ToString() => $"LaboratoryObject(ID: {Identifier}, Name: {DisplayName})";

        public bool HasProperty(string key) {
            return properties.ContainsKey(key);
        }

        public bool HasTag(string tag) {
            return Tags.Contains(tag);
        }

        public virtual void OnHover() {
            Debug.Log($"Hovering over {DisplayName} (ID: {Identifier})");
        }

        public virtual void OnExitHover() {
            Debug.Log($"Stopped hovering over {DisplayName} (ID: {Identifier})");
        }

    }
}
