using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    private Quiz[] quizList;
    private Quiz activeQuiz;
    private StartScreen startScreen;
    private int quizCount;
    private ShowScoreScreen showScoreScreen;
    private bool activeQuizComplete;
    private TextMeshProUGUI scoreText;
    
     
    // Start is called before the first frame update
    void Start()
    {   
        quizList = FindObjectsOfType<Quiz>();
        startScreen = FindObjectOfType<StartScreen>();
        showScoreScreen = FindObjectOfType<ShowScoreScreen>();
        scoreText = showScoreScreen.GetComponentInChildren<TextMeshProUGUI>();
        foreach(Quiz quiz in quizList)
        {
            quiz.gameObject.SetActive(false);
        }

        showScoreScreen.gameObject.SetActive(false);
        startScreen.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (activeQuizComplete)
        {
            
            activeQuiz.ClearQuiz();
            activeQuiz.gameObject.SetActive(false);    
        }
        else if (activeQuiz)
        {
            startScreen.gameObject.SetActive(false);
            activeQuiz.gameObject.SetActive(true);
        }
        
    }

    
    public void OnFinishTest(int testType)
    {
        
        Debug.Log("OnFinishTest Activated!");
        // Calculate the total scores 
        if (testType == (int) TestType.CombinedTest)
        {
            if (quizCount == quizList.Length - 1)
            {
                activeQuiz.gameObject.SetActive(false);
                showScoreScreen.gameObject.SetActive(true);
            }
            else
            {
                quizCount++;
                activeQuiz = quizList[quizCount];
            }
        }
        else
        {
            Debug.Log("showing score screen");
            activeQuizComplete = true;
            showScoreScreen.gameObject.SetActive(true);
            scoreText.SetText("Your Score: " + activeQuiz.GetScore() + "/" + activeQuiz.GetNumberOfQuestions());
            
            
        }
    }

    public void OnRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnStartTest(int testType)
    {
        
        switch ((TestType)testType)
        {
            case TestType.Mct:
                activeQuiz = quizList.Where(quiz => quiz.name.Contains("MCT")).Select(quiz => quiz).ToArray()[0];
               
                break;
            case TestType.Mrt:
                activeQuiz = quizList.Where(quiz => quiz.name.Contains("MRT")).Select(quiz => quiz).ToArray()[0];
                break;
            case TestType.CombinedTest:
                activeQuiz = quizList[0];
                quizCount = 0;
                break;

        }
    }
}
