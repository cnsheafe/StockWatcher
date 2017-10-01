import * as React from "react";
import Header from "./Header";
import { Body } from "./Body";
import { Footer } from "./Footer";

// Presentation Component for the entire app
export default class App extends React.Component {
  render() {
    return (
      <main>
        <Header />
        <Body />
        <Footer />
      </main>
    );
  }
}
