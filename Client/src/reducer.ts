import {ADD_GRAPH, addGraph, IAddGraphAction} from "./actions";

interface IState {
    graphs: Array<string>
}
const initalState: IState = {
    graphs: []
}

export default function stockApp(state: IState = initalState, action: IAddGraphAction): IState {

    switch(action.type) {
        case ADD_GRAPH:
            return Object.assign({}, state, {
                graphs: [...state.graphs, {equity: action.equity}]
            });
        default:
            return state;
    }
}