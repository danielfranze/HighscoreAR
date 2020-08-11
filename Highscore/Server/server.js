/**
 * The server holds the current highscore and manages the communication of the
 * individual clients
 */

const WebSocket = require("ws");
const wss = new WebSocket.Server({ port: 3030 });
const _ = require("lodash");
const defaults = require("./databases/defaults.json");
const debuggingEnabled = true;

var index;
var currentID;
var currentPlayer;
var avatars;
var currentHighscore;

loadDefaults();

/**
 * Runs when a client has connected to the server.
 */
wss.on("connection", function connection(ws, req) {
  if (debuggingEnabled)
    console.log(
      "[Server INFO] >> Client with IP address " +
        req.connection.remoteAddress.toString() +
        " has connected to the server"
    );

  /**
   * Runs when a client sends a message to the server
   */
  ws.on("message", function incoming(data) {
    /**
     * Checks the received message for whether the client wants to reset the
     * high score or insert a new score (!isNaN).
     * The expression "!isNaN" checks if the client has sent a number.
     */
    if (data == "reset") {
      loadDefaults();

      if (debuggingEnabled) console.log("[Server INFO] >> Highscore reset!");

    } else if (!isNaN(data)) {

      /**
       * creates a new player and pushes him with the received 
       * value (score) from the client into the highscore list (json)
       */
      currentHighscore.push({
        id: currentID,
        points: parseInt(data),
        avatar: avatars[index][0],
        avatarnumber: avatars[index][1],
      });

      /**
       * sorts the highscore list (json) with all players (including the new player)
       */
      currentHighscore = _.orderBy(
        currentHighscore,
        ["points", "id"],
        ["desc", "asc"]
      );

      /**
       * the data of the current player will be saved to send the data 
       * back to the client who sent the score
       */
      currentPlayer = [];
      currentPlayer.push({
        id: currentID,
        position: currentHighscore.findIndex((obj) => obj.id == currentID) + 1,
        points: data,
        avatar: avatars[index][0],
        avatarnumber: avatars[index][1],
      });

      /**
       * increments the index values for the next score 
       */
      avatars[index][1] += 1;
      currentID += 1;

      /**
       * when you reach the last avatar, it will jump back to the first one
       */
      if (index == avatars.length - 1) {
        index = 0;
      } else {
        index += 1;
      }

      if (debuggingEnabled)
        console.log(
          "[Server INFO] >> New score added! > ID: " +
            (currentID - 1) +
            " score: " +
            data
        );
    }

    /**
     * Runs for each connected client
     */
    wss.clients.forEach(function each(client) {
      if (data === "update" && client === ws) {
        /**
         * Will only be sent to the client who wants the current score
         */
        client.send(JSON.stringify(currentHighscore));

        if (debuggingEnabled)
          console.log(
            "[Server INFO] >> Client with IP address " +
              req.connection.remoteAddress.toString() +
              " was updated!  "
          );
      } else {
        if (data !== "reset" && client === ws) {
          /**
           * The currently created player is only sent to the client that has
           * sent the score of this player
           */
          client.send(JSON.stringify(currentPlayer));
        }

        /**
         * Is executed if the high score has changed
         */
        client.send(JSON.stringify(currentHighscore));
      }
    });

    if (debuggingEnabled && data !== "update") {
      console.log(
        "[Server INFO] >> " + wss.clients.size + " clients were updated!"
      );
    }
  });

  /**
   * Runs when a client has closed the connection to the server
   */
  ws.on("close", function close() {
    if (debuggingEnabled)
      console.log(
        "[Server INFO] -- Client with IP address " +
          req.connection.remoteAddress.toString() +
          " has disconnected!"
      );
  });
});

/**
 * Resets the server to its initial state
 */
function loadDefaults() {
  index = _.cloneDeep(defaults.index);
  currentID = _.cloneDeep(defaults.currentID);
  currentPlayer = _.cloneDeep(defaults.currentPlayer);
  avatars = _.cloneDeep(defaults.avatars);
  currentHighscore = _.cloneDeep(defaults.highscore);
}
