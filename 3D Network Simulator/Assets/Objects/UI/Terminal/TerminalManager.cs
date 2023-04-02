using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Terminal
{
    /*
    * Creates the process and uses it's input and output with unity
    */
    public class TerminalManager : MonoBehaviour
    {
        public GameObject DirectoryLine;
        public GameObject ResponseLine;

        public TMP_InputField TerminalInput;
        public GameObject UserInputLine;
        public ScrollRect ScrollRect;
        public GameObject MessageList;

        private bool writtenBefore;


        private void OnGUI()
        {
            if (TerminalInput.isFocused && TerminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
            {
                string userInput = TerminalInput.text;

                ClearInputField();

                AddDirectoryLine(userInput);

                UserInputLine.transform.SetAsLastSibling();

                TerminalInput.ActivateInputField();
                TerminalInput.Select();
            }
        }

        private void ClearInputField()
        {
            TerminalInput.text = "";
        }

        private void AddDirectoryLine(string userInput)
        {
            Vector2 msgListSize = MessageList.GetComponent<RectTransform>().sizeDelta;
            MessageList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 30);

            var msg = Instantiate(DirectoryLine, MessageList.transform);
            msg.transform.SetSiblingIndex(MessageList.transform.childCount - 1);

            msg.GetComponentsInChildren<TextMeshProUGUI>()[1].text = userInput;

            LayoutRebuilder.ForceRebuildLayoutImmediate(MessageList.GetComponent<RectTransform>());
        }
    }
}