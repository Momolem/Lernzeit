#import "assets/abgabe_layout.typ": layout
#import "@preview/oxdraw:0.1.0": *
#import "@preview/mmdr:0.2.1": mermaid
#show: layout

#outline()
#pagebreak()

= Grundlegende Problemstellung
Studierende stehen regelmäßig vor der Herausforderung, gemeinsame Termine für Lern- oder Projektgruppen zu koordinieren. Aufgrund individueller und häufig stark unterschiedlicher Stundenpläne gestaltet sich die Abstimmung geeigneter Zeitfenster als zeitaufwendig und ineffizient.

In der Praxis erfolgt die Terminfindung meist über Gruppenchats oder persönliche Absprachen. Dabei werden Vorschläge von einzelnen Gruppenmitgliedern eingebracht und anschließend von den übrigen Teilnehmenden auf mögliche Konflikte geprüft. Dieser iterative Abstimmungsprozess führt häufig zu einem "Hin und Her"-Austausch, der sowohl zeitintensiv als auch organisatorisch aufwendig ist.

Die Anwendung *Lernzeit* adressiert dieses Problem, indem sie die Terminfindung automatisiert unterstützt. Nutzerinnen und Nutzer können Gruppen erstellen und ihre individuell blockierten Zeiten hinterlegen. Auf Basis dieser Informationen generiert die Anwendung geeignete Terminvorschläge, zu denen alle Gruppenmitglieder verfügbar sind. Ziel ist es, den Abstimmungsaufwand zu reduzieren und eine effiziente sowie transparente Terminplanung zu ermöglichen.

= Architekturvorschlag
Es wurden im Rahmen des Pflichtenhefts verschiedene Systemarchitekturen verglichen. Eine Gegenüberstellung hierzu ist in #ref(<vergleich_architektur>) zu finden.

Es wurde sich im Rahmen des Projekts für eine Client-Server Architektur entschieden. Die erhöhte Komplexität alternativer Architekturstile steht in keinem angemessenen Verhältnis zu deren potenziellen Vorteilen für das Lernzeitprojekt im Rahmen des Labors.

Durch eine konsequente modulare Trennung der Funktionalitäten wird jedoch sichergestellt, dass eine spätere Migration zu einer Microservice-Architektur grundsätzlich möglich ist.

#figure(
  caption: [Vergleich System Architekturen],
  table(
    columns: 3,
    align: left,
    table.header([*Architektur*], [*Pro*], [*Kontra*]),

    [*Client-Server*],
    [
      - Zentrale Kontrolle und Verwaltung
      - Einfache Implementierung
      - Gute Sicherheit durch Backend
      - Anonymisierung zentral möglich
    ],
    [
      - Single Point of Failure
      - Skalierungsprobleme bei hoher Last
      - Kann ggf. zu unwartbarem Monolithen wachsen
    ],

    [*Microservice Architektur*],
    [
      - Gute Skalierbarkeit
      - Klare Trennung der Verantwortlichkeiten
      - Services unabhängig entwickelbar
      - Flexibel erweiterbar
    ],
    [
      - Höhere Komplexität
      - Aufwendiges Deployment
      - Kommunikation zwischen Services notwendig
      - Fehler schwerer zu debuggen
    ],

    [*Peer-to-Peer*],
    [
      - Keine zentrale Instanz nötig
      - Gute Skalierbarkeit
      - Geringe Serverlast
      - Hohe Ausfallsicherheit
    ],
    [
      - Komplexe Client-Logik
      - Sicherheits- und Datenschutzprobleme
      - Synchronisation schwierig
      - Abhängigkeit von Client-Verfügbarkeit
    ],
  ),
)<vergleich_architektur>

Als Protokoll zur Kommunikation wird eine REST-Schnittstelle verwendet. Es wurde als Alternative GraphQL betrachtet, allerdings scheint auch hier durch die höhere Komplexität bei der Implementierung im Rahmen des Projekts keinen Mehrwert zu bieten.

#figure(
  image("assets/Client-Server.png"),
  caption: "Systemarchitektur Lernzeit",
)

= Technologieauswahl
Im Rahmen der Analyse wurden für das Backend die Programmiersprachen _Rust, Go_ und _C\#_ sowie für das Frontend die Frameworks _Blazor, Vue_ und _React_ evaluiert. Die Tabellen #ref(<vergleich_backend>) und #ref(<vergleich_frontend>) listen die jeweiligen Stärken und Schwächen übersichtlich auf.
#figure(
  caption: [Vergleich Backend Technologien],
  table(
    columns: 3,
    align: left,
    table.header([], [Pro], [Kontra]),
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

= Use Cases
*1. Stundenplan importieren:* Die App kann den eigenen Uni-Stundenplan übernehmen. Dafür meldet man sich auf der Hochschul-Plattform an, erstellt einen Freigabe-Link und fügt diesen in Lernzeit ein.\
*2. Lerngruppe erstellen:* Um gemeinsame Termine zu planen, erstellt man eine neue Lerngruppe und gibt ihr einen aussagekräftigen Namen - zum Beispiel „Mathe-Lerngruppe".\
*3. Teilnehmer einladen:* Wer eine Gruppe erstellt hat, kann andere über einen Link oder QR-Code weitere Personen einladen. Der QR-Code dient zu einer schnellen und unkomplizierten Art einer Lerngruppe beizutreten.\
*4. Freie Zeiten finden und buchen:* Die App zeigt Zeiten, in denen alle Gruppenmitglieder frei sind. Diese freien Zeiten kann man als gemeinsame Lernzeit auswählen und optional einen Treffpunkt sowie eine Uhrzeit festlegen.\

= Muss-/Kann-Kriterien
#table(
  columns: 2,
  align: (left, left),

  [ ], [],
  [*Mindestanforderungen*],
  [
    - Importieren des eigenen Stundenplanes \
    - Erstellen von Lerngruppe \
    - Hinzufügen von neuen Mitgliedern durch einen Einladungslink oder einen QR-Code \
    - Abgleich von allen Kalenderdaten der Gruppenmitglieder \
    - Visuell überschaubare Darstellung der freie Zeitblöcke \
  ],

  [*Nice to have*],
  [
    - Möglichkeit zur Abstimmung eines Termins \
    - Anzeige aller frei verfügbarer Räume auf dem Campus \
    - Benachrichtigungen bei neuen Terminvorschlägen oder Änderungen \
    - Integration von Kalender-Apps (Google Calendar, Outlook) \
  ],
)

= Umsetzung / Implementierung
== Frontend (React)
=== Designprozess (Figma)
Zu Beginn wurde ein Prototyp in Figma erstellt. Dabei wurde das in Figma integrierte KI-Tool genutzt,
um die Vorteile von KI im Designprozess zu nutzen. Dies ermöglichte eine schnellere Erledigung von
Routineaufgaben (Resizing, Layouts, Styles), ein schnelleres Testen mehrerer Ideen und
Designvarianten sowie Unterstützung bei barrierefreiem und inklusivem Design.
@figma

Dabei gab es jedoch auch Herausforderungen: Das resultierende Design wirkte weniger durchdacht und
generisch.

#figure(
  caption: "Iniziale Seite von Figma AI",
  image("assets/Figma_AI_Beispiel.png"),
)

Mithilfe des KI-Tools wurde eine grobe Oberflächenstruktur erarbeitet, die anschließend verfeinert
wurde. Dazu wurde der User-Flow von Seite zu Seite skizziert und sich am KI-generierten Seitenaufbau
orientiert.
(Bild von Skizzen)

Um das visuelle Erscheinungsbild der Oberfläche festzulegen, wurde ein Moodboard erstellt.
#figure(
  caption: [Das Moodboard @moodboard_img1 @moodboard_img2],
  image("assets/Moodboard.png"),
)


Die Referenzen helfen dabei, die visuellen Komponenten im Designprozess greifbar zu machen.
Von den Referenzen ausgehend, wurden die einzelnen Komponenten erstellt und in den Seiten zusammengeführt.
#figure(
  caption: "Iniziale Seite von Figma AI",
  image("assets/Figma_Comps.png"),
)

Die Seitenstruktur ist wie folgt aufgebaut:
(UserFlow-Diagramm einfügen)

=== Komponenten
Die Frontend-Architektur von *Lernzeit* folgt einem streng modularen Ansatz unter Verwendung von React. Das Ziel war es, eine hochgradig interaktive Benutzeroberfläche zu schaffen, die komplexe Kalenderdaten verständlich visualisiert.

#figure(
  caption: "Übersicht der Hauptkomponenten in der Web-Oberfläche",
  image("assets/overview_mainpage.png"),
)

Zentrale Bausteine der UI sind:
- *Timetable-Komponente*: Dies ist das Herzstück der Anwendung. Sie visualisiert Zeitfenster in einem wöchentlichen Raster. Die Logik zur Berechnung der relativen Positionen der Event-Boxen ist in der Komponente gekapselt.
- *GroupCard*: Ein wiederverwendbares Element zur Darstellung von Gruppeninformationen, das den schnellen Wechsel zwischen verschiedenen Lerngruppen ermöglicht.
- *Interaktive Modals*: Für die Erstellung von Gruppen und die Einladung von Mitgliedern wurden spezialisierte Pop-Up-Komponenten entwickelt, die einen geführten Workflow bieten.
- *Header & Navigation*: Eine konsistente Navigationsleiste, die den Nutzerstatus (Login/Logout via Google) reflektiert und schnellen Zugriff auf die Profil- und Gruppeneinstellungen bietet.

=== API-Kommunikation und State-Management
Die Anbindung an das Backend erfolgt über eine dedizierte Schicht im Frontend, die in `client.ts` definiert ist. Wir setzen hierbei auf die native `fetch`-API, ergänzt um Error-Handling-Wrapper.

#block(fill: luma(240), inset: 10pt, radius: 4pt, width: 100%)[
  #set align(center)
  *TODO: Sequenzdiagramm eines API-Requests (z.B. GetGroupCalendar) hier einfügen* \
  _Abbildung: Fluss eines Datenabrufs vom UI-Trigger bis zum Backend-Response_
]

Wichtige Aspekte der Implementierung sind:
- *Asynchrone Hooks*: Daten werden mittels React-Hooks (`useState`, `useEffect`) geladen. Für komplexere Datenflüsse wurden eigene Hooks wie `useTimetableICS` erstellt, die die Transformation von Rohdaten in das für die UI benötigte Format übernehmen.
- *Typisierung*: Durch den Einsatz von TypeScript werden die DTOs (Data Transfer Objects) des Backends im Frontend gespiegelt, was die Fehlerquote bei der Datenverarbeitung massiv reduziert.
- *Fehlerbehandlung*: Jeder Request prüft den HTTP-Statuscode. Bei Fehlern (z.B. 401 Unauthorized oder 500 Server Error) erhält der Nutzer über die UI direktes Feedback.

== Backend (.NET API)
Das Backend ist als ASP.NET Core Web API realisiert und folgt den Prinzipien der Clean Architecture (Trennung von Domain, Application und Infrastruktur).

=== Architekturprinzipien
Strukturell haben wir uns im Backend für eine hexagonale Architektur (auch bekannt als „Ports und Adapter“) entschieden. Hierbei werden spezifische Technologien in Adaptern abgekapselt und somit strikt von der Domänenlogik separiert. Die verschiedenen Schichten und Adapter werden in C\# als separate Projekte modelliert. Hierdurch können Zugriffe der Schichten in falscher Richtung, also von innen nach außen, strukturell verhindert werden. Im folgenden Abschnitt werden die verschiedenen Schichten erläutert:
#figure(
  caption: "Architektur des Projekts",
  image("assets/Hexagonal.drawio.png"),
)


- *Domain*: Hier ist das Domänenmodell in Form von C\#-Records definiert. Im C\#-Kontext werden Records als Datenhüllen verwendet, da diese standardmäßig immutable (unveränderlich) sind. Dies hilft dabei, unerwartete Seiteneffekte in der Domäne zu vermeiden. Es wird empfohlen, in diesem Projekt den Prinzipien des Domain-Driven Designs (DDD) zu folgen und beispielsweise nach Möglichkeit Value Objects zu verwenden

- *Application*: In dieser Schicht erfolgt die Verknüpfung der Schichten. Hier werden die Schnittstellen (Ports) in Form von Interfaces definiert. Zudem werden hier Services implementiert, welche die Verarbeitung von Domänenobjekten und die Logik der Anwendungsfälle (Use Cases) definieren. In unserem Beispiel erfolgt hier die Berechnung der Gruppenkalender.

- *Adapter*:
  - *ASP.NET Core Webprojekt*: Hier wird die REST-Schnittstelle implementiert, über die Anwender*innen mit der Anwendung interagieren. In diesem Adapter werden Domänenobjekte zu DTOs (Data Transfer Objects) gemappt, um sie an das Frontend zu übergeben
  - *EFCore*: Entity Framework Core dient als ORM für die Interaktion mit der Postgres-Datenbank. Hier werden Repositories implementiert, um Domänenobjekte zu speichern und zu laden. Die Domänenmodelle werden nicht direkt via EFCore persistiert, sondern zunächst auf separate Persistenz-Objekte gemappt, um keine technischen Details (wie Datenbank-Annotationen) in die Domäne propagieren zu lassen
  - *DataProtection*: Hier wird ein Dienst implementiert, der Tokens sicher verschlüsselt, damit keine unverschlüsselten Tokens in die Datenbank geschrieben werden.
  - *RaumzeitAPI*: Dieser Adapter realisiert die Anbindung an die externe Raumzeit-Schnittstelle via REST. Über den DataProtection-Adapter werden die Tokens sicher verwaltet. Geladene iCal-Kalender werden mit einer Library geparsed und in das interne Domänenmodell überführt.

=== API-Endpunkte
Um die Zuständigkeitsbereiche der Adapter klar zu gliedern, wurden diese in die drei Hauptbereiche
Authentication, Groups und User unterteilt. Das Adaptermodell wurde wie folgt in die Endpunkte
überführt:

*Authentication*
(Quelle: Google) Für die Authentifizierung der Nutzer fiel die Entscheidung auf einen
Drittanbieter: in diesem Fall Google. Der Google-Authentifizierungsprozess bietet Nutzern eine
komfortable Möglichkeit, sich mit nur einem Schritt bei Apps oder Websites anzumelden, indem sie
ihren bestehenden Google-Account verwenden. Damit entfällt die Notwendigkeit separater
Zugangsdaten, komplexer Registrierungsformulare und vergessener Passwörter. Mit nur wenigen
Codezeilen lässt sich der Login-Prozess integrieren.
Weitere Details zum Authentifizierungsprozess finden sich unter (Hyperlink) Kapitel 2.5.

- `GET api/auth/login`: Der Endpunkt leitet den Nutzer mit der entsprechend aufgerufenen URL zur
  Google-Anmeldeseite weiter.
- `GET api/auth/logout`: Der Nutzer wird abgemeldet.
- `GET api/auth/me`: Der Nutzer registriert sich über den Google-Authentifizierungsprozess in der
  LernZeit-Anwendung. Entsprechend wird der Nutzer anschließend in der Datenbank hinterlegt.

*Group*
- `GET api/group/`: Die Response gibt alle vorhandenen Lerngruppen zurück.
- `GET api/group/{groupId}`: Die Response gibt die Gruppe mit der entsprechenden ID zurück.
- `POST (protected) api/group`: Nimmt aus dem Request-Body ein GroupDTO-Objekt sowie den Token des
  Nutzers entgegen und erstellt daraufhin eine neue Gruppe.
- `POST (protected) api/group/join/{groupId}`: Fügt den eingeloggten Nutzer der Gruppe mit der
  entsprechenden groupId hinzu.
- `POST api/group/leave/{groupId}`: Entfernt den Nutzer aus der Gruppe mit der entsprechenden
  groupId.
- `GET api/group/{groupId}/calendar`: Response mit dem Gruppenkalender.

*User*
- `GET api/user`: Response mit allen registrierten Nutzern.
- `GET api/user/{userId}`: Response des Nutzers mit der entsprechenden ID.
- `PUT api/user`: Nimmt aus dem Request-Body das UserDTO-Objekt des aktualisierten Nutzers entgegen.
- `DELETE api/user/{userId}`: Löscht den Nutzer aus der Datenbank.
- `POST api/user/raumzeit/login`: Nimmt aus dem Request-Body die RaumZeit-Credentials entgegen und
  ordnet die RaumZeit-Kalenderdaten dem Nutzerkonto zu.
- `GET (protected) api/user/calendar`: Die Response enthält die Kalenderdaten des Nutzers.

=== Kalenderdaten-Verarbeitung
Die Kernlogik zur Ermittlung gemeinsamer Termine ist im `GroupCalendarService` implementiert. Dieser führte folgende Schritte nacheinander aus:
1. *Laden der Gruppe*
  - Die Gruppe wird anhand ihrer eindeutigen ID aus dem Repository geladen.
  - Existiert die Gruppe nicht, wird kein Gruppenkalender erzeugt.
2. *Abrufen der persönlichen Kalender*
  - Für jedes Gruppenmitglied wird der persönliche Kalender über den Kalenderservice geladen.
  - Die geladenen Kalender werden zu einer gemeinsamen Sammlung zusammengeführt.
3. *Zusammenführen aller Termine*
  - Alle Termine aus den persönlichen Kalendern werden in einer Liste gesammelt.
  - Die Termine werden nach ihrer Startzeit sortiert.
4. *Ermittlung der belegten Zeiträume*
  - Die sortierte Terminliste wird nacheinander durchlaufen.
  - Für jeden Termin wird geprüft, ob er sich mit dem zuletzt gespeicherten Zeitraum überschneidet.
    - Keine Überschneidung: Es wird ein neuer Belegungsblock angelegt.
    - Überschneidung: Die Zeiträume werden zu einem gemeinsamen Block zusammengeführt, indem das spätere Enddatum übernommen wird.
  - Dadurch entstehen zusammenhängende Zeitintervalle, die alle belegten Zeiten der Gruppenmitglieder abdecken.

=== Gruppenverwaltung
Gruppen können von jedem Nutzer erstellt werden. Dies geschieht auf der Hauptseite über den Button
„_Gruppe erstellen_". Im daraufhin erscheinenden Dialogfenster kann ein Gruppenname vergeben werden,
nach dessen Bestätigung die Gruppe erstellt wird. Um weitere Nutzer zu einer Gruppe hinzuzufügen,
müssen diese vom Ersteller eingeladen werden. Die Einladung kann über einen Link oder das Scannen
eines QR-Codes erfolgen, wofür der Nutzer auf der GroupCard auf das QR-Icon tippt.

Der Gruppenkalender kann durch einen Klick auf die Gruppenkarte aufgerufen werden. Er stellt das
Ergebnis aller einzelnen Kalender der Gruppenmitglieder dar. Im Gruppenkalender werden ausschließlich
bereits belegte Zeitblöcke angezeigt. Durch einen Klick auf eine freie Spalte lässt sich ein
Gruppen-Event hinzufügen.

Zum Verlassen einer Gruppe wird der Button „_Gruppe verlassen_" oberhalb des Gruppenkalenders
betätigt.

Die Verknüpfung zwischen Gruppen und ihren Mitgliedern ist in der Datenbank hinterlegt.
Näheres dazu findet sich im folgenden Kapitel. Sind einer Gruppe keine Nutzer mehr zugeordnet,
wird diese automatisch aus der Datenbank entfernt.


=== Datenbank
==== Datenbankstruktur

#figure(
  image("assets/database_diagram.png"),
  caption: "Datenbank-Schema",
) <dbSchema>

Abbildung @dbSchema zeigt das Datenbankschema zwischen der User- und der Gruppentabelle. Die
User_Groups-Tabelle ist eine Relationstabelle und steht zu den Tabellen users und groups jeweils
in einer 1-zu-n-Beziehung. Die Relation wird über die Primärschlüssel von users und groups
hergestellt.

==== Implementierung
Das gewählte Datenbankmanagementsystem ist PostgreSQL. Im Rahmen der Domain-Driven-Architektur
erfolgt der Datenbankzugriff der LernZeit-Anwendung über den LernZeit.PostgresAdapter. Dieser
enthält unter anderem den Datenbankkontext, die für das .NET-Framework EntityFrameworkCore
erforderlichen Migrationen sowie die Repositories zu den Gruppen- und Nutzerobjekten,
Datenbank-Entity-Klassen und Mapper-Klassen.

Im LernZeitDBContext werden die Datenbank-Entities festgelegt und die UserGroups-Relation
hergestellt. Das Group- und das UserRepository stellen diverse Schnittstellen zur Modifizierung
der Datenbankobjekte bereit.

=== Authentifizierung (Google Auth)
Die Sicherheit der Anwendung wird durch die Integration von Google OAuth 2.0 gewährleistet. Das Backend fungiert hierbei als Validierungsschicht:
- Der Client sendet ein von Google ausgestelltes ID-Token.
- Das Backend validiert dieses Token gegen die Google-Server (Signatur- und Audience-Check).
- Nach erfolgreicher Validierung wird die `GoogleUserId` extrahiert. Diese dient als Primärschlüssel in unserer internen `User`-Entität, wodurch wir keine sensiblen Passwortdaten speichern müssen.

=== Kommunikation mit der RaumZeit API
Um den manuellen Aufwand für Studierende zu minimieren, integriert *Lernzeit* die RaumZeit-API der Hochschule.
- *RaumzeitService*: Diese Komponente kapselt die HTTP-Kommunikation mit dem Hochschul-System. Sie übernimmt das Mapping der proprietären RaumZeit-Strukturen auf unsere internen Domain-Modelle.
- *Sicherheit & Token-Management*: Da der Zugriff auf Stundenpläne autorisiert erfolgen muss, speichern wir die notwendigen Zugriffstoken verschlüsselt in der PostgreSQL-Datenbank. Hierfür kommt der `TokenEncryptionService` zum Einsatz, der die `IDataProtectionProvider`-Infrastruktur von .NET nutzt.

#block(fill: luma(240), inset: 10pt, radius: 4pt, width: 100%)[
  #set align(center)
  *TODO: Klassendiagramm des Raumzeit-Integrationsmoduls einfügen* \
  _Abbildung: Struktur des Raumzeit-Services und dessen Datenfluss_
]

== Deployment und Infrastruktur
Das Projekt verfolgt einen modernen "Infrastructure as Code" Ansatz.

=== Deployment-Umgebung
Die Applikation ist so konzipiert, dass sie vollständig in Docker-Containern lauffähig ist. Dies garantiert eine identische Laufzeitumgebung zwischen Entwicklung (Localhost), Testing und Produktion. Als Datenbank wird eine PostgreSQL-Instanz verwendet, die ebenfalls containerisiert bereitgestellt wird.

=== Containerisierung und Orchestrierung mit .NET Aspire
Ein technologisches Highlight ist der Einsatz von .NET Aspire. Dies ermöglicht eine nahtlose Orchestrierung der Microservices (oder modularen Monolithen) während der Entwicklung:
- *Service Discovery*: Das Frontend findet das Backend automatisch über logische Namen, statt über statische IP-Adressen.
- *Observability*: .NET Aspire bietet ein integriertes Dashboard, in dem Logs, Traces und Metriken aller Komponenten in Echtzeit eingesehen werden können.
- *Konfigurationsmanagement*: Connection Strings für die Datenbank und API-Keys werden zentral im `AppHost` verwaltet und sicher an die jeweiligen Services durchgereicht.

#block(fill: luma(240), inset: 10pt, radius: 4pt, width: 100%)[
  #set align(center)
  *TODO: Screenshot des .NET Aspire Dashboards einfügen* \
  _Abbildung: Laufzeit-Überwachung der Container-Infrastruktur_
]


= Fazit
== Rückblick
Im Rahmen dieses Moduls lag der Fokus auf der Software-Architektur des Projekts. Ein besonderes
Augenmerk galt dabei der Strukturierung und Einordnung von Zuständigkeitsbereichen. Die Entscheidung
fiel auf ein Domain-Driven-Modell, das eine klare Trennung zwischen Anwendungslogik und Adaptern
gewährleistet.

Eine zentrale Fragestellung war, welche Nutzerdaten gespeichert werden müssen, damit die Anwendung
funktioniert. Das Ziel war es, so wenig wie möglich zu speichern, da der Großteil der
nutzerbezogenen Daten von Drittanbietern wie Google Authentication und der RaumZeit-API
bereitgestellt wird.

Ebenfalls war die Authentifizierung der Nutzer zu Beginn ein wichtiges Thema. Die Auslagerung
dieses Problems an einen Drittanbieter stellte sich als komfortable und zuverlässige Lösung heraus.

Im Frontend stellte die Auswahl einer geeigneten Kalenderkomponente eine Herausforderung dar.
Die Entscheidung für eine vorgefertigte React-Kalenderkomponente ermöglichte ein schnelleres
Ergebnis, anstatt alle Funktionen von Grund auf selbst zu entwickeln. Da die Auswahl an verfügbaren
Komponenten groß ist, wurden mithilfe des KI-Modells OpenCode mehrere Komponenten iterativ getestet,
um die für den Anwendungsfall am besten geeignete zu ermitteln.

Im Verlauf des Entwicklungsprozesses kam es vereinzelt dazu, dass Komponenten fehlerhafte Zustände
annahmen oder nicht mehr korrekt funktionierten, was eine weitere Herausforderung darstellte.

Darüber hinaus funktionierte die Authentifizierung aus verschiedenen Gründen nicht immer
zuverlässig. Insbesondere die CORS-Policies stellten zu Beginn aufgrund von Client-Weiterleitungen
ein Problem dar.

Gegen Ende des Projekts ergaben sich in der Testphase weitere Herausforderungen. Das Testen der
Anwendung mit mehreren Nutzern ist nicht trivial, da Nutzerkonten eng mit dem jeweiligen
Google-Account verknüpft sind. Daher waren zusätzliche Google-Accounts erforderlich, um die
Anwendung vollständig testen zu können.

== Ausblick
Das LernZeit-Projekt hat alle Mindestanforderungen erfüllt, die zu Beginn im Pflichtenheft
festgelegt wurden. Es ist möglich, Lerngruppen zu erstellen, eigene Kalender aus RaumZeit zu
importieren, diese mit den Mitgliedern der Lerngruppe abzugleichen und in einer übersichtlichen
Benutzeroberfläche einen gemeinsamen freien Zeitraum festzulegen.

Neben den funktionalen Anforderungen sind auch die nicht-funktionalen Anforderungen erfüllt.
Die LernZeit-Anwendung entspricht den Prinzipien des Domain-Driven Designs: Komponenten sind klar
in ihren Zuständigkeitsbereichen getrennt, was eine reibungslose Wartung ermöglicht. Ebenso können
Komponenten bei Bedarf unkompliziert ausgetauscht oder erweitert werden. Darüber hinaus bietet eine klare Trennung eine einfache Möglichkeit Komponenten isoliert zu testen.

Für zukünftige Erweiterungen wären weitere Use-Cases denkbar. So wäre eine Funktion zur
gemeinsamen Terminabstimmung innerhalb einer Gruppe eine sinnvolle Ergänzung. Darüber hinaus würde
eine Anzeige aller freien Räume auf dem Campus die Suche nach einem geeigneten Lernort erleichtern.
Die Integration von Benachrichtigungen bei neuen Terminvorschlägen oder Änderungen wäre eine
nützliche Erinnerungsfunktion. Schließlich wäre auch die Anbindung weiterer Kalender-Apps,
beispielsweise Google Calendar oder Outlook, eine denkbare Erweiterung.

= Literaturverzeichnis
// TODO: Quellen und AI-Verzeichnis (wo wurde ChatGPT/Copilot eingesetzt?).

#bibliography("quellen.yml")
