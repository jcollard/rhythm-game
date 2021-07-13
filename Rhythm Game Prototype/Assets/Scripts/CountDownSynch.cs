using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownSynch : MonoBehaviour
{
    public double timeRemaining = 3;
    public Text countdownText;
    public BeatMapper beatMapper;
    public bool showCountdown = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void displayCountDown()
    {
        countdownText.gameObject.SetActive(true);
        showCountdown = true;
        timeRemaining = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (showCountdown)
        {
            timeRemaining -= Time.deltaTime;
            countdownText.text = ((int)timeRemaining + 1).ToString();

            if (timeRemaining <= 0 && !beatMapper.isPlaying)
            {
                beatMapper.StartPlaySynch();
                countdownText.gameObject.SetActive(false);
            }
        }
        
    }
}
