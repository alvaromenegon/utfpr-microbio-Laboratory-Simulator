using System.Collections.Generic;
using static Interaction;

public class InstantEffect {
    public InteractionType InteractionType;

    public string SourceObject;
    public string TargetObject;

    public string Property;
    public object Value;

    public List<When> Conditions;
}