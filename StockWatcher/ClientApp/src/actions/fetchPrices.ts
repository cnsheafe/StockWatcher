const fetch = require("isomorphic-fetch");

export function fetchPrices(symbols: string[]) {
  const url = "/stockprice";
  const requestOptions = {
    method: "POST",
    cache: "default",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json"
    },
    body: JSON.stringify({
      symbols: symbols
    })
  };
  return fetch(url, requestOptions)
  .then(res => res.json());
}
