using Assets.Scripts;
using System.Collections.Generic;
using static Interaction;

public class InteractionContext {
    public InteractionType Type;

    public Dictionary<RoleType, LaboratoryObject> Roles = new();

    public HashSet<string> Tags = new();

    public InteractionContext BuildContext(LaboratoryObject currentHeld, LaboratoryObject currentHovered) {
        var ctx = new InteractionContext();

        var held = currentHeld;
        var hovered = currentHovered;

        if (held != null && hovered != null) {
            ctx.Type = InteractionType.USE;

            ctx.Roles[RoleType.TOOL] = held;
            ctx.Roles[RoleType.TARGET] = hovered;
            ctx.Roles[RoleType.ACTOR] = held;
        }
        else if (hovered != null) {
            ctx.Type = InteractionType.ACTIVATE;

            ctx.Roles[RoleType.DEVICE] = hovered;
            ctx.Roles[RoleType.ACTOR] = hovered;
        }

        return ctx;
    }
}