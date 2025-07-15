using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text;
using RPG_System.API;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPTerminal : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // Références UI
    private TextMeshProUGUI textDisplay;

    [Header("Prompt")]
    [SerializeField] private string promptSymbol = "> ";
    [SerializeField] private Color promptColor   = new(0.7f, 0.7f, 0.7f);

    [Header("Couleurs")]
    [SerializeField] private Color inputColor     = Color.black;
    [SerializeField] private Color validColor     = new(0.3f, 1f, 0.3f);
    [SerializeField] private Color invalidColor   = new(1f, 0.5f, 0.3f);
    [SerializeField] private Color cursorColor    = Color.black;
    [SerializeField] private Color selectionColor = new(0.4f, 0.6f, 1f, 0.5f);

    [Header("Curseur")]
    [SerializeField] private string cursorSymbol = "|";

    // État interne
    private StringBuilder buffer       = new();
    private StringBuilder currentInput = new();
    private int cursorPosition = 0; // 0..currentInput.Length
    private readonly char spacer = '\u2005'; // four-per-em space

    // Sélection
    private bool isSelecting     = false;
    private int  selectionAnchor = 0;
    private int  selectionStart  = 0;
    private int  selectionEnd    = 0;

    private bool isFocused = true;

    void Awake()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
        textDisplay.characterSpacing = -10f;
        buffer.Append(SpacedColored(promptSymbol, promptColor));
        RefreshDisplay();
    }

    void Update()
    {
        if (!isFocused) return;

        bool shift = Input.GetKey(KeyCode.LeftShift)  || Input.GetKey(KeyCode.RightShift);
        bool ctrl  = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // Ctrl+X (cut)
        if (ctrl && Input.GetKeyDown(KeyCode.X))
        {
            if (HasSelection())
            {
                string toCut = currentInput.ToString(selectionStart, selectionEnd - selectionStart);
                GUIUtility.systemCopyBuffer = toCut;
                DeleteSelection();
                RefreshDisplay();
            }
            return;
        }

        // Ctrl+C (copy)
        if (ctrl && Input.GetKeyDown(KeyCode.C))
        {
            string toCopy = HasSelection()
                ? currentInput.ToString(selectionStart, selectionEnd - selectionStart)
                : currentInput.ToString();
            GUIUtility.systemCopyBuffer = toCopy;
            return;
        }

        // Ctrl+←/→ saut mot à mot
        if (ctrl && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var s = currentInput.ToString();
            int idx = s.LastIndexOf(' ', Mathf.Max(0, cursorPosition - 1));
            MoveCursorTo(idx >= 0 ? idx : 0, shift);
            return;
        }
        if (ctrl && Input.GetKeyDown(KeyCode.RightArrow))
        {
            var s = currentInput.ToString();
            int idx = s.IndexOf(' ', cursorPosition);
            MoveCursorTo(idx >= 0 ? idx + 1 : s.Length, shift);
            return;
        }

        // Saisie / Backspace / Entrée
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (HasSelection()) DeleteSelection();
                else if (cursorPosition > 0)
                {
                    currentInput.Remove(cursorPosition - 1, 1);
                    cursorPosition--;
                }
                CancelSelection();
            }
            else if (c == '\n' || c == '\r')
            {
                ProcessCommand();
            }
            else
            {
                if (HasSelection()) DeleteSelection();
                currentInput.Insert(cursorPosition, c);
                cursorPosition++;
                CancelSelection();
            }
            RefreshDisplay();
        }

        // ←/→ simples
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveCursorTo(cursorPosition - 1, shift);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveCursorTo(cursorPosition + 1, shift);

        // Ctrl+V (paste)
        if (ctrl && Input.GetKeyDown(KeyCode.V))
        {
            if (HasSelection()) DeleteSelection();
            string clip = GUIUtility.systemCopyBuffer;
            currentInput.Insert(cursorPosition, clip);
            cursorPosition += clip.Length;
            CancelSelection();
            RefreshDisplay();
        }
    }

    // Clic souris : positionne curseur et initialise sélection
    public void OnPointerDown(PointerEventData eventData)
    {
        PlaceCursorFromMouse(eventData, shift: false);
    }

    // Drag souris : étend la sélection
    public void OnDrag(PointerEventData eventData)
    {
        PlaceCursorFromMouse(eventData, shift: true);
    }

    private void PlaceCursorFromMouse(PointerEventData eventData, bool shift)
    {
        // On essaie d’abord de trouver le caractère exact sous la souris
        int charIndexVis = TMP_TextUtilities.FindIntersectingCharacter(
            textDisplay, eventData.position, eventData.pressEventCamera, true);

        int newPos;
        if (charIndexVis == -1)
        {
            // Pas de caractère ciblé → on est hors du texte : curseur à la fin
            newPos = currentInput.Length;
        }
        else
        {
            // Force update pour être sûr d’avoir textInfo à jour
            textDisplay.ForceMeshUpdate();
            int totalVis = textDisplay.textInfo.characterCount;
            // Nombre de visibles correspondant à l’input
            int visInput = currentInput.Length * 2
                        + ((cursorPosition == currentInput.Length) ? 1 : 0);
            int bufferVis = totalVis - visInput;

            int inputIndexVis = charIndexVis - bufferVis;
            // Si on clique sur un spacer ou sur un vrai char, on ramène /2
            newPos = Mathf.Clamp(inputIndexVis / 2, 0, currentInput.Length);
        }

        MoveCursorTo(newPos, shift);
    }


    private void MoveCursorTo(int newPos, bool shift)
    {
        newPos = Mathf.Clamp(newPos, 0, currentInput.Length);
        if (shift)
        {
            if (!isSelecting)
            {
                isSelecting = true;
                selectionAnchor = cursorPosition;
            }
            cursorPosition = newPos;
            UpdateSelectionRange();
        }
        else
        {
            cursorPosition = newPos;
            CancelSelection();
        }
        RefreshDisplay();
    }

    private void UpdateSelectionRange()
    {
        selectionStart = Mathf.Min(selectionAnchor, cursorPosition);
        selectionEnd   = Mathf.Max(selectionAnchor, cursorPosition);
    }

    private bool HasSelection() => isSelecting && selectionEnd > selectionStart;

    private void CancelSelection()
    {
        isSelecting = false;
        selectionStart = selectionEnd = 0;
    }

    private void DeleteSelection()
    {
        int len = selectionEnd - selectionStart;
        currentInput.Remove(selectionStart, len);
        cursorPosition = selectionStart;
        CancelSelection();
    }

    private void ProcessCommand()
    {
        string cmd = currentInput.ToString();
        var router = new stringToFunction();
        bool ok = router.Execute(cmd, out string outText);

        // 1) Prompt + commande, resserrés à 80%
        buffer.Append(
            $"<line-height=80%>" +
            $"{SpacedColored(promptSymbol + cmd, inputColor)}\n" +
            $"</line-height>"
        );

        // 2) Réponse, même resserrement
        buffer.Append(
            $"<line-height=80%>" +
            $"{SpacedColored(outText, ok ? validColor : invalidColor)}\n" +
            $"</line-height>"
        );

        // 3) Ligne vide “tampon” pour grand saut (100%)
        buffer.Append(
            $"<line-height=100%>" +
            "\u200B\n" +  // caractère invisible pour forcer la ligne
            $"</line-height>"
        );

        // 4) Nouveau prompt (à 80% de hauteur de ligne)
        currentInput.Clear();
        cursorPosition = 0;
        CancelSelection();
        buffer.Append(
            $"<line-height=80%>{SpacedColored(promptSymbol, promptColor)}</line-height>"
        );

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        var sb = new StringBuilder(buffer.ToString());

        for (int i = 0; i < currentInput.Length; i++)
        {
            bool sel = HasSelection() && i >= selectionStart && i < selectionEnd;

            // spacer/curseur/surligné
            if (i == cursorPosition)
                sb.Append(GetColored(cursorSymbol, cursorColor));
            else if (sel)
                sb.Append(GetMarked(spacer.ToString(), selectionColor));
            else
                sb.Append(spacer);

            // caractère réel
            string ch = currentInput[i].ToString();
            sb.Append(sel
                ? GetMarked(ch, selectionColor)
                : GetColored(ch, inputColor));
        }
        if (cursorPosition == currentInput.Length)
            sb.Append(GetColored(cursorSymbol, cursorColor));

        textDisplay.text = sb.ToString();
    }

    private string SpacedColored(string text, Color col)
    {
        var sb = new StringBuilder();
        string hex = ColorUtility.ToHtmlStringRGB(col);
        sb.Append($"<color=#{hex}>");
        foreach (char c in text)
        {
            sb.Append(spacer);
            sb.Append(c);
        }
        sb.Append("</color>");
        return sb.ToString();
    }

    private string GetColored(string txt, Color col)
        => $"<color=#{ColorUtility.ToHtmlStringRGB(col)}>{txt}</color>";

    private string GetMarked(string txt, Color bg)
    {
        string fgHex = ColorUtility.ToHtmlStringRGB(inputColor);
        string bgHex = ColorUtility.ToHtmlStringRGBA(bg);
        return $"<mark=#{bgHex}><color=#{fgHex}>{txt}</color></mark>";
    }
}
