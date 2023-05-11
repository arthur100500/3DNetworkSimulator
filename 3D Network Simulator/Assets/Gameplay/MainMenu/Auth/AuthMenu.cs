using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Menu.Json;
using Menu.ProjectSelect;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using NsJProject = Project.Json.NsJProject;

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
            var request = new UnityWebRequest(RequestUrlList);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbGET;
            request.SetRequestHeader("Cookie", cookie);

            StartCoroutine(SendRequest(request, InitSelector));
        }

        private void InitSelector(UnityWebRequest request)
        {
            var NsJProjectListJson = request.downloadHandler.text;
            var NsJProjectList = JsonConvert.DeserializeObject<List<NsJProject>>(NsJProjectListJson);

            selector.Init(NsJProjectList);

            gameObject.SetActive(false);
        }
    }
}