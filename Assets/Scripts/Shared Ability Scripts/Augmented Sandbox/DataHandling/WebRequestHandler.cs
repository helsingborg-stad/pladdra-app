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

namespace Pladdra
{
    //TODO Clean up
    //TODO Account for duplicate models
    enum Result { Success, Failure, PartialSuccess }
    public class WebRequestHandler : MonoBehaviour
    {
        [Min(1)]
        [SerializeField] protected int maxCoroutines = 3;

        [Min(1)]
        [SerializeField] protected int maxDownloadTimePerCoroutine = 10;
        protected List<Coroutine> coroutines = new List<Coroutine>();
        protected string filePath;

        void Start()
        {
            filePath = $"{Application.persistentDataPath}/Files/";
        }

        #region Load files
        internal IEnumerator LoadText(string url, Action<Result, string, string> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(Result.Failure, request.error, null);
            }
            else
            {
                callback(Result.Success, request.error, request.downloadHandler.text);
            }
            request.Dispose();
        }

        internal IEnumerator LoadFiles(List<(string name, string url)> urlsToDownload, Action<Result, string, List<(string name, string path)>> callback)
        {
            Queue<(string name, string url)> urls = new Queue<(string name, string url)>(urlsToDownload);
            List<(string name, string path)> pathsToReturn = new List<(string name, string url)>();
            string errors = "";

            Debug.Log($"Loading {urls.Count} GLB files");
            while (urls.Count > 0)
            {
                if (coroutines.Count < maxCoroutines)
                {
                    (string name, string url) obj = urls.Dequeue();
                    if (obj.url.IsNullOrEmptyOrFalse())
                    {
                        Debug.Log($"Broken url for {obj.name}, skipping");
                        continue;
                    }
                    string path = GetFilePath(obj.url);
                    if (File.Exists(path))
                    {
                        // Debug.Log($"Found {resource.name} locally, loading...");
                        pathsToReturn.Add((obj.name, path));
                    }
                    else
                    {
                        // Debug.Log($"Starting download for {resource.name} from url {resource.url}");
                        Coroutine c = StartCoroutine(DownloadFile(obj.url, (UnityWebRequest req) =>
                        {
                            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
                            {
                                string error = $"{(req != null ? req.error : "null")} : {(req.downloadHandler is DownloadHandlerBuffer ? req.downloadHandler.text : "string access not supported")}";
                                Debug.Log(error);
                                errors += $"\n Error downloading {obj.name}: {error}";
                                pathsToReturn.Add((obj.name, "failed"));

                            }
                            else
                            {
                                // resource.gameObject = LoadModel(path, resource.name);
                                pathsToReturn.Add((obj.name, path));
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

            if (pathsToReturn.Count == 0)
                callback(Result.Failure, errors, null);
            else if (pathsToReturn.Count == urlsToDownload.Count)
                callback(Result.Success, errors, pathsToReturn);
            else
                callback(Result.PartialSuccess, errors, pathsToReturn);
        }

        // TODO Add timer
        protected IEnumerator DownloadFile(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.timeout = maxDownloadTimePerCoroutine; // TODO Fix
                req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        internal IEnumerator LoadImage(string url, Action<Result, string, Texture2D> callback)
        {
            string error = "";
            Result result = Result.Success;
            Texture2D texture = null;

            if (!url.IsNullOrEmptyOrFalse())
            {
                Debug.Log($"Downloading file from {url}");
                UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    error = req.error;
                    result = Result.Failure;
                }
                else
                {
                    texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
                    result = Result.Success;
                }
                req.Dispose();
            }
            else
            {
                Debug.Log($"Url {url} is broken");
            }
            callback(result, error, texture);
        }
        #endregion Load files

        #region Load projects
        internal IEnumerator LoadProjectCollections(string collectionsUrl, Action<Result, string, List<ProjectCollection>> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(collectionsUrl);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(Result.Failure, request.error, null);
            }
            else
            {
                // We need to add a top layer to the json to make it a valid json object for Unity's JsonUtility
                string json = request.downloadHandler.text;
                json = json.Insert(0, "{\"collections\":");
                json = json.Insert(json.Length, "}");
                Debug.Log($"Downloaded JSON: {json}");

                WordpressData_ProjectCollections wordpressData = JsonUtility.FromJson<WordpressData_ProjectCollections>(json);
                callback(Result.Success, request.error, wordpressData.MakeProjectCollections());
            }
            request.Dispose();
        }

        internal IEnumerator LoadProjectsFromIDs(string[] listUrls, string projectBaseUrl, Action<Result, string, List<ProjectReference>> callback)
        {
            List<ProjectReference> projects = new List<ProjectReference>();
            string error = "";
            foreach (string url in listUrls)
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    callback(Result.Failure, request.error, null);
                    error += ", " + request.error;
                }
                else
                {
                    Debug.Log($"Downloaded project JSON: {request.downloadHandler.text}");
                    WordpressData_ProjectReference wordpressData = JsonUtility.FromJson<WordpressData_ProjectReference>(request.downloadHandler.text);
                    projects.Add(wordpressData.MakeProjectReference(projectBaseUrl));
                }
                request.Dispose();
            }
            callback(Result.Success, error, projects);
        }

        internal IEnumerator LoadProjectFromID(string id, string projectBaseUrl, Action<Result, string, ProjectReference> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(string.Format(projectBaseUrl, id));
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(Result.Failure, request.error, null);
            }
            else
            {
                Debug.Log($"Downloaded project JSON: {request.downloadHandler.text}");
                WordpressData_ProjectReference wordpressData = JsonUtility.FromJson<WordpressData_ProjectReference>(request.downloadHandler.text);
                callback(Result.Success, request.error, wordpressData.MakeProjectReference(projectBaseUrl));
            }
            request.Dispose();
        }

        internal IEnumerator LoadProjectsFromURL(string url, string projectBaseUrl, Action<Result, string, List<ProjectReference>> callback)
        {

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(Result.Failure, request.error, null);
            }
            else
            {
                // We need to add a top layer to the json to make it a valid json object for Unity's JsonUtility
                string json = request.downloadHandler.text;
                json = json.Insert(0, "{\"projects\":");
                json = json.Insert(json.Length, "}");
                Debug.Log($"Downloaded project JSON: {json}");

                ProjectList wordpressData = JsonUtility.FromJson<ProjectList>(json);
                callback(Result.Success, request.error, wordpressData.MakeProjectList(projectBaseUrl));
            }
            request.Dispose();
        }
        #endregion Load projects

        /// <summary>
        /// Creates a local file path for a file at a given url.
        /// </summary>
        /// <param name="url">The url to create a path from.</param>
        /// <returns></returns>
        protected string GetFilePath(string url)
        {
            string[] pieces = url.Split('/');
            string filename = pieces[pieces.Length - 1];

            return $"{filePath}{filename}";
        }

        #region Obsolete
        // protected GameObject LoadModel(string path, string name)
        // {
        //     // Debug.Log($"Loading model {name} from {path}");
        //     try
        //     {
        //         ImportSettings settings = new ImportSettings();
        //         settings.animationSettings = new AnimationSettings();
        //         settings.animationSettings.useLegacyClips = true;
        //         settings.animationSettings.interpolationMode = InterpolationMode.STEP;
        //         GameObject model = Importer.LoadFromFile(path, settings, out AnimationClip[] animations);

        //         if (animations != null && animations.Length > 0)
        //         {
        //             Animation animation = model.AddComponent<Animation>();
        //             animation.AddClip(animations[0], animations[0].name);
        //             animation.clip = animation.GetClip(animations[0].name);
        //             animation.Play();
        //             animation.wrapMode = WrapMode.Loop;
        //         }

        //         model.name = name;
        //         model.SetActive(false);
        //         return model;
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log($"Error loading model {name}: {e.Message}");
        //         return null;
        //     }
        // }
        #endregion

    }
}