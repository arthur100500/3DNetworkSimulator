using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gameplay.MainMenu.ProjectSelect;
using Menu.Json;
using Menu.ProjectSelect;
using Newtonsoft.Json;
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
        [SerializeField] private ProjectSelector selector;

        private const string RequestUrl = "http://127.0.0.1:10203/login";
        private const string RequestUrlList = "http://127.0.0.1:10203/ns/projects";

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

            StartCoroutine(SendRequest(request, OnRequest));
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
            var request = new UnityWebRequest(RequestUrlList);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbGET;

            if (cookie is not null)
                request.SetRequestHeader("Cookie", cookie);

            StartCoroutine(SendRequest(request, InitSelector));
        }

        private void InitSelector(UnityWebRequest request)
        {
            Debug.Log("Entered selector");

            var nsJProjectListJson = request.downloadHandler.text;
            var nsJProjectList = JsonConvert.DeserializeObject<List<NsJProject>>(nsJProjectListJson);

            Debug.Log("Init nsJProjectList");
            selector.Init(nsJProjectList);
            selector.gameObject.SetActive(true);

            gameObject.SetActive(false);
        }
    }
}