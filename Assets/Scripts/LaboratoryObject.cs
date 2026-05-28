using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Assets.Scripts {
    public class LaboratoryObject : MonoBehaviour, IInteractable {
        public string InstanceId;
        [SerializeField] private string objectName;
        public string canonicalIdentifier;

        private Dictionary<string, object> properties = new();
        public List<string> Tags;
        public string Identifier => canonicalIdentifier;
        public string DisplayName => objectName;

        public HashSet<string> CurrentZoneIds { get; private set; } = new();
        public HashSet<string> CurrentZoneTags { get; private set; } = new();

        private readonly HashSet<EnvironmentZone> currentZones = new();

        public void EnterZone(EnvironmentZone zone) {
            currentZones.Add(zone);
            RebuildZoneTags();
            Debug.Log($"{DisplayName} (ID: {InstanceId}) entered zone {zone.ZoneId} with tags [{string.Join(", ", zone.Tags)}]");
        }

        public void ExitZone(EnvironmentZone zone) {
            currentZones.Remove(zone);
            RebuildZoneTags();
            Debug.Log($"{DisplayName} (ID: {InstanceId}) exited zone {zone.ZoneId} with tags [{string.Join(", ", zone.Tags)}]");
        }

        private void RebuildZoneTags() {
            CurrentZoneIds.Clear();
            CurrentZoneTags.Clear();

            foreach (var zone in currentZones) {
                CurrentZoneIds.Add(zone.ZoneId);

                foreach (var tag in zone.Tags)
                    CurrentZoneTags.Add(tag);
            }
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

        public Dictionary<string, object> GetAllProperties() {
            return new Dictionary<string, object>(properties);
        }

        public T GetProperty<T>(string key) {
            return properties.ContainsKey(key) ? (T)properties[key] : default;
        }

        public void SetProperty(string key, object value) {
            properties[key] = value;
        }

        public override string ToString() => $"LaboratoryObject(InstanceId: {InstanceId}, Name: {DisplayName}, CanonicalIdentifier: {canonicalIdentifier})";

        public bool HasProperty(string key) {
            return properties.ContainsKey(key);
        }

        public bool HasTag(string tag) {
            return Tags.Contains(tag);
        }

        public bool HasAnyTag(List<string> tags) {
            foreach (var tag in tags) {
                if (Tags.Contains(tag)) {
                    return true;
                }
            }
            return false;
        }

        public virtual void OnHover() {
            Debug.Log($"Hovering over {DisplayName} (InstanceId: {InstanceId}), Properties: [{string.Join(", ", properties.Keys)}], Tags: [{string.Join(", ", Tags)}], ZoneIDs: [{string.Join(", ", CurrentZoneIds)}], ZoneTags: [{string.Join(", ", CurrentZoneTags)}]");
        }

        public virtual void OnExitHover() {
            Debug.Log($"Stopped hovering over {DisplayName} (ID: {InstanceId})");
        }


        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<EnvironmentZone>(out var zone)) {
                EnterZone(zone);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent<EnvironmentZone>(out var zone)) {
                ExitZone(zone);
            }
        }

    }
}
