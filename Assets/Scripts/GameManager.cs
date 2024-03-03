using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //score tracker
    //timer  

    //count down

    public TMP_Text scoreText; // ������ �� ��������� ���� ��� ����������� �����
    public TMP_Text timerText; // ������ �� ��������� ���� ��� ����������� �������
   
    // ������ ���������� � ������ �������� ���������

    public static int score = 0; // ���������� ��� ������������ �����
    private float counter = 0; // ��������� �������� �������

    public static GameManager instance;

 
    void Start()
    {
        score = 0;

        instance = this;
    }
    
    void FixedUpdate()
    {
        // Update the timer value every FixedUpdate
       
        UpdateTimer();
        UpdateTimerUI();
         
    }
    // ����� ��� ���������� ����������� �����
    void UpdateScoreUI(int score)
    {      
        scoreText.text = score.ToString(); // ��������� ����� �����
    }   
  
    void UpdateTimer()
    {
        // Calculate the time elapsed between frames and subtract it from the timer
        //if(!sceneController.IsGameOver)
        //{
        float timeElapsed = Time.fixedDeltaTime;
        counter += timeElapsed;
        //}

        //// If the timer goes below 0, clamp it to 0
        //if(counter < 1f && !isEndStarted)
        //{
        //    isEndStarted = true;
        //    AudioSource audioSource = FindObjectOfType<AudioManager>().GetComponent<AudioSource>();
        //    StartCoroutine(sceneController.StartFadeAudio(1.0f, 0, audioSource));
        //}
        //if (counter < 0f)
        //{
        //    counter = 0f;
        //    // Optionally, you can handle game over logic here
        //    Debug.Log("Time's up!");
        //    sceneController.IsGameOver = true;
        //}
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(counter / 60);
        int seconds = Mathf.FloorToInt(counter % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = timerString;

    }
   

    // ����� ��� ���������� �����
    public static void IncreaseScore(int points)
    {
        score += points; // ����������� ���� �� ��������� ���������� �����
        instance.UpdateScoreUI(score); // ��������� ����������� �����
    }

    public static IEnumerator DepleteSliderValue(Slider slider, float duration, float delay = 0f)
    {
        // Wait for the delay
        yield return new WaitForSeconds(delay);

        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float progress = (Time.time - startTime) / duration;
            slider.value = Mathf.Lerp(slider.maxValue, slider.minValue, progress);

            yield return null;
        }

        // Set the final value to zero to ensure completeness
        slider.value = slider.minValue;
    }

}
