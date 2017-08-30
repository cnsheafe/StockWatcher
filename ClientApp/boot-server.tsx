import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
import * as React from 'react';
import { Provider } from 'react-redux';
import { renderToString } from 'react-dom/server';

import {BrowserRouter as Router, Route, Switch} from "react-router-dom";
import {StaticRouter} from "react-router-dom";
import {createStore} from "redux";

import Index from "./src/Index";

// import { replace } from 'react-router-redux';
// import { createMemoryHistory } from 'history';

export default createServerRenderer(params => {
    return new Promise<RenderResult>((resolve, reject) => {

        // Prepare Redux store with in-memory history, and dispatch a navigation event
        // corresponding to the incoming URL

        // const store = configureStore(createMemoryHistory());
        // store.dispatch(replace(urlAfterBasename));

        // // Prepare an instance of the application and perform an inital render that will
        // // cause any async tasks (e.g., data access) to begin
        // const routerContext: any = {};

        const appString = renderToString(Index(true, params));
        // // If there's a redirection, just send this information back to the host application
        // if (routerContext.url) {
        //     resolve({ redirectUrl: routerContext.url });
        //     return;
        // }
        
        // Once any async tasks are done, we can perform the final render
        // We also send the redux store state, so the client can continue execution where the server left off
        params.domainTasks.then(() => {
            resolve({
                html: appString,
            });
        }, reject); // Also propagate any errors back into the host application
    });
});
