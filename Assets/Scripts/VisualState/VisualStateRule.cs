using System.Collections.Generic;

public class VisualStateRule {
    public string Id;

    public List<Condition> Conditions;
    public ObjectReference Subject;

    public string VisualAssetId;

    public VisualActionType ActionType;
    public string Slot;
    public int Priority;
    public object Value;
    public string AnimatorParameter;

    public object AnimatorValue;

    public override string ToString() {
        return $"VisualStateRule(Id={Id}, Subject={Subject}, ActionType={ActionType}, Slot={Slot}, Value={Value}, VisualAsset={VisualAssetId}, Conditions={string.Join(", ", Conditions)})";
    }
}

public enum VisualActionType {
    SET_ACTIVE,
    SET_MATERIAL,
    SET_IMAGE,
    SPAWN_PREFAB,
    PLAY_ANIMATION,
    SHOW_UI_IMAGE,
    PLAY_PARTICLE
}