export const getSeriesType = (libelle) => {
    const upperLibelle = libelle.toUpperCase();

    const typeChecks = [
      { type: "TOR", check: (l) => l.includes("ETOR") || l.includes("STOR") },
      {
        type: "NIVEAU",
        check: (l) =>
          l.startsWith("Y") || l.includes("CUMUL") || l.includes("POV"),
      },
      { type: "DEBIT", check: (l) => l.startsWith("Q") },
      { type: "TEMPERATURE", check: (l) => l.includes("TEMP") },
      {
        type: "TURBIDITE",
        check: (l) => l.startsWith("T") && !l.startsWith("TEMP"),
      },
      {
        type: "VITESSE",
        check: (l) => l.startsWith("V") && !l.startsWith("VOL"),
      },
      { type: "VOLUME", check: (l) => l.startsWith("VOL") },
      { type: "CONCENTRATION", check: (l) => l.includes("GAZ") },
    ];

    for (const { type, check } of typeChecks) {
      if (check(upperLibelle)) {
        return type;
      }
    }

    return "OTHER";
  };