using System.Collections.Generic;
using Managers;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviourPun
{
    [Header("Exam")]
    [SerializeField] private GameObject examCanvas;
    [SerializeField] private Transform wrapper;
    [SerializeField] private GameObject questionPrefab;
    
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(transform.Find("Locomotion System").gameObject);
            foreach (var controller in GetComponentsInChildren<ActionBasedController>())
            {
                controller.enableInputActions = false;
                controller.enableInputTracking = false;
                controller.gameObject.GetComponent<XRRayInteractor>().enabled = false;
            }

            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;
        if(ExamManager.Instance == null) return;
        
        SendExam(ExamManager.Instance.GetExam());
    }
    
    void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                examCanvas.SetActive(!examCanvas.activeInHierarchy);
            }
        }

    [PunRPC]
    void ShowExam(List<string> questionTexts, List<string[]> variantOptions, List<string> correctAnswers)
    {
        var questionIndex = 1;
        for (int i = 0; i < questionTexts.Count; i++)
        {
            var questionObject = Instantiate(questionPrefab, wrapper);
            questionObject.transform.Find("Title").GetComponent<TMP_Text>().text =
                questionIndex + ". " + questionTexts[i];
            var answersContainer = questionObject.transform.Find("Variants");
            for (int j = 0; j < variantOptions.Count; j++)
            {
                var answer = answersContainer.GetChild(j);
                answer.Find("Label").GetComponent<TMP_Text>().text = variantOptions[i][j];
                answer.name = j.ToString(); 
            }

            questionIndex++;
        }
    }

    [PunRPC]
    void SendExam(Exam exam)
    {
        if (exam != null)
        {
            List<string> questionTexts = new List<string>();
            List<string[]> variantOptions = new List<string[]>();
            List<string> correctAnswers = new List<string>();

            foreach (Question question in exam.questions)
            {
                questionTexts.Add(question.question);
                variantOptions.Add(question.variants);
                correctAnswers.Add(question.correct);
            }
            
            photonView.RPC(nameof(ShowExam), RpcTarget.Others, questionTexts, variantOptions, correctAnswers);
        }
    }
}