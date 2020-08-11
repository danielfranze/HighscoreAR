#pragma warning disable 0649
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the deactivation and activation of the highscore
/// </summary>
public class HighscoreActivator : MonoBehaviour
{
    [SerializeField] private GameObject highscoreWebSocket;
    [SerializeField] private GameObject highscoreComponents;
    [SerializeField] private AudioSource highscoreDisableSound;
    [SerializeField] private AudioSource highscoreEnableSound;

    private void OnEnable()
    {
        EnableHighscore();
        highscoreWebSocket.GetComponent<HighscoreWebSocket>().Enable();
    }

    private void EnableHighscore()
    {
        highscoreEnableSound.Play();
        highscoreComponents.SetActive(true);
    }

    public void DisableHighscore()
    {
        highscoreComponents.SetActive(false);
        StartCoroutine(PlaySoundAndDisable());
    }

    IEnumerator PlaySoundAndDisable()
    {
        highscoreDisableSound.Play();
        yield return new WaitWhile(() => highscoreDisableSound.isPlaying);
        gameObject.SetActive(false);
    }
}
