using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Security.Cryptography;
using Image = UnityEngine.UI.Image;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    

    private enum QuestionStatus
    {
        Unanswered,
        CorrectAnswer,
        TimeElapsed,
        SlicesUsed
    }
    struct QuestionData
    {
        public int SlicesMade;
        public int UndoCount;
        public double TimeTaken;
        public QuestionStatus QuestionResult;
    }
    
    // Input System
    private PlayerInput _playerInput;
    private InputAction _undoAction;
    private InputAction _nextQuestionAction;
    
    // Game Data
    [SerializeField] private List<QuestionSO> questions;
    private int _currentQuestionIndex;
    private List<GameObject> _componentsList;
    private List<GameObject> _pillarList;
    private GameObject _questionObject;
    private QuestionSO _currentQuestion;
    private List<int> _objectsSliced;
    private List<string> _fragmentRoots;
    private List<string> _slicedObjs;
    
    // User question data
    private double _timerValue;
    private double _timerChange;
    public double maxQuestionTime;
    private double _timeTaken;
    private int _slicesMade;
    private int _undoCount;
    private int _gameScore;
    private QuestionStatus _answerStatus;
    private List<QuestionData> _allQuestionsData;
    
    // Game assets
    [SerializeField] private GameObject cuttingDesk;
    [SerializeField] private GameObject cmpObjPillar;

    // UI Elements
    [SerializeField] private GameObject _sliceImagePrefab;
    [SerializeField] private Sprite _sliceUsedSprite;
    [SerializeField] private Sprite _sliceUnusedSprite;
    [SerializeField] private GameObject _sliceMarkers;
    private List<GameObject> _sliceMarkersList;
    [SerializeField] private Text _scoreText;

    
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

        _scoreText.text = $"Score: {_gameScore}";
        
        ResetCurrentQuestionData();
        InitialiseSliceUI();

    }

    private void InitialiseSliceUI()
    {
        _sliceMarkersList = new List<GameObject>();
        for (int i = 0; i < _currentQuestion.maxCuts; i++)
        {
            _sliceMarkersList.Add(Instantiate(_sliceImagePrefab, _sliceMarkers.transform));
            _sliceMarkersList[i].GetComponent<Image>().sprite = _sliceUnusedSprite;
        }
    }
    private void UpdateSliceUI()
    {
        _sliceMarkersList[^_slicesMade].GetComponent<Image>().sprite = _sliceUsedSprite;
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

        if (_answerStatus == QuestionStatus.CorrectAnswer)
        {
            int unusedSlicesBonus =
                ( _currentQuestion.maxCuts - _slicesMade > _currentQuestion.componentObjects.Count + 1 )? (_currentQuestion.maxCuts - _currentQuestion.componentObjects.Count + 1 - _slicesMade) * 50 : 0;
            int timeBonus = Math.Min(2000, Convert.ToInt32(maxQuestionTime / _timeTaken * 100));
            _gameScore += 5000
                          + timeBonus
                          + unusedSlicesBonus;
            Debug.Log("Time Bonus: " + timeBonus);
            Debug.Log("Unused Slices Bonus: " + unusedSlicesBonus);
        }
        Debug.Log("Game Score is: " + _gameScore);
        _allQuestionsData.Add(new QuestionData {
                SlicesMade = _slicesMade,
                TimeTaken = _timeTaken,
                QuestionResult = _answerStatus,
                UndoCount = _undoCount 
        });
        DisplayQuestionResult();
        GetNextQuestion();
    }

    void DisplayQuestionResult()
    {
        
    }
    
    void GetNextQuestion()
    {
        _currentQuestionIndex++;
        _currentQuestion = questions[_currentQuestionIndex];
        
        foreach (var component in _componentsList)
            Destroy(component);
        foreach (var pillar in _pillarList) 
            Destroy(pillar);
        foreach (var sliceMarker in _sliceMarkersList) 
            Destroy(sliceMarker);

        _objectsSliced = new List<int>();
        _componentsList = new List<GameObject>();
        _pillarList = new List<GameObject>();
        Destroy(GameObject.Find($"{_questionObject.name}Slices"));
        Destroy(_questionObject);
        DisplayQuestion();
    }

    

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
            _answerStatus = QuestionStatus.CorrectAnswer;
            Debug.Log("Correct!");
        }
        else if (_currentQuestion.maxCuts - _slicesMade <
            _currentQuestion.componentObjects.Count - 1 - consecutiveCorrectSlices) _answerStatus = QuestionStatus.SlicesUsed;
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
        _answerStatus = QuestionStatus.Unanswered;
    }

    void StopTimer()
    {
        _timerChange = 0;
    }
}
