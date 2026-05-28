using System;

[Serializable]
public class ExperimentDataDto {
    public StepDto[] Steps { get; set; }
    public BackgroundProcessDto[] BackgroundProcesses { get; set; }
    public ExperimentObjectDto[] Objects { get; set; }
    public string Description { get; set; }
    public int Time { get; set; }
    public EffectDefinitionDto[] Effects { get; set; }
    public ReactiveEffectDto[] ReactiveEffects { get; set; }
    public ActionEffectDto[] ActionEffects { get; set; }
    public DerivedPropertyDto[] DerivedProperties { get; set; }
    public VisualStateRuleDto[] VisualStateRules { get; set; }
    public VisualAssetDto[] VisualAssets { get; set; } = new VisualAssetDto[0];
}
