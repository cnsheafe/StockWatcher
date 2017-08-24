export const ADD_GRAPH = "ADD_GRAPH";



export function addGraph(equity: string) {
    return {
        type: ADD_GRAPH,
        equity
    }
}