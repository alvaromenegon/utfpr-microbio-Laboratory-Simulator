public class VisualStateRuleDto {
    public string Id { get; set; }
    public ObjectReferenceDto Subject { get; set; }
    public WhenDto[] Conditions { get; set; }
    public string VisualAssetId { get; set; }
    public int Priority { get; set; }
    public string Slot { get; set; } = string.Empty;
    public object Value { get; set; }
    public string AnimatorParameter { get; set; }
    public object AnimatorValue { get; set; }
    public VisualActionType ActionType { get; set; }

}