# Data

The plant data the docs site and the apps are built from. Everything here is plain text so changes review well in pull requests.

| Path | Format | Contents |
|------|--------|----------|
| `plants/<id>.yaml` | YAML, one file per plant | Overview, planting, spacing, NPK needs, fertilizers, care, images |
| `fertilizers.yaml` | YAML list | Organic fertilizers and their NPK values; plants refer to them by `id` |
| `companions.csv` | CSV | One row per plant pair: `a,b,score,reason` |
| `materials/` | | Lumber sizes and default prices for the calculator (not yet populated) |
| `schema/` | | Schemas used to validate the files above (not yet populated) |

## Conventions

- An `id` is lowercase kebab-case (`bone-meal`, `tomato`) and is the file name for plants.
- Plants and companions refer to each other by `id`. Companion relationships are stored once, in `companions.csv`, not in the plant files.
- `images` entries name files in `docs/assets/plants/`.

## `plants/<id>.yaml` spacing

Square-foot method. `plants_per_block` plants fit in a square block `block_ft` feet on a side: carrots are `16` per `1` ft block, a tomato is `1` per `2` ft block.

## `companions.csv` scores

Whole numbers from 1 to 5. A pair that is not listed is neutral (3). `reason` is optional.

| Score | Label | Meaning |
|-------|-------|---------|
| 5 | Great | Strongly beneficial |
| 4 | Good | Beneficial |
| 3 | Neutral | No known effect (default) |
| 2 | Poor | Mildly harmful |
| 1 | Bad | Strongly harmful |

Rules: a pair appears once (`a,b` and `b,a` are the same pair), a plant is never paired with itself, and both ids must exist in `plants/`.

Validation of these rules is not automated yet.
