using System;

[Serializable]
public class ReactiveEffectDto {
    public string Id { get; set; }
    public WhenDto[] Conditions { get; set; }
    public EffectOperationDto[] Operations { get; set; }
    public ObjectReferenceDto Subject { get; set; } = new ObjectReferenceDto();
    public bool ExecuteOncePerSubject { get; set; } = false;
}
