using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ExamManager : MonoBehaviour
    {
        [SerializeField] private ExamSO[] exams;

        [SerializeField] private int examIndex;

        [SerializeField] private GameObject[] clipboards; // TODO: make this single GameObject prefab and instantiate clipboard for each student (table)

        [SerializeField] private GameObject questionPrefab;

        private void Start()
        {
            var exam = exams[examIndex];
            foreach (var clipboard in clipboards)
            {
                var wrapper = clipboard.transform.Find("Canvas").Find("Wrapper");
                wrapper.Find("Title").GetComponent<TMP_Text>().text = exam.examTitle;
                var questionIndex = 1;
                foreach (var question in exam.questions)
                {
                    var questionObject = Instantiate(questionPrefab, wrapper);
                    questionObject.transform.Find("Title").GetComponent<TMP_Text>().text =
                        questionIndex + ". " + question.title;
                    var answersContainer = questionObject.transform.Find("Answers");
                    for (int i = 0; i < 5; i++)
                    {
                        var answer = answersContainer.GetChild(i);
                        answer.Find("Label").GetComponent<Text>().text = question.answers[i];
                        answer.name = i.ToString();
                    }

                questionIndex++;
                }
            }
        }
        
        public void SendResult(GameObject clipboard)
        {
        }

        public void ResetResult(GameObject clipboard)
        {
            var wrapper = clipboard.transform.GetChild(2).GetChild(0);
            foreach (var inputField in wrapper.transform.GetComponentsInChildren<TMP_InputField>())
                inputField.text = "";
        }
    }
}