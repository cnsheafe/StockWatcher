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
  ADD_GRAPH,
  REM_GRAPH,
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
    case ADD_GRAPH:
      const count: number = state.graphs.length;
      const index: number = count > 0 ? state.graphs[count - 1].index + 1 : 0;
      const newGraph: Graph = {
        index: index,
        graphId: `graph${index}`,
        company: action.company,
        dataset: action.dataPoints,
        labels: action.labels
      }

      return Object.assign({}, state,
      {
        graphs: [
          ...state.graphs,
          newGraph
        ],
        searchResults: []
      });
    case REM_GRAPH:
      const indexToRemove = state.graphs.findIndex(elm => {
        return elm.graphId === action.graphId;
      });
      console.log(indexToRemove);
      const newGraphList = [...state.graphs];
      newGraphList.splice(indexToRemove, 1);
      console.log(newGraphList);
      return Object.assign({}, state,
      {
        graphs: newGraphList
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