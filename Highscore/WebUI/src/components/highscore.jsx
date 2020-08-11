import _ from "lodash";
import React, { Component } from "react";
import { Menu, Icon, Header, Modal, Button, Label, Image, Transition, Popup, Loader, Grid } from "semantic-ui-react";
import defaults from "../assets/json/defaults.json";
import img from "../assets/images/logo_clear_white_without_text.png";
import img_donald from "../assets/images/player/Donald.png";
import img_gangnam from "../assets/images/player/Gangnam.png";
import img_kenny from "../assets/images/player/Kenny.png";
import img_meninblack from "../assets/images/player/MenInBlack.png";
import img_yoda from "../assets/images/player/Yoda.png";
import img_unicorn from "../assets/images/player/Unicorn.png";
import img_myLittlePony from "../assets/images/player/MyLittlePony.png";
import img_nerd from "../assets/images/player/Nerd.png";
import img_wario from "../assets/images/player/Wario.png";
import img_wizard from "../assets/images/player/wizard.png";
import img_placeholder from "../assets/images/player/placeholder.png";

/**
 * The Highscore class manages the communication with the server
 * and generates the code for the web interface of the highscore
 */
export default class Highscore extends Component {
  constructor(props) {
    super(props);
    this.child = React.createRef();
    this.connect = this.connect.bind(this);
    this.state = {
      ws: new WebSocket("ws://localhost:3030"),
      modalOpen: false,
      loadingScreenOpen: false,
      numberOfRows: 3,
      numberOfColumns: 8,
      isConnected: false,
      showResetWindow: true,
      data: defaults,
    };
  }

  /**
   * Opens the confirm reset menu
   */
  handleModalOpen = () => {
    this.setState({ modalOpen: true });
  };

  /**
   * Closes the confirm reset menu
   */
  handleModalClose = () => {
    this.setState({ modalOpen: false });
  };

  /**
   * Returns for each matching avatar string the corresponding avatar image
   */
  getImageByName = (name) => {
    switch (name) {
      case "Donald":
        return img_donald;
      case "PSY":
        return img_gangnam;
      case "Kenny":
        return img_kenny;
      case "Agent":
        return img_meninblack;
      case "Yoda":
        return img_yoda;
      case "Unicorn":
        return img_unicorn;
      case "Pony":
        return img_myLittlePony;
      case "Nerd":
        return img_nerd;
      case "Wario":
        return img_wario;
      case "Wizard":
        return img_wizard;
      default:
        return img_placeholder;
    }
  };

  /**
   * Will be executed when the react component is mounted
   */
  componentDidMount() {
    this.connect();
  }

  /**
   * The connection to the server is newly established
   */
  reconnect = () => {
    let ws2 = new WebSocket("ws://localhost:3030");
    this.setState({ ws: ws2 });
  };

  /**
   * Includes the standard websocket events
   */
  connect = () => {
    this.state.ws.onopen = () => {
      this.setState({ loadingScreenOpen: false });
      console.log("connected");
      this.setState({ isConnected: true });
      this.sendUpdateMessage();
    };

    this.state.ws.onmessage = (evt) => {
      console.log(evt.data);
      this.setState({ data: JSON.parse(evt.data) });
    };

    this.state.ws.onclose = () => {
      console.log("disconnected");

      // automatically try to reconnect on connection loss
      this.setState({ isConnected: false });
      setTimeout(() => {
        if (!this.state.isConnected) {
          console.log("Reconnecting...");
          this.setState({ loadingScreenOpen: true });
          this.reconnect();
          this.connect();
        }
      }, 100);
    };
  };

  /**
   * Sends an update message to the server to get the current highscore
   */
  sendUpdateMessage = () => {
    console.log("Update");
    this.state.ws.send("update");
  };

  /**
   * Sends a reset message to the server to reset the current highscore to default
   */
  sendResetMessage = () => {
    console.log("sendResetMessage");
    this.state.ws.send("reset");
    this.setState({ modalOpen: false });
  };

  /**
   * The render method includes the jsx code of the react component.
   */
  render() {
    const header = (
      <div>
        <Menu borderless stackable inverted size="massive">
          {/***********************************
           *  Logo
           ***********************************/}
          <Menu.Item id="logo" position="left" className="link" href="">
            <Image src={img} size="small" className="logo_uni" />
          </Menu.Item>

          {/***********************************
           *  Header Title
           ***********************************/}
          <Menu.Item id="menu-item-center">
            <Header as="h1" underline="false">
              <span className="header_title">HIGHSCORES</span>
            </Header>
          </Menu.Item>

          {/***********************************
           *  Reset Button
           ***********************************/}
          <Menu.Item position="right" onClick={this.handleModalOpen}>
            <Transition animation={"jiggle"} duration={500}>
              <div className="player_count">{this.state.data.length - 24} Players</div>
            </Transition>
          </Menu.Item>

          <Icon name="trash alternate" size="small" className="link delete-icon" onClick={this.handleModalOpen} />

          <Modal dimmer="blurring" open={this.state.modalOpen} onClose={this.handleModalClose} basic size="small">
            <Header size="medium" icon="warning sign" content="Confirm Reset" />
            <Modal.Content>
              <p>
                <Icon size="large" name="angle double right" />
                You're trying to reset the high score!
              </p>
              <p>
                <Icon size="large" name="angle double right" />
                Are you sure you want to reset the high score?
              </p>
            </Modal.Content>
            <Modal.Actions>
              <Button size="big" color="green" onClick={this.sendResetMessage} inverted>
                <Icon size="big" name="checkmark" /> <p>Yes</p>
              </Button>
              <Button size="big" color="red" onClick={this.handleModalClose} inverted>
                <Icon size="big" name="delete" /> <p>No</p>
              </Button>
            </Modal.Actions>
          </Modal>

          <Modal dimmer="blurring" open={this.state.loadingScreenOpen} basic size="small">
            {/* nice css-loader, but high cpu usage*/}
            {/*<Loader indeterminate size="massive">TRYING TO RECONNECT...</Loader>*/}
            <Header size="medium" icon="" content="TRYING TO RECONNECT..." />
          </Modal>
        </Menu>
      </div>
    );

    const grid = (
      <Grid columns={this.state.numberOfColumns} className="table-area">
        {/***********************************
         *  High Scores (Grid)
         ***********************************/}
        {_.times(this.state.numberOfRows, (row) => (
          <Grid.Row key={"row" + row.toString()}>
            {_.times(this.state.numberOfColumns, (column) => (
              <Grid.Column key={"row" + row.toString() + "column" + column.toString()}>
                <span className="place-number-text">{column + 1 + row * this.state.numberOfColumns}. Platz</span>
                <Image
                  centered
                  className="place-number-img"
                  src={this.getImageByName(this.state.data[column + row * this.state.numberOfColumns].avatar)}
                />
                {true ? (
                  <div>
                    <Popup
                      inverted
                      content={"ID: " + this.state.data[column + row * this.state.numberOfColumns].id}
                      trigger={
                        <span className="place-number-name">
                          {this.state.data[column + row * this.state.numberOfColumns].avatar} #
                          {this.state.data[column + row * this.state.numberOfColumns].avatarnumber}{" "}
                        </span>
                      }
                    />
                    <br />
                    <Label>
                      <span>{this.state.data[column + row * this.state.numberOfColumns].points} P</span>
                    </Label>
                  </div>
                ) : (
                  <div />
                )}
              </Grid.Column>
            ))}
          </Grid.Row>
        ))}
      </Grid>
    );

    /**
     * Here is defined, which parts of the render method should be generated by the react preprocessor
     */
    return (
      <div
        style={{
          //maxWidth: "1920px",
          textAlign: "center",
          marginLeft: "auto",
          marginRight: "auto",
        }}
      >
        {header}
        {grid}
      </div>
    );
  }
}
