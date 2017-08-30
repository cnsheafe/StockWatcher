export const ADD_GRAPH = "ADD_GRAPH";
export const TOGGLE_LOGIN = "TOGGLE_LOGIN";

interface IAction {
    type: string,
    payload: any
}

export function toggleLogin(): IAction {
    return {
        type: TOGGLE_LOGIN,
        payload: null
    }
}

export function addGraph(equity: string) {
    return {
        type: ADD_GRAPH,
        equity
    }
}

