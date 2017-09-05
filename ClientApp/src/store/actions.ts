import { ActionCreator, Dispatch } from "redux";
import thunk, { ThunkAction } from "redux-thunk";

import { Company, Graph } from "./data-blocks";


// Action Commands
export const SEARCH = "SEARCH_RESULT";
export const RES_INDEX = "SEARCH_RESULT_INDEX";
export const LOGIN = "LOGGED_IN";

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

export type ValidAction = SearchResult & LoginAction & CompanyFromIndex;

export const ListSearchResults: ActionCreator<SearchResult> = 
    (results: Array<Company>) => {
        return {
            type: SEARCH,
            results: results
        }
    }

export const QueryCompanyStock: ActionCreator<CompanyFromIndex> = 
    (index: number) => {
        return {
            type: RES_INDEX,
            index: index
        }
    }

// export const GetCompanyAsync: ThunkAction =
//     ()
export function GetCompanyAsync () {
    return function (dispatch) {
        let url = "";
        return fetch(url)
    }
}