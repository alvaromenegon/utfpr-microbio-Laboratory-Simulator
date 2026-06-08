using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ObjectResolver é responsável por resolver referências a objetos em diferentes contextos, como condições e efeitos.
// Ele suporta resolução baseada em papéis, IDs de instância e consultas semânticas.
public class ObjectResolver {
    public ObjectReference Reference { get; private set; }
    public InteractionContext Context { get; private set; } = null;
    public List<LaboratoryObject> ResolvedObjects { get; private set; } = new List<LaboratoryObject>();

    public ObjectResolver(ObjectReference reference, InteractionContext context = null) {
        Reference = reference;
        Context = context;
    }

    private bool IsEmptyReference(ObjectReference reference) {
        return reference == null
            || (
                !reference.Role.HasValue
                && string.IsNullOrEmpty(reference.InstanceId)
                && string.IsNullOrEmpty(reference.CanonicalIdentifier)
                && (reference.RequiredTags == null || reference.RequiredTags.Count == 0)
                && (reference.ExcludedTags == null || reference.ExcludedTags.Count == 0)
            );
    }

    private List<LaboratoryObject> ApplyFilter(List<LaboratoryObject> candidates, ObjectReference reference) {
        ResolvedObjects = candidates.Where(obj => MatchesReference(obj, reference)).ToList();
        return ResolvedObjects;
    }

    public List<LaboratoryObject> Resolve() {
        ResolvedObjects.Clear();
        if (IsEmptyReference(Reference)) {
            Debug.LogWarning("Attempting to resolve an empty reference. Returning empty list.");
            return ResolvedObjects;
        }
        var candidates = new List<LaboratoryObject>();
        // 1. resolve via role/context
        if (Reference.Role.HasValue) {
            if (Context == null) {
                Debug.LogWarning($"ObjectReference uses role {Reference.Role.Value}, but context is null.");
                return ApplyFilter(candidates, Reference);
            }

            if (!Context.Roles.TryGetValue(Reference.Role.Value, out var obj)) {
                Debug.LogWarning($"Role {Reference.Role.Value} not found in context.");
                return ApplyFilter(candidates, Reference);
            }

            candidates.Add(obj);
            return ApplyFilter(candidates, Reference);
                
        }
        // 2. resolve via instance id
        if (!string.IsNullOrEmpty(Reference.InstanceId)) {
            if (ExperimentObjectRegistry.GetInstance().TryGetObjectInstance(Reference.InstanceId, out var obj)) {
                candidates.Add(obj);
                return ApplyFilter(candidates, Reference);
            }

        }
        // 3. semantic query
        
        candidates = ExperimentObjectRegistry.GetInstance().Query(Reference.CanonicalIdentifier, Reference.RequiredTags);
        
            
        return ApplyFilter(candidates, Reference);
    }

    private bool MatchesReference(
    LaboratoryObject obj,
    ObjectReference reference) {
        if (obj == null)
            return false;

        if (!string.IsNullOrEmpty(reference.InstanceId) &&
            obj.InstanceId != reference.InstanceId)
            return false;

        if (!string.IsNullOrEmpty(reference.CanonicalIdentifier) &&
            obj.canonicalIdentifier != reference.CanonicalIdentifier)
            return false;

        // Validação de Tags: o objeto deve conter todas as tags requeridas e não conter nenhuma das tags excluídas
        if (reference.RequiredTags != null) {
            foreach (var tag in reference.RequiredTags) {
                if (obj.Tags == null || !obj.Tags.Contains(tag))
                    return false;
            }
        }

        if (reference.ExcludedTags != null) {
            foreach (var tag in reference.ExcludedTags) {
                if (obj.Tags != null && obj.Tags.Contains(tag))
                    return false;
            }
        }

        // Validação de Zona: o objeto deve estar em uma zona que contenha todas as tags de zona requeridas e não contenha nenhuma das tags de zona excluídas
        if (reference.RequiredZoneTags != null) {
            foreach (var tag in reference.RequiredZoneTags) {
                if (!obj.CurrentZoneTags.Contains(tag))
                    return false;
            }
        }

        if (reference.ExcludedZoneTags != null) {
            foreach (var tag in reference.ExcludedZoneTags) {
                if (obj.CurrentZoneTags.Contains(tag))
                    return false;
            }
        }

        if (reference.RequiredZoneIds != null) {
            foreach (var zoneId in reference.RequiredZoneIds) {
                if (!obj.CurrentZoneIds.Contains(zoneId))
                    return false;
            }
        }

        return true;
    }

    public List<LaboratoryObject> GetResolvedObjects() {
        return ResolvedObjects;
    }
}
