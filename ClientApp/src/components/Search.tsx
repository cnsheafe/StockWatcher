import * as React from "react";
import * as Rx from "rxjs";
import { connect } from "react-redux";
import store from "../store/store";
import { IState } from "../store/store";
import { Company } from "../store/data-blocks";
import { ListSearchResults,  addGraphAsync } from "../store/actions";

interface SearchProps {
  searchResults: Array<Company>
}

class Search extends React.Component<SearchProps, {}> {

  suggestionHandler(event: React.MouseEvent<HTMLUListElement>) {
    let element = event.target as HTMLElement;
    const company: Company = {
      name: element.dataset.name,
      symbol: element.dataset.symbol
    }
    store.dispatch(addGraphAsync(company));
  }
  render() {
    const suggestions = this.props.searchResults.map<JSX.Element>((company, index) =>
      <li 
        key={index} 
        data-symbol={company.symbol}
        data-company-name={company.name}>
        {company.symbol}: {company.name}
      </li>
    );
    return (
      <section className="search">
        <label htmlFor="search-companies" className="search-label">Search</label>
        <input id="search-companies" type="text" className="search-input" placeholder="Type in an Stock Symbol (e.g. MSFT)"/>
        <ul id="search-suggestions" onClick={e => this.suggestionHandler(e)}>
          {suggestions}
        </ul>
      </section>
    );
  }

  componentDidMount() {
    const searchElement = document.getElementById("search-companies") as HTMLInputElement;

    Rx.Observable.fromEvent(searchElement, "keyup")
      .debounceTime(300)
      .subscribe(() =>
        fetchCompanies(searchElement.value, true)
          .then(json => {
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