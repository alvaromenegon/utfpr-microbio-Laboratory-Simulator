using Assets.Scripts;
using static Interaction;

public class ActionExecutor {
    private ExperimentObjectRegistry registry;

    public void Execute(PlayerAction action) {
        registry.TryGetInteractable(action.Target.GetIdentifier(), out var obj);

        switch (action.InteractionType) {
            case InteractionType.ACTIVATE:
                obj.Set("isOn", true);
                break;

            case InteractionType.DEACTIVATE:
                obj.Set("isOn", false);
                break;
            case InteractionType.SET_TIMER:
                obj.Set("timer", action.Value);
                break;
            case InteractionType.SET_TEMPERATURE:
                obj.Set("temperature", action.Value);
                break;
            case InteractionType.SET_QUANTITY:
                obj.Set("quantity", action.Value);
                break;

            case InteractionType.SET_PROPERTY:
                obj.Set(action.Property, action.Value);
                break;
        }
    }
}