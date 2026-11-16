# CloverBot: AI-Driven Customer Support Automation (Master's Research Artefact)

This repository contains the source code, knowledge base, execution logs and analysis
scripts for the CloverBot prototype developed for the Master's dissertation by
**Diana Mudau** on AI-driven customer support automation within an Intelligent Process
Automation (IPA) framework, integrating a conversational agent with Robotic Process
Automation (RPA).

Supervisors: Prof. Absalom Ezugwu and Dr. Kisten.

The study compares an automated FAQ chatbot against a human baseline answering the same
customer-support questions, measuring **response time**, **accuracy** and **error rate**.

## Headline results

| Group | N | Avg response time (s) | Accuracy | Error rate |
|---|---|---|---|---|
| Chatbot (Automated) | 140 | 0.000694 | 100.00% | 0.00% |
| Human (Baseline) | 101 | 66.41 | 17.82% | 82.18% |

Regenerate these with `python analysis/compute_metrics.py` (output: `results/summary_metrics.csv`).

## Repository structure

```
.
├── src/CloverBotWebApplication/   ASP.NET Core 8.0 MVC application (the chatbot system)
│   ├── Controllers/               Chat UI, REST API (/api/chatbot/ask), human-baseline capture
│   ├── ServiceInterface/          ChatService (FAQ matching), RpaService (UiPath Orchestrator trigger)
│   ├── Models/                    Request/response models, EF Core context, CSV logging
│   ├── Views/                     Razor views: chat widget, human-baseline questionnaire
│   └── wwwroot/
│       ├── data/clover_faqs.json  Knowledge base loaded by the chatbot at start-up
│       └── logs/                  Empty log files (headers only); the app appends here when run
├── data/
│   ├── knowledge-base/            Copy of the FAQ knowledge base (102 FAQs, 9 categories)
│   ├── raw/                       Execution logs exactly as written by the application
│   └── processed/                 Logs as cleaned in Excel and used for the statistical review
├── analysis/compute_metrics.py    Reproduces the metrics in the dissertation
├── results/summary_metrics.csv    Output of the analysis script
└── docs/data_dictionary.md        Column definitions and notes on each dataset
```

## How the chatbot works

`ChatService` loads `clover_faqs.json` (each FAQ has a question, answer, keywords, tags and
phrasing variations) and scores every FAQ against the user's message:

- +3 for an exact match on the FAQ question, +2 for an exact match on a variation
- +1 per keyword present in the message
- +1 per word shared with the FAQ question and with each variation

The highest-scoring FAQ is returned when its score is 3 or more (`FAQ_Matched`); below that
the bot asks "Did you mean …?" (`FAQ_Suggested`); otherwise it asks the user to rephrase
(`NoMatch`). Every request is timed with a `Stopwatch` and appended to
`wwwroot/logs/execution_logs.csv` by `CsvLogger`.

The human baseline (`/HumanBaseline`) replays the questions from the chatbot log to a human
participant, records their typed answer and response time, and writes
`execution_logs_baseline.csv`.

`RpaService` triggers a UiPath Orchestrator job when `UiPath:OrchestratorUrl` and an API
token are configured; without them it returns without doing anything.

## Running the application

Requirements: .NET 8 SDK, SQL Server LocalDB (Visual Studio installs it).

```bash
cd src/CloverBotWebApplication
dotnet restore
dotnet run
```

Then open `https://localhost:7258` (chat) or `https://localhost:7258/HumanBaseline`.

Secrets are not stored in this repository. If you enable the optional integrations, supply
them through user secrets or environment variables, for example:

```bash
dotnet user-secrets set "Groq:ApiKey" "<your key>"
setx UIPATH_API_KEY "<your token>"
```

## Running the analysis

Python 3.8+ with the standard library only:

```bash
python analysis/compute_metrics.py
```

## Notes on the data

See [docs/data_dictionary.md](docs/data_dictionary.md) for full details. In short:

- Logs are semicolon-delimited. Raw chatbot logs use `.` as the decimal separator; the
  processed files were saved from Excel with a South African locale, so they use `,`.
- The processed files end with two summary rows (values, then labels) computed in Excel.
- `IsCorrect` for the human baseline is an exact, case-insensitive match against the
  chatbot's answer. For the chatbot it is `True` for every `FAQ_Matched` response.
- Rows dated 10 May 2026 in the raw logs are post-analysis test entries and are not part
  of the processed datasets.
- The Clover logo has been removed from `wwwroot/image`; the layout will show alt text
  instead.

## Licence

To be confirmed.
