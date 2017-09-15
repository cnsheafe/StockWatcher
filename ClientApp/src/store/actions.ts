import { ActionCreator, Dispatch } from "redux";
import thunk, { ThunkAction } from "redux-thunk";

import { Company, Graph } from "./schema";
import { IState } from "./store";

// Action Commands
export const SEARCH = "SEARCH_RESULT";
const LOGIN = "LOGGED_IN";
export const ADD_GRAPH = "ADD_GRAPH";
export const REM_GRAPH = "REMOVE_GRAPH";
export const TOGGLE_MODAL = "TOGGLE_MODAL";
export const ADD_WATCH = "ADD_WATCH";

// Action Interfaces
export interface LoginAction { type: "LOGGED_IN" }

export interface SearchResult {
  type: "SEARCH_RESULT",
  results: Array<Company>
}

export interface AddGraph {
  type: "ADD_GRAPH",
  company: Company,
  dataPoints: Array<number>,
  labels: Array<string>
}

export interface RemoveGraph {
  type: "REMOVE_GRAPH",
  graphId: string
}

export interface ToggleModalDisplay {
  type: "TOGGLE_MODAL"
}

export interface AddWatch {
  type: "ADD_WATCH",
  company: Company,
  targetPrice: number
}

export type ValidAction = SearchResult & AddGraph & RemoveGraph & ToggleModalDisplay & AddWatch;

// Action for updating state with list of matching company names from database
// Used on Search.tsx
export const ListSearchResults: ActionCreator<SearchResult> =
  (results: Array<Company>) => {
    return {
      type: SEARCH,
      results: results
    }
  }
// Action for Adding a Graph
export const addGraphAsync = 
  (company: Company) => {

    return function(dispatch: Dispatch<IState>) {
      let stockRequest = new Request(
        `/stockprice?stocksymbol=${company.symbol}`,
        { method: "GET" });
      
      return fetch(stockRequest)
        .then(res => {
          return res.json();
        })
        .then((json) => {
          console.log(json);
          let dataPoints = [];
          let labels = [];

          for (var i = json.length - 1; i >= 0 ; i--) {
            dataPoints.push(json[i].price);
            labels.push(json[i].timeStamp);
          }

          dispatch<AddGraph>({
            type: ADD_GRAPH,
            company: company,
            dataPoints: dataPoints,
            labels: labels
          });
        })
    }
  }


export const removeGraph: ActionCreator<RemoveGraph> = (graphId: string) => {
  return {
    type: REM_GRAPH,
    graphId: graphId
  }
}

export const toggleModalDisplay: ActionCreator<ToggleModalDisplay> = () => {
  return {
    type: TOGGLE_MODAL
  }
}
export const addWatch = (company: Company, targetPrice: number, phoneNumber: string) => {
  return function(dispatch: Dispatch<IState>) {
    let watchRequest = new Request("/notifications/watchprice", {
      method: "POST",
      body: {
        equity: company.symbol,
        phone: phoneNumber,
        price: targetPrice
      }
    });
    return fetch(watchRequest)
      .then(res => {
        return res.json()
      })
      .then(json => {
        if (json.success) {
          dispatch<ToggleModalDisplay>({
            type: TOGGLE_MODAL
          });
        }
      });
  }
}