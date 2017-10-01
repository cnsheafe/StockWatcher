import * as React from "react";

// Presentation Component for references to APIs and Github repo
export class Footer extends React.Component {
  render() {
    return (
      <footer className="footer">
        <ul className="footer-links">
          <li><a href="https://twilio.com" target="_blank"><img src="dist/twilio.ico" alt="Twilio API"/></a></li>
          <li><a href="https://alphavantage.co" target="_blank"><img src="dist/alphavantage.ico" alt="AlphaVantage API"/></a></li>
          <li><a href="https://github.com/cnsheafe/StockWatcher" target="_blank"><img src="dist/github.png" alt="Github Portfolio"/></a></li>
        </ul>
      </footer>
    );
  }
}