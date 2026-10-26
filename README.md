# Gardening

[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/Chris-Wolfgang/Gardening/badge)](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/Gardening)

Gardening tools and guides: a raised-bed calculator, structured plant data, and planting documentation for home gardeners.

Documentation site: <https://chris-wolfgang.github.io/Gardening/> (published once the docs workflow lands).

## What's here

| Folder | Contents |
|--------|----------|
| `docs/` | Guides for gardeners, written in Markdown and published with MkDocs Material |
| `data/` | The plant data the docs and apps are built from (see [data/README.md](data/README.md)) |
| `src/` | Apps and libraries, named `Wolfgang.Gardening.<Name>` |
| `tests/` | Test projects, named `Wolfgang.Gardening.<Name>.Tests.Unit` |

## Planned apps

- **Wolfgang.Gardening.Calculator**: enter the length, width and height of a raised bed, choose the lumber, and get the cut list, the materials, how much soil to buy and the cost. Imperial units; rectangular beds first.
- Later: bed layout planner, soil mix calculator, fertilizer planner, planting calendar and garden journal.

## Building the docs locally

```bash
pip install mkdocs-material
mkdocs serve
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Security issues: see [SECURITY.md](SECURITY.md).

## License

MIT. See [LICENSE](LICENSE).
