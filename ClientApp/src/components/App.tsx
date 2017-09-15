import * as React from "react";
import Header from "./Header";
import { Body } from "./Body";
import {connect} from "react-redux";
import store, {IState} from "../store/store";
import { toggleModalDisplay } from "../store/actions";

interface AppProps {
  showModal: boolean
}


class App extends React.Component<AppProps, {}> {
  render() {
    return (
      <main className={this.props.showModal ? "DimBody" : ""}>
        <Header />
        <Body />
      </main>
    );
  }
}

function mapStateToProps(state: IState) {
  return {
    showModal: state.showModal
  }
}

export default connect(mapStateToProps)(App);