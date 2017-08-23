export const ADD_GRAPH = "ADD_GRAPH";


export interface IAddGraphAction {
    type: string,
    equity: string
}

export function addGraph(equity: string): IAddGraphAction {
    return {
        type: ADD_GRAPH,
        equity
    }
}