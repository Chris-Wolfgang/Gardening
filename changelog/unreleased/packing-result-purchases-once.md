type: internal

`PackingResult.Purchases` builds its grouped purchase list once instead of copying it on every read (InspectCode S2365, issue #33); the list returned is the same.
