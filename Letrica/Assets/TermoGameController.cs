using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

// Este script cuida APENAS da lógica interna do jogo Termo
public class TermoGameController : MonoBehaviour
{
    // --- Variáveis do Jogo ---
    private const int WORD_LENGTH = 5;
    private const int MAX_GUESSES = 6;
    private int currentRow = 0;
    private int currentCol = 0;
    private string secretWord = "TERMO"; // Palavra secreta (exemplo)

    // --- Referências da UI ---
    private VisualElement root;
    private List<Button> keyboardButtons;
    private Button deleteButton;
    private Button submitButton;
    // (Note que o "menu-button" não está aqui, o UIManager cuida dele)

    // Guarda todos os Labels do grid (6 linhas x 5 colunas)
    private List<List<Label>> gridLabels;

    // Esta função é chamada QUANDO o UIManager "acorda" este script
    void OnEnable()
    {
        Debug.Log("TermoGameController foi ACORDADO!");

        // Pega a UI que o UIManager acabou de carregar
        root = GetComponent<UIDocument>().rootVisualElement;

        QueryUIElements();
        RegisterCallbacks();
        StartNewGame();
    }

    // Esta função é chamada QUANDO o UIManager "desliga" este script
    void OnDisable()
    {
        Debug.Log("TermoGameController foi DORMIR!");

        // Limpa os callbacks para evitar erros de memória
        // (Verifica se as listas não são nulas antes de limpar)
        if (keyboardButtons != null)
        {
            foreach (var button in keyboardButtons)
            {
                // Remove o callback específico para não afetar outros
                button.UnregisterCallback<ClickEvent>(OnLetterPressed);
            }
        }
        if (deleteButton != null)
            deleteButton.UnregisterCallback<ClickEvent>(OnDeletePressed);
        if (submitButton != null)
            submitButton.UnregisterCallback<ClickEvent>(OnSubmitPressed);
    }

    // Encontra os elementos na UI e os armazena
    private void QueryUIElements()
    {
        // Pega TODOS os botões com a classe "key-button"
        keyboardButtons = root.Query<Button>(className: "key-button").ToList();

        // Pega os botões de ação pelo NOME que demos no UXML
        deleteButton = root.Q<Button>("delete-button");
        submitButton = root.Q<Button>("submit-button");

        // --- Mapear o Grid ---
        gridLabels = new List<List<Label>>();

        // Pega todas as linhas
        var rows = root.Query<VisualElement>(className: "grid-row").ToList();

        foreach (var row in rows)
        {
            // Para cada linha, pega todos os "cell-letter" (Labels) dentro dela
            var labelsInRow = row.Query<Label>(className: "cell-letter").ToList();
            gridLabels.Add(labelsInRow);
        }
    }

    // "Escuta" os eventos de clique
    private void RegisterCallbacks()
    {
        // Teclado (para cada tecla, registra uma função)
        foreach (var button in keyboardButtons)
        {
            button.RegisterCallback<ClickEvent>(OnLetterPressed);
        }

        // Botões de Ação
        deleteButton.RegisterCallback<ClickEvent>(OnDeletePressed);
        submitButton.RegisterCallback<ClickEvent>(OnSubmitPressed);
    }


    // --- LÓGICA DAS FUNÇÕES ---

    private void StartNewGame()
    {
        // Você pode pegar uma palavra de uma lista aqui
        secretWord = "CASAS"; // Nova palavra secreta
        currentRow = 0;
        currentCol = 0;

        // Limpa o grid
        foreach (var row in gridLabels)
        {
            foreach (var label in row)
            {
                var cell = label.parent; // .grid-cell
                label.text = "";

                // Remove as classes de feedback (verde, amarelo, cinza)
                cell.RemoveFromClassList("cell--correct");
                cell.RemoveFromClassList("cell--present");
                cell.RemoveFromClassList("cell--absent");
            }
        }

        Debug.Log("Novo Jogo! A palavra é: " + secretWord);
    }

    private void OnLetterPressed(ClickEvent evt)
    {
        // Pega o botão que foi clicado
        var button = evt.currentTarget as Button;

        // Se a linha não estiver cheia
        if (currentCol < WORD_LENGTH)
        {
            gridLabels[currentRow][currentCol].text = button.text;
            currentCol++;
        }
    }

    private void OnDeletePressed(ClickEvent evt)
    {
        // Se a linha não estiver vazia
        if (currentCol > 0)
        {
            currentCol--; // Volta uma coluna
            gridLabels[currentRow][currentCol].text = ""; // Limpa o texto
        }
    }

    private void OnSubmitPressed(ClickEvent evt)
    {
        // 1. Palavra está incompleta?
        if (currentCol != WORD_LENGTH)
        {
            Debug.Log("Palavra incompleta!");
            return;
        }

        // 2. Montar a palavra
        string guessedWord = "";
        for (int i = 0; i < WORD_LENGTH; i++)
        {
            guessedWord += gridLabels[currentRow][i].text;
        }

        // 3. DAR O FEEDBACK (Verde, Amarelo, Cinza)
        for (int i = 0; i < WORD_LENGTH; i++)
        {
            var cell = gridLabels[currentRow][i].parent; // .grid-cell

            if (guessedWord[i] == secretWord[i])
            {
                cell.AddToClassList("cell--correct");
            }
            else if (secretWord.Contains(guessedWord[i]))
            {
                cell.AddToClassList("cell--present");
            }
            else
            {
                cell.AddToClassList("cell--absent");
            }
        }

        // 4. Verificar se o jogo acabou
        if (guessedWord == secretWord)
        {
            Debug.Log("Você Ganhou!");
            // (Travar o teclado aqui, desregistrando os callbacks)
            return;
        }

        // 5. Se não ganhou, ir para a próxima linha
        currentRow++;
        currentCol = 0;

        if (currentRow >= MAX_GUESSES)
        {
            Debug.Log("Você Perdeu! A palavra era: " + secretWord);
        }
    }
}