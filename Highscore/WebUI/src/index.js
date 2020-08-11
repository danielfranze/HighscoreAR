import React, { Component } from "react";
import ReactDOM from "react-dom";
import Highscore from "./components/highscore.jsx";
import "semantic-ui-css/semantic.min.css";
import "./assets/css/main.sass";

class Index extends Component {
  render() {
    return <Highscore />;
  }
}

/**
 * Here is the starting point of the web interface
 */
ReactDOM.render(<Index />, document.getElementById("root"));
