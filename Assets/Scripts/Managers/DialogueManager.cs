using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LMNT;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private LMNTSpeech teacherSpeech;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject startLessonButton;
    [SerializeField] private GameObject continueButton;
    
    private string[] _dialogue;
    private string _lesson;
    private Queue<string> _sentences;
    
    private void Start()
    {
        _sentences = new Queue<string>();
        
        if (AILessonManager.Instance.GetDialogue() != null)
        {
            _dialogue = AILessonManager.Instance.GetDialogue();
            _lesson = AILessonManager.Instance.GetLesson();
            Debug.Log("Dialogue was got from API");
        }
    }

    public void StartLesson()
    {
        startLessonButton.SetActive(false);
        continueButton.SetActive(true);
        _sentences.Clear();
        
        teacherSpeech.dialogue = _lesson;
        StartCoroutine(teacherSpeech.Talk());

        foreach (string sentence in _dialogue)
        {
            _sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    
    public void LeaveLesson()
    {
        SceneManager.LoadScene("Menu");
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        StopCoroutine(nameof(TypeSentence));
        string sentence = _sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }
    
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    private void EndDialogue()
    {
        continueButton.SetActive(false);
        startLessonButton.SetActive(true);
        Debug.Log("Lesson Finished");
    }
}
