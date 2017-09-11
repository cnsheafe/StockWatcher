import * as React from "react";

import { connect } from "react-redux";
import { Link } from "react-router-dom";
import { bindActionCreators } from "redux";

export interface ILoginProps {
  loggedIn: boolean
}

class Header extends React.Component<ILoginProps> {

  render() {
    let header: JSX.Element;

    if (this.props.loggedIn) {
      header =
        <div className="header" role="header">
          <h1 className="header-title">StockWatcher</h1>
          <button className="header-account">Account</button>
        </div>
    }
    else {
      header =
        <div className="header" role="header">
          <h1 className="header-title">StockWatcher</h1>
          <span className="header-link-group">
            <Link className="header-link" to="/signup">Signup</Link>
            <Link className="header-link" to="/login">Login</Link>
          </span>
        </div>;
    }

    return header;
  }
}

const mapStatetoProps = state => state;

export default connect(mapStatetoProps)(Header);