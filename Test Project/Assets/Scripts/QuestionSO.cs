using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Quiz Question", fileName = "New Question")]
public class QuestionSO : ScriptableObject
{
    [TextArea(2,6)] 
    [SerializeField] string question = "Enter new question text here";

    [SerializeField] private string[] answers = {"A", "B", "C", "D"};
    [SerializeField] private int correctAnswerIndex;

    public string GetQuestion()
    {
        return question;
    }
    public string GetAnswer(int index)
    {
        return answers[index];
    }
    public int GetCorrectAnswerIndex()
    {
        return correctAnswerIndex;
    }
}
