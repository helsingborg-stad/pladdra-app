using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pladdra.Data;
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

        internal IEnumerator LoadProjectResources(Project project, Action<Result, string> callback)
        {
            Queue<PladdraResource> resourcesToDownload = new Queue<PladdraResource>(project.Resources);
            project.StaticResources.ForEach(resourcesToDownload.Enqueue);

            List<string> failedDownloads = new List<string>();
            string errors = "";

            Debug.Log($"Queue length {resourcesToDownload.Count} and project resources is {project.Resources.Count} and static resources is {project.StaticResources.Count}");
            while (resourcesToDownload.Count > 0)
            {
                if (coroutines.Count < maxCoroutines)
                {
                    PladdraResource resource = resourcesToDownload.Dequeue();
                    Debug.Log($"Starting coroutine for {resource.Name} with url {resource.ModelURL}");
                    string path = GetFilePath(resource.ModelURL);
                    if (File.Exists(path))
                    {
                        Debug.Log("Found file locally, loading...");
                        resource.Model = LoadModel(path, resource.Name);
                    }
                    else
                    {
                        Debug.Log($"Downloading file from {resource.ModelURL}");
                        Coroutine c = StartCoroutine(DownloadResource(resource.ModelURL, (UnityWebRequest req) =>
                        {
                            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
                            {
                                string error = $"{req.error} : {req.downloadHandler.text}";
                                Debug.Log(error);
                                errors += $"\n Error downloading {resource.Name}: {error}";
                                failedDownloads.Add(resource.Name);
                            }
                            else
                            {
                                resource.Model = LoadModel(path, resource.Name);
                            }
                            coroutines.Remove(coroutines[0]);
                        }));
                        coroutines.Add(c);
                    }
                }
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
            GameObject model = Importer.LoadFromFile(path);
            model.name = name;
            model.SetActive(false);
            return model;
        }
        
        string GetFilePath(string url)
        {
            string[] pieces = url.Split('/');
            string filename = pieces[pieces.Length - 1];

            return $"{filePath}{filename}";
        }
    }
}