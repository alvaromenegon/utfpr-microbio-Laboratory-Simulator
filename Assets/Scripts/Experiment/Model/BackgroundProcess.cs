
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BackgroundProcess {
    public string Id { get; set; }
    public string Description { get; set; }

    public List<When> StartConditions;
    public List<When> CancelConditions;
    public List<When> SuccessConditions;

    public string OnSuccessEffectId;
    public string OnFailureEffectId;
    public int Duration { get; set; }
    public int? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }

}