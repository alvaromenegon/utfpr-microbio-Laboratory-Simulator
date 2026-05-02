using System.Collections.Generic;

public class ContinuousEffect {
    public Interaction.InteractionType InteractionType;
    
    public string SourceObject; // similar ao InteractionTargetSecondary do Step
    public string TargetObject;  // similar ao InteractionTarget do Step

    public string Property;
    public float Rate;

    public List<When> Conditions;
}