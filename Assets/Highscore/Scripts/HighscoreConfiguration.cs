#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

/// <summary>
/// Offers methods to control and initialize the gui elements
/// </summary>
public class HighscoreConfiguration : MonoBehaviour
{
    [Header("UI Game Objects")]
    [SerializeField] private GameObject reconnect;
    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject highscoreEntries;
    [SerializeField] private GameObject currentPlayer;

    [Header("Others")]
    [SerializeField] private SimpleFloatValue currentPlayerScore;
    [SerializeField] private AudioSource updateSound;

    [Header("Avatars")]
    [SerializeField] private Sprite img_donald;
    [SerializeField] private Sprite img_gangnam;
    [SerializeField] private Sprite img_kenny;
    [SerializeField] private Sprite img_meninblack;
    [SerializeField] private Sprite img_yoda;
    [SerializeField] private Sprite img_unicorn;
    [SerializeField] private Sprite img_mylittlepony;
    [SerializeField] private Sprite img_nerd;
    [SerializeField] private Sprite img_wario;
    [SerializeField] private Sprite img_wizard;
    [SerializeField] private Sprite img_placeholder;


    private void OnEnable()
    {

        DisplayCurrentPlayerScore();
    }

    public void SetUICurrentPlayerActive(bool value)
    {
        if (value)
        {
            foreach (Transform child in currentPlayer.transform)
            {
                if (child.gameObject.name != "Points")
                    child.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in currentPlayer.transform)
            {
                if (child.gameObject.name != "Points")
                    child.gameObject.SetActive(false);
            }
        }
    }

    public GameObject GetReconnect()
    {
        return reconnect;
    }
    public GameObject GetHighscoreEntries()
    {
        return highscoreEntries;
    }

    public GameObject GetLoading()
    {
        return loading;
    }

    public int GetCurrentPlayerScore()
    {
        return ((int)currentPlayerScore.value);
    }

    public void DisplayLoadingScreenOnly()
    {
        GetReconnect().SetActive(false);
        SetUICurrentPlayerActive(false);
        GetHighscoreEntries().SetActive(false);
        GetLoading().SetActive(true);
    }

    public void DisplayReconnectScreenOnly()
    {
        GetLoading().SetActive(false);
        GetHighscoreEntries().SetActive(false);
        GetReconnect().SetActive(true);
    }

    public void DisplayCurrentPlayerScore()
    {
        currentPlayer.transform.GetChild(1).GetComponent<Text>().text
            = GetCurrentPlayerScore().ToString() + " P";
    }

    private Sprite GetImageByName(string name)
    {
        switch (name)
        {
            case "Donald":
                return (img_donald);
            case "PSY":
                return (img_gangnam);
            case "Kenny":
                return (img_kenny);
            case "Agent":
                return (img_meninblack);
            case "Yoda":
                return (img_yoda);
            case "Unicorn":
                return (img_unicorn);
            case "Pony":
                return (img_mylittlepony);
            case "Nerd":
                return (img_nerd);
            case "Wario":
                return (img_wario);
            case "Wizard":
                return (img_wizard);
            default:
                return (img_placeholder);
        }
    }

    /// <summary>
    /// Updates either the highscore list or the current player in the header, depending 
    /// on whether the json object contains: one element (current player) or several (highscore list)
    /// </summary>
    public bool UpdateHighscore(string jsonAsString)
    {
        JSONNode json = JSON.Parse(jsonAsString);

        int numberOfRows = highscoreEntries.transform.childCount;


        if (json.Count == 1)
        {
            // current player (header)
            string position = json[0]["position"].Value.ToString() + ". Platz";
            string points = json[0]["points"].Value.ToString() + " P";
            string playername = json[0]["avatar"].Value.ToString() + " #" + json[0]["avatarnumber"].Value.ToString();
            Sprite avatar = GetImageByName(json[0]["avatar"].Value.ToString());

            currentPlayer.transform.GetChild(0).GetComponent<Text>().text = position;
            currentPlayer.transform.GetChild(1).GetComponent<Text>().text = points;
            currentPlayer.transform.GetChild(2).GetComponent<Text>().text = playername;
            currentPlayer.transform.GetChild(3).GetComponent<Image>().sprite = avatar;

            return false;

        }
        else
        {
            // entries (body)
            for (int row = 0; row < numberOfRows; row++)
            {
                int numberOfColumns = highscoreEntries.transform.GetChild(row).transform.childCount;

                for (int column = 0; column < numberOfColumns; column++)
                {

                    int index = (column + (row * numberOfColumns));
                    Transform entry = highscoreEntries.transform.GetChild(row).transform.GetChild(column).transform;

                    string position = (index + 1).ToString() + ". Platz";
                    string points = json[index]["points"].Value.ToString() + " P";
                    string playername = json[index]["avatar"].Value.ToString() + " #" + json[index]["avatarnumber"].Value.ToString();
                    Sprite avatar = GetImageByName(json[index]["avatar"].Value.ToString());

                    entry.GetChild(0).GetComponent<Text>().text = position;
                    entry.GetChild(1).GetComponent<Text>().text = points;
                    entry.GetChild(2).GetComponent<Text>().text = playername;
                    entry.GetChild(3).GetComponent<Image>().sprite = avatar;

                }
            }

            if(updateSound.isActiveAndEnabled)
                updateSound.Play();

            return true;

        };

    }

}
