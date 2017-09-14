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
      <div className="signup">
        <h2 className="signup-title">Signup</h2>
        <form 
          id="signup-form" 
          onSubmit={event => this.sendSignupRequest(event)}
          className="signup-form">
          <div className="signup-username">
            <label htmlFor="signup-input-username">Username</label>
            <input 
              type="text" 
              name="Username" 
              id="signup-input-username" 
              required={true}/>
          </div>
          <div className="signup-password">
            <label htmlFor="signup-input-password">Password</label>
            <br/>
            <input 
              type="password" 
              name="Password" 
              id="signup-input-password"
              required={true}/>
          </div>
          <div className="signup-phone">
            <label htmlFor="signup-input-phone">Phone Number</label>
            <br/>
            <input 
              type="tel" 
              name="Phone" 
              id="signup-input-phone"
              required={true}/>
          </div>
          <div className="signup-submit">
            <button type="submit">Signup</button>
          </div>
        </form>
        <Link to="/login" className="goto-login">Already have an account? Login!</Link>
      </div>
    );
  }
}