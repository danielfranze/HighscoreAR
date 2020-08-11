#pragma warning disable 0649
using UnityEngine;
using NativeWebSocket;

/// <summary>
/// Manages the websocket connection with the server and displays the 
/// corresponding windows based on the connection state with the server
/// </summary>
public class HighscoreWebSocket : MonoBehaviour
{
    [SerializeField] private GameObject activator; //
    [SerializeField] private HighscoreConfiguration guiConfig;
    [SerializeField] private bool debuggingEnabled = false;

    private WebSocket websocket;
    private bool closeWebsocket = false;


    void Start()
    {
        closeWebsocket = false;

        if (debuggingEnabled)
            Application.runInBackground = true;

        // Activates all child objects so that initializations can take place, otherwise exeptions are thrown
        activator.SetActive(true);
        activator.SetActive(false);

    }

    public void Enable()
    {
 

        if (guiConfig != null)
            guiConfig.DisplayLoadingScreenOnly();

        if (websocket != null)
        {
            if (websocket.State == WebSocketState.Open)
            {
                if (debuggingEnabled)
                    Debug.Log("Send score to the server");

                // send value to the server, when the WebSocket is open

                if (guiConfig != null)
                    guiConfig.DisplayLoadingScreenOnly();

                string score = guiConfig.GetCurrentPlayerScore().ToString();
                SendWebSocketMessage(score);
                if (debuggingEnabled)
                    Debug.Log("Send score (value: " + score + ") to the server");
            }

        }
        else
        {
            Connect();
        }

    }

    async void Connect()
    {
        websocket = new WebSocket("ws://localhost:3030");

        websocket.OnOpen += () =>
        {
            if (debuggingEnabled)
                Debug.Log("Connection open!");

            if (guiConfig != null)
                guiConfig.DisplayLoadingScreenOnly();

            SendWebSocketMessage("update");
        };

        websocket.OnError += (e) =>
        {
            if (debuggingEnabled)
                Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            if (debuggingEnabled)
                Debug.Log("Connection closed!");

            if (guiConfig != null)
                guiConfig.DisplayReconnectScreenOnly();

            if (!closeWebsocket)
                Connect();
        };

        websocket.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);

            if (debuggingEnabled)
                Debug.Log("OnMessage: " + message);

            bool currentPlayerScreenUpdated = !(guiConfig.UpdateHighscore(message.ToString()));
            if (currentPlayerScreenUpdated)
            {
                guiConfig.SetUICurrentPlayerActive(true);
            }
            else
            {
                guiConfig.GetLoading().SetActive(false);
                guiConfig.GetHighscoreEntries().SetActive(true);
            }


        };

        // waiting for messages
        await websocket.Connect();
    }

    async void SendWebSocketMessage(string message)
    {
        if (websocket.State == WebSocketState.Open)
        {

            if (guiConfig != null)
                guiConfig.DisplayLoadingScreenOnly();

            // sending plain text
            await websocket.SendText(message);
        }
    }

    private async void OnDestroy()
    {
        closeWebsocket = true;
        
        if(websocket != null)
            await websocket.Close();
    }

    private async void OncloseWebsocket()
    {
        closeWebsocket = true;
        
        if(websocket != null)
            await websocket.Close();
    }

}
