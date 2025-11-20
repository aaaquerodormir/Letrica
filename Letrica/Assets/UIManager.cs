using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManager : MonoBehaviour
{
    // --- ARRASTE SEUS UXMLs AQUI NO INSPECTOR ---
    [Header("Arquivos UXML das Telas")]
    public VisualTreeAsset uxmlMainMenu;
    public VisualTreeAsset uxmlTermo;
    public VisualTreeAsset uxmlSudoku;
    public VisualTreeAsset uxmlConfiguracoes;

    // --- Componentes Internos ---
    private UIDocument uiDocument;
    private VisualElement root;

    // --- Referências para os Scripts de Lógica ---
    private TermoGameController termoController;
    private SudokuGameController sudokuController;
    //private ConfigController configController;

    void Awake()
    {
        // Pega todos os componentes que estão neste GameObject
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        termoController = GetComponent<TermoGameController>();
         sudokuController = GetComponent<SudokuGameController>();
        // configController = GetComponent<ConfigController>();

        // Desliga todos os scripts de lógica por padrão
        if (termoController != null) termoController.enabled = false;
        if (sudokuController != null) sudokuController.enabled = false;
        // if (configController != null) configController.enabled = false;
    }

    void Start()
    {
        // Carrega a tela inicial (Main Menu)
        LoadScreen(uxmlMainMenu);
    }

    // A função principal que troca as telas
    public void LoadScreen(VisualTreeAsset uxml)
    {
        // 1. Limpa a tela atual
        root.Clear();

        // 2. Desliga todos os scripts de lógica (para garantir)
        if (termoController != null) termoController.enabled = false;
        if (sudokuController != null) sudokuController.enabled = false;
        // if (configController != null) configController.enabled = false;

        // 3. "Clona" (desenha) o novo UXML na tela
        uxml.CloneTree(root);

        // 4. Registra os botões de navegação E ativa o script de lógica certo
        if (uxml == uxmlMainMenu)
        {
            RegisterMainMenuCallbacks();
        }
        else if (uxml == uxmlTermo)
        {
            RegisterBackButton(uxmlMainMenu); // Diz ao botão "Voltar" para carregar o Menu
            if (termoController != null) termoController.enabled = true; // ACORDA o script do Termo!
        }
        else if (uxml == uxmlSudoku)
        {
            RegisterBackButton(uxmlMainMenu);
             if (sudokuController != null) sudokuController.enabled = true; // ACORDA o script do Sudoku
        }
        else if (uxml == uxmlConfiguracoes)
        {
            RegisterBackButton(uxmlMainMenu);
            // if (configController != null) configController.enabled = true; // ACORDA o script de Config
        }
    }

    // Registra os botões do Menu Principal
    private void RegisterMainMenuCallbacks()
    {
        var termoBtn = root.Q<Button>("termo-button");
        var sudokuBtn = root.Q<Button>("sudoku-button");
        var configBtn = root.Q<Button>("config-button");

        if (termoBtn != null)
            termoBtn.RegisterCallback<ClickEvent>(evt => LoadScreen(uxmlTermo));

        if (sudokuBtn != null)
            sudokuBtn.RegisterCallback<ClickEvent>(evt => LoadScreen(uxmlSudoku));

        if (configBtn != null)
            configBtn.RegisterCallback<ClickEvent>(evt => LoadScreen(uxmlConfiguracoes));
    }

    // Registra o botão "Voltar" (menu-button) em uma tela
    private void RegisterBackButton(VisualTreeAsset targetScreen)
    {
        var backBtn = root.Q<Button>("menu-button");
        if (backBtn != null)
            backBtn.RegisterCallback<ClickEvent>(evt => LoadScreen(targetScreen));
    }
}