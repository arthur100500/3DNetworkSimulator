using System;
using System.Collections;
using System.Text;
using Gameplay.MainMenu.Auth;
using Menu.ProjectSelect;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Gameplay.MainMenu.Register
{
    public class RegisterMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField loginInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button backToLoginButton;
        [SerializeField] private TMP_Text errors;
        [SerializeField] private AuthMenu auth;

        private const string RequestUrl = "http://127.0.0.1:10203/register";
        
        private void Start()
        {
            submitButton.onClick.AddListener(TryRegister);
            backToLoginButton.onClick.AddListener(ActivateAuth);
        }

        private void ActivateAuth()
        {
            auth.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        
        private void TryRegister()
        {
            var loginData = loginInputField.text;
            var passwordData = passwordInputField.text;
            var emailData = emailInputField.text;
            var loginJson = $"{{\"Email\": \"{emailData}\", \"Username\": \"{loginData}\", \"Password\": \"{passwordData}\"}}";

            var request = new UnityWebRequest(RequestUrl);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(loginJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;

            StartCoroutine(SendRequest(request, OnRequest));
        }
        
        IEnumerator SendRequest(UnityWebRequest request, Action<UnityWebRequest> callback)
        {
            yield return request.SendWebRequest();

            callback(request);
        }
        
        private void OnRequest(UnityWebRequest request)
        {
            var code = request.responseCode;
            var errorsArray = request.downloadHandler.text;

            errors.text = errorsArray;

            if (code != 200)
                return;
            
            var loginData = loginInputField.text;
            var passwordData = passwordInputField.text;


            ActivateAuth();
            
            auth.TryAuthWithData(loginData, passwordData);
        }
    }
}
