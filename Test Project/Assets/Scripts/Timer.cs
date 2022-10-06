using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timeToCompleteQuiz = 300f;
    private float timerValue;
    private float timerChange;
    public float fillFraction;
    private TimerState _timerState;


    void Start()
    {
        _timerState = TimerState.QuizStart;
    }


    // Update is called once per frame
    void Update()
    {
    
        // Debug.Log(timerChange);
        
        fillFraction = timerValue / timeToCompleteQuiz;
        switch (_timerState)
        {
            case TimerState.QuizStart:
                timerValue = timeToCompleteQuiz;
                _timerState = TimerState.QuizRunning;
                Debug.Log(_timerState);
                break;
            case TimerState.QuizRunning:
                // timerValue -= Time.deltaTime;
                break;
            case TimerState.QuizComplete:
                timerValue = float.MaxValue;
                break;
            case TimerState.TimeElapsed:
                timerValue = timeToCompleteQuiz;
                break;
            
        }
        if (timerValue <= 0)
        {
            _timerState = TimerState.TimeElapsed;
        }

    }

    public void SetTimerState(int state)
    {
        

        _timerState = (TimerState)state;

    }

    public int GetTimerState()
    {
        return (int)_timerState;
    } 
}
