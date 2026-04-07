#let layout(doc) = {
  [

    #set page(
      numbering: "1",
      header: context {
        if counter(page).get().first() > 1 [
          #let h2s = query(selector(heading.where(level: 2)))
          #let h3s = query(selector(heading.where(level: 3)))

          #let h2 = if h2s.len() > 0 { h2s.last() } else { none }
          #let h3 = if h3s.len() > 0 { h3s.last() } else { none }

          #if counter(page).get().first() > 1 [
            #set text(size: 9pt)

            #h(0pt)
            #if h2 != none { h2.body }
            #h(1fr)
            #if h3 != none { h3.body }

            #block(line(length: 100%, stroke: 0.5pt), above: 0.4em)
          ]
        ]
      },
    )


    #set text(size: 12pt, lang: "de")
    #set par(justify: true)

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

    #let title-block() = {
      /* Make the title */
      columns(3, gutter: 3pt)[
        #align(left + bottom, {
          text()[Software Architektur Labor \ ]
          text()[Prof. Dr. Carsten Sinz]
        })

        #colbreak()

        #align(center + top, {
          text(size: 1.6em, weight: "bold")[Pflichtenheft \ ]
        })

        #colbreak()

        #align(right + bottom, {
          text()[Simon Trinkl \ Moritz Vogel]
        })
      ]

      block(line(length: 100%, stroke: 0.5pt))
    }


    #title-block()
    #doc
  ]
}

