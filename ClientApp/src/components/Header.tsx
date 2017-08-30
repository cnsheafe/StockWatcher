import * as React from "react";

import { connect } from "react-redux";
import { Link } from "react-router-dom";
import { bindActionCreators } from "redux";

import { toggleLogin } from "../store/actions";

export interface ILoginProps {
    loggedIn: boolean
}

class Header extends React.Component<ILoginProps> {

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

const mapStatetoProps = state => state;
// function mapDispatchtoProps(dispatch) {
//     return bindActionCreators( { toggleLogin },dispatch);
// }

export default connect(mapStatetoProps)(Header);