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
    struct QuestionData
    {
        public int SlicesMade;
        public int UndoCount;
        public double TimeTaken;
        public bool AnsweredCorrectly;
    }
    private List<int> _objectsSliced;
    private List<string> _fragmentRoots;
    private List<string> _slicedObjs;
    private List<QuestionData> _allQuestionsData;
    private int _undoCount;
    
    private PlayerInput _playerInput;
    private InputAction _undoAction;
    private InputAction _nextQuestionAction;

    [SerializeField] private List<QuestionSO> questions;
    private int _currentQuestionIndex;
    private List<GameObject> _componentsList;
    private List<GameObject> _pillarList;
    private GameObject _questionObject;
    private QuestionSO _currentQuestion;
    private int _gameScore;
    
    [SerializeField] private double timeToCompleteQuiz = 300f;
    private double _timerValue;
    private double _timerChange;
    public double maxQuestionTime;
    private double _timeTaken;
    private double _slicesMade;
    private int _answerStatus;
    [SerializeField] private GameObject cuttingDesk;
    [SerializeField] private GameObject cmpObjPillar;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Player");
        _undoAction = _playerInput.currentActionMap.FindAction("UndoCut");
        _nextQuestionAction = _playerInput.currentActionMap.FindAction("NextQuestion");

        _allQuestionsData = new List<QuestionData>();

        _objectsSliced = new List<int>();
        _fragmentRoots = new List<string>();
        _slicedObjs = new List<string>();
        _undoCount = 0;
        
        
        _pillarList = new List<GameObject>();
        _componentsList = new List<GameObject>();
        _currentQuestion = questions[_currentQuestionIndex];
        DisplayQuestion();
        _timerChange = Time.deltaTime/10;
        maxQuestionTime = 40.0;
        
    }

    // Update is called once per frame
    void Update()
    {
        _timerValue += _timerChange;
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
        
        _questionObject = Instantiate(_currentQuestion.questionObject, questionObjectCoords, Quaternion.identity);
        Collider questionObjectCollider = _questionObject.GetComponentInChildren<Collider>();
        _questionObject.transform.Translate(0,
            cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y*0.5f, 0);

        float pillarStartCoordZ = questionObjectCoords.z + _currentQuestion.componentObjects.Count - 1;
        for (int i = 0; i < _currentQuestion.componentObjects.Count; i++)
        {
            pillarObjectCoords = new Vector3(pillarObjectCoords.x, pillarObjectCoords.y,
                pillarStartCoordZ - i * 4.0f); 
            _pillarList.Add(Instantiate(cmpObjPillar,pillarObjectCoords, Quaternion.identity));  
            Collider pillarCollider = _pillarList[i].GetComponentInChildren<Collider>();
          
            _componentsList.Add(Instantiate(_currentQuestion.componentObjects[i], 
                pillarObjectCoords,
                Quaternion.identity));
            Collider componentCollider = _componentsList[i].GetComponentInChildren<Collider>();
            _componentsList[i].transform.Translate(0,pillarCollider.bounds.size.y + componentCollider.bounds.size.y * 0.5f,0);
            

        }
        ResetCurrentQuestionData();
        
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
            _undoCount++;
            _fragmentRoots.RemoveAt(_fragmentRoots.Count - 1);
            _slicedObjs.RemoveAt(_slicedObjs.Count - 1);
            _objectsSliced.RemoveAt(_objectsSliced.Count - 1);
        }
    }
    
    private void QuestionComplete()
    {
        
        StopTimer();
        _timeTaken = _timerValue;
        
        
        Debug.Log(" time score: " + maxQuestionTime / _timeTaken*100);
        Debug.Log(" cuts score: " + _currentQuestion.maxCuts / _slicesMade * 200);
        if (_answerStatus == 1)
        {
            _gameScore += 5000
                         + Math.Min(2000, Convert.ToInt32(maxQuestionTime / _timeTaken*100))
                         - Math.Min(500, Convert.ToInt32(_currentQuestion.maxCuts / _slicesMade * 200));
        }
        Debug.Log("Game Score is: " + _gameScore);
        _allQuestionsData.Add(new QuestionData {
                SlicesMade = Convert.ToInt16(_slicesMade),
                TimeTaken = _timeTaken,
                AnsweredCorrectly = _answerStatus == 1,
                UndoCount = _undoCount 
        });
        GetNextQuestion();
    }
    
    void GetNextQuestion()
    {
        _currentQuestionIndex++;
        _currentQuestion = questions[_currentQuestionIndex];
        
        foreach (var component in _componentsList)
        {
            Destroy(component);
        }

        foreach (var pillar in _pillarList)
        {
            Destroy(pillar);
        }

        _objectsSliced = new List<int>();
        _componentsList = new List<GameObject>();
        _pillarList = new List<GameObject>();
        Destroy(GameObject.Find($"{_questionObject.name}Slices"));
        Destroy(_questionObject);
        DisplayQuestion();
    }

    private void UpdateSliceUI(){}

    public void ObjectSliced(int value, string fragmentRootName, string slicedObjName)

    {   
        _objectsSliced.Add(value);
        _fragmentRoots.Add(fragmentRootName);
        _slicedObjs.Add(slicedObjName);
        _slicesMade++;
        CheckAnswer();
        if (_answerStatus != 0)
        {
            QuestionComplete();
            return;
        }
        UpdateSliceUI();
    }

    private void CheckAnswer()
    {
        var consecutiveCorrectSlices = 0;
        foreach (var s in _objectsSliced)
        {
            consecutiveCorrectSlices += s != -1 ? 1 : 0;
            if (s == -1) break;
        }

        if (_objectsSliced.TrueForAll(s => s != -1) &&
            _objectsSliced.Count == _currentQuestion.componentObjects.Count - 1)
        {
            _answerStatus = 1;
            Debug.Log("Correct!");
        }
        else if (_currentQuestion.maxCuts - _slicesMade <
            _currentQuestion.componentObjects.Count - 1 - consecutiveCorrectSlices) _answerStatus = -1;
    }

    void QuestionTimeElapsed()
    {
        // Pause Game
        //
    }
    private void ResetCurrentQuestionData()
    {
        _timerValue = 0;
        _timerChange = Time.deltaTime/10;
        _slicesMade = 0;
        _undoCount = 0;
        _answerStatus = 0;
    }

    void StopTimer()
    {
        _timerChange = 0;
    }
    
    
    
}
