import * as React from "react";
import * as Rx from "rxjs";
import { connect } from "react-redux";
import store from "../store/store";
import { IState } from "../store/store";
import { Company } from "../store/data-blocks";
import { ListSearchResults, QueryCompanyStock } from "../store/actions";

interface SearchProps {
    searchResults: Array<Company>
}

class Search extends React.Component<SearchProps, {}> {

    suggestionHandler(event: React.MouseEvent<HTMLUListElement>) {
        let element = event.target as HTMLElement;
        console.log(element);
        console.log(element.dataset.index);
        store.dispatch(QueryCompanyStock(element.dataset.index));
    }
    render() {
        const suggestions = this.props.searchResults.map<JSX.Element>((company, index) =>
            <li key={index} data-index={index}>{company.symbol}: {company.name}</li>
        );
        return (
            <div>
                <input id="search-companies" type="text"/>
                <ul id="search-suggestions" onClick={e => this.suggestionHandler(e)}>
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
                store.dispatch(ListSearchResults(json));
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