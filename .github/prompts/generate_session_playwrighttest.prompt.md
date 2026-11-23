---
agent: agent
---
## Aufgabe

Erstelle einen MSTest-basierten Playwright-Test in C# (Playwright for .NET), der auf der offiziellen Seite der .NET Developer Conference (DDC) 2026 (23.–26. November 2026, Hyatt Regency Köln) https://www.developer-world.de/ddc eine *Session* (kein Workshop, keine DevSession) findet, die sich mit "Playwright" beschäftigt. Prüfe anschließend:

- Ob zu dieser Session ein Abstract vorhanden ist.
- Ob **Harald Binkle** oder **Nico Orschel** als Vortragende gelistet sind.

Beachte die folgenden Anforderungen und Ausgabevorgaben.

## Anforderungen

- Framework: MSTest (Attribute: `[TestClass]`, `[TestMethod]`).
- Sprache: C# (.NET) mit Playwright for .NET (async/await).
- Erzeuge eine neue, eigenständige Klasse (verwende keinen bestehenden Code als Vorlage).
- Vorgeschlagener Klassenname: `DDC2026PlaywrightSessionTest`.
- Vorgeschlagener Dateiname: `DDC2026PlaywrightSessionTest.cs`.
- Testinhalt:
	- Öffne die Konferenzseite `https://www.developer-world.de/ddc` und navigiere von dort zum Programm bzw. zur Session-Übersicht.
	- Behandle ggf. einen Cookie-Consent-Dialog, bevor Inhalte geprüft werden.
	- Suche nach Einträgen, die eine Session (Di./Mi., 24./25.11.2026, ca. 60 Minuten – nicht Workshop/DevSession) darstellen und in Titel oder Beschreibung "Playwright" enthalten.
	- Wähle eine passende Session und prüfe, dass ein Abstract vorhanden ist (z. B. nicht-leerer Abstract-Text oder sichtbares Abstract-Element).
	- Prüfe, ob "Harald Binkle" oder "Nico Orschel" in den Speaker-Angaben auftauchen.
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