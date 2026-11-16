# Data dictionary

All log files are semicolon-delimited (`;`).

## Files

| File | Rows | Description |
|---|---|---|
| `data/raw/execution_logs_chatbot.csv` | 358 | Every chatbot request logged by `CsvLogger`, 19 Feb – 10 May 2026, with question and answer. Converted from Windows-1252 to UTF-8; values unchanged. |
| `data/raw/execution_logs_human_baseline.csv` | 105 | Human-baseline answers captured by `HumanBaselineController`, as later edited in Excel (includes the summary rows and two 10 May 2026 test rows appended afterwards). |
| `data/raw/execution_logs_phase3_2026-02-08.csv` | 72 | Earlier test phase (8 Feb 2026): timing only, before question/answer logging was added. |
| `data/raw/execution_logs_phase4_2026-02-08.csv` | 110 | Phase 4: the Phase 3 rows plus 38 further runs on the evening of 8 Feb 2026. |
| `data/processed/chatbot.csv` | 140 + 2 summary | Chatbot records used for the statistical review (19–20 Feb 2026). |
| `data/processed/human.csv` | 101 + 2 summary | Human-baseline records used for the statistical review. |
| `data/knowledge-base/clover_faqs.json` | 102 FAQs | Knowledge base used by the chatbot. |

## Columns

| Column | Meaning |
|---|---|
| `TaskType` | Chatbot outcome: `FAQ_Matched`, `FAQ_Suggested`, `NoMatch`, `EmptyMessage`. Not present in human logs. |
| `ExecutionTimeSeconds` | Chatbot: server-side processing time from `Stopwatch`. Human: seconds from question display to answer submission. |
| `ExecutionMode` | `Chatbot` or `Human`. |
| `Phase` | `Automated` (chatbot) or `Baseline` (human). |
| `UserId` | `Anonymous` for chatbot sessions, `Human` for baseline participants. No personal identifiers are recorded. |
| `Timestamp` | UTC time the record was written. Format differs between files (see raw files). |
| `Question` | The question asked. |
| `Answer` | Chatbot response or the human participant's typed answer. |
| `IsCorrect` | See below. |

## How `IsCorrect` is determined

- **Chatbot:** `True` when the response is a matched FAQ answer (`FAQ_Matched`).
- **Human:** `True` when the typed answer exactly matches the reference answer (case-insensitive).

## Known data-quality points

- `human.csv` contains five rows where only `ExecutionTimeSeconds` and a `TRUE`/`FALSE`
  value in `ExecutionMode` survived (Excel artefacts). They are counted as incorrect,
  matching the figures in the dissertation; excluding them gives N = 96 and 18.75% accuracy.
- Four human rows read "No question available", where the replay controller could not
  parse a question from the log line.
- The processed files use comma decimals (`0,000694`), the raw chatbot log uses dot decimals.
