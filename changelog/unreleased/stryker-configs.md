type: internal

The web app's entry point is an explicit `Program.Main` instead of top-level statements so Stryker can build the mutated project, and both test projects get a `stryker-config.json` with a mutation-score floor; no behaviour change.
