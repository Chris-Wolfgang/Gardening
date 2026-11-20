type: fix

`StockPacker.Pack` now rejects a null piece, a piece with no lumber kind, a non-positive length or a negative quantity with an `ArgumentException` instead of returning impossible leftovers.
