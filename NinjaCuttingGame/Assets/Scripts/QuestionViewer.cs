using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class QuestionViewer : MonoBehaviour
{

    [SerializeField] private List<QuestionSO> questions;
    private int currentQuestionIndex;
    private List<GameObject> componentsList;
    private QuestionSO currentQuestion;
    // Start is called before the first frame update
    void Start()
    {
        componentsList = new List<GameObject>();
        currentQuestion = questions[currentQuestionIndex];
        DisplayQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayQuestion()
    {
        GameObject component;
        
        for (int i = 0; i < currentQuestion.componentObjects.Count; i++)
        {
            
            component = currentQuestion.componentObjects[i];
            componentsList.Add(Instantiate(component, new Vector3(i*2.0f, 2, 2), Quaternion.identity));
            
        }
        

        //Cube = currentQuestion.componentObjects[0];
    }

    void GetNextQuestion()
    {
        currentQuestionIndex++;
        currentQuestion = questions[currentQuestionIndex];
        componentsList.Clear();
        DisplayQuestion();
    }
}
