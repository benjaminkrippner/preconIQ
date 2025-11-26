/** RFC4180-ish CSV parser (handles quotes, escaped quotes, CRLF, BOM). */
export function parseCSV(text: string): string[][] {
  const rows: string[][] = [];
  let row: string[] = [];
  let cell = "";
  let i = 0;
  let inQuotes = false;

  // Remove BOM if present
  if (text.length && text.charCodeAt(0) === 0xfeff) text = text.slice(1);

  while (i < text.length) {
    const ch = text[i];

    if (inQuotes) {
      if (ch === '"') {
        const next = text[i + 1];
        if (next === '"') {
          cell += '"'; i += 2; continue;      // escaped quote ""
        } else {
          inQuotes = false; i++; continue;    // closing quote
        }
      } else { cell += ch; i++; continue; }
    } else {
      if (ch === '"') { inQuotes = true; i++; continue; }
      if (ch === ",") { row.push(cell); cell = ""; i++; continue; }
      if (ch === "\n" || ch === "\r") {
        row.push(cell); cell = ""; rows.push(row); row = [];
        if (ch === "\r" && text[i + 1] === "\n") i++; // CRLF
        i++; continue;
      }
      cell += ch; i++;
    }
  }
  row.push(cell); rows.push(row);
  if (rows.length && rows[rows.length - 1].every((c) => c === "")) rows.pop();
  return rows;
}

export function csvToObjects(rows: string[][]): Record<string, string>[] {
  if (!rows.length) return [];
  const header = rows[0].map((h) =>
    h.trim().toLowerCase().replace(/\s+|[^a-z0-9]/g, "_")
  );
  const out: Record<string, string>[] = [];
  for (let r = 1; r < rows.length; r++) {
    const obj: Record<string, string> = {};
    const line = rows[r];
    if (!line.length || line.every((v) => v === "")) continue;
    for (let c = 0; c < header.length; c++) {
      obj[header[c]] = (line[c] ?? "").trim();
    }
    out.push(obj);
  }
  return out;
}