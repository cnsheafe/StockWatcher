import * as React from "react";
import * as Rx from "rxjs";

export class Search extends React.Component<{}, {}> {

    render() {
        return (
            <input id="search-companies" type="text"/>
        );
    }

    componentDidMount() {
        const searchElement = document.getElementById("search-companies") as HTMLInputElement;

        Rx.Observable.fromEvent(searchElement,"keyup")
            .debounceTime(300)
            .subscribe(() => fetchCompanies(
                searchElement.value,
                true
        ));
    }
    
}

function fetchCompanies(searchPhrase: string, isSymbol: boolean):Promise<JSON> {
    let searchRequest = new Request(`/company/?searchphrase=${searchPhrase}&issymbol=${isSymbol.toString()}`, {
        method: "GET"
    });

    return fetch(searchRequest)
        .then(res => {
            console.log(res.json());
            return res.json();
        });
}
