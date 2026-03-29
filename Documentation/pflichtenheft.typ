#import "assets/layout.typ": layout
#show: layout

= Lernzeit
== Grundlegende Problemstellung
Studierende stehen regelmäßig vor der Herausforderung, gemeinsame Termine für Lern- oder Projektgruppen zu koordinieren. Aufgrund individueller und häufig stark unterschiedlicher Stundenpläne gestaltet sich die Abstimmung geeigneter Zeitfenster als zeitaufwendig und ineffizient.

In der Praxis erfolgt die Terminfindung meist über Gruppenchats oder persönliche Absprachen. Dabei werden Vorschläge von einzelnen Gruppenmitgliedern eingebracht und anschließend von den übrigen Teilnehmenden auf mögliche Konflikte geprüft. Dieser iterative Abstimmungsprozess führt häufig zu einem "Hin und Her"-Austausch, der sowohl zeitintensiv als auch organisatorisch aufwendig ist.

Die Anwendung *Lernzeit* adressiert dieses Problem, indem sie die Terminfindung automatisiert unterstützt. Nutzerinnen und Nutzer können Gruppen erstellen und ihre individuell blockierten Zeiten hinterlegen. Auf Basis dieser Informationen generiert die Anwendung geeignete Terminvorschläge, zu denen alle Gruppenmitglieder verfügbar sind. Ziel ist es, den Abstimmungsaufwand zu reduzieren und eine effiziente sowie transparente Terminplanung zu ermöglichen.

== Architekturvorschlag

== Technologieauswahl
Im Rahmen der Analyse wurden für das Backend die Programmiersprachen _Rust, Go_ und _C\#_ sowie für das Frontend die Frameworks _Blazor, Vue_ und _React_ evaluiert. Die Tabellen #ref(<vergleich_backend>) und #ref(<vergleich_frontend>) listen die jeweiligen Stärken und Schwächen übersichtlich auf.
#figure(
  caption: [Vergleich Backend Technologien],
  table(
    columns: 3,
    align: left,
    table.header([], [Pro], [Kontra]),
    // Rust
    [Rust],
    [
      - Höchste Performance
      - Memory-Safe (Ownership & Borrowing)
    ],
    [
      - Noch keine Erfahrung im Team
      - Steile Lernkurve
      - Saubere Schichtentrennung komplex
      - Wenige SDKs / externe Bibliotheken
    ],
    // Go
    [Go],
    [
      - Erfahrung im Team
      - Sehr schnelle Entwicklung & einfache Syntax
      - Einfaches Deployment / kleine Container
      - Minimalistisch
    ],
    [
      - Fördert keine saubere Architektur (wenig Guidance)
      - DI nur durch externe Bibliothek
      - Weniger Features für komplexe Patterns
    ],

    // C#
    [C\#],
    [
      - Erfahrung im Team
      - Ausgereiftes Ecosystem, viele SDKs
      - Built-in DI & klare Layer-Struktur
    ],
    [
      - Container größer, Startup langsamer
      - Framework kann "Magic" verstecken
    ],
  ),
)<vergleich_backend>

#figure(
  caption: [Vergleich Frontend Technologien],
  table(
    columns: 3,
    align: left,
    table.header([], [Pro], [Kontra]),
    // Blazor
    [Blazor],
    [
      - Fullstack mit C\# Backend möglich
      - Starke Typisierung & Compile-Time Checks
      - Gute Integration in .NET Ecosystem
    ],
    [
      - Wenig Erfahrung im Team
      - Weniger reifes Frontend-Ökosystem als JS Frameworks
      - Lernkurve für WebAssembly-spezifische Konzepte
      - Weniger Community-Beispiele & Tutorials
    ],

    // Vue
    [Vue],
    [
      - Erfahrung im Team
      - Sehr einfache Lernkurve, leicht verständlich
      - Flexible Komponentenstruktur
      - Große Community & viele Plugins
    ],
    [
      - Architektur nicht erzwungen
      - Bei großen Projekten kann State-Management komplex werden
      - TypeScript optional, sonst lose Typisierung
    ],

    // React
    [React],
    [
      - Erfahrung im Team
      - Riesiges Ecosystem & Community
      - Flexibles Komponentenmodell & Hooks
      - TypeScript-Integration möglich
    ],
    [
      - Kein festes Architektur-Muster vorgegeben
      - Boilerplate bei komplexem State / Redux
      - Lernkurve bei Hooks & Patterns für Anfänger
    ],
  ),
) <vergleich_frontend>

Nach Abwägung der Vor- und Nachteile fiel die Wahl auf die Kombination C\# / ASP.NET als Backend-Framework und React als Frontend-Framework.

Diese Entscheidung basiert auf folgenden Kriterien:

- *Architektur*: C\# ermöglicht klare Layer-Strukturen und eingebaute Dependency Injection, während React eine flexible Komponentenstruktur bietet, die eine saubere Trennung von Zuständen und Logik unterstützt.
- *Team-Expertise*: Beide Technologien sind im Team bekannt, was eine schnelle Entwicklung und Wissenstransfer erleichtert.
- *Ökosystem & Community*: React unnd C\# bieten jeweils ein umfangreiches Ökosystem, was die Implementierung von komplexen Anforderungen erleichtert. Beide Technologien sind etabliert und haben große Communities.
- *Lern- und Weiterentwicklung*: Die Kombination erlaubt es dem Team, vorhandenes Wissen zu vertiefen und neue Best Practices im Bereich Softwarearchitektur gemeinsam zu erlernen.

== Use Cases
== Muss-/Kann-Kriterien
