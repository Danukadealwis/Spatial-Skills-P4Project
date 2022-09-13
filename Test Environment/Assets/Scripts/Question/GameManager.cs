using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Timers;
using Image = UnityEngine.UI.Image;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
        public float TimeTaken;
        public QuestionStatus QuestionResult;
    }

    private int _correctAnswerPoints;

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
    private float _timerValue;
    private int _timeRemaining;
    private float _timerChange;
    public float maxQuestionTime;
    private float _timeTaken;
    private int _slicesMade;
    private int _undoCount;
    private int _gameScore;
    private int _unusedSlicesBonus;
    private int _speedBonus;
    private bool _questionComplete;
    private QuestionStatus _answerStatus;
    private List<QuestionData> _allQuestionsData;

    // Game assets
    [SerializeField] private GameObject cuttingDesk;
    [SerializeField] private GameObject cmpObjPillar;

    // UI Elements
    [SerializeField] private GameObject sliceImagePrefab;
    [SerializeField] private Sprite sliceUsedSprite;
    [SerializeField] private Sprite sliceUnusedSprite;
    [SerializeField] private GameObject gameInformation;
    private GameObject _sliceMarkers;
    private Text _scoreText;
    private Text _timeRemainingText;
    private List<GameObject> _sliceMarkersList;
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private GameObject resultCanvasText;
    private List<GameObject> _resultTexts;
    private Transform _correctAnswerMessages;



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

        maxQuestionTime = 10.0f;
        _correctAnswerPoints = 5000;

        _correctAnswerMessages = resultCanvas.transform.Find("Messages");
        _sliceMarkers = gameInformation.transform.Find("SlicesUsed/SliceMarkers").gameObject;
        _scoreText = gameInformation.transform.Find("TimeAndScore/ScoreText").gameObject.GetComponent<Text>();
        _timeRemainingText =
            gameInformation.transform.Find("TimeAndScore/TimeRemaining").gameObject.GetComponent<Text>();
        _resultTexts = new List<GameObject>();
        _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));

        DisplayQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (_answerStatus == QuestionStatus.Unanswered)
        {
            _timerValue += _questionComplete ? 0:Time.deltaTime;
            _timeRemaining = Math.Max(0, Convert.ToInt32(maxQuestionTime - _timerValue));
            _timeRemainingText.text = $"Time Remaining: {_timeRemaining}";
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

        if (_timeRemaining == 0)
        {
            _timeRemaining = Int32.MaxValue;
            _answerStatus = QuestionStatus.TimeElapsed;
            Debug.Log("time elapsed");
            QuestionComplete();
            return;
        }

    }



    void DisplayQuestion()
    {
        resultCanvas.SetActive(false);
        Vector3 questionObjectCoords = cuttingDesk.GetComponentInChildren<Transform>().position;
        Vector3 pillarObjectCoords = new Vector3(questionObjectCoords.x + 4, questionObjectCoords.y,
            questionObjectCoords.z);

        Collider cuttingDeskCollider = cuttingDesk.GetComponentInChildren<Collider>();
        // questionObjectCoords.y += cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y*0.5f;

        _questionObject = Instantiate(_currentQuestion.questionObject, questionObjectCoords, Quaternion.identity);
        Collider questionObjectCollider = _questionObject.GetComponentInChildren<Collider>();
        _questionObject.transform.Translate(0,
            cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y * 0.5f, 0);

        float pillarStartCoordZ = questionObjectCoords.z + _currentQuestion.componentObjects.Count - 1;
        for (int i = 0; i < _currentQuestion.componentObjects.Count; i++)
        {
            pillarObjectCoords = new Vector3(pillarObjectCoords.x, pillarObjectCoords.y,
                pillarStartCoordZ - i * 4.0f);
            _pillarList.Add(Instantiate(cmpObjPillar, pillarObjectCoords, Quaternion.identity));
            Collider pillarCollider = _pillarList[i].GetComponentInChildren<Collider>();

            _componentsList.Add(Instantiate(_currentQuestion.componentObjects[i],
                pillarObjectCoords,
                Quaternion.identity));
            Collider componentCollider = _componentsList[i].GetComponentInChildren<Collider>();
            _componentsList[i].transform
                .Translate(0, pillarCollider.bounds.size.y + componentCollider.bounds.size.y * 0.5f, 0);


        }

        _scoreText.text = $"Score: {_gameScore}";

        ResetCurrentQuestionData();
        InitialiseSliceUI();
        _questionComplete = false;

    }

    private void InitialiseSliceUI()
    {
        _sliceMarkersList = new List<GameObject>();
        for (int i = 0; i < _currentQuestion.maxCuts; i++)
        {
            _sliceMarkersList.Add(Instantiate(sliceImagePrefab, _sliceMarkers.transform));
            _sliceMarkersList[i].GetComponent<Image>().sprite = sliceUnusedSprite;
        }
    }

    private void UpdateSliceUI()
    {
        _sliceMarkersList[^_slicesMade].GetComponent<Image>().sprite = sliceUsedSprite;
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
        ResetGameScene();
        _questionComplete = true;
        _timeTaken = _timerValue;
        _timerValue = Single.MaxValue;

        if (_answerStatus == QuestionStatus.CorrectAnswer)
        {
            _unusedSlicesBonus =
                (_currentQuestion.maxCuts - _slicesMade > _currentQuestion.componentObjects.Count - 1)
                    ? (_currentQuestion.maxCuts - _currentQuestion.componentObjects.Count + 1 - _slicesMade) * 50
                    : 0;
            _speedBonus = Math.Min(2000, Convert.ToInt32(maxQuestionTime / _timeTaken * 100));
            _gameScore += _correctAnswerPoints
                          + _speedBonus
                          + _unusedSlicesBonus;
            Debug.Log("Time Bonus: " + _speedBonus);
            Debug.Log("Unused Slices Bonus: " + _unusedSlicesBonus);
        }

        Debug.Log("Game Score is: " + _gameScore);
        _allQuestionsData.Add(new QuestionData
        {
            SlicesMade = _slicesMade,
            TimeTaken = _timeTaken,
            QuestionResult = _answerStatus,
            UndoCount = _undoCount
        });
        DisplayQuestionResult();
    }

    void DisplayQuestionResult()
    {
        
        foreach (var text in _resultTexts)
        {
            Destroy(text);
        }
        _resultTexts = new List<GameObject>();
        resultCanvas.SetActive(true);
        switch (_answerStatus)
        {
            case QuestionStatus.CorrectAnswer:
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts[0].GetComponent<Text>().text = "Nice Work!";
                _resultTexts[1].GetComponent<Text>().text = $"CorrectAnswer: {_correctAnswerPoints}";
                _resultTexts[2].GetComponent<Text>().text = $"Speed Bonus: {_speedBonus}";
                _resultTexts[3].GetComponent<Text>().text = $"Unused Slices Bonus: {_unusedSlicesBonus}";
                _resultTexts[4].GetComponent<Text>().text = $"Game Score: {_gameScore}";
                break;
            case QuestionStatus.SlicesUsed:
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts[0].GetComponent<Text>().text = "Oh no...";
                _resultTexts[1].GetComponent<Text>().text =
                    "You ran out of enough slices to answer the question correctly!";
                break;
            case QuestionStatus.TimeElapsed:
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(resultCanvasText, _correctAnswerMessages));
                _resultTexts[0].GetComponent<Text>().text = "Oh no...";
                _resultTexts[1].GetComponent<Text>().text = "You ran out of time!";
                break;
        }
    }

    void ResetGameScene()
    {
        foreach (var component in _componentsList)
            Destroy(component);
        foreach (var pillar in _pillarList)
            Destroy(pillar);
        foreach (var sliceMarker in _sliceMarkersList)
            Destroy(sliceMarker);

        _objectsSliced = new List<int>();
        _componentsList = new List<GameObject>();
        _pillarList = new List<GameObject>();
        if (GameObject.Find($"{_questionObject.name}Slices") != null)
            Destroy(GameObject.Find($"{_questionObject.name}Slices"));
        else
        {
            Debug.Log("Null found");
        }

        Destroy(_questionObject);

    }

    public void GetNextQuestion()
    {

        _currentQuestionIndex++;
        _currentQuestion = questions[_currentQuestionIndex];
        
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
                 _currentQuestion.componentObjects.Count - 1 - consecutiveCorrectSlices)
            _answerStatus = QuestionStatus.SlicesUsed;
    }

    void QuestionTimeElapsed()
    {
        // Pause Game
        //
    }

    private void ResetCurrentQuestionData()
    {
        _timerValue = 0;
        _slicesMade = 0;
        _undoCount = 0;
        _answerStatus = QuestionStatus.Unanswered;
    }
}

    
