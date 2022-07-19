using System;
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
    private GameObject questionObject;
    private QuestionSO currentQuestion;
    private int[] playerCorrect;
    private int gameScore;
    
    [SerializeField] private double timeToCompleteQuiz = 300f;
    private double timerValue;
    private double timerChange;
    public double maxQuestionTime;
    public double[] timeTaken;
    public double[] cutsMade; 

    // Start is called before the first frame update
    void Start()
    {
        componentsList = new List<GameObject>();
        currentQuestion = questions[currentQuestionIndex];
        DisplayQuestion();
        timerChange = Time.deltaTime/10;
        playerCorrect = new int[questions.Count];
        timeTaken = new double[questions.Count];
        cutsMade = new double[questions.Count];
        maxQuestionTime = 40.0;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Debugging cases
        
        if (Input.GetKeyDown("n"))
        {
            GetNextQuestion();
        }
        if (Input.GetKeyDown("s"))
        {
            QuestionComplete();
        }
        timerValue += timerChange;
        if (Input.GetKeyDown("t"))
        {
            Debug.Log("value of timer is: " + timerValue);
        }
        if (Input.GetKeyDown("c"))
        {
            Debug.Log("player moves set");
            playerCorrect[currentQuestionIndex] = 1;
            cutsMade[currentQuestionIndex] = 3;
        }

        if (timerValue >= maxQuestionTime)
        {
            QuestionTimeElapsed();
        }
    }

    void DisplayQuestion()
    {
        questionObject = Instantiate(currentQuestion.questionObject, new Vector3(2.0f, 2, 1), Quaternion.identity);
        
        for (int i = 0; i < currentQuestion.componentObjects.Count; i++)
        {
            
            componentsList.Add(Instantiate(currentQuestion.componentObjects[i], new Vector3(i*2.0f, 2, 2), Quaternion.identity));
            
        }
        ResetTimer();
        
        

        //Cube = currentQuestion.componentObjects[0];
    }

    void GetNextQuestion()
    {
        currentQuestionIndex++;
        currentQuestion = questions[currentQuestionIndex];
        Destroy(questionObject);
        foreach (var component in componentsList)
        {
            Destroy(component);
        }
        componentsList.Clear();
        DisplayQuestion();
    }

    
    void QuestionComplete()
    {
        StopTimer();
        timeTaken[currentQuestionIndex] = timerValue;
        Debug.Log(" time score: " + maxQuestionTime / timeTaken[currentQuestionIndex]*100);
        Debug.Log(" cuts score: " + currentQuestion.maxCuts / cutsMade[currentQuestionIndex] * 200);
        if (playerCorrect[currentQuestionIndex] == 1)
        {
            gameScore += 5000
                         + Math.Min(2000, Convert.ToInt32(maxQuestionTime / timeTaken[currentQuestionIndex]*100))
                            + Math.Min(500, Convert.ToInt32(currentQuestion.maxCuts/cutsMade[currentQuestionIndex]*200));
        }
        Debug.Log("Game Score is: " + gameScore);
        GetNextQuestion();
    }

    void QuestionTimeElapsed()
    {
        // Pause Game
        //
    }
    void ResetTimer()
    {
        timerValue = 0;
        timerChange = Time.deltaTime/10;
    }

    void StopTimer()
    {
        timerChange = 0;
    }

    
}


