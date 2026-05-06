using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Registry {
    public class DerivedPropertyRegistry  {
        private readonly Dictionary<string, DerivedProperty> propertiesById = new Dictionary<string, DerivedProperty>();
        private LoggerService logger = new LoggerService("DerivedPropertyRegistry");

        public IEnumerable<DerivedProperty> GetAllProperties() {
            return propertiesById.Values;
        }

        public IEnumerable<DerivedProperty> GetPropertiesByPriority() {
            return GetAllProperties().OrderBy(p => p.Priority);
        }

        public bool TryGetProperty(string identifier, out DerivedProperty property) {
            return propertiesById.TryGetValue(identifier, out property);
        }

        public void SetupRegistry(List<DerivedProperty> properties) {
            foreach (DerivedProperty property in properties) {
                if (string.IsNullOrWhiteSpace(property.Identifier)) continue;
                if (propertiesById.ContainsKey(property.Identifier)) {
                    Debug.LogError($"Duplicate identifier: {property.Identifier}");
                    continue;
                }
                propertiesById.Add(property.Identifier, property);
            }
            logger.Log($"Registry setup with {propertiesById.Count} properties.");

        }



    }
}