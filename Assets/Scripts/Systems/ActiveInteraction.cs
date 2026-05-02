using static Interaction;

public class ActiveInteraction {
    public InteractionType Type;
    public string Source;
    public string Target;

    public override bool Equals(object obj) {
        if (obj is not ActiveInteraction other) return false;
        return Type == other.Type && Source == other.Source && Target == other.Target;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 23 + Type.GetHashCode();
            hash = hash * 23 + (Source != null ? Source.GetHashCode() : 0);
            hash = hash * 23 + (Target != null ? Target.GetHashCode() : 0);
            return hash;
        }
    }
}