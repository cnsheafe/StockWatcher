FROM microsoft/aspnetcore

RUN apt-get update

#Dependency of Nodejs
RUN apt-get install -y gnupg

# NodeJs required for server-side rendering
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash -
RUN apt-get install -y nodejs

WORKDIR /dotnetapp

COPY out .

# Heroku offers $PORT for app to interface
CMD ASPNETCORE_URLS=http://*:$PORT  dotnet StockWatcher.dll