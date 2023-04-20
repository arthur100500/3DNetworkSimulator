using System;
using GNS3.GNSThread;
using UnityEngine;

namespace Interfaces.Requests
{
    /// <summary>
    ///     Interface for web requests
    /// </summary>
    public interface IRequestTaskMaker
    {
        /// <summary>
        ///     Makes REST DELETE Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeDeleteRequest(string url, string data, Action start, Action finish);

        /// <summary>
        ///     Makes REST GET Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeGetRequest(string url, Action start, Action finish);

        /// <summary>
        ///     Makes REST POST Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakePostRequest(string url, string data, Action start, Action finish);

        /// <summary>
        ///     Makes REST DELETE Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeDeleteRequest(Func<string> url, string data, Action start, Action finish);

        /// <summary>
        ///     Makes REST GET Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeGetRequest(Func<string> url, Action start, Action finish);

        /// <summary>
        ///     Makes REST POST Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakePostRequest(Func<string> url, string data, Action start, Action finish);

        /// <summary>
        ///     Makes a response and returns serialized object of the T class
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <typeparam name="T">type of object to be serialized</typeparam>
        /// <returns></returns>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(string url, Action start, Action<T> finish);

        /// <summary>
        ///     Makes a response and returns serialized object of the T class
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <typeparam name="T">type of object to be serialized</typeparam>
        /// <returns></returns>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakePostRequest<T>(string url, string data, Action start, Action<T> finish);

        /// <summary>
        ///     Makes REST GET Request and returns serialized object of the T class
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(string url, string data, Action start, Action<T> finish);

        /// <summary>
        ///     Makes a response and returns serialized object of the T class
        /// </summary>
        /// <param name="url">function returning endpoint url</param>
        /// <typeparam name="T">type of object to be serialized</typeparam>
        /// <returns></returns>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(Func<string> url, Action start, Action<T> finish);

        /// <summary>
        ///     Makes a response and returns serialized object of the T class
        /// </summary>
        /// <param name="url">function returning endpoint url</param>
        /// <typeparam name="T">type of object to be serialized</typeparam>
        /// <returns></returns>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakePostRequest<T>(Func<string> url, Func<string> data, Action start, Action<T> finish);

        /// <summary>
        ///     Makes REST GET Request and returns serialized object of the T class
        /// </summary>
        /// <param name="url">function returning endpoint url</param>
        /// <param name="data">additional data</param>
        /// <param name="start">action to be done on start</param>
        /// <param name="finish">action to be done on end</param>
        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(Func<string> url, string data, Action start, Action<T> finish);
    }
}