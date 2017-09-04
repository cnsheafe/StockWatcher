import * as React from "react";
import * as Rx from "rxjs";
import { connect } from "react-redux";
import store from "../store/store";
import { createSearchResult, IState, Company } from "../store/store";

interface SearchProps {
    searchResults: Array<Company>
}

class Search extends React.Component<SearchProps, {}> {

    render() {
        const suggestions = this.props.searchResults.map<JSX.Element>((company, index) =>
            <li key={index}>{company.symbol}: {company.name}</li>
        );
        return (
            <div>
                <input id="search-companies" type="text"/>
                <ul id="search-suggestions">
                    {suggestions}
                </ul>
            </div>
        );
    }

    componentDidMount() {
        const searchElement = document.getElementById("search-companies") as HTMLInputElement;

        Rx.Observable.fromEvent(searchElement,"keyup")
            .debounceTime(300)
            .subscribe(() => 
            fetchCompanies(searchElement.value, true)
            .then(json => {
                console.log(json);
                store.dispatch(createSearchResult(json));
            }));
    }
}

function fetchCompanies(searchPhrase: string, isSymbol: boolean): Promise<JSON> {
    let searchRequest = new Request(`/company/?searchphrase=${searchPhrase}&issymbol=${isSymbol.toString()}`, {
        method: "GET"
    });

    return fetch(searchRequest)
        .then(res => {
            return res.json();
        });
}

function mapStateToProps(state: IState): SearchProps {
    return {
        searchResults: state.searchResults
    }
}


export default connect(mapStateToProps)(Search);