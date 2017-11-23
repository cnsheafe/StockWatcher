StockWatcher
=

View stock prices from different companies and set SMS Alerts when stocks reach a certain price.

![Graph of Microsoft Stock](/readme-assets/stockwatcher-graph.png)
About
-

This repositiory contains the backend of StockWatcher. For the repository of just the front-end go [here](https://github.com/cnsheafe/stock-watcher-client).

User Features
-

* NASDAQ stock symbol lookup
* Graphs generated from company stock price history
* Custom SMS Alerts

Technical Features
-

* Server-side rendering for faster display
* Background Scheduler for alerts
* Docker for portability

Tech Stack
-

### Back-end

* ASP.NET CORE
* Node.js (for server-side rendering)
* EF Core w/ PostgreSql
* Hangfire
* Docker Container

### Front-end

* React-Redux
* RxJs
* Typescript
* Chart.js
* Fetch API
* Material Icons

Third-Party APIs
-

* Twilio SMS and Notify
* AlphaVantage

Development Tools
-

* Xunit
* Webpack
* SCSS
* Jest
* Travis CI
* Enzyme

Architecture
-

StockWatcher is built on an REST API architecture to handle all business processes.
The controllers act as thin endpoints for handing off requests to service providers which lie in
the model layer.
The service providers handle database operations, Third-Party API access, and what data to return to 
the controllers.
The app is wrapped within a Docker container for portability.

Instructions for Development
-

Developing StockWatcher requires ASPNET Core v2.0, Node.js, Webpack, Sass/SCSS, and Typescript.

Always start with the following:

```bash
dotnet restore
```

For Postgres, copy the processed csv of companies to your database.

For Docker, publish the code and upload it to the host of your choice.

```bash
cd StockWatcher/
dotnet publish -o out/
docker build -t <your-docker-image> .
docker push <your-docker-repo>
```

To view in browser use a .NET Core debugger such as in VS Code or Visual Studio.

Future Plans
-

* Add accounts
* Add StripeJs