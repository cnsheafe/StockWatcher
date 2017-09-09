import * as React from "react";
import { Link } from "react-router-dom";

export class Login extends React.Component {

  sendLoginRequest(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const form = new FormData(document.getElementById("login-form") as HTMLFormElement);
    let loginRequest = new Request("/user/loginuser",
      {
        method: "POST",
        body: form
      }
    );

    return fetch(loginRequest)
      .then(res => {
        return res.text();
      })
      .then(text => {
        console.log(text);
      });
  }
  render() {
    return (
      <div>
        <h2>Login</h2>
        <form id="login-form" onSubmit={event => this.sendLoginRequest(event)}>
          <div><input type="text" name="Username"/></div>
          <div><input type="password" name="Password"/></div>
          <button type="submit">Submit</button>
        </form>
        <Link to="/Signup">Don't have an account? Signup here!</Link>
      </div>
    );
  }
}