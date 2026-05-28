using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class VisualSlot : MonoBehaviour {
    public string SlotId;

    [Header("Material Target")]
    public Renderer TargetRenderer;
    public int MaterialIndex = 0;

    [Header("Image Target")]
    public Image TargetImage;

    private LaboratoryObject owner;

    private void SetOwner() {
        owner = GetComponentInParent<LaboratoryObject>();
        if (owner == null) { // se não encontrar um LaboratoryObject no pai, tenta encontrar em si mesmo
            owner = GetComponent<LaboratoryObject>();
            Debug.LogWarning($"VisualSlot '{name}' não encontrou um LaboratoryObject no pai, tentando encontrar em si mesmo.");
        }
        if (owner == null) {
            Debug.LogError($"VisualSlot '{name}' não encontrou um LaboratoryObject no pai ou em si mesmo.");
        }
        else {
            Debug.Log($"VisualSlot '{name}' encontrou o LaboratoryObject com InstanceId: {owner.InstanceId}");
        }
    }

    private void Awake() {
        Debug.Log($"VisualSlot '{name}' Awake called. Attempting to find LaboratoryObject in parent.");
        if (owner == null) {
            SetOwner();
        }
    }

    public void SetOwner(LaboratoryObject owner) {
        this.owner = owner;
        Debug.Log($"VisualSlot '{name}' owner set to LaboratoryObject with InstanceId: {owner?.InstanceId ?? "None"}");
    }

    public LaboratoryObject Owner => owner;

    public override string ToString() => $"VisualSlot(SlotId: {SlotId}, Owner: {Owner?.InstanceId ?? "None"}, TargetRenderer: {(TargetRenderer != null ? TargetRenderer.name : null ?? "None")}, MaterialIndex: {MaterialIndex})";
}