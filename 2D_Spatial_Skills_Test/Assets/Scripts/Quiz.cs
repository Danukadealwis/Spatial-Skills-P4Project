using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Linq;
using DefaultNamespace;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using TimerState = DefaultNamespace.TimerState;


public class Quiz : MonoBehaviour
{
    private QuestionSO currentQuestion;
    [SerializeField] private List<QuestionSO> questions;
    [SerializeField] private GameObject[] answerGroups;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private TextMeshProUGUI questionText;
    private GameManager gameManager;
    private int[] userAnswersList; 
    private Timer timer;
    [SerializeField] private Image questionImage;
    [SerializeField] private Image timerImage;
    private int currentQuestionIndex;
    private bool resetNextButtonOnClick;
    private ConfirmationPanel confirmationPanel;
    
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
        nextButton.onClick.AddListener(delegate { NavigateQuestion(currentQuestionIndex+1);});
        previousButton.onClick.AddListener(delegate { NavigateQuestion(currentQuestionIndex-1);});
        confirmationPanel = transform.Find("Confirmation Panel").GetComponent<ConfirmationPanel>();
        confirmationPanel.gameObject.SetActive(false);
    }
    
    //Call this when the next button is selected
    public void NavigateQuestion(int questionIndex)
    {
        currentQuestionIndex = questionIndex;
        GetQuestion();
        DisplayQuestion();
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
        
        Image answerImage;
        Button answerButton;
        TextMeshProUGUI questionNumberIndicator;
        TextMeshProUGUI answerButtonText;
        Button[] questionListButtons;

        questionText.text = currentQuestion.questionText;
        for (int i = 0; i < answerGroups.Length; i++)
        {
            answerButton = answerGroups[i].GetComponentInChildren<Button>();
            answerButtonText = answerGroups[i].GetComponentInChildren<TextMeshProUGUI>();
          
            if (userAnswersList != null)
            {
                if (userAnswersList[currentQuestionIndex] == i)
                {
                    answerButton.image.color = Color.black;
                    answerButtonText.color = Color.white;
                }else
                {
                    answerButton.image.color = Color.white;
                    answerButtonText.color = Color.black;
                }
            }
            answerImage = answerGroups[i].GetComponentInChildren<Image>();
            answerImage.sprite = currentQuestion.GetSprite(i);
            answerImage.preserveAspect = true;
            answerButtonText.text = currentQuestion.GetAnswer(i);
            
        }

        questionNumberIndicator = transform.Find("Question Number Indicator").GetComponent<TextMeshProUGUI>();
        questionNumberIndicator.text = "Question: " + (currentQuestionIndex + 1) + "/" + questions.Count;
        
        questionImage.sprite = currentQuestion.GetQuestionImage();
        questionImage.preserveAspect = true;
        
        if (currentQuestionIndex == questions.Count - 1)
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Finish Test!";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowConfirmationPanel);
            // nextButton.onClick.AddListener(delegate { gameManager.OnFinishTest((int) TestType.Mct); });
            previousButton.interactable = true;
            resetNextButtonOnClick = true;

        }else if (currentQuestionIndex == 0)
        {
            previousButton.interactable = false;
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
            if (resetNextButtonOnClick)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(delegate { NavigateQuestion(currentQuestionIndex+1); });
                resetNextButtonOnClick = false;
            }
        }
        
        else
        {
            previousButton.interactable = true;
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";

            if (resetNextButtonOnClick)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(delegate { NavigateQuestion(currentQuestionIndex+1); });
                resetNextButtonOnClick = false;
            }
        }

        questionListButtons = transform.Find("Question List").GetComponentsInChildren<Button>();
        foreach (var button in questionListButtons)
        {
            button.interactable = true;
        }
        questionListButtons = questionListButtons
            .Where(button => button.name.
                    Contains((currentQuestionIndex + 1).
                        ToString())).Select(button => button).ToArray();
   
        foreach (var button in questionListButtons)
        {
            button.interactable = false;
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

    public void ShowConfirmationPanel()
    {
        
        TextMeshProUGUI questionsUnanswered;
        
        confirmationPanel.gameObject.SetActive(true);
        questionsUnanswered = confirmationPanel.GetComponentsInChildren<TextMeshProUGUI>()
            .Where(text => text.name.Contains("Questions Unanswered"))
                .Select(text => text).ToArray()[0];
        questionsUnanswered.gameObject.SetActive(userAnswersList.Contains(-1));
    }

    public void HideConfirmationPanel()
    {
        confirmationPanel.gameObject.SetActive(false);
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
        Button answerButton;
        TextMeshProUGUI buttonText;
        
        foreach (GameObject answerGroup in answerGroups)
        {
            answerButton = answerGroup.GetComponentInChildren<Button>();
            buttonText = answerGroup.GetComponentInChildren<TextMeshProUGUI>();
            switch ((AnswerButtonState)state)
            {
                case AnswerButtonState.Active:
                    answerButton.interactable = true;
                    break;
                case AnswerButtonState.Disabled:
                    answerButton.interactable = false;
                    break;
                case AnswerButtonState.Reset:
                    // Set button color to what it was
                    answerButton.image.color = Color.white;
                    buttonText.color = Color.black;
                    answerButton.interactable = true;
                    break;
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        timerImage.fillAmount = timer.fillFraction;
        if (timer.GetTimerState() == (int) TimerState.TimeElapsed)
        {
            SetButtonState((int) AnswerButtonState.Disabled);
        }
        
        
    }
}