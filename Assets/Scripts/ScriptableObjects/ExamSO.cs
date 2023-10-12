using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Exam Data",menuName = "Classroom/Exam")]
    public class ExamSO : ScriptableObject
    {
        public string examTitle;
        public Question[] questions;
    }

    [Serializable]
    public class Question
    {
        public string title;
        public string[] answers = new string[5];
        public int correctAnswerIndex;
    }
}
