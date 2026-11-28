type: fix

The exact lumber search keys its memo on the piece counts instead of a joined string per state and drops per-state LINQ buffers, cutting its allocations by about three quarters on large cut lists with the same plans.
