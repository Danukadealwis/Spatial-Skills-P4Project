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
    public int timerState = 0;
    public float fillFraction;


    void Start()
    {
        timerChange = Time.deltaTime;
    }


    // Update is called once per frame
    void Update()
    {
    
        timerValue -= timerChange;
        Debug.Log(timerChange);
        if (timerValue <= 0)
        {
            SetTimerState((int)TimerState.TimeElapsed);
        }
        fillFraction = timerValue / timeToCompleteQuiz;

    }

    public void SetTimerState(int state)
    {
        switch ((TimerState)state)
        {
            case TimerState.QuizStart:
                timerChange = Time.deltaTime;
                timerValue = timeToCompleteQuiz;
                break;
            case TimerState.QuizComplete:
                timerChange = 0;
                break;
            case TimerState.TimeElapsed:
                timerChange = 0;
                timerValue = timeToCompleteQuiz;
                break;
                
        }

        timerState = state;

    }

    public int GetTimerState()
    {
        return timerState;
    } 
}
