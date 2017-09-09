import * as React from "react";
import { Link } from "react-router-dom";

export class Signup extends React.Component {

  sendSignupRequest(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const form = new FormData(document.getElementById("signup-form") as HTMLFormElement);
    const headers = new Headers();
    let signupRequest = new Request(`/user/createuser`, 
      {
        method: "POST",
        body: form,
      }
    );

    return fetch(signupRequest)
      .then(res => {
        return res.text();
      })
      .then(text => {
        console.log(text);
        window.localStorage.setItem("StockWatcherToken", text);
        window.location.href = "/";
      });
  }

  render() {
    return(
      <div>
        <h2>Signup</h2>
        <form id="signup-form" onSubmit={event => this.sendSignupRequest(event)}>
          <div>Username<input type="text"name="Username" required={true}/></div>
          <div>Password<input type="password" name="Password" required={true}/></div>
          <div>Phone Number<input type="tel" name="Phone" required={true}/></div>
          <div><button type="submit">Signup</button></div>
        </form>
        <Link to="/login">Already have an account? Login!</Link>
      </div>
    );
  }
}