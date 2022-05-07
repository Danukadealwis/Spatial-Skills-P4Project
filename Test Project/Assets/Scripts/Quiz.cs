using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quiz : MonoBehaviour
{
    [SerializeField]private QuestionSO question;
    [SerializeField] private TextMeshProUGUI questionTextArea;
    [SerializeField] private GameObject[] answerGroups;
    
    
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI buttonText;
        questionTextArea.text = question.GetQuestion();
        for (int i = 0; i < answerGroups.Length; i++)
        {
            buttonText = answerGroups[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = question.GetAnswer(i);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
