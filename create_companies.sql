DROP TABLE IF EXISTS companies;

CREATE TABLE IF NOT EXISTS companies(
    "Symbol" TEXT PRIMARY KEY,
    "Name" TEXT,
    "LastSale" TEXT,
    "MarketCap" TEXT,
    "ADRTSO" TEXT,
    "IPOyear" TEXT,
    "Sector" TEXT,
    "Industry" TEXT,
    "SummaryQuote" TEXT
);

\copy companies FROM '~/Programming/NET/StockWatcher/processed-companylist.csv' WITH (FORMAT CSV, HEADER TRUE);