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
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

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

    struct LastGameData
    {
        public int GameScore;
        public int QuestionsAnsweredCorrectly;
    }
    
    //Constants
    private const int CorrectAnswerPoints = 5000;
    private const float DistanceToPillar = 3.0f;
    private const float DistanceBetweenPillar = 2.5f;
    private const float socketDeactivateTime = 3.0f;

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
    private bool isSocketDeactivated;

    // User question data
    private float _timerValue;
    private int _timeRemaining;
    private float _timerChange;
    private float _timeTaken;
    private int _slicesMade;
    private int _undoCount;
    private int _gameScore;
    private int _unusedSlicesBonus;
    private int _speedBonus;
    private bool _questionComplete;
    private QuestionStatus _answerStatus;
    private List<QuestionData> _allQuestionsData;
    private LastGameData _lastGameData;

    // Game assets
    [SerializeField] private GameObject cuttingDesk;
    [SerializeField] private GameObject cmpObjPillar;
    private Transform currentRotation;
    [SerializeField] private GameObject leftHandController;
    XRRayInteractor rayInteractor;
    Transform benchSocket;
    XRSocketInteractor benchSocketInteractor;
    float socketDeactivationTimer = 0;

    // UI Elements
    [SerializeField] private GameObject sliceImagePrefab;
    [SerializeField] private Sprite sliceUsedSprite;
    [SerializeField] private Sprite sliceUnusedSprite;
    [SerializeField] private GameObject gameInformation;
    [SerializeField] private GameObject endScreen;
    private GameObject _sliceMarkers;
    private Text _scoreText;
    private Text _timeRemainingText;
    private List<GameObject> _sliceMarkersList;
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private GameObject gameText;
    private List<GameObject> _resultTexts;
    private Transform _correctAnswerMessages;
    private Text _endScoreText;
    private Text _endQuestionsAnsweredText;



    // Start is called before the first frame update
    void Start()
    {
        StartGame();
        DisplayQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (_answerStatus == QuestionStatus.Unanswered)
        {
            
            if(isSocketDeactivated){
                socketDeactivationTimer += Time.deltaTime;
                if(socketDeactivationTimer >= socketDeactivateTime){
                    cuttingDesk.transform.Find("Socket").gameObject.SetActive(true);
                    isSocketDeactivated = false;
                }
            }
            _timerValue += _questionComplete ? 0:Time.deltaTime;
            _timeRemaining = Math.Max(0, Convert.ToInt32(_currentQuestion.maxQuestionTime - _timerValue));
            _timeRemainingText.text = $"Time Remaining: {_timeRemaining}";
            if (_undoAction.WasPressedThisFrame())
            {   cuttingDesk.transform.Find("Socket").gameObject.SetActive(false);
                leftHandController.GetComponent<XRRayInteractor>().enabled = false;
                UndoCut();
                cuttingDesk.transform.Find("Socket").gameObject.SetActive(true);
                leftHandController.GetComponent<XRRayInteractor>().enabled = true;
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
            QuestionComplete();
            return;
        }

        if (rayInteractor.interactablesSelected.Count > 0 && benchSocket && !benchSocket.GetComponent<XRSocketInteractor>().hasSelection) {
            benchSocket.transform.Find("rotation").transform.rotation = rayInteractor.interactablesSelected[0].transform.rotation;
        }

        
    }

    public void DeactivateSocket() {
        // calling this function will deactivate the socket for 3 seconds
        // 
        socketDeactivationTimer = 0;
        cuttingDesk.transform.Find("Socket").gameObject.SetActive(false);
        isSocketDeactivated = true;
    }

    void DisplayQuestion()
    {
        resultCanvas.SetActive(false);
        Vector3 questionObjectCoords = cuttingDesk.GetComponentInChildren<Transform>().position;
        Vector3 pillarObjectCoords = new Vector3(questionObjectCoords.x + DistanceToPillar, questionObjectCoords.y,
            questionObjectCoords.z);
        GameObject socketInteractor;
        Collider cuttingDeskCollider = cuttingDesk.GetComponentInChildren<Collider>();
        // questionObjectCoords.y += cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y*0.5f;

        _questionObject = Instantiate(_currentQuestion.questionObject, questionObjectCoords, Quaternion.identity);
        Collider questionObjectCollider = _questionObject.GetComponentInChildren<Collider>();
        _questionObject.transform.Translate(0,
            cuttingDeskCollider.bounds.size.y + questionObjectCollider.bounds.size.y * 0.5f, 0);

        float pillarStartCoordZ = questionObjectCoords.z +(_currentQuestion.componentObjects.Count - 1)*DistanceBetweenPillar/2;
        for (int i = 0; i < _currentQuestion.componentObjects.Count; i++)
        {
            pillarObjectCoords = new Vector3(pillarObjectCoords.x, pillarObjectCoords.y,
                pillarStartCoordZ - i * DistanceBetweenPillar);
            _pillarList.Add(Instantiate(cmpObjPillar, pillarObjectCoords, Quaternion.identity));
            Collider pillarCollider = _pillarList[i].GetComponentInChildren<Collider>();
            socketInteractor = _pillarList[i].transform.Find("Socket Interactor").gameObject;
            _componentsList.Add(Instantiate(_currentQuestion.componentObjects[i],
                socketInteractor.GetComponent<Transform>().position,_currentQuestion.componentObjects[i].transform.rotation));
            _componentsList[i].transform.SetParent(_pillarList[i].transform);
            _componentsList[i].transform.localPosition = socketInteractor.transform.localPosition;
            
        }
        _scoreText.text = $"Score: {_gameScore}";

        //benchSocket = cuttingDesk.transform.Find("Socket");
        benchSocket = cuttingDesk.transform.Find("Socket").transform;
        benchSocketInteractor = benchSocket.GetComponent<XRSocketInteractor>();

        // HoverEnterEventArgs hoverEnterEventArgs =
        //     new HoverEnterEventArgs();
        // benchSocketInteractor.hoverEntered.AddListener(hoverEnterEventArgs);
        // benchSocketInteractor.hoverEntered.Invoke(hoverEnterEventArgs);

        // Hover entered needs to call the function XRSocketInteractor.attachtransform
        // and have the transform of the held object as a parameter.
        
        ResetCurrentQuestionData();
        InitialiseSliceUI();
        

    }

    public void HeldObjectRotation(){
        currentRotation = cuttingDesk.transform.Find("Socket/rotation");
        
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
        Debug.Log("UNDO CUT!");
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
        
        _questionComplete = true;
        _timeTaken = _timerValue;
        _timerValue = Single.MaxValue;

        if (_answerStatus == QuestionStatus.CorrectAnswer)
        {
            _unusedSlicesBonus =
                (_currentQuestion.maxCuts - _slicesMade > _currentQuestion.componentObjects.Count - 1)
                    ? (_currentQuestion.maxCuts - _currentQuestion.componentObjects.Count + 1 - _slicesMade) * 50
                    : 0;
            _speedBonus = Math.Min(2000, Convert.ToInt32(_currentQuestion.maxQuestionTime / _timeTaken * 100));
            _gameScore += CorrectAnswerPoints
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
        
        
        _resultTexts = new List<GameObject>();
        resultCanvas.SetActive(true);
        switch (_answerStatus)
        {
            case QuestionStatus.CorrectAnswer:
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts[0].GetComponent<Text>().text = "Nice Work!";
                _resultTexts[1].GetComponent<Text>().text = $"CorrectAnswer: {CorrectAnswerPoints}";
                _resultTexts[2].GetComponent<Text>().text = $"Speed Bonus: {_speedBonus}";
                _resultTexts[3].GetComponent<Text>().text = $"Unused Slices Bonus: {_unusedSlicesBonus}";
                _resultTexts[4].GetComponent<Text>().text = $"Game Score: {_gameScore} Points!";
                break;
            case QuestionStatus.SlicesUsed:
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts[0].GetComponent<Text>().text = "Oh no...";
                _resultTexts[1].GetComponent<Text>().text =
                    "You ran out of enough slices to answer the question correctly!";
                break;
            case QuestionStatus.TimeElapsed:
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
                _resultTexts.Add(Instantiate(gameText, _correctAnswerMessages));
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
        foreach (var text in _resultTexts)
            Destroy(text);

        _slicedObjs = new List<string>();
        _fragmentRoots = new List<string>();
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
        ResetGameScene();
        if (_currentQuestionIndex == questions.Count)
        {
            
            resultCanvas.SetActive(false);
            DisplayEndScreen();
            return;
        }
        _currentQuestion = questions[_currentQuestionIndex];
        DisplayQuestion();
    }

    private void DisplayEndScreen()
    {
        int questionsAnsweredCorrectly = 0;
        foreach (var data in _allQuestionsData)
        {
            questionsAnsweredCorrectly += data.QuestionResult == QuestionStatus.CorrectAnswer ? 1 : 0;
        }

        LastGameData gameData = new LastGameData
        {
            GameScore = _gameScore,
            QuestionsAnsweredCorrectly = questionsAnsweredCorrectly
        };
        
        _endScoreText.text = $"Final Score: {_gameScore}";
        _endQuestionsAnsweredText.text = $"QuestionsAnsweredCorrectly: {questionsAnsweredCorrectly}/{questions.Count}";
        endScreen.SetActive(true);

        _gameScore = 0;
    }

    public void RestartGame()
    {
        ResetCurrentQuestionData();
        StartGame();
        DisplayQuestion();
    }
    private void  StartGame()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Player");
        _undoAction = _playerInput.currentActionMap.FindAction("UndoCut");
        _nextQuestionAction = _playerInput.currentActionMap.FindAction("NextQuestion");
        Debug.Log(_undoAction.bindings);

        _allQuestionsData = new List<QuestionData>();
        _objectsSliced = new List<int>();
        _fragmentRoots = new List<string>();
        _slicedObjs = new List<string>();
        _undoCount = 0;
        _pillarList = new List<GameObject>();
        _componentsList = new List<GameObject>();
        _currentQuestionIndex = 0;
        _currentQuestion = questions[_currentQuestionIndex];

        _correctAnswerMessages = resultCanvas.transform.Find("Messages");
        _sliceMarkers = gameInformation.transform.Find("SlicesUsed/SliceMarkers").gameObject;
        _scoreText = gameInformation.transform.Find("TimeAndScore/ScoreText").gameObject.GetComponent<Text>();
        _timeRemainingText =
            gameInformation.transform.Find("TimeAndScore/TimeRemaining").gameObject.GetComponent<Text>();

        _endScoreText = endScreen.transform.Find("Messages/EndScoreText").gameObject.GetComponent<Text>();
        _endQuestionsAnsweredText = endScreen.transform.Find("Messages/EndQuestionsAnsText").gameObject.GetComponent<Text>();
        endScreen.SetActive(false);
        rayInteractor = leftHandController.GetComponent<XRRayInteractor>();
    }

    public void ObjectSliced(int value, string fragmentRootName, string slicedObjName)

    {
        if (_questionComplete) return;
        _objectsSliced.Add(value);
        _fragmentRoots.Add(fragmentRootName);
        _slicedObjs.Add(slicedObjName);
        _slicesMade++;
        CheckAnswer();
        UpdateSliceUI();
        if (_answerStatus == 0) return;
        QuestionComplete();
        
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
            Debug.Log("_objectsSliced.Count" + _objectsSliced.Count);
            _answerStatus = QuestionStatus.CorrectAnswer;
            Debug.Log("Correct!");
        }
        else if (_currentQuestion.maxCuts - _slicesMade <
                 _currentQuestion.componentObjects.Count - 1 - consecutiveCorrectSlices)
            _answerStatus = QuestionStatus.SlicesUsed;
    }

    private void ResetCurrentQuestionData()
    {
        _timerValue = 0;
        _slicesMade = 0;
        _undoCount = 0;
        _answerStatus = QuestionStatus.Unanswered;
        _questionComplete = false;
    }
}

    
