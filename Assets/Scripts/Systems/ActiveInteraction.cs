
using System.Collections.Generic;

public class ActiveInteraction {
    public InteractionContext Context { get; }
    public float ElapsedTime { get; private set; }

    // Lista de efeitos contínuos associados a esta interação ativa
    public List<EffectDefinition> Effects { get; set; } 

    public ActiveInteraction(InteractionContext context) {
        Context = context;
        ElapsedTime = 0f;
        Effects = new List<EffectDefinition>();
    }

    public override bool Equals(object obj) {
        if (obj is ActiveInteraction other) {
            return Context.Equals(other.Context);
        }
        return false;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }


    public override string ToString() {
        return $"ActiveInteraction(Context={Context}, ElapsedTime={ElapsedTime})";
    }
}