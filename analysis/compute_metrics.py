"""
Reproduces the performance metrics reported in the dissertation:
average response time, accuracy (%) and error rate (%) for the
automated chatbot (Phase: Automated) and the human baseline (Phase: Baseline).

Usage (from the repository root):
    python analysis/compute_metrics.py

Input:  data/processed/chatbot.csv, data/processed/human.csv
        (semicolon-delimited, comma decimal separator; the last two lines of
        each file are the summary rows computed in Excel and are skipped here)
Output: results/summary_metrics.csv and a printout
"""
import csv
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROCESSED = ROOT / "data" / "processed"
RESULTS = ROOT / "results"


def to_float(value: str) -> float:
    return float(value.strip().replace(",", "."))


def load(path: Path, time_col: str):
    """Return (times, correct_flags) for data rows, skipping the Excel summary rows."""
    with path.open(encoding="utf-8-sig", newline="") as fh:
        rows = list(csv.DictReader(fh, delimiter=";"))
    times, correct = [], []
    for row in rows:
        t = (row.get(time_col) or "").strip()
        flag = (row.get("IsCorrect") or "").strip().upper()
        # Summary rows have no IsCorrect value; the label row is non-numeric.
        if flag not in ("TRUE", "FALSE"):
            continue
        times.append(to_float(t))
        correct.append(flag == "TRUE")
    return times, correct


def summarise(name, times, correct):
    n = len(times)
    acc = 100 * sum(correct) / n
    return {
        "Group": name,
        "N": n,
        "Correct": sum(correct),
        "AvgResponseTimeSeconds": round(sum(times) / n, 9),
        "MinResponseTimeSeconds": min(times),
        "MaxResponseTimeSeconds": max(times),
        "AccuracyPercent": round(acc, 8),
        "ErrorRatePercent": round(100 - acc, 8),
    }


def main():
    bot_t, bot_c = load(PROCESSED / "chatbot.csv", "ExecutionTimeSeconds")
    hum_t, hum_c = load(PROCESSED / "human.csv", "ExecutionTimeSeconds")

    results = [summarise("Chatbot (Automated)", bot_t, bot_c),
               summarise("Human (Baseline)", hum_t, hum_c)]

    speedup = results[1]["AvgResponseTimeSeconds"] / results[0]["AvgResponseTimeSeconds"]

    RESULTS.mkdir(exist_ok=True)
    out = RESULTS / "summary_metrics.csv"
    with out.open("w", newline="", encoding="utf-8") as fh:
        w = csv.DictWriter(fh, fieldnames=list(results[0].keys()))
        w.writeheader()
        w.writerows(results)

    for r in results:
        print(f"{r['Group']}: n={r['N']}, avg={r['AvgResponseTimeSeconds']:.6f}s, "
              f"accuracy={r['AccuracyPercent']:.2f}%, error={r['ErrorRatePercent']:.2f}%")
    print(f"Average response-time ratio (human / chatbot): {speedup:,.0f}x")
    print(f"Written: {out.relative_to(ROOT)}")


if __name__ == "__main__":
    main()
