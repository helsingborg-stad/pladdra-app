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

namespace Pladdra.DialogueAbility
{
    public class WebRequestHandler_Dialogues : WebRequestHandler
    {
        internal IEnumerator LoadDialogueProject(string url, Action<Result, string, Project> callback)
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
                WordpressData_Dialogues wordpressData = JsonUtility.FromJson<WordpressData_Dialogues>(request.downloadHandler.text);
                callback(Result.Success, request.error, wordpressData.MakeProject());
            }
            request.Dispose();
        }

        internal IEnumerator LoadDialogueProjectResources(Project project, Action<Result, string> callback)
        {
            Queue<DialogueResource> resourcesToDownload = new Queue<DialogueResource>(project.resources);
            if (project.groundPlane != null) resourcesToDownload.Enqueue(project.groundPlane);

            List<string> failedDownloads = new List<string>();
            string errors = "";

            Debug.Log($"Downloading project {project.name}, queue length {resourcesToDownload.Count} and project resources is {(project.resources != null ? project.resources.Count : "null")}");
            while (resourcesToDownload.Count > 0)
            {
                if (coroutines.Count < maxCoroutines)
                {
                    DialogueResource resource = resourcesToDownload.Dequeue();
                    if (resource.url.IsNullOrEmptyOrFalse())
                    {
                        Debug.Log($"No url for {resource.name}, skipping");
                        continue;
                    }
                    string path = GetFilePath(resource.url);
                    if (File.Exists(path))
                    {
                        // Debug.Log($"Found {resource.name} locally, loading...");
                        resource.gameObject = LoadModel(path, resource.name);
                        yield return null;
                    }
                    else
                    {
                        // Debug.Log($"Starting download for {resource.name} from url {resource.url}");
                        Coroutine c = StartCoroutine(DownloadResource(resource.url, (UnityWebRequest req) =>
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
                                resource.gameObject = LoadModel(path, resource.name);
                            }
                            req.Dispose();
                            coroutines.Remove(coroutines[0]);
                        }));
                        coroutines.Add(c);
                        yield return null;
                    }
                    yield return null;
                }
                yield return null;
            }

            // Wait for all coroutines to finish
            while (coroutines.Count > 0)
            {
                yield return null;
            }

            // download texture2d from markerURL
            //TODO Make this into it's own coroutine
            if (project.marker.url.IsNullOrEmptyOrFalse())
            {
                Debug.Log($"Downloading file from {project.marker.url}");
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(project.marker.url);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    project.marker.image = ((DownloadHandlerTexture)www.downloadHandler).texture;
                }
                www.Dispose();
            }

            if (failedDownloads.Count == 0)
                callback(Result.Success, "");
            else if (failedDownloads.Count == resourcesToDownload.Count)
                callback(Result.Failure, errors);
            else
                callback(Result.PartialSuccess, errors);
        }
    }
}