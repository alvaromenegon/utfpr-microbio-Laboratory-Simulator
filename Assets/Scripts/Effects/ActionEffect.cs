public class ActionEffect : IEffect {

    public override string ToString() {
        return $"ActionEffect(Id={Id}, Operations=[{string.Join(", ", Operations)}])";
    }
}