type: fix

`StockPacker.Pack` no longer crashes the process with a stack overflow on a cut list of a few thousand pieces; the exact search now walks an explicit stack.
