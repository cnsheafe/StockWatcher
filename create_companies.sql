DROP TABLE IF EXISTS companies;

CREATE TABLE IF NOT EXISTS companies(
    "Symbol" TEXT PRIMARY KEY,
    "Name" TEXT,
    "LastSale" TEXT,
    "MarketCap" TEXT,
    "ADR TSO" TEXT,
    "IPOyear" TEXT,
    "Sector" TEXT,
    "Industry" TEXT,
    "Summary Quote" TEXT
);

\copy companies FROM '~/Programming/NET/StockWatcher/processed-companylist.csv' DELIMITER ',' CSV HEADER;