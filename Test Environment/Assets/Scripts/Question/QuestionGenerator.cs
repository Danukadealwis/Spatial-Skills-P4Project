using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class QuestionGenerator : MonoBehaviour
{

    [SerializeField] private List<QuestionSO> questions;
    private int currentQuestionIndex;
    private List<GameObject> componentsList;
    private List<GameObject> pillarList;
    private GameObject questionObject;
    private QuestionSO currentQuestion;
    private int[] playerCorrect;
    private int gameScore;
    
    [SerializeField] private double timeToCompleteQuiz = 300f;
    private double timerValue;
    private double timerChange;
    public double maxQuestionTime;
    private double[] timeTaken;
    private double[] cutsMade;
    [SerializeField] private GameObject cuttingDesk;
    [SerializeField] private GameObject cmpObjPillar;

    // Start is called before the first frame update
    void Start()
    {
        componentsList = new List<GameObject>();
        pillarList = new List<GameObject>();
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
        
        /*if (Input.GetKeyDown("n"))
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
        }*/

        if (timerValue >= maxQuestionTime)
        {
            QuestionTimeElapsed();
        }
    }

    void DisplayQuestion()
    {
        Vector3 questionObjectCoords = cuttingDesk.GetComponentInChildren<Transform>().position;   
        Vector3 pillarObjectCoords = new Vector3(questionObjectCoords.x + 2, questionObjectCoords.y,
                                                                     questionObjectCoords.z);
        
        Collider cuttingDeskCollider = cuttingDesk.GetComponentInChildren<Collider>();
        // questionObjectCoords.y += cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y*0.5f;
        
        questionObject = Instantiate(currentQuestion.questionObject, questionObjectCoords, Quaternion.identity);
        Collider questionObjectCollider = questionObject.GetComponentInChildren<Collider>();
        questionObject.transform.Translate(0,
            cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y*0.5f, 0);

        float pillarStartCoordZ = questionObjectCoords.z + currentQuestion.componentObjects.Count - 1;
        for (int i = 0; i < currentQuestion.componentObjects.Count; i++)
        {
            pillarObjectCoords = new Vector3(pillarObjectCoords.x, pillarObjectCoords.y,
                pillarStartCoordZ - i * 2.0f); 
            pillarList.Add(Instantiate(cmpObjPillar,pillarObjectCoords, Quaternion.identity));  
            Collider pillarCollider = pillarList[i].GetComponentInChildren<Collider>();
            
            // Debug.Log("pillar bounds: " + pillarCollider.bounds.size);
            // Debug.Log("component bounds: " + componentCollider.bounds.size);
            componentsList.Add(Instantiate(currentQuestion.componentObjects[i], 
                pillarObjectCoords,
                Quaternion.identity));
            Collider componentCollider = componentsList[i].GetComponentInChildren<Collider>();
            componentsList[i].transform.Translate(0,pillarCollider.bounds.size.y + componentCollider.bounds.size.y * 0.5f,0);
            

        }
        ResetTimer();
        
        // So depending on the number of objects, their start position is different. 
        // each pillar is 2 away from another pillar.
        // so if there is 2 pillars, they are each 1 unit away from the middle. 1
        // if there are 3 pillars, one is in the middle and the others are 2 away from it on either side. 2
        // 3
        // 4
        
        // so the leftmost pillar is equal to 

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


