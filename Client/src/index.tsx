import * as React from "react";
import * as ReactDOM from "react-dom";
import {Provider} from "react-redux";
import {createStore} from "redux";
import {BrowserRouter as Router, Route, Switch} from "react-router-dom";
import {createBrowserHistory} from "history";

import {App} from "./components/App";
import {Signup} from "./components/Signup";
import {Login} from "./components/Login";
import storeApp from "./reducer";

let store = createStore(storeApp);


ReactDOM.render(
    <Provider store={store}>
            <div>
                <Router>
                    <Switch>
                        <Route exact path="/" component={App}/>
                        <Route path="/signup" component={Signup}/>
                        <Route path="/login" component={Login}/>
                    </Switch>
                </Router>
            </div>
    </Provider>,
    document.getElementById("root")
);