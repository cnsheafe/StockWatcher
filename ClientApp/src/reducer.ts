import {ADD_GRAPH, addGraph, TOGGLE_LOGIN, toggleLogin} from "./actions";

interface IState {
    loggedIn: boolean
    graphs: Array<string>
}

const initalState = {
    loggedIn: false,
    graphs: new Array<string>()
};

export default function reduce(state: IState = initalState, action: any) {

    switch(action.type) {

        case TOGGLE_LOGIN:
            return Object.assign({}, state, {
                loggedIn: !state.loggedIn
            });

        case ADD_GRAPH:
            return Object.assign({}, state, {
                graphs: [...state.graphs, {equity: action.equity}]
            });

        default:
            return state;
    }
}