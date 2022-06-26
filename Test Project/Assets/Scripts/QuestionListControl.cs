using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Button = UnityEngine.UI.Button;
public class QuestionListControl : MonoBehaviour
{
   
    private Quiz quiz;

    [SerializeField]
    private GameObject buttonTemplate;

    private GameObject content;
    // Start is called before the first frame update
    void Start()
    {
        quiz = FindObjectOfType<Quiz>();
        // button = transform.Find("Question List button").GetComponent<Button>();
        
        for (int i = 0; i < quiz.GetNumberOfQuestions(); i++)
        {
            GameObject button = Instantiate(buttonTemplate);
            button.SetActive(true);
            button.GetComponent<QuestionListButton>().SetText("Question " + (i + 1));
            button.GetComponent<QuestionListButton>().SetOnClick(i);
            button.transform.SetParent(buttonTemplate.transform.parent,false);
           
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
