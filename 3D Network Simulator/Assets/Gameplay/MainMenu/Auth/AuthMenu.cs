using System;
using System.Collections;
using System.Text;
using Gameplay.MainMenu.ProjectSelect;
using Gameplay.MainMenu.Register;
using Menu.ProjectSelect;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Gameplay.MainMenu.Auth
{
    public class AuthMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField loginInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private TMP_Text errors;
        [SerializeField] private ProjectSelector selector;
        [SerializeField] private RegisterMenu register;

        private const string RequestUrl = "http://127.0.0.1:10203/login";

        private string _authCookie;

        private void Start()
        {
            submitButton.onClick.AddListener(TryAuth);
            registerButton.onClick.AddListener(ActivateRegister);
        }

        private void ActivateRegister()
        {
            register.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void TryAuthWithData(string login, string password)
        {
            var loginJson = $"{{\"Username\": \"{login}\", \"Password\": \"{password}\"}}";
            
            var request = new UnityWebRequest(RequestUrl);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(loginJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;

            StartCoroutine(SendRequest(request, OnRequest));
        }

        private void TryAuth()
        {
            var loginData = loginInputField.text;
            var passwordData = passwordInputField.text;
            
            TryAuthWithData(loginData, passwordData);
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

            var headers = request.GetResponseHeaders();

            if (headers is null || !headers.ContainsKey("Set-Cookie"))
                return;

            var authCookie = headers["Set-Cookie"];

            Authorize(authCookie);
        }

        private void Authorize(string cookie)
        {
            _authCookie = cookie;
            InitSelector();
        }

        private void InitSelector()
        {
            selector.gameObject.SetActive(true);
            selector.Init(_authCookie);

            gameObject.SetActive(false);
        }
    }
}