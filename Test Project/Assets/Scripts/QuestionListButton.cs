using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestionListButton : MonoBehaviour
{

    [SerializeField]private TextMeshProUGUI text;

    private Quiz quiz;
    // Start is called before the first frame update

    void Awake()
    {
        quiz = FindObjectOfType<Quiz>();
    }

    public void SetText(string textInput)
    {
        text.text = textInput;
        name = textInput;
    }

    public void SetOnClick(int questionIndex)
    {
        GetComponentInParent<Button>().onClick.AddListener(delegate { quiz.NavigateQuestion(questionIndex); });
        if (name.Contains(1.ToString()))
        {
            GetComponentInParent<Button>().interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
