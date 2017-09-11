import * as React from "react";

import Search from "./Search";
import Graphs from "./Graphs";

export class Body extends React.Component {
  render() {
    let body =
      <section>
        <Search />
        <Graphs />
      </section>;
    return body;
  }
}