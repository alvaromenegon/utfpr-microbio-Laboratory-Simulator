using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public class LaboratoryObject : MonoBehaviour {
        [SerializeField] private string objectName;
        [SerializeField] private string canonicalIdentifier;

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
