using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using LitJson;
using Core.save;

namespace Core
{
    public class API
    {
        public delegate void OnErrorHandler(string error);
        public event OnErrorHandler OnError;

        protected int timeout;
        protected int maxAttempts;

        public bool WaitingResponse => waitingResponse;
        protected bool waitingResponse = false;

        protected string authorization;

        public API(string bearerKey, int timeout, int maxAttempts)
        {
            this.authorization = $"Bearer {bearerKey}";
            this.timeout = timeout;
            this.maxAttempts = maxAttempts;
        }

        public API(string bearerKey)
        {
            this.authorization = $"Bearer {bearerKey}";
            this.timeout = 60000;
            this.maxAttempts = 5;
        }

        protected IEnumerator apiCall<B, T>(string url, B body, System.Action<T> successCallback) where T : class where B : class
        {
            Debug.Log("Start api call");
            int currentAttempt = 1;
            while (currentAttempt < maxAttempts)
            {
                UnityWebRequest request = new UnityWebRequest(url, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonMapper.ToJson(body));
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                if (authorization != string.Empty)
                {
                    request.SetRequestHeader("Authorization", authorization);
                }

                request.timeout = this.timeout;

                this.waitingResponse = true;
                yield return request.SendWebRequest();
                this.waitingResponse = false;

                try
                {
                    if (request.error != null)
                    {
                        throw new APIRequestError(request.error);
                    }
                    else
                    {
                        string downloaded = request.downloadHandler.text;
                        successCallback.Invoke(MemoryCard.revertFromJson<T>(downloaded));
                        break;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    if (currentAttempt < maxAttempts)
                        currentAttempt++;
                    else
                    {
                        if (OnError != null)
                            OnError.Invoke(request.error);
                    }
                }
            }

            Debug.Log("Done");
        }

        public class APIRequestError : System.Exception
        {
            public APIRequestError(string message) : base(message) { }
        }
    }
}
