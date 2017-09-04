import { createStore, Reducer, ActionCreator } from "redux";
import * as Rx from "rxjs/Rx";

export interface IState {
    loggedIn: boolean,
    searchResults: Array<Company>
}
export interface Company {
    symbol: string,
    name: string
}

const SEARCH = "SEARCH_RESULT";
const LOGIN = "LOGGED_IN";

interface SearchResult {
    type: "SEARCH_RESULT",
    results: Array<Company>
}

interface Login { type: "LOGGED_IN" }

type ValidAction = SearchResult | Login;

export const createSearchResult: ActionCreator<SearchResult> = (results : Array<Company>) => {
    return {
        type: SEARCH,
        results: results
    } as SearchResult;
}

// export const actions = {
//     // increment: {type: INC} as Increment,
//     // decrement: {type: DEC} as Decrement,
//     search: 

// };



function reducer(state: IState, action: ValidAction): IState {
    switch (action.type) {
        case SEARCH:
            return Object.assign({}, state, { searchResults: action.results })
        case LOGIN:
            return Object.assign({}, state, { loggedIn: !state.loggedIn })
        default:
            return state;
    }
}

const initialState: IState = {
    loggedIn: false,
    searchResults: []
};

export default createStore(reducer, initialState);
