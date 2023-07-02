using System;
using System.Collections;
using System.Text;
using Gameplay.MainMenu.ProjectSelect;
using Gameplay.MainMenu.Register;
using Menu.Json;
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
        [SerializeField] private Button useLocalGnsButton;
        [SerializeField] private TMP_Text errors;
        [SerializeField] private ProjectSelector selector;
        [SerializeField] private RegisterMenu register;

        private const string RequestUrl = "http://127.0.0.1:10203/login";

        private string _authCookie;

        private void Start()
        {
            submitButton.onClick.AddListener(TryAuth);
            registerButton.onClick.AddListener(ActivateRegister);
            useLocalGnsButton.onClick.AddListener(UseLocalGns);
        }

        private void ActivateRegister()
        {
            register.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        private void UseLocalGns()
        {
            
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
            Debug.Log($"Started sending request to {request.url}");

            yield return request.SendWebRequest();

            Debug.Log($"Got {request.downloadHandler.text}");

            callback(request);
        }

        private void OnRequest(UnityWebRequest request)
        {
            Debug.Log("Got into OnRequest");
            var code = request.responseCode;
            var errorsArray = request.downloadHandler.text;

            errors.text = errorsArray;

            if (code != 200)
                return;

            Debug.Log("Retrieving cookie");
            var headers = request.GetResponseHeaders();

            Debug.Log($"Headers null? {headers is null}");
            Debug.Log($"Contains cookie? {headers.ContainsKey("Set-Cookie")}");

            string authCookie = null;

//#if !UNITY_WEBGL || UNITY_EDITOR
            if (headers is null || !headers.ContainsKey("Set-Cookie"))
            {
                Authorize(null);
                return;
            }

            authCookie = headers["Set-Cookie"];
            Debug.Log("Authorizing...");
//#endif
            
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