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

        

        // TODO Add timer
        protected IEnumerator DownloadResource(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.timeout = maxDownloadTimePerCoroutine; // TODO Fix
                req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        protected GameObject LoadModel(string path, string name)
        {
            // Debug.Log($"Loading model {name} from {path}");
            try
            {
                ImportSettings settings = new ImportSettings();
                settings.animationSettings = new AnimationSettings();
                settings.animationSettings.useLegacyClips = true;
                settings.animationSettings.interpolationMode = InterpolationMode.STEP;
                GameObject model = Importer.LoadFromFile(path, settings, out AnimationClip[] animations);

                if (animations != null && animations.Length > 0)
                {
                    Animation animation = model.AddComponent<Animation>();
                    animation.AddClip(animations[0], animations[0].name);
                    animation.clip = animation.GetClip(animations[0].name);
                    animation.Play();
                    animation.wrapMode = WrapMode.Loop;
                }

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

        protected string GetFilePath(string url)
        {
            string[] pieces = url.Split('/');
            string filename = pieces[pieces.Length - 1];

            return $"{filePath}{filename}";
        }
    }
}