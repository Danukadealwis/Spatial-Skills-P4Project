using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Linq;
using DefaultNamespace;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Quiz : MonoBehaviour
{
    private QuestionSO currentQuestion;
    [SerializeField] private List<QuestionSO> questions = new List<QuestionSO>();
    [SerializeField] private GameObject[] answerGroups;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    private GameManager gameManager;
    private int[] userAnswersList; 
    private Timer timer;
    [SerializeField] private Image questionImage;
    [SerializeField] private Image timerImage;
    private int currentQuestionIndex;
    private bool resetNextButtonOnClick;
    
    /*
    private int correctAnswerIndex;
    [SerializeField] private Sprite defaultAnswerSprite;
    [SerializeField] private Sprite correctAnswerSprite;
    */
    
    
    
    // Start is called before the first frame update
    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        timer.SetTimerState((int)TimerState.QuizStart);
        currentQuestion = questions[0];
        DisplayQuestion();
        userAnswersList = Enumerable.Repeat(-1, questions.Count).ToArray();
        gameManager = FindObjectOfType<GameManager>();
        nextButton.onClick.AddListener(delegate { NavigateQuestion(1);});
        previousButton.onClick.AddListener(delegate { NavigateQuestion(-1);});


    }
    
    //Call this when the next button is selected
    public void NavigateQuestion(int questionNumberChange)
    {
        //Needs to be changed to the state is showing the previously selected answer.
        // SetButtonState((int)AnswerButtonState.Reset);
        Debug.Log("current question index before change: " + currentQuestionIndex);
        if (0 <= (currentQuestionIndex + questionNumberChange) && (currentQuestionIndex + questionNumberChange) < questions.Count)
        {
            
            currentQuestionIndex += questionNumberChange;
            Debug.Log("current question index: " + currentQuestionIndex);
            GetQuestion();
            DisplayQuestion();
        }

    }

    
    
    

    void GetQuestion()
    {
        if (questions[currentQuestionIndex])
        {
            currentQuestion = questions[currentQuestionIndex];
        }
    }

    public int GetNumberOfQuestions()
    {
        return questions.Count;
    }

    
    void DisplayQuestion()
    {
        TextMeshProUGUI button;
        Image answerImage;
        Button answerButton;
        
        
        for (int i = 0; i < answerGroups.Length; i++)
        {
            answerButton = answerGroups[i].GetComponentInChildren<Button>();
            TextMeshProUGUI buttonText = answerGroups[i].GetComponentInChildren<TextMeshProUGUI>();

            if (userAnswersList != null)
            {
                if (userAnswersList[currentQuestionIndex] == i)
                {
                    answerButton.image.color = Color.black;
                    buttonText.color = Color.white;
                }else
                {
                    answerButton.image.color = Color.white;
                    buttonText.color = Color.black;
                }
            }
            
        }
        
        Debug.Log("Displaying Question for " + this.name);
        questionImage.sprite = currentQuestion.GetQuestionImage();
        if (currentQuestionIndex == questions.Count - 1)
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Finish Test!";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(delegate { gameManager.OnFinishTest((int) TestType.Mct);});
            previousButton.interactable = true;
            resetNextButtonOnClick = true;

        }else if (currentQuestionIndex == 0)
        {
            previousButton.interactable = false;
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
        else
        {
            previousButton.interactable = true;
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";

            if (resetNextButtonOnClick)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(delegate { NavigateQuestion(1); });
                resetNextButtonOnClick = false;
            }
        }       
        for (int i = 0; i < answerGroups.Length; i++)
        {
            answerImage = answerGroups[i].GetComponentInChildren<Image>();
            button = answerGroups[i].GetComponentInChildren<TextMeshProUGUI>();
            answerImage.sprite = currentQuestion.GetSprite(i);
            button.text = currentQuestion.GetAnswer(i);
        } 
    }

    public int GetScore()
    {
        int score = 0;
        for(int i = 0; i < questions.Count; i++)
        {
            if (questions[i].GetCorrectAnswerIndex() == userAnswersList[i])
            {
                score++;
            }
        }

        return score;
    }

    // Use scene manager instead
    public void ClearQuiz()
    {
        currentQuestionIndex = 0;
        userAnswersList = Enumerable.Repeat(-1, questions.Count).ToArray();
    }

    public void OnAnswerSelected(int index)
    {
        userAnswersList[currentQuestionIndex] = index;
        Button button;
        for (int i = 0; i < answerGroups.Length; i++)
        {
            button = answerGroups[i].GetComponentInChildren<Button>();
            TextMeshProUGUI buttonText = answerGroups[i].GetComponentInChildren<TextMeshProUGUI>();
            if (i == index)
            {
                button.image.color = Color.black;
                buttonText.color = Color.white;
            }
            else
            {
                button.image.color = Color.white;
                buttonText.color = Color.black;
            }
        }
  
    }

    void SetButtonState(int state)
    {
        Button button;
        TextMeshProUGUI buttonText;
        foreach (GameObject answerGroup in answerGroups)
        {
            button = answerGroup.GetComponentInChildren<Button>();
            buttonText = answerGroup.GetComponentInChildren<TextMeshProUGUI>();
            switch ((AnswerButtonState)state)
            {
                case AnswerButtonState.Active:
                    button.interactable = true;
                    break;
                case AnswerButtonState.Disabled:
                    button.interactable = false;
                    break;
                case AnswerButtonState.Reset:
                    // Set button color to what it was
                    button.image.color = Color.white;
                    buttonText.color = Color.black;
                    button.interactable = true;
                    break;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // timerImage.fillAmount = timer.fillFraction;
        // if (timer.GetTimerState() == (int) TimerState.TimeElapsed)
        // {
        //     SetButtonState((int) AnswerButtonState.Disabled);
        // }
        
        
    }
}
