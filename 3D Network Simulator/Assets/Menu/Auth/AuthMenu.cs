using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Menu.Auth
{
    public class AuthMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField loginInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private TMP_Text errors;
        
        private const string RequestUrl = "http://127.0.0.1:10203/login";

        private void Start()
        {
            submitButton.onClick.AddListener(TryAuth);
        }

        private void TryAuth()
        {
            var loginData = loginInputField.text;
            var passwordData = passwordInputField.text;
            var loginJson = $"{{\"Username\": \"{loginData}\", \"Password\": \"{passwordData}\"}}";
            
            var request = new UnityWebRequest(RequestUrl);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(loginJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            
            StartCoroutine(SendAuth(request, OnRequest));
        }
        
        IEnumerator SendAuth(UnityWebRequest request, Action<UnityWebRequest> callback)
        {
            using (request)
            {
                yield return request.SendWebRequest();

                callback(request);
            }
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
            
        }
    }
}
            
