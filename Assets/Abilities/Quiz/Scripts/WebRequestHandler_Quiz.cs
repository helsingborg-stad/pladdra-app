using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pladdra.Data;
using Pladdra.DialogueAbility.Data;
using UnityEngine;
using Siccity.GLTFUtility;
using UnityEngine.Networking;
using System.Linq;
using UntoldGarden.Utils;
using Pladdra.QuizAbility.Data;

namespace Pladdra.QuizAbility
{
    public class WebRequestHandler_Quiz : WebRequestHandler
    {
        internal IEnumerator LoadQuizCollection(string url, Action<Result, string, QuizCollection> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(Result.Failure, request.error, null);
            }
            else
            {
                Debug.Log($"Downloaded project JSON: {request.downloadHandler.text}");
                WordpressData_QuizCollection wordpressData = JsonUtility.FromJson<WordpressData_QuizCollection>(request.downloadHandler.text);
                QuizCollection collection = wordpressData.MakeQuizCollection(out string error);
                callback(Result.Success, error, collection);
            }
            request.Dispose();
        }

    }
}