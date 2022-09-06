using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using UnityEngine.InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    private List<int> _objectsSliced;
    private List<string> _fragmentRoots;
    private List<string> _slicedObjs;
    private int undoCount;
    
    private PlayerInput _playerInput;
    private InputAction _undoAction;
    private InputAction _nextQuestionAction;

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
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Player");
        _undoAction = _playerInput.currentActionMap.FindAction("UndoCut");
        _nextQuestionAction = _playerInput.currentActionMap.FindAction("NextQuestion");
        

        _objectsSliced = new List<int>();
        _fragmentRoots = new List<string>();
        _slicedObjs = new List<string>();
        undoCount = 0;
        
        
        pillarList = new List<GameObject>();
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
        if (_undoAction.WasPressedThisFrame())
        {
            UndoCut();
            return;
        }
        if (_nextQuestionAction.WasPressedThisFrame())
        {
            GetNextQuestion();
            return;
        }
    }

    void DisplayQuestion(){
        Vector3 questionObjectCoords = cuttingDesk.GetComponentInChildren<Transform>().position;   
        Vector3 pillarObjectCoords = new Vector3(questionObjectCoords.x + 4, questionObjectCoords.y,
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
                pillarStartCoordZ - i * 4.0f); 
            pillarList.Add(Instantiate(cmpObjPillar,pillarObjectCoords, Quaternion.identity));  
            Collider pillarCollider = pillarList[i].GetComponentInChildren<Collider>();
          
            componentsList.Add(Instantiate(currentQuestion.componentObjects[i], 
                pillarObjectCoords,
                Quaternion.identity));
            Collider componentCollider = componentsList[i].GetComponentInChildren<Collider>();
            componentsList[i].transform.Translate(0,pillarCollider.bounds.size.y + componentCollider.bounds.size.y * 0.5f,0);
            

        }
        ResetTimer();
        
    }

    public void UndoCut()
    {
        if (_slicedObjs.Count != 0)
        {
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject obj = allObjects.Single(o => o.name == _slicedObjs.Last());
            GameObject root = GameObject.Find(_fragmentRoots.Last()).gameObject;

            Destroy(root.transform.gameObject);
            obj.SetActive(true);
            undoCount++;
            _fragmentRoots.RemoveAt(_fragmentRoots.Count - 1);
            _slicedObjs.RemoveAt(_slicedObjs.Count - 1);
            _objectsSliced.RemoveAt(_objectsSliced.Count - 1);
        }
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
                         - Math.Min(500, Convert.ToInt32(undoCount*200));
        }
        Debug.Log("Game Score is: " + gameScore);
        GetNextQuestion();
    }
    
    void GetNextQuestion()
    {
        currentQuestionIndex++;
        currentQuestion = questions[currentQuestionIndex];
        
        foreach (var component in componentsList)
        {
            Destroy(component);
        }

        foreach (var pillar in pillarList)
        {
            Destroy(pillar);
        }
        componentsList.Clear();
        pillarList.Clear();
        Destroy(GameObject.Find($"{questionObject.name}Slices"));
        Destroy(questionObject);
        DisplayQuestion();
    }

    public void AddObjectSliced(int value, string fragmentRootName, string slicedObjName)

    {
        _objectsSliced.Add(value);
        _fragmentRoots.Add(fragmentRootName);
        _slicedObjs.Add(slicedObjName);
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
