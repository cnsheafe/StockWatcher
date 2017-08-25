import * as React from "react";

import {Link} from "react-router-dom";

export interface ILoginProps {
    loggedIn: boolean
}

export class Header extends React.Component<ILoginProps> {
    render() {
        let header: JSX.Element;
        if (this.props.loggedIn) {
            header = 
                <div>
                    <h1>StockWatcher</h1>
                    <button id="account">Account</button>
                </div>
        }
        else {
            header =
                <div>
                    <h1>StockWatcher</h1>
                    <Link to="/signup">Signup</Link>
                    <Link to="/login">Login</Link>
                </div>;
        }

        return header;
    }
}