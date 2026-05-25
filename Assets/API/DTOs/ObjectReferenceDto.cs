public class ObjectReferenceDto {
    public string InstanceId { get; set; }
    public string CanonicalIdentifier { get; set; }
    public string Role { get; set; }
    public string[] RequiredTags { get; set; }
    public string[] ExcludedTags { get; set; }
    public string[] RequiredZoneIds { get; set; }
    public string[] RequiredZoneTags { get; set; }
    public string[] ExcludedZoneTags { get; set; }

}