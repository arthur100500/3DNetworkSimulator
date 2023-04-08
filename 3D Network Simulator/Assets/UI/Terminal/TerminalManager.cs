using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Terminal
{
    /*
    * Creates the process and uses it's input and output with unity
    */
    [RequireComponent(typeof(CanvasGroup))]
    public class TerminalManager : MonoBehaviour
    {
        private const int messageHeight = 25;
        [SerializeField] private GameObject DirectoryLine;
        [SerializeField] private GameObject ResponseLine;
        [SerializeField] private TMP_InputField TerminalInput;
        [SerializeField] private GameObject UserInputLine;
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private GameObject MessageList;
        [SerializeField] private TextMeshProUGUI TitleText;
        [SerializeField] private Button CloseButton;
        private readonly Queue<string> messages = new();
        private Canvas baseCanvas;
        private RectTransform baseCanvasRectTransform;
        private Vector3 CachedPosition;
        private Vector3 CachedScale;
        private CanvasGroup canvasGroup;

        private GNSConsole.GNSConsole console;
        private string lastInput = "";
        private PlayerMovement playerMovement;
        public Canvas ScreenCanvas { private get; set; }

        private void OnGUI()
        {
            InstantiateMessages();

            if (TerminalInput.isFocused && TerminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
            {
                var userInput = TerminalInput.text;
                ClearInputField();
                AddDirectoryLine(userInput);
                Send(userInput);
                lastInput = userInput;
                UserInputLine.transform.SetAsLastSibling();
                TerminalInput.ActivateInputField();
                TerminalInput.Select();
                UserInputLine.SetActive(false);
                UpdateMessagesHeight();
            }

            if (Input.GetKeyDown(KeyCode.T))
                Hide();

            UserInputLine.transform.SetAsLastSibling();
        }

        public void SetTitle(string title)
        {
            TitleText.text = title;
        }

        public void Initialize(Canvas screenCanvas)
        {
            ScreenCanvas = screenCanvas;
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            baseCanvas = gameObject.transform.parent.gameObject.GetComponent<Canvas>();
            baseCanvasRectTransform = gameObject.GetComponent<RectTransform>();
            CachedPosition = baseCanvasRectTransform.localPosition;
            CachedScale = baseCanvasRectTransform.localScale;
            Hide();
            LayoutRebuilder.ForceRebuildLayoutImmediate(MessageList.GetComponent<RectTransform>());
            CloseButton.onClick.AddListener(Hide);
            playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }

        private void Hide()
        {
            SetVisible(false);

            // Unfreeze the player
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerMovement.InControl = true;
        }

        private void UpdateMessagesHeight()
        {
            var msgListSize = MessageList.GetComponent<RectTransform>().sizeDelta;
            MessageList.GetComponent<RectTransform>().sizeDelta =
                new Vector2(msgListSize.x, MessageList.transform.childCount * messageHeight);
            ScrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            ScrollRect.verticalNormalizedPosition = 0;
        }

        private void InstantiateMessages()
        {
            if (messages.Count == 0) return;

            while (messages.Count > 0)
            {
                var stringText = messages.Dequeue();

                if (stringText.Trim().EndsWith(">"))
                {
                    UserInputLine.GetComponentsInChildren<TextMeshProUGUI>()[0].text = stringText;
                    UserInputLine.SetActive(true);
                    continue;
                }

                var msg = Instantiate(ResponseLine, MessageList.transform);
                msg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stringText;
                msg.transform.SetAsLastSibling();
            }

            ScrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            ScrollRect.verticalNormalizedPosition = 0;

            UpdateMessagesHeight();
            LayoutRebuilder.ForceRebuildLayoutImmediate(MessageList.GetComponent<RectTransform>());
        }

        private void ClearInputField()
        {
            TerminalInput.text = "";
        }

        private void AddDirectoryLine(string userInput)
        {
            var msg = Instantiate(DirectoryLine, MessageList.transform);
            msg.transform.SetSiblingIndex(MessageList.transform.childCount - 1);
            msg.GetComponentsInChildren<TextMeshProUGUI>()[1].text = userInput;
        }

        public void LinkTo(GNSConsole.GNSConsole console)
        {
            this.console = console;
            console.AddOnMessageListener(DisplayMessage);
        }

        private void DisplayMessage(byte[] text)
        {
            var stringText = Encoding.ASCII.GetString(text);

            foreach (var line in stringText.Split('\n'))
                if (ValidateLine(line))
                    messages.Enqueue(ReplaceCVTS(line));
        }

        private bool ValidateLine(string line)
        {
            if (line.Trim() == lastInput.Trim()) return false;
            if (line.Contains("??????")) return false;

            return true;
        }

        public void Show()
        {
            SetVisible(true);

            TerminalInput.ActivateInputField();
            TerminalInput.Select();

            // Freeze the player
            playerMovement.InControl = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void SetVisible(bool state)
        {
            canvasGroup.interactable = state;
            // canvasGroup.alpha = state ? 1 : 0;
            ChangeCanvas(state);
            canvasGroup.blocksRaycasts = state;
        }

        private void ChangeCanvas(bool active)
        {
            if (active)
            {
                gameObject.transform.SetParent(baseCanvas.transform);

                transform.SetLocalPositionAndRotation(CachedPosition, Quaternion.Euler(Vector3.zero));
                transform.localScale = CachedScale;

                baseCanvasRectTransform.localPosition = CachedPosition;
                baseCanvasRectTransform.localScale = CachedScale;
            }
            else
            {
                gameObject.transform.SetParent(ScreenCanvas.transform);

                transform.localScale = Vector3.one * 1.5f;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        private void Send(string msg)
        {
            console.SendMessage(msg);
            console.SendMessage("\n");
        }

        private string ReplaceCVTS(string response)
        {
            return Regex.Replace(response, "\\[\\dm", "");
        }
    }
}