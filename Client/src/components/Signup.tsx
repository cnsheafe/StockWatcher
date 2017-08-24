import * as React from "react";
import {Link} from "react-router-dom";

export class Signup extends React.Component {
    render() {
        let signupForm: JSX.Element = 
            <div>
                <h2>Signup</h2>
                <form action="" method="post">
                    <div><input type="text"/></div>
                    <div><input type="text"/></div> 
                </form>
                <Link to="/login">Already have an account? Login!</Link>
            </div>
        ;
        return signupForm;
    }
}