using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pladdra.DefaultAbility.Data;
using UnityEngine;
using Siccity.GLTFUtility;
using UnityEngine.Networking;

namespace Pladdra
{
    enum Result { Success, Failure, PartialSuccess }
    public class WebRequestHandler : MonoBehaviour
    {
        [Min(1)]
        [SerializeField] int maxCoroutines;

        [Min(1)]
        [SerializeField] int maxDownloadTimePerCoroutine = 10;
        List<Coroutine> coroutines = new List<Coroutine>();
        string filePath;

        void Start()
        {
            filePath = $"{Application.persistentDataPath}/Files/";
        }

        internal IEnumerator LoadProjectJSON(string url, Action<Result, string, WordpressData> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError($"Error downloading project JSON: {request.error}");
                callback(Result.Failure, request.error, null);
            }
            else
            {
                Debug.Log($"Downloaded project JSON: {request.downloadHandler.text}");
                WordpressData wordpressData = JsonUtility.FromJson<WordpressData>(request.downloadHandler.text);
                callback(Result.Success, request.error, wordpressData);
            }
        }

        internal IEnumerator LoadProjectResources(Project project, Action<Result, string> callback)
        {
            Queue<PladdraResource> resourcesToDownload = new Queue<PladdraResource>(project.resources);
            project.staticResources.ForEach(resourcesToDownload.Enqueue);
            if (project.groundPlane != null && project.groundPlane.modelURL != "")
                resourcesToDownload.Enqueue(project.groundPlane);

            List<string> failedDownloads = new List<string>();
            string errors = "";

            Debug.Log($"Downloading project {project.name}, queue length {resourcesToDownload.Count} and project resources is {project.resources.Count} and static resources is {project.staticResources.Count}");
            while (resourcesToDownload.Count > 0)
            {
                if (coroutines.Count < maxCoroutines)
                {
                    PladdraResource resource = resourcesToDownload.Dequeue();
                    Debug.Log($"Starting coroutine for {resource.name} with url {resource.modelURL}");
                    string path = GetFilePath(resource.modelURL);
                    if (File.Exists(path))
                    {
                        Debug.Log("Found file locally, loading...");
                        resource.model = LoadModel(path, resource.name);
                    }
                    else
                    {
                        Debug.Log($"Downloading file from {resource.modelURL}");
                        Coroutine c = StartCoroutine(DownloadResource(resource.modelURL, (UnityWebRequest req) =>
                        {
                            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
                            {
                                string error = $"{(req != null ? req.error : "null")} : {(req.downloadHandler is DownloadHandlerBuffer ? req.downloadHandler.text : "string access not supported")}";
                                Debug.Log(error);
                                errors += $"\n Error downloading {resource.name}: {error}";
                                failedDownloads.Add(resource.name);
                            }
                            else
                            {
                                resource.model = LoadModel(path, resource.name);
                            }
                            coroutines.Remove(coroutines[0]);
                        }));
                        coroutines.Add(c);
                    }
                }
                yield return null;
            }

            // Wait for all coroutines to finish
            while (coroutines.Count > 0)
            {
                yield return null;
            }

            if (failedDownloads.Count == 0)
                callback(Result.Success, "");
            else if (failedDownloads.Count == resourcesToDownload.Count)
                callback(Result.Failure, errors);
            else
                callback(Result.PartialSuccess, errors);
        }

        // TODO Add timer
        // 
        IEnumerator DownloadResource(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.timeout = maxDownloadTimePerCoroutine; // TODO Fix
                req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        GameObject LoadModel(string path, string name)
        {
            Debug.Log($"Loading model from {path} with name {name}");
            try
            {
                GameObject model = Importer.LoadFromFile(path);
                model.name = name;
                model.SetActive(false);
                return model;
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading model {name}: {e.Message}");
                return null;
            }
        }

        string GetFilePath(string url)
        {
            string[] pieces = url.Split('/');
            string filename = pieces[pieces.Length - 1];

            return $"{filePath}{filename}";
        }
    }
}