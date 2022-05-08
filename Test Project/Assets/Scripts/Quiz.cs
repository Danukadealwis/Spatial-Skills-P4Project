using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    private QuestionSO currentQuestion;
    [SerializeField] private List<QuestionSO> questions = new List<QuestionSO>();
    [SerializeField] private TextMeshProUGUI questionTextArea;
    [SerializeField] private GameObject[] answerGroups;
    private Timer timer;
    [SerializeField] private Image timerImage;
    private int currentQuestionIndex;
    
    /*
    private int correctAnswerIndex;
    [SerializeField] private Sprite defaultAnswerSprite;
    [SerializeField] private Sprite correctAnswerSprite;
    */
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        timer = FindObjectOfType<Timer>();
        timer.SetTimerState((int)TimerState.QuizStart);
        currentQuestion = questions[0];
        DisplayQuestion();

    }

    //Call this when the next button is selected
    public void NavigateQuestion(int questionNumberChange)
    {
        //Needs to be changed to the state is showing the previously selected answer.
        SetButtonState((int)AnswerButtonState.Reset);

        if (currentQuestionIndex + questionNumberChange == questions.Count - 1)
        {
            currentQuestionIndex += questionNumberChange;
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

    void DisplayQuestion()
    {
        TextMeshProUGUI button;
        Image image;
        questionTextArea.text = currentQuestion.GetQuestionText();
        
        for (int i = 0; i < answerGroups.Length; i++)
        {
            image = answerGroups[i].GetComponentInChildren<Image>();
            button = answerGroups[i].GetComponentInChildren<TextMeshProUGUI>();
            image.sprite = currentQuestion.GetSprite(i);
            button.text = currentQuestion.GetAnswer(i);
        } 
    }

    public void OnAnswerSelected(int index)
    {
        Button button;
        button = answerGroups[index].GetComponentInChildren<Button>();
        button.image.color = Color.black;
        TextMeshProUGUI buttonText = answerGroups[index].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = Color.white;
        
        //SetButtonState((int) ButtonState.Answered);
        
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
        timerImage.fillAmount = timer.fillFraction;
        if (timer.GetTimerState() == (int) TimerState.TimeElapsed)
        {
            SetButtonState((int) AnswerButtonState.Disabled);
        }
        
        
    }
}
