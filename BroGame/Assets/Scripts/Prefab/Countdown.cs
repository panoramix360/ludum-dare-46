using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;

public class Countdown : MonoBehaviour
{
    [SerializeField] private UnityEvent countdownEnded;
    private TextMeshProUGUI countdownText;
    private float countdown;

    private int period = 60;

    private void Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
            countdownText.text = (countdown % period).ToString("00");
        }

        // finished countdown
        if (countdown < 0)
        {
            countdownEnded.Invoke();
            countdown = 0;
        }
    }

    public void SetCountdown(int seconds)
    {
        countdown = seconds;
    }
}
