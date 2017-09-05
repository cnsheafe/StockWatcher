import { 
    createStore, 
    Reducer, 
    ActionCreator, 
    applyMiddleware 
} from "redux";

import thunk from "redux-thunk";
import * as Rx from "rxjs/Rx";

import { Graph, Company } from "./data-blocks";
import { 
    SEARCH, 
    RES_INDEX, 
    LOGIN,
    ValidAction
} from "./actions";


// Shape of the App State
export interface IState {
    loggedIn: boolean,
    searchResults: Array<Company>,
    graphs: Array<Graph>
}


function reducer(state: IState, action: ValidAction): IState {
    switch (action.type) {
        case SEARCH:
            return Object.assign({}, state, { searchResults: action.results });
        case LOGIN:
            return Object.assign({}, state, { loggedIn: !state.loggedIn });
        case RES_INDEX:
            let index = 0;
            let graphId = "graph0";
            if (state.graphs.length > 0) {
                index = state.graphs[state.graphs.length-1].index + 1
                graphId = `graph${index}`
            }
            const newGraph: Graph = {
                index: index,
                graphId: graphId,
                company: state.searchResults[action.index],
                dataset: [1,2,3,5],
                timeInterval: "Hello"
            }
            return Object.assign({}, state,
                { 
                    graphs: [
                        ...state.graphs,
                        newGraph
                    ]
                });
        default:
            return state;
    }
}

const initialState: IState = {
    loggedIn: false,
    searchResults: [],
    graphs: []
};

export default createStore(reducer, initialState, applyMiddleware(thunk));