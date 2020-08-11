#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Counts down time and shows this in the highscore window, 
/// as soon as the counter reaches 0, the highscore window closes.
/// </summary>
public class HighscoreCountdown : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private string text;
    [SerializeField] private Text countdown;
    [SerializeField] private GameObject activator;

    private float timeLeft;

    private void Start()
    {
        timeLeft = time;
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            timeLeft -= Time.deltaTime;
            countdown.text = text + Mathf.Round(timeLeft);
            if (timeLeft < 0)
            {
                activator.GetComponent<HighscoreActivator>().DisableHighscore();
            }
        }

    }
    private void OnDisable()
    {
        timeLeft = time;
    }
}
