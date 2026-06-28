#import "@preview/metropolyst:0.1.0": brands, metropolyst-theme, config-info, slide, title-slide, focus-slide

// --- CONFIGURATION & THEMING (Metropolyst Theme) ---
#show: metropolyst-theme.with(
config-info(
title: [Lernzeit — Softwarearchitektur],
subtitle: [Moderne Terminfindung im SWA-Labor],
author: [SWA Laborteam],
date: datetime(year: 2026, month: 6, day: 28),
institution: [Hochschule Karlsruhe],
),
accent-color: rgb("#4f46e5"),             // Elegantes Indigo
header-background-color: rgb("#0f172a"),  // Dunkles Slate 900 für Kontrast
main-background-color: rgb("#f8fafc"),    // Minimalistischer, heller Folien-Hintergrund
main-text-color: rgb("#1e293b"),          // Gut lesbares Dunkelgrau
)

// Globale Schrift- und Texteinstellungen
#set text(font: "Fira Sans", size: 17pt)
#set strong(delta: 100)

// --- REUSABLE COMPONENTS ---
#let card(title, stroke-color: rgb("#4f46e5"), body) = {
rect(
fill: rgb("#ffffff"),
stroke: 1.5pt + stroke-color,
radius: 8pt,
width: 100%,
inset: 15pt,
)[
#text(weight: "bold", fill: stroke-color, size: 1.1em)[#title]
#v(6pt)
#body
]
}

#let math-card(title, body) = {
rect(
fill: rgb("#f1f5f9"),
stroke: (left: 4pt + rgb("#4f46e5")),
radius: (right: 4pt),
width: 100%,
inset: 12pt,
)[
#text(weight: "bold", fill: rgb("#0f172a"))[#title]
#v(4pt)
#body
]
}

// --- SLIDES START ---

#title-slide()

== Einführung & Problemstellung

#slide[
= Problemstellung & Lösungsansatz

#grid(
columns: (1fr, 1fr),
gutter: 20pt,
card("Das Problem (Schmerzpunkte)", stroke-color: rgb("#ef4444"))[
- Stark asynchrone Stundenpläne erschweren die Terminkoordination im Semester.
- Manueller Abgleich führt zu zeitaufwendigem "Hin und Her" in Gruppenchats.
- Keine aggregierte Sicht auf blockierte Termine.
],
card("Die Lösung: Lernzeit", stroke-color: rgb("#10b981"))[
- Automatisierte Aggregation der blockierten Zeiten.
- Direkter iCal-Stundenplanimport (Campus-Schnittstelle).
- Algorithmische Ermittlung freier Slots & QR-Code-Beitritt.
]
)
]

== Systemarchitektur

#slide[
= Systemarchitektur-Vergleich

#align(center)[
#table(
columns: (1.5fr, 3fr, 2.5fr),
inset: 8pt,
align: left + horizon,
fill: (col, row) => if row == 0 { rgb("#0f172a") } else if calc.even(row) { rgb("#f1f5f9") } else { rgb("#ffffff") },
stroke: 0.5pt + rgb("#cbd5e1"),

  table.header(
    [*Architekturstil*], [*Pro (Vorteile)*], [*Contra (Risiken)*]
  ),
  [#text(weight: "bold")[Client-Server (Monolith)]],
  [Zentrale Kontrolle, einfache Implementierung, hohe Datensicherheit],
  [Single Point of Failure, Skalierbarkeit bei Spitzenlasten],
  
  [#text(weight: "bold")[Microservices]],
  [Exzellente Skalierbarkeit, getrennte Deployments, Technologieunabhängigkeit],
  [Hoher Kommunikationsaufwand, komplexes Debugging & Deployment],
  
  [#text(weight: "bold")[Peer-to-Peer (P2P)]],
  [Keine Serverinfrastruktur nötig, hohe Ausfallsicherheit],
  [Sicherheitsrisiken, komplexe Client-Logik & Synchronisation]
)


]

#v(2pt)
#card("SWA-Entscheidung", stroke-color: rgb("#4f46e5"))[
Modularer Monolith: Bietet hervorragende Wartbarkeit und klare Schichtenverteilung bei gleichzeitig beherrschbarer Komplexität im Rahmen des Labors.
]
]

== Technologie-Evaluation

#slide[
= Backend-Evaluation

#grid(
columns: (1fr, 1.2fr),
gutter: 20pt,
[
Kriterien für das Backend:
- Systemstabilität & Typsicherheit
- Entwicklungskomplexität & DI-Support
- Etablierung SWA-relevanter Entwurfsmuster

  #v(10pt)
  *Evaluierte Sprachen:*
  - *Rust:* Extrem performant, aber zu steile Lernkurve im Team.
  - *Go:* Schlank, bietet aber wenig architektonische Leitplanken (eingebautes DI fehlt).
],
card("Entscheidung: C# (ASP.NET Core)", stroke-color: rgb("#10b981"))[
  - *Strenge Typsicherheit:* Perfekt zur Modellierung langlebiger Geschäftsdomänen.
  - *Integrierte DI:* Erleichtert saubere Entkopplung im Sinne von SOLID.
  - *Reichhaltiges SWA-Ecosystem:* Native Unterstützung für Schichtenarchitektur.
]


)
]

#slide[
= Frontend-Evaluation

#grid(
columns: (1.2fr, 1fr),
gutter: 20pt,
card("Entscheidung: React + TypeScript", stroke-color: rgb("#10b981"))[
- Modulare Komponenten: Passt hervorragend zur Schichtentrennung im Backend.
- TypeScript: Verhindert fehlerhafte Datenflüsse durch stark typisierte API-DTOs.
- Ecosystem: Reichhaltige Auswahl an interaktiven UI-Komponenten (Modals, Kalender).
],
[
Evaluierte Alternativen:
- Blazor: Gute .NET-Integration, aber kleineres Ecosystem und WebAssembly-Overhead.
- Vue: Flache Lernkurve, erzwingt jedoch weniger standardisierte Schichtenstrukturen.

  #v(10pt)
  *Schnittstellen-Vertrag (API):*
  - REST-Protokoll mit standardisierten DTOs liefert eine klare Schranke zwischen Präsentations- und Geschäftslogik.
]


)
]

== SWA-Muster & Datenfluss

#slide[
= Hexagonale Architektur (Ports & Adapter)

#grid(
columns: (1fr, 1fr),
gutter: 20pt,
[
Zur strikten Entkopplung von Geschäftslogik und technischer Infrastruktur ist das Backend in separate C\#-Projekte unterteilt:

  - *Domain:* Kern-Entitäten als unmodifizierbare C\#-Records (Vermeidung von Seiteneffekten).
  - *Application:* Enthält Use-Case-Services (z. B. `GroupCalendarService`) und definiert die Ports (Interfaces).
  - *Adapter:* Technologie-Anbindungen (EFCore-Postgres, Web-API Controller, Raumzeit-API-Client).
],
card("Compiler-erzwungene Richtung", stroke-color: rgb("#4f46e5"))[
  #align(center)[
    *Domain* $arrow.l$ *Application* $arrow.l$ *Adapter*
  ]
  
  #v(10pt)
  Durch die Trennung auf Ebene der *.NET Project References* ist ein direkter Schichtenverstoß (z. B. Zugriff der Domain auf Datenbank-Klassen) physisch unmöglich.
]
)
]

#slide[
  #align(center)[
    #image("assets/Hexagonal.drawio.png")
  ]  
  
]

#slide[
= Interaktions- & Datenfluss

Der typische Ablauf bei der Lerngruppen-Terminplanung in 4 Stufen:

#v(10pt)
#grid(
columns: (1fr, 1fr, 1fr, 1fr),
gutter: 12pt,
card("1. Import", stroke-color: rgb("#4f46e5"))[
User bindet iCal-Schnittstelle von Campus-System an.
],
card("2. Gruppe", stroke-color: rgb("#4f46e5"))[
Erstellung einer Gruppe (z. B. "SWA Labor").
],
card("3. Beitritt", stroke-color: rgb("#4f46e5"))[
Mitglieder treten via QR-Code der Gruppe bei.
],
card("4. Match", stroke-color: rgb("#4f46e5"))[
Vollautomatischer Slot-Abgleich im Backend.
]
)
]

#slide[
= Kalenderdaten-Verarbeitung

Verarbeitungsschritte im Service:
1. Sammeln: Laden aller Gruppenkalender aus dem Repository.
2. Aggregieren: Zusammenführen aller blockierten Termine in eine Liste.
3. Verschmelzen: Sortieren nach Startzeit und Zusammenführen überlappender Blöcke.
4. Komplementärzeit: Berechnung freier Blöcke innerhalb des wöchentlichen Rasters.
]

#slide[
= Backend-for-Frontend (BFF) & Auth

#grid(
columns: (1fr, 1.2fr),
gutter: 20pt,
card("100% Token-Sicherheit", stroke-color: rgb("#4f46e5"))[
Sensible Token (Google OAuth, Campus-Credentials) verbleiben ausschließlich im Backend und werden niemals an den Browser übertragen.
],
[
Architekturmuster BFF (Backend-for-Frontend):
- Sicherer OAuth-Flow: Die Authentifizierung läuft direkt über den Server ab.
- HttpOnly Cookies: Der Session-Zustand wird über verschlüsselte, SameSite-geschützte Cookies gehalten (wirksamer Schutz gegen XSS).
- Data Protection: Verschlüsselte Persistierung der Campus-Token in PostgreSQL mittels .NET DataProtection API.
]
)
]

== Persistenz & Deployment

#slide[
= Persistenz & Datenmodell

#grid(
columns: (1fr, 1fr),
gutter: 20pt,
[
Datenbank-Entkopplung:
- PostgreSQL als relationale Datenbank.
- n:m-Verknüpfung zwischen Nutzern und Gruppen über Relationstabelle user_groups.
- Modell-Trennung: Keine DB-Annotationen im Domänenmodell. EFCore-Klassen sind rein technische Speicherobjekte; die Konvertierung erfolgt über dedizierte Mapper.
- Kaskadierung: Verwaiste Gruppen (0 Mitglieder) werden automatisiert gelöscht.
],
card("Datenbank-Schema (Relational)", stroke-color: rgb("#0f172a"))[
- users (id, google_id, name, encrypted_token)
- groups (id, name)
- user_groups (user_id FK, group_id FK)
]
)
]



#slide[
= Integrationstests & Qualitätssicherung

#grid(
columns: (1.2fr, 1fr),
gutter: 20pt,
[
Testing an der Systemgrenze (REST-API):
- Virtueller TestHost: Bootstrapping des gesamten App-Kontexts im Speicher mittels WebApplicationFactory.
- Echte HTTP-Anfragen: Validierung von Routing, DTO-Mapping und Statuscodes ohne Netzwerk-Overhead.
- Keine Mock-Hölle: Echte Zusammenarbeit der Services und Middleware-Pipelines wird verifiziert.
],
card("Datenbank-Realismus via Testcontainers", stroke-color: rgb("#f59e0b"))[
- PostgreSQL im Docker: Automatischer Start einer isolierten PostgreSQL-Container-Instanz pro Testlauf.
- Echtes SQL-Verhalten: Verhindert False-Positives von In-Memory-Datenbanken (z.B. bei komplexen n:m-Joins).
- Isolierter Zustand: Garantiert komplett unbeeinflusste Testläufe in einer Sandbox.
]
)
]

#slide[
= Deployment & Orchestrierung

#grid(
columns: (1fr, 1fr),
gutter: 20pt,
card("Containerisierung (Docker)", stroke-color: rgb("#0ea5e9"))[
- Vollständig containerisiertes Multi-Container-Setup (Postgres, Web-API, React).
- Garantiert absolute Umgebungsgleichheit zwischen Entwicklung und Produktion.
],
card("Orchestrierung (.NET Aspire)", stroke-color: rgb("#10b981"))[
- Service Discovery: Automatische, namensbasierte Auflösung der Dienste anstelle statischer IPs.
- Observability: Integriertes OpenTelemetry Dashboard zur Live-Analyse von Traces, Metriken und Log-Abfolgen.
]
)
]

#focus-slide[
Fragen & Diskussion

#v(20pt)
Vielen Dank für Ihre Aufmerksamkeit!

]