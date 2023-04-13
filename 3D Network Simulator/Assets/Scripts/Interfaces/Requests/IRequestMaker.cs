namespace Interfaces.Requests
{
    /// <summary>
    /// Interface for web requests
    /// </summary>
    public interface IRequestMaker
    {
        /// <summary>
        /// Makes REST GET Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        public void MakeGetRequest(string url);
        /// <summary>
        /// Makes REST GET Request and returns serialized object of the T class
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        public T MakeGetRequest<T>(string url, string data);
        
        /// <summary>
        /// Makes REST POST Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        public void MakePostRequest(string url, string data);
        
        /// <summary>
        /// Makes a response and returns serialized object of the T class
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <typeparam name="T">type of object to be serialized</typeparam>
        /// <returns></returns>
        public T MakeGetRequest<T>(string url);
        
        /// <summary>
        /// Makes a response and returns serialized object of the T class
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <typeparam name="T">type of object to be serialized</typeparam>
        /// <returns></returns>
        public T MakePostRequest<T>(string url, string data);
        
        /// <summary>
        /// Makes REST DELETE Request
        /// </summary>
        /// <param name="url">endpoint url</param>
        /// <param name="data">additional data</param>
        public void MakeDeleteRequest(string url, string data);
    }
}
