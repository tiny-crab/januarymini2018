using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private float currentTime;

    private float timeInPercent;

    private float attention; // how alerted is the Pig.


    private void Start()
    {
        sliderDistance.value = 0;
        StartCoroutine(Timer());
        currentTime = maxTime;
        attention = 0;

    }


    private void Update()
    {
        sliderDistance.value = distance * -1; //the closer the hand to the border, the higher the alert-meter thing

        timeInPercent = currentTime / maxTime / 100;
        sliderTimer.value = timeInPercent;
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);
        currentTime--;
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
        attention = incoming;
    }

    public void EndGame(bool positive)
    {
        if (positive)
            Debug.Log("You win");
        else
            Debug.Log("You lose");
    }
}
