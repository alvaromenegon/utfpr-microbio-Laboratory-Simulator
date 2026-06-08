
using System;
using System.Collections.Generic;

[Serializable]
public class BackgroundProcess {
    public string Id { get; set; }
    public string Description { get; set; }

    public List<Condition> StartConditions;
    public List<Condition> CancelConditions;
    public List<Condition> SuccessConditions;
    public ObjectReference BoundObject;

    public List<string> OnStartEffectIds;
    public List<string> OnSuccessEffectIds;
    public List<string> OnFailureEffectIds;
    public int Duration { get; set; }
    public int? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }

    public override string ToString() {
        return $"BackgroundProcess(Id={Id}, Description={Description},BoundObject={BoundObject}, Duration={Duration}, Tolerance={Tolerance}, StartConditions=[{string.Join(", ", StartConditions)}], CancelConditions=[{string.Join(", ", CancelConditions)}], SuccessConditions=[{string.Join(", ", SuccessConditions)}], OnStartEffectIds={OnStartEffectIds}, OnSuccessEffectIds={OnSuccessEffectIds}, OnFailureEffectIds={OnFailureEffectIds}, OnSuccessMessage={OnSuccessMessage}, OnFailureMessage={OnFailureMessage}, ActionOnFailure={ActionOnFailure})";
    }

}