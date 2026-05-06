using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using static Interaction;

public class InteractionContext {
    public InteractionType Type;

    public Dictionary<RoleType, LaboratoryObject> Roles = new();

    public HashSet<string> Tags = new();

    public bool HasTag(string tag) => Tags.Contains(tag);

    public override string ToString() {
        var rolesStr = string.Join(", ", Roles.Select(kv => $"{kv.Key}: {kv.Value.Identifier}"));
        var tagsStr = string.Join(", ", Tags);
        return $"InteractionContext(Type={Type}, Roles={{ {rolesStr} }}, Tags=[{tagsStr}])";
    }
}