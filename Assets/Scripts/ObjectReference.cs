using System.Collections.Generic;

public class ObjectReference {
    public string InstanceId;

    public string CanonicalIdentifier;

    public RoleType? Role;

    public List<string> RequiredTags;
    public List<string> ExcludedTags;


    public List<string> RequiredZoneTags;
    public List<string> ExcludedZoneTags;
    public List<string> RequiredZoneIds;

    public override string ToString() {
        return $"ObjectReference(InstanceId: {InstanceId}, CanonicalIdentifier: {CanonicalIdentifier}, Role: {Role}, RequiredTags: [{string.Join(", ", RequiredTags)}], ExcludedTags: [{string.Join(", ", ExcludedTags)}], RequiredZoneTags: [{string.Join(", ", RequiredZoneTags)}], ExcludedZoneTags: [{string.Join(", ", ExcludedZoneTags)}], RequiredZoneIds: [{string.Join(", ", RequiredZoneIds)}])";
    }
}