# Demoskript – DDC 2026 Live-Demo „Finde meine Session“

**Test:** [DDC2026LiveDemoTests.cs](./DDC2026LiveDemoTests.cs) → `DDC2026_LiveDemo_FindMySession`
**Dauer:** ca. 5–7 Minuten
**Ziel:** Die Zuschauer sehen, wie ein lesbarer Playwright-Test (C#/MSTest) einen echten Nutzer-Flow abbildet: Seite öffnen, im iFrame navigieren, Session finden, prüfen, Screenshot.

---

## Vorbereitung (vor dem Talk)

- [ ] WLAN/Hotspot geprüft, https://www.developer-world.de/ddc/ddc-programm lädt im Browser
- [ ] Build einmal vorab ausführen (spart Zeit auf der Bühne):
  ```bash
  cd PlaywrightDemos
  dotnet build PlayDemo.csproj
  ```
- [ ] Browser installiert (einmalig):
  ```bash
  pwsh bin/Debug/net11.0/playwright.ps1 install chromium
  ```
- [ ] Generalprobe (schnell):
  ```bash
  DEMO_SLOWMO=0 dotnet test PlayDemo.csproj --filter "FullyQualifiedName~DDC2026LiveDemoTests"
  ```
- [ ] Auf dem Bühnen-Mac mit dem verwendeten Display prüfen: Chromium startet unter macOS mit `--force-device-scale-factor=1`, passend zu `DeviceScaleFactor = 1` im Browser-Kontext. So bleiben die Klickkoordinaten im Sessionize-iFrame auch auf Retina-Displays korrekt.
- [ ] IDE: Schriftgröße hoch (≥ 18 pt), Datei `Demo/DDC2026LiveDemoTests.cs` geöffnet
- [ ] Benachrichtigungen aus (Fokus-Modus), Terminal-Font groß

---

## Ablauf auf der Bühne

### 1. Einstieg (≈ 30 s)

> „Ich zeige euch jetzt, wie ihr meine Session im DDC-Programm findet – aber nicht ich klicke, sondern Playwright.“

Die Startseite https://www.developer-world.de/ kurz im Browser zeigen und den Weg über das Menü **DDC → Programm** demonstrieren. Dann darauf hinweisen, dass das eigentliche Programm in einem **Sessionize-iFrame** steckt. Für viele UI-Test-Tools ist das schwierig.

### 2. Code durchgehen (≈ 2 min)

Die Methode von oben nach unten erklären. Die Kommentare sind nummeriert:

| Schritt | Code | Botschaft |
|---|---|---|
| 1 | `LaunchAsync(browserOptions)`, `NewContextAsync(...)` | Sichtbar und verlangsamt, damit alle mitlesen können. Unter macOS gleicht `--force-device-scale-factor=1` die Retina-Skalierung an `DeviceScaleFactor = 1` im Kontext an. Für eine Pipeline `Headless = true` setzen und die macOS-Startoption nur bei sichtbarem Browser verwenden |
| 2 | `AddLocatorHandlerAsync(...)` | Consent-Banner wird **automatisch** weggeklickt, egal wann es erscheint. Ohne `if`, ohne `Sleep` |
| 3 | `GotoAsync(...)`, Menü „DDC“ → „Programm“, `ToHaveURLAsync(...)` | Wie ein echter Besucher navigieren. Das Untermenü ist ein Button mit sprechendem Namen, also wieder `GetByRole`. `ToHaveURLAsync` wartet, bis die Navigation abgeschlossen ist |
| 4 | `FrameLocator(...)` | Ein Locator reicht für den iFrame, kein manuelles `SwitchTo().Frame()` wie bei Selenium |
| 5 | `GetByRole(AriaRole.Link, Name = ...)` | Elemente werden so gefunden, wie der **Nutzer** sie sieht. Kein CSS, kein XPath, robust gegen Redesigns |
| 6 | `Assertions.Expect(...).ToBeVisibleAsync()` | **Web-First-Assertions** warten selbst und wiederholen die Prüfung. Keine Flaky-Tests durch Timing |
| 7 | `ScreenshotAsync(...)` | Beweisfoto, landet als Ergebnisdatei im Testreport |

> Tipp: Bei Schritt 5 betonen: „Wenn der Test das Element nicht findet, findet es ein Screenreader-Nutzer vermutlich auch nicht. Barrierefreiheit gibt es gratis dazu.“

### 3. Test live ausführen (≈ 1 min)

```bash
dotnet test PlayDemo.csproj --filter "FullyQualifiedName~DDC2026LiveDemoTests"
```

Alternativ in der IDE über den Test Explorer: **Run** auf `DDC2026_LiveDemo_FindMySession`.

Worauf das Publikum achten soll:
1. Chromium öffnet die Startseite, ein Consent-Banner würde „von allein“ verschwinden
2. Das Menü **„DDC“** klappt auf, **„Programm“** wird hervorgehoben und geklickt
3. **„Dienstag“** wird farbig hervorgehoben (`HighlightAsync`) und dann geklickt
4. Die Session wird hervorgehoben, angescrollt und geöffnet
5. Der Dialog mit Titel und Speaker erscheint, der Test ist grün

> Zu schnell oder zu langsam? Das Tempo lässt sich ohne Codeänderung steuern:
> ```bash
> DEMO_SLOWMO=2000 dotnet test PlayDemo.csproj --filter "FullyQualifiedName~DDC2026LiveDemoTests"
> ```

### 4. Ergebnis zeigen (≈ 30 s)

Screenshot öffnen:

```bash
open bin/Debug/net11.0/screenshot_ddc2026_livedemo.png
```

### 5. Bonus: „Was, wenn's kaputtgeht?“ (≈ 1 min, optional)

Den Session-Titel in Schritt 5 live ändern, z. B. ein Wort entfernen, und den Test erneut starten.
→ Playwright wartet bis zum Timeout und meldet dann **genau**, welcher Locator fehlt. Danach die Änderung zurücknehmen.

### 6. Überleitung (≈ 15 s)

> „Das war ein Browser. Gleich zeige ich euch denselben Test gegen Firefox, WebKit, Edge und Chrome – mit einer einzigen Zeile mehr pro Browser.“

(→ weiter mit `DDC2026_DataDrivenSmokeTest` in [PlaywrightE2ETests_DDC2026.cs](../PlaywrightE2ETests_DDC2026.cs))

---

## Plan B – wenn etwas schiefgeht

| Problem | Lösung |
|---|---|
| Kein Internet / Seite lädt nicht | Screenshot aus der Generalprobe zeigen (`bin/Debug/net11.0/screenshot_ddc2026_livedemo.png`) |
| Browser öffnet sich nicht | Build und Browser-Installation prüfen (siehe nächste Zeile) |
| Browser fehlt | `pwsh bin/Debug/net11.0/playwright.ps1 install chromium` |
| Klick im iFrame scheitert unter macOS mit „outside of the viewport“ oder „intercepts pointer events“ | Prüfen, dass `--force-device-scale-factor=1` beim Browserstart und `DeviceScaleFactor = 1` im Kontext gesetzt sind; danach neu starten |
| Website oder Sessionize hat das Layout geändert | Mit Codegen live einen neuen Locator ermitteln. Das ist selbst eine gute Demo: `pwsh bin/Debug/net11.0/playwright.ps1 codegen https://www.developer-world.de/` |
| Das DDC-Menü klappt nicht auf | In Schritt 3 die Menü-Klicks durch `GotoAsync("https://www.developer-world.de/ddc/ddc-programm")` ersetzen |
| Test ist zu langsam für das Zeitbudget | `DEMO_SLOWMO=300` setzen |

---

## Kernbotschaften zum Mitnehmen

1. **Auto-Waiting**: kein `Thread.Sleep`, keine expliziten Waits
2. **Nutzerzentrierte Locators** (`GetByRole`) machen Tests robust und lesbar
3. **iFrames, Consent-Banner und Co.** sind mit Playwright kein Sonderfall
4. **Lesbar wie eine Anleitung**: Der Test beschreibt den Nutzer-Flow Schritt für Schritt
