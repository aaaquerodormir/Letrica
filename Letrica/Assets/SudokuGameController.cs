// Nome do arquivo: SudokuGameController.cs
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

// Este script cuida APENAS da lógica interna do jogo Sudoku
public class SudokuGameController : MonoBehaviour
{
    // --- Referências da UI ---
    private VisualElement root;
    private List<Button> numpadButtons;
    private Button clearButton;
    // (O "menu-button" é controlado pelo UIManager)

    // Guarda todos os 81 Labels do grid
    private List<Label> gridLabels;

    // Guarda o Label da célula selecionada no momento
    private Label selectedLabel;

    // String de puzzle (0 ou . para vazio)
    private string puzzleString = "530070000" +
                                "600195000" +
                                "098000060" +
                                "800060003" +
                                "400803001" +
                                "700020006" +
                                "060000280" +
                                "000419005" +
                                "000080079";

    void OnEnable()
    {
        Debug.Log("SudokuGameController foi ACORDADO!");
        root = GetComponent<UIDocument>().rootVisualElement;

        QueryUIElements();
        RegisterCallbacks();
        LoadPuzzle();
    }

    void OnDisable()
    {
        Debug.Log("SudokuGameController foi DORMIR!");

        // Limpa os callbacks para evitar erros
        if (numpadButtons != null)
        {
            foreach (var button in numpadButtons)
            {
                button.UnregisterCallback<ClickEvent>(OnNumpadPressed);
            }
        }
        if (clearButton != null)
            clearButton.UnregisterCallback<ClickEvent>(OnClearPressed);

        if (gridLabels != null)
        {
            foreach (var label in gridLabels)
            {
                // Remove o callback de clique de cada célula
                label.parent.UnregisterCallback<ClickEvent>(OnCellPressed);
            }
        }
    }

    private void QueryUIElements()
    {
        // Pega todos os botões do numpad (assumindo a classe "numpad-key")
        numpadButtons = root.Query<Button>(className: "numpad-key").ToList();

        // Pega o botão de limpar (assumindo o nome "clear-button")
        clearButton = root.Q<Button>("clear-button");

        // Pega todos os labels de número do grid
        gridLabels = root.Query<Label>(className: "cell-number").ToList();
    }

    private void RegisterCallbacks()
    {
        // Registra clique para cada botão do numpad
        foreach (var button in numpadButtons)
        {
            button.RegisterCallback<ClickEvent>(OnNumpadPressed);
        }

        // Registra clique para o botão de limpar
        if (clearButton != null)
            clearButton.RegisterCallback<ClickEvent>(OnClearPressed);

        // Registra clique para cada CÉLULA do grid
        foreach (var label in gridLabels)
        {
            // Registra o clique no "pai" da label (a .sudoku-cell)
            label.parent.RegisterCallback<ClickEvent>(OnCellPressed);
        }
    }

    // Carrega o puzzle inicial
    private void LoadPuzzle()
    {
        for (int i = 0; i < gridLabels.Count; i++)
        {
            var label = gridLabels[i];
            var cell = label.parent; // A .sudoku-cell
            char puzzleChar = puzzleString[i];

            if (puzzleChar != '0' && puzzleChar != '.')
            {
                label.text = puzzleChar.ToString();
                // Adiciona uma classe para "travar" a célula
                cell.AddToClassList("cell--given");
            }
            else
            {
                label.text = "";
                cell.RemoveFromClassList("cell--given");
            }
        }
    }

    // --- Funções de Evento ---

    private void OnCellPressed(ClickEvent evt)
    {
        var clickedCell = evt.currentTarget as VisualElement;

        // Se a célula for uma das "dadas" (iniciais), não faz nada
        if (clickedCell.ClassListContains("cell--given"))
        {
            return;
        }

        // Remove a seleção da célula antiga
        if (selectedLabel != null)
        {
            selectedLabel.parent.RemoveFromClassList("cell--selected");
        }

        // Pega o Label dentro da célula clicada
        selectedLabel = clickedCell.Q<Label>(className: "cell-number");

        // Adiciona a classe de seleção na célula nova
        selectedLabel.parent.AddToClassList("cell--selected");
    }

    private void OnNumpadPressed(ClickEvent evt)
    {
        // Se nenhuma célula estiver selecionada, não faz nada
        if (selectedLabel == null) return;

        // Pega o texto do botão do numpad
        var button = evt.currentTarget as Button;

        // Coloca o número na célula selecionada
        selectedLabel.text = button.text;
    }

    private void OnClearPressed(ClickEvent evt)
    {
        // Se nenhuma célula estiver selecionada, não faz nada
        if (selectedLabel == null) return;

        // Limpa o texto da célula selecionada
        selectedLabel.text = "";
    }
}