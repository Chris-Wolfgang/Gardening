# Gardening

[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/Chris-Wolfgang/Gardening/badge)](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/Gardening)

Gardening tools and guides: a raised-bed calculator, structured plant data, and planting documentation for home gardeners.

Documentation site: <https://chris-wolfgang.github.io/Gardening/>, built from `docs/` by the MkDocs workflow on every push to `main`.

## What's here

| Folder | Contents |
|--------|----------|
| `docs/` | Guides for gardeners, written in Markdown and published with MkDocs Material |
| `data/` | The plant data the docs and apps are built from (see [data/README.md](data/README.md)) |
| `src/` | Apps and libraries, named `Wolfgang.Gardening.<Name>` |
| `tests/` | Test projects, named `Wolfgang.Gardening.<Name>.Tests.Unit` |

## Raised-bed calculator

A Blazor WebAssembly app for rectangular raised beds built from stacked 2x boards:

- **Bed list**: add several bed designs, each with a quantity; the list is saved in the browser.
- **Each bed**: length and width (feet or inches), board size (2x4 to 2x12), how many boards high, pressure-treated or untreated lumber, optional 2x2 corner posts, fill depth, settling allowance, and bagged soil or a three-part mix.
- **Results**: bed sizes, the cut list, which stock boards to buy and how to cut each one (offcuts are shared between beds), screws, soil, and the total cost.

Prices come from a placeholder catalog (`MaterialCatalog.Placeholder`), not a live store. Imperial units only.

| Project | What it is |
|---------|------------|
| `src/Wolfgang.Gardening.Calculator` | The calculation library: bed geometry, cut list, stock packing, fasteners and soil |
| `src/Wolfgang.Gardening.Calculator.Web` | The Blazor WebAssembly front end |

### Building, testing and running

The .NET SDK version is pinned by `global.json` (10.0.100, rolling forward to the latest feature band).

```bash
dotnet build Gardening.slnx
dotnet test Gardening.slnx
dotnet run --project src/Wolfgang.Gardening.Calculator.Web
```

`pwsh ./scripts/build-pr.ps1` runs the same build, test and coverage gate as the pull-request workflow.

## Planned apps

Bed layout planner, soil mix calculator, fertilizer planner, planting calendar and garden journal.

## Building the docs locally

```bash
pip install mkdocs-material
mkdocs serve
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Security issues: see [SECURITY.md](SECURITY.md).

## License

MIT. See [LICENSE](LICENSE).
