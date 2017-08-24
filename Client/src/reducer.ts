import {ADD_GRAPH, addGraph} from "./actions";

const initalState = {
    graphs: new Array<string>()
}

export default function stockApp(state: any, action: any) {

    switch(action.type) {
        case ADD_GRAPH:
            return Object.assign({}, state, {
                graphs: [...state.graphs, {equity: action.equity}]
            });
        default:
            return state;
    }
}