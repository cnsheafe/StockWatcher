import { ActionCreator, Dispatch } from "redux";
import thunk, { ThunkAction } from "redux-thunk";

import { Company, Graph } from "./data-blocks";
import { IState } from "./store";

// Action Commands
export const SEARCH = "SEARCH_RESULT";
export const RES_INDEX = "SEARCH_RESULT_INDEX";
export const LOGIN = "LOGGED_IN";
export const GRAPH = "ADD_GRAPH";
// Action Interfaces
export interface LoginAction { type: "LOGGED_IN" }

export interface SearchResult {
  type: "SEARCH_RESULT",
  results: Array<Company>
}

export interface CompanyFromIndex {
  type: "SEARCH_RESULT_INDEX",
  index: number
}

export interface AddGraph {
  type: "ADD_GRAPH",
  company: Company,
  dataPoints: Array<number>,
  labels: Array<string>
}

export type ValidAction = SearchResult & LoginAction & CompanyFromIndex & AddGraph;

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
            type: GRAPH,
            company: company,
            dataPoints: dataPoints,
            labels: labels
          });
        })
    }
  }

