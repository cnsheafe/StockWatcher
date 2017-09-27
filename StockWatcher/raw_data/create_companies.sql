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
-- Replace path with absolute location of your csv data
\copy companies FROM '~/Programming/NET/StockWatcher/StockWatcher/raw_data/processed-companylist.csv' WITH (FORMAT CSV, HEADER TRUE);