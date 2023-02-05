using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pladdra.Data;
using Pladdra.ARSandbox.Quizzes.Data;
using Pladdra.UI;

using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden.Utils;
using UntoldGarden.AR;

namespace Pladdra.ARSandbox.Quizzes
{
    // TODO Clean up
    // TODO Update to UI calls
    public class QuizManager : MonoBehaviour
    {
        #region Public
        //TODO Move to settings
        public QuizAbilitySettings settings;

        [Header("Debug")]
        [GrayOut] public QuizCollection currentQuizCollection;
        [GrayOut] public Quiz currentQuiz;
        [GrayOut] public Question currentQuestion;

        #endregion Public

        #region Scene References
        [Header("Scene References")]
        [SerializeField] protected ARSessionManager arSessionManager;
        public ARSessionManager ARSessionManager { get => arSessionManager; }
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected QuizUserManager quizUserManager { get { return transform.parent.gameObject.GetComponentInChildren<QuizUserManager>(); } }
        protected QuizAppManager appManager { get { return transform.parent.gameObject.GetComponentInChildren<QuizAppManager>(); } }
        protected WebRequestHandler webRequestHandler { get { return transform.parent.gameObject.GetComponentInChildren<WebRequestHandler>(); } }
        #endregion Scene References

        #region Private
        protected Dictionary<string, QuizCollection> quizzes = new Dictionary<string, QuizCollection>();
        public Dictionary<string, QuizCollection> Quizzes { get => quizzes; }
        protected List<AnswerController> answerControllers = new List<AnswerController>();
        Transform answerParent;
        bool firstQuestion = true;
        #endregion

        void Start()
        {
            answerParent = new GameObject("AnswerParent").transform;
        }

        public void LoadQuizCollection(ProjectReference quizProject)
        {
            if (quizzes.ContainsKey(quizProject.name))
            {
                currentQuizCollection = quizzes[quizProject.name];
                return;
            }

            if (quizProject.url != "")
            {
                StartCoroutine(webRequestHandler.LoadText(quizProject.url, (Result result, string errors, string json) =>
                {
                    switch (result)
                    {
                        case Result.PartialSuccess:
                        case Result.Failure:
                            uiManager.ShowError("DownloadFailure", new string[] { quizProject.url, errors });
                            return;
                        case Result.Success:
                            WordpressData_QuizCollection wordpressData = JsonUtility.FromJson<WordpressData_QuizCollection>(json);
                            QuizCollection collection = wordpressData.MakeQuizCollection(out string error);
                            if (collection == null)
                            {
                                uiManager.ShowError($"Frågespelet {quizProject.name} går inte att ladda. Felmeddelande: {error}");
                                return;
                            }
                            quizzes.Add(quizProject.name, collection);
                            currentQuizCollection = collection;
                            currentQuizCollection.isLoadedAndInit = true;
                            ShowQuizCollection();
                            break;
                    }
                }));
            }
            else
            {
                string json = quizProject.json.ToString();
                QuizCollection quizCollection = JsonUtility.FromJson<QuizCollection>(json);
                if (quizCollection != null)
                {
                    quizzes.Add(quizProject.name, quizCollection);
                    currentQuizCollection = quizCollection;
                    currentQuizCollection.isLoadedAndInit = true;
                }
            }
        }

        public void ShowQuizCollection()
        {
            Debug.Log("Show quiz collection");
            if (currentQuizCollection == null)
            {
                Debug.LogError("No quiz collection to show");
                return;
            }

            Action[] actions = new Action[currentQuizCollection.quizzes.Count];
            for (int i = 0; i < currentQuizCollection.quizzes.Count; i++)
            {
                int index = i;
                Quiz quiz = currentQuizCollection.quizzes[i];
                actions[i] = () =>
                {
                    StartQuiz(quiz);
                };
            }

            uiManager.DisplayUI("quiz-list", root =>
            {
                ListView listView = root.Q<ListView>("quizzes");
                listView.makeItem = () =>
                                {
                                    var button = uiManager.uiAssets.Find(x => x.name == "quiz-button-template").visualTreeAsset.Instantiate();
                                    return button;
                                };
                listView.bindItem = (element, i) =>
                                {

                                    element.Q<Button>("project-button").clicked += () => { actions[i](); };

                                    element.Q<Label>("name").text = currentQuizCollection.quizzes[i].name;
                                    element.Q<Label>("description").text = currentQuizCollection.quizzes[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = currentQuizCollection.quizzes;

                if (currentQuizCollection.title != "")
                {
                    root.Q<Label>("title").text = currentQuizCollection.title;
                }

                if (currentQuizCollection.info != "")
                {
                    root.Q<Label>("info").text = currentQuizCollection.info;
                }
                root.Q<Button>("close").clicked += () =>
                {
                    ClearQuiz();
                    appManager.DisplayRecentProjectList(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby"); });
                };
            });

        }

        protected void StartQuiz(Quiz quiz)
        {
            currentQuiz = quiz;
            // TODO Set Trigger on User.SphereCollider from Quiz settings
            quizUserManager.AddQuiz(quiz);
            ShowQuestion();
        }

        protected void ShowQuestion()
        {
            uiManager.ClearUI();
            currentQuestion = currentQuiz.questions.Where(x => x.answered == false).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (currentQuestion != null)
            {
                uiManager.DisplayUI("quiz-default", root =>
                {
                    root.Q<Label>("question").text = currentQuestion.text;
                    root.Q<Button>("next").clicked += () =>
                    {
                        currentQuestion.answered = true;
                        ShowQuestion();
                    };
                    root.Q<Button>("close").clicked += () =>
                    {
                        ClearQuiz();
                        ShowQuizCollection();
                    };
                    root.Q<Label>("score").text = quizUserManager.GetScore().ToString();
                    root.Q<Label>("questions-left").text = currentQuiz.questions.Where(x => x.answered == false).Count().ToString();
                });

                StartCoroutine(CreateAnswers(currentQuestion));
            }
            else
            {
                uiManager.DisplayUI("quiz-finished", root =>
                {
                    root.Q<Label>("text").text = "Du har svarat på alla frågor i " + currentQuiz.name;
                    root.Q<Button>("next").clicked += () =>
                    {
                        ClearQuiz();
                        ShowQuizCollection();
                    };
                    root.Q<Button>("next").text = "Tillbaka till " + currentQuizCollection.name;
                });
            }
        }

        private IEnumerator CreateAnswers(Question currentQuestion)
        {
            if (answerControllers.Count > 0)
            {
                foreach (var answerController in answerControllers)
                {
                    answerController.HideAnswer();
                }
            }

            foreach (var answer in currentQuestion.answers)
            {
                if (answerControllers.Where(x => x.answer.text == answer.text).FirstOrDefault() != null)
                {
                    answerControllers.Where(x => x.answer.text == answer.text).FirstOrDefault().DestroyAnswer();
                    answerControllers.Remove(answerControllers.Where(x => x.answer.text == answer.text).FirstOrDefault());
                }

                Vector3 offset = Vector3.zero;
                if (firstQuestion && answer.isCorrect)
                {
                    offset = new Vector3(0, 2, 3);
                    firstQuestion = false;
                }
                else
                {
                    offset = new Vector3(RandomWithinDistance(settings.distance, settings.range), 2, RandomWithinDistance(settings.distance, settings.range));
                }
                Vector3 pos = arSessionManager.GetUser().gameObject.RelativeToObjectOnGround(offset, VectorExtensions.RelativeToObjectOptions.OnGroundLayers);
                pos.y = 2;

                GameObject go = Instantiate(settings.answerPrefab, pos, Quaternion.identity);
                go.transform.SetParent(answerParent);

                AnswerController controller = go.GetComponent<AnswerController>();
                controller.Init(answer, this);
                answerControllers.Add(controller);

                yield return new WaitForEndOfFrame();

                // Annoying solution to an issue with the 3D text plugin where MeshColliders kept adding themselves
                List<MeshCollider> colliders = new List<MeshCollider>();
                colliders.AddRange(go.GetComponentsInChildren<MeshCollider>());
                while (colliders.Count > 0)
                {
                    foreach (var collider in colliders)
                        Destroy(collider);

                    colliders.Clear();
                    yield return new WaitForEndOfFrame();
                    colliders.AddRange(go.GetComponentsInChildren<MeshCollider>());
                }
                // End of annoying solution

                controller.InitComponents(false);
                yield return new WaitForSeconds(0.5f);
            }
        }
        public void CorrectAnswer(Answer answer)
        {
            currentQuestion.answered = true;
            uiManager.DisplayUI("quiz-correct", root =>
            {
                root.Q<Button>("next").clicked += () => { ShowQuestion(); };
                root.Q<Label>("text").text = currentQuestion.response != "" ? currentQuestion.response : ("Rätt svar! " + currentQuestion.text + " = " + answer.text);
            });
            quizUserManager.UpdateScore();
        }

        public void ClearQuiz()
        {
            foreach (Question question in currentQuiz.questions)
                question.answered = false;

            foreach (var answerController in answerControllers)
                answerController.DestroyAnswer();

            firstQuestion = true;

            answerControllers.Clear();
        }

        float RandomWithinDistance(float distance, float range)
        {
            return (UnityEngine.Random.value >= 0.5) ? UnityEngine.Random.Range(distance - range, distance + range) : UnityEngine.Random.Range(-distance - range, -distance + range);
        }
    }
}