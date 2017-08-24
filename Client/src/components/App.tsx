import * as React from "react";
import {Header} from "./Header";
import {Body} from "./Body";

export class App extends React.Component {
    render() {
        let jsx =
            <div>
                <Header/>
                <Body/>
            </div>
        return jsx;
    }
}