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

    [SerializeField] private Sprite[] sprites = new Sprite[4];
    
    

    public string GetQuestionText()
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

    public Sprite GetSprite(int index)
    {
        return sprites[index];
    }
}
