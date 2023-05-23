// Name: Jason Leech
// Date: 10/25/2022
// Desc:

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Tooltip("The amount of time in seconds after which the time up event should be invoked")]
    public float time = 0;

    //[Tooltip("The event that will be invoked when the time is up")]
    //public UnityEvent timeUp = new UnityEvent();

    public TMP_Text timerText;
    private void Start()
    {

    }
    private void Update()
    {
        time += Time.deltaTime;
        timerText.text = "" + (int)time;
    }
}