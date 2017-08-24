import * as React from "react";

import {Link} from "react-router-dom";

export class Header extends React.Component {
    render() {
        let header =
            <div>
                <h1>StockWatcher</h1>
                <Link to="/signup">Signup</Link>
                <Link to="/login">Login</Link>
            </div>;

        return header;
    }
}