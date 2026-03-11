// Exemplo de como configurar o InputFacade programaticamente
// Este script pode ser usado como referência ou para setup automático

using UnityEngine;

[ExecuteInEditMode]
public class InputFacadeSetupExample : MonoBehaviour
{
    [Header("Auto Setup on Play")]
    [SerializeField] private bool autoSetup = true;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CursorChanger cursorChanger;
    
    [Header("Settings")]
    [SerializeField] private float interactionRange = 1.3f;
    [SerializeField] private LayerMask interactableLayer = -1;

    private void Start()
    {
        if (!autoSetup || !Application.isPlaying)
            return;

        SetupInputFacade();
    }

    [ContextMenu("Setup InputFacade")]
    public void SetupInputFacade()
    {
        // Encontra o InputFacade na cena
        InputFacade inputFacade = FindFirstObjectByType<InputFacade>();
        
        if (inputFacade == null)
        {
            Debug.LogError("InputFacade não encontrado na cena!");
            return;
        }

        // Configura a câmera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Nenhuma câmera atribuída e Camera.main não encontrada!");
                return;
            }
        }

        // Configura o CursorChanger
        if (cursorChanger == null)
        {
            cursorChanger = FindFirstObjectByType<CursorChanger>();
            if (cursorChanger == null)
            {
                Debug.LogWarning("CursorChanger não encontrado na cena!");
            }
        }

        // Atribui valores via reflexão (já que InputFacade não tem setters públicos)
        // Em produção, seria melhor adicionar setters públicos ao InputFacade
        var type = typeof(InputFacade);
        
        var cameraField = type.GetField("playerCamera");
        if (cameraField != null)
        {
            cameraField.SetValue(inputFacade, mainCamera.transform);
            Debug.Log($"InputFacade.playerCamera configurado: {mainCamera.name}");
        }

        var cursorField = type.GetField("cursorChanger");
        if (cursorField != null)
        {
            cursorField.SetValue(inputFacade, cursorChanger);
            Debug.Log($"InputFacade.cursorChanger configurado: {cursorChanger?.name ?? "null"}");
        }

        var rangeField = type.GetField("interactionRange");
        if (rangeField != null)
        {
            rangeField.SetValue(inputFacade, interactionRange);
            Debug.Log($"InputFacade.interactionRange configurado: {interactionRange}");
        }

        var layerField = type.GetField("interactableLayer");
        if (layerField != null)
        {
            layerField.SetValue(inputFacade, interactableLayer);
            Debug.Log($"InputFacade.interactableLayer configurado");
        }

        Debug.Log("InputFacade configurado com sucesso!");
    }

    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        InputFacade inputFacade = FindFirstObjectByType<InputFacade>();
        
        if (inputFacade == null)
        {
            Debug.LogError("? InputFacade não encontrado na cena!");
            return;
        }

        var type = typeof(InputFacade);
        bool isValid = true;

        // Verifica playerCamera
        var cameraField = type.GetField("playerCamera");
        if (cameraField != null)
        {
            var camera = cameraField.GetValue(inputFacade);
            if (camera == null)
            {
                Debug.LogError("? InputFacade.playerCamera não está configurado!");
                isValid = false;
            }
            else
            {
                Debug.Log($"? InputFacade.playerCamera: {camera}");
            }
        }

        // Verifica cursorChanger
        var cursorField = type.GetField("cursorChanger");
        if (cursorField != null)
        {
            var cursor = cursorField.GetValue(inputFacade);
            if (cursor == null)
            {
                Debug.LogWarning("? InputFacade.cursorChanger não está configurado (opcional)");
            }
            else
            {
                Debug.Log($"? InputFacade.cursorChanger: {cursor}");
            }
        }

        if (isValid)
        {
            Debug.Log("? InputFacade está configurado corretamente!");
        }
    }
}
