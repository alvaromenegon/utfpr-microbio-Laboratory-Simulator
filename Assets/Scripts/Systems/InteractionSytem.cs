using Assets.Scripts;
using System.Collections.Generic;

public class InteractionSystem {
    private List<ActiveInteraction> activeInteractions = new();
    private LoggerService logger = new ("InteractionSystem");

    public void RegisterInteraction(ActiveInteraction interaction) {
        activeInteractions.Add(interaction);
        logger.Log($"Registered interaction: {interaction}");
    }

    public void UnregisterInteraction(ActiveInteraction interaction) {
        activeInteractions.Remove(interaction);
        logger.Log($"Unregistered interaction: {interaction}");
    }

    public void Update() {
        
    }
}