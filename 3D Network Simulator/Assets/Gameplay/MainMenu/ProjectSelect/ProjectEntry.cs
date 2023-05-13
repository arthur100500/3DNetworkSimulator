using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menu.ProjectSelect
{
    public class ProjectEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text textName;
        [SerializeField] private Button button;
        
        public void SetText(string text)
        {
            textName.text = text;
        }
        
        public void AddOnClickListener(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}
