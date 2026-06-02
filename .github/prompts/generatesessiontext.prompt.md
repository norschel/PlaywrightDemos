---
agent: agent
---
## Aufgabe

Erstelle einen MSTest-basierten Playwright-Test in C# (Playwright for .NET), der auf der Webseite https://www.entwicklertag.de/ eine *Session* (kein Workshop) findet, die sich mit "Playwright" beschäftigt. Prüfe anschließend:

- Ob zu dieser Session ein Abstract vorhanden ist.
- Ob **Harald** oder **Nico** als Vortragende gelistet sind.

Beachte die folgenden Anforderungen und Ausgabevorgaben.

## Anforderungen

- Framework: MSTest (Attribute: `[TestClass]`, `[TestMethod]`).
- Sprache: C# (.NET) mit Playwright for .NET (async/await).
- Erzeuge eine neue, eigenständige Klasse (verwende keinen bestehenden Code als Vorlage).
- Vorgeschlagener Klassenname: `KET2026PlaywrightSessionTest`.
- Vorgeschlagener Dateiname: `KET2026PlaywrightSessionTest.cs`.
- Testinhalt:
	- Öffne die Startseite `https://www.entwicklertag.de/`.
	- Suche nach Einträgen, die eine Session (nicht Workshop) darstellen und in Titel oder Beschreibung "Playwright" enthalten.
	- Wähle eine passende Session und prüfe, dass ein Abstract vorhanden ist (z. B. nicht-leerer Abstract-Text oder sichtbares Abstract-Element).
	- Prüfe, ob "Harald" oder "Nico" in den Speaker-Angaben auftauchen.
-- Implementiere robuste Selektoren und Fehlerbehandlung (z. B. Warte-Strategien, Timeouts und Null-Checks).
-- Assertions: Verwende `Assert.IsTrue`, `Assert.IsFalse`, `Assert.IsNotNull` oder ähnliche MSTest-Assertions mit aussagekräftigen Fehlermeldungen.
-- Der Test soll deterministisch und idempotent sein (keine manuelle Interaktion nötig).
-- Füge kurze, prägnante Kommentare im Code zur Erklärung entscheidender Schritte ein.

## Einschränkungen

- Keine Nutzung oder Kopie von existierendem Repository-Code als Vorlage.
- Keine externen Abhängigkeiten außer Playwright for .NET und MSTest.

## Erwartete Ausgabe

Antwort: Gib ausschließlich den vollständigen Inhalt der C#-Datei zurück (inkl. `using`-Direktiven, `namespace`, und der `TestClass`). Keine weiteren Erklärungen, Kommentare außerhalb des Codes oder zusätzliche Dateien.

## Optional (Empfehlung)

- Verwende `await page.Locator(...).InnerTextAsync()` oder ähnliche Methoden, um Text zu prüfen.
- Starte den Browser headless, damit der Test in CI läuft.

Liefere die komplette, lauffähige Testklasse als Antwort und erstelle eine neue Datei.