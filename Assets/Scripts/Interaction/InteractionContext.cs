using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using static Interaction;

public class InteractionContext : ICloneable {
    public InteractionType Type;

    public Dictionary<RoleType, LaboratoryObject> Roles = new();

    public HashSet<string> Tags = new();

    // implementação direta da interface ICloneable, que retorna um objeto do tipo object
    public object Clone() {
        var clone = (InteractionContext)this.MemberwiseClone();
        clone.Roles = new Dictionary<RoleType, LaboratoryObject>(Roles);
        clone.Tags = new HashSet<string>(Tags);
        return clone;
    }

    // método de conveniência para clonar e já retornar o tipo correto, evitando a necessidade de cast externo
    public InteractionContext CloneAndCast() {
        var clone = Clone();
        return (InteractionContext)clone;
    }

    public bool HasTag(string tag) => Tags.Contains(tag);

    public override string ToString() {
        var rolesStr = string.Join(", ", Roles.Select(kv => $"{kv.Key}: {kv.Value.Identifier}"));
        var tagsStr = string.Join(", ", Tags);
        return $"InteractionContext(Type={Type}, Roles={{ {rolesStr} }}, Tags=[{tagsStr}])";
    }
}