#!/usr/bin/env python3

import csv
import sys

def main():
    totals = {}
    reader = csv.DictReader(sys.stdin)
    for row in reader:
        totals[row['team']] = totals.get(row['team'], 0) + int(row['points'])

    print("""
<table>
  <caption>Points</caption>
  <thead>
    <tr>
      <th>Team</th>
      <th>Points</th>
    </tr>
  </thead>
  <tbody>""")

    for team, points in sorted(totals.items(), key=lambda entry: entry[1], reverse=True):
        print(f"    <tr><td>{team}</td><td>{points}</td></tr>")

    print("""  </tbody>
</table>
""")

if __name__ == "__main__":
    main()
