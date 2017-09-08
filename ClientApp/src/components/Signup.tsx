import * as React from "react";
import { Link } from "react-router-dom";

export class Signup extends React.Component {
  render() {
    let signupForm: JSX.Element =
      <div>
        <h2>Signup</h2>
        <form action="/user/createuser" method="post" encType="application/json">
          <div>Username<input type="text"name="Username" required={true}/></div>
          <div>Password<input type="password" name="Password" required={true}/></div>
          <div>Phone Number<input type="tel" name="Phone" required={true}/></div>
          <div><button type="submit">Signup</button></div>
        </form>
        <Link to="/login">Already have an account? Login!</Link>
      </div>
      ;
    return signupForm;
  }
}