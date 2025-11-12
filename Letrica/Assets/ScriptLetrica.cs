using UnityEngine;
using UnityEngine.UIElements;

public class ScreenManager : MonoBehaviour
{
    [Header("UIDocument das Telas")]
    public UIDocument mainMenuDocument;
    public UIDocument configDocument;

    private VisualElement mainMenuRoot;
    private VisualElement configRoot;

    private void Start()
    {
        // Garante que os documentos foram atribuídos
        if (mainMenuDocument == null || configDocument == null)
        {
            Debug.LogError("Os UIDocuments das telas precisam ser atribuídos no Inspector!");
            return;
        }

        // Obtém o elemento raiz de cada UXML
        mainMenuRoot = mainMenuDocument.rootVisualElement;
        configRoot = configDocument.rootVisualElement;

        // Esconde a tela de Configurações no início
        configRoot.style.display = DisplayStyle.None;

        // Conecta o botão "Configurações" na tela principal
        Button settingsButton = mainMenuRoot.Q<Button>("settingss-button"); // Ajuste o nome do botão se for diferente
        if (settingsButton != null)
        {
            settingsButton.RegisterCallback<ClickEvent>(ev => ShowConfigScreen());
        }
        else
        {
            Debug.LogError("Botão 'settingss-button' não encontrado no MainMenu.uxml.");
        }

        // Conecta o botão "Menu" na tela de configurações
        Button homeButton = configRoot.Q<Button>("home-button");
        if (homeButton != null)
        {
            homeButton.RegisterCallback<ClickEvent>(ev => ShowMainMenuScreen());
        }
        else
        {
            Debug.LogError("Botão 'home-button' não encontrado no Configuracoes.uxml.");
        }
    }

    // Função para mostrar a tela de Configurações e esconder o Menu
    public void ShowConfigScreen()
    {
        mainMenuRoot.style.display = DisplayStyle.None;
        configRoot.style.display = DisplayStyle.Flex;
    }

    // Função para mostrar a tela Principal e esconder as Configurações
    public void ShowMainMenuScreen()
    {
        configRoot.style.display = DisplayStyle.None;
        mainMenuRoot.style.display = DisplayStyle.Flex;
    }
}