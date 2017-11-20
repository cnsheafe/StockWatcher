import * as React from "react";
import { connect } from "react-redux";
import store from "../store/store";
import { IState } from "../store/typings";
// import { Ticker, AddTickers, fetchPrices} from "../actions/fetchPrices";
import {Ticker, dispatchTickers} from '../actions/dispatchTickers';

export interface TickerProps {
  tickers: Ticker[];
}
export class TickerList extends React.Component<TickerProps, {}> {
  render() {
    const listOfTickers = this.props.tickers.map(ticker => <li>{ticker.symbol}</li>);
    return <ul>{listOfTickers}</ul>;
  }
  componentDidMount() {
    store.dispatch(dispatchTickers(["msft", "amd", "nflx"]));
  }
}

function mapStateToProps(state: IState): TickerProps {
  return {
    tickers: state.tickers
  };
}
export default connect(mapStateToProps)(TickerList);
