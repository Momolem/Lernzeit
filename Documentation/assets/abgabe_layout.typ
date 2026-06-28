#let layout(doc) = {
  [

    #set page(
      fill: white,
      margin: (left: 1in),
      header: none,
    )

    #line(start: (0%, 5%), end: (8.5in, 5%), stroke: (thickness: 2pt))

    #align(left + top)[
      #text(size: 28pt, [Lernzeit])

      #text(size: 18pt, [Der smarte Gruppenkalender])

      HKA - Software Labor
      Sommersemester 2026

      Moritz Vogel\
      Simon Trinkl

    ]
    #align(bottom + left)[#datetime.today().display()]

    #pagebreak()

    #set page(
      numbering: "1",
      header: none,
    )

    #set text(size: 12pt, lang: "de")
    #set par(justify: true)

    #set heading(numbering: "1.1.1.")

    #show table.cell.where(y: 0): strong
    #show table: set par(justify: false)
    #set table(
      stroke: (x, y) => (
        top: if y > 0 { 0.7pt + black },
        left: if x > 0 { 0.7pt + black },
      ),
      align: (x, y) => (
        if x > 0 { center } else { left }
      ),
    )
    #doc
  ]
}

