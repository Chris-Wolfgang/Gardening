type: internal

`BedEstimate.CutPatterns` builds its cut-pattern list once instead of copying it on every read and render (InspectCode S2365, issue #30); the list returned is the same.
