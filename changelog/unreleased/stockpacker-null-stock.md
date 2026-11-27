type: fix

`StockPacker.Pack` now rejects a null entry in the stock list with an `ArgumentException` naming `stock` instead of throwing `NullReferenceException`.
