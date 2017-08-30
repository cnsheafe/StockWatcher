import * as React from "react";
import Header from "./Header";
import {Body} from "./Body";

export class App extends React.Component {
    render() {
        return (
            <div>
                <Header/>
                <Body/>
            </div>
        );
    }
}