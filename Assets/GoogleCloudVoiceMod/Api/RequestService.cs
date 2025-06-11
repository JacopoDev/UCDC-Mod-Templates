using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GoogleCloudVoiceMod.Api.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleCloudVoiceMod.Api
{
    struct PostData
    {
        public string url;
        public string bodyJsonString;
        public Action<string> requestReceived;
        public Action<BadRequestData> errorReceived;
        public Dictionary<string, string> headers;
    }
    
    public class RequestService : MonoBehaviour
    {
        public static RequestService instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }


        public void SendDataToGoogle(string url, DataToSend dataToSend, string apiKey, Action<string> requestReceived,
            Action<BadRequestData> errorReceived)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("X-Goog-Api-Key", apiKey);
            headers.Add("Content-Type", "application/json; charset=utf-8");
            PostData pd = new PostData()
            {
                url = url,
                bodyJsonString = JsonUtility.ToJson(dataToSend),
                requestReceived = requestReceived,
                errorReceived = errorReceived,
                headers = headers
            };
                
            StartCoroutine(nameof(Post), pd);
        }
        
        private IEnumerator Post(PostData pd)
        {
            var request = new UnityWebRequest(pd.url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(pd.bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (pd.headers != null)
            {
                foreach (var header in pd.headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            var operation = request.SendWebRequest();
            
            while (!operation.isDone)
                yield return null;

            if (HasError(request, out var badRequest))
            {
                pd.errorReceived?.Invoke(badRequest);
            }
            else
            {
                pd.requestReceived?.Invoke(request.downloadHandler.text);
            }
            
            request.Dispose();
        }

        private static bool HasError(UnityWebRequest request, out BadRequestData badRequestData)
        {
            if (request.responseCode is 200 or 201)
            {
                badRequestData = null;
                return false;
            }

            badRequestData = JsonUtility.FromJson<BadRequestData>(request.downloadHandler.text);

            try
            {
                badRequestData = JsonUtility.FromJson<BadRequestData>(request.downloadHandler.text);
                return true;
            }
            catch (Exception)
            {
                badRequestData = new BadRequestData
                {
                    error = new Error
                    {
                        code = (int)request.responseCode,
                        message = request.error
                    }
                };

                return true;
            }
        }
    }
}