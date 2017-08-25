import * as React from "react";
import { Link } from "react-router-dom";

export class Login extends React.Component {
    render() {
        let loginForm: JSX.Element =
            <div>
                <h2>Login</h2>
                <form action="" method="post">
                    <div><input type="text"/></div>
                    <div><input type="text"/></div> 
                    <button type="submit">Submit</button>
                </form>
                <Link to="/Signup">Don't have an account? Signup here!</Link>
            </div>
        ;
        return loginForm;
    }
}