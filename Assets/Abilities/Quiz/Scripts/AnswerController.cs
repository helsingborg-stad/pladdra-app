using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pladdra.ARSandbox.Quizzes.Data;
using UnityEngine;
using UntoldGarden.Utils;

namespace Pladdra.ARSandbox.Quizzes
{
    public class AnswerController : MonoBehaviour
    {
        [GrayOut] public bool isCorrect;
        public Answer answer;
        QuizManager quizManager;
        int damping = 2;
        bool disabled = false;

        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        public void Init(Answer answer, QuizManager quizManager)
        {
            this.quizManager = quizManager;
            this.answer = answer;
            gameObject.name = answer.text;
            isCorrect = answer.isCorrect;

            if (answer.text == "")
            {
                Debug.Log("Answer is empty");
                return;
            }

            Writer writer = gameObject.GetComponent<Writer>();
            if (writer != null)
            {
                writer.TextBoxChange(answer.text);
                writer.CreateWord();
            }
        }

        public void InitComponents(bool isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = isKinematic;

            // TODO Move to collider extensions
            BoxCollider col = GetComponent<BoxCollider>();
            Bounds bounds = gameObject.GetBounds();
            bounds.center = bounds.center - transform.position;
            col.size = bounds.size;
            col.center = bounds.center;

            meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        }

        void Update()
        {
            if (quizManager != null && !disabled)
            {
                var lookPos = quizManager.ARSessionManager.GetUser().transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
        }
        void OnTriggerEnter(Collider other)
        {
            // Debug.Log("OnTriggerEnter, other: " + other.gameObject.name);
            if (other.gameObject.tag == "MainCamera")
            {
                CheckAnswer();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            // Debug.Log("OnCollisionEnter, other: " + collision.gameObject.name);
            if (collision.gameObject.tag == "MainCamera")
            {
                CheckAnswer();
            }
        }

        void CheckAnswer()
        {
            if (disabled)
            {
                return;
            }

            if (isCorrect)
            {
                quizManager.CorrectAnswer(answer);
                SetMeshRenderersMaterial(quizManager.settings.correctAnswer);
            }
            else
            {
                SetMeshRenderersMaterial(quizManager.settings.wrongAnswer);
            }
            disabled = true;
        }

        void SetMeshRenderersMaterial(Material material)
        {
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.sharedMaterial = material;
            }
        }

        public void HideAnswer()
        {
            SetMeshRenderersMaterial(quizManager.settings.disabledAnswer);
            disabled = true;
        }

        public void DestroyAnswer()
        {
            Destroy(gameObject);
        }
    }
}