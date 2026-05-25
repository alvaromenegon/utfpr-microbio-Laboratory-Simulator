using System.Collections.Generic;

public class VisualStateRule {
    public string Id;

    public List<When> Conditions;
    public ObjectReference Subject;

    public string VisualAssetId;

    public VisualTargetType TargetType;
    public string Slot;

    public int Priority;
}

public enum VisualTargetType {
    WORLD_OBJECT,
    MICROSCOPE_VIEW,
    UI_PANEL,
    MATERIAL_OVERRIDE
}