using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quiz : MonoBehaviour
{
    [SerializeField]private QuestionSO question;
    [SerializeField] private TextMeshProUGUI textArea;
    
    
    // Start is called before the first frame update
    void Start()
    {
        textArea.text = question.GetQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
