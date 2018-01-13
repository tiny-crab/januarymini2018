using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private
        Slider sliderDistance; // for the distance check

    [SerializeField]
    private
     Slider sliderTimer; // for the timer check

    private float distance;

    [SerializeField]
    private float maxTime = 60;

    [SerializeField]
    private float maxDistance = 100;

    private float currentTime;

    private float timeInPercent;

    private float attention; // how alerted is the Pig.

    [SerializeField]
    private float simulationSpeed;

    [SerializeField]
    private Image currentPortrait;
    [SerializeField]
    private Image currentPunctuation;

    [SerializeField]
    private Sprite[] portrait;

    [SerializeField]
    private Sprite[] punctuation;

    public enum State {Unaware, Suspicious, Alerted,EndGame};

    State state;

    [SerializeField]
    private GameObject gameOverScreen;

    private void Start()
    {
        state = State.Unaware;
        sliderDistance.value = 0;
        StartCoroutine(Timer());
        currentTime = maxTime;
        attention = 0;
        StartCoroutine(Timer());

    }


    private void Update()
    {
        Debug.Log("Distance:" + distance);
        sliderDistance.value = (distance / maxDistance); //the closer the hand to the border, the higher the alert-meter thing

        timeInPercent = currentTime / maxTime;
        sliderTimer.value = timeInPercent;
        
        if (Input.GetKeyDown(KeyCode.Escape))
            {
            SceneManager.LoadScene(0);
            }
        

    }

    IEnumerator Timer()
    {
        
        yield return new WaitForSeconds(1f/simulationSpeed);
        currentTime -= 1/simulationSpeed;
        if (currentTime <= 0)
        {
            EndGame(false);
        }
        else
        StartCoroutine(Timer());
    }

    public void UpdateDistance(float incoming)
    {
        distance = incoming;

    }

    public void UpdateAttention(float incoming)
    {
        attention += incoming;
    }

    public void EndGame(bool positive)
    {
        if (positive)
            Debug.Log("You win");
        else
            gameOverScreen.SetActive(true);
    }

    public void UpdateState()
    {
        state ++;
        switch (state)
        {
            case State.Unaware:
                currentPortrait.sprite = portrait[0];
                currentPunctuation.sprite = punctuation[0];
                break;
            case State.Suspicious:
                currentPortrait.sprite = portrait[1];
                currentPunctuation.sprite = punctuation[1];

                break;
            case State.Alerted:
                currentPortrait.sprite = portrait[2];
                currentPunctuation.sprite = punctuation[2];

                break;
            case State.EndGame:
                gameOverScreen.SetActive(true);
                break;
            default:
                Debug.LogError("No. Just no.");
                break;
        }
            
            
    }
}
