import { getVersionColor } from "@/utils/colorStaUtils";

export const getColumnDefs = (traitement, isSansTraitement = false) => {
  if (isSansTraitement) {
    return [
      {
        headerName: "Station",
        field: "stationInitiales",
        width: 100,
        headerTooltip: "Initiales de la station",
      },
      {
        headerName: "Voie",
        field: "voieLibelle",
        flex: 1,
        headerTooltip: "Libellé de la voie",
        tooltipField: "voieLibelle",
      },
    ];
  }

  const baseColumns = [
    {
      headerName: "Initiales",
      field: "stationInitiales",
      width: 100,
      minWidth: 100,
      maxWidth: 120,
      headerTooltip: "Initiales de la station",
      cellClass: (params) => {
        const colorClass = getVersionColor(
          params.data.enregistreurVersion,
          params.data.enregistreurLiaison,
          params.data.stationInitiales
        );
        return [
          "font-bold",
          colorClass,
          "text-black",
          "flex",
          "items-center",
          "justify-center",
          "h-full",
        ];
      },
      cellRenderer: (params) => {
        return params.value;
      },
    },
    {
      headerName: "Voie",
      field: "voieLibelle",
      width: 150,
      flex: 1,
      cellStyle: { fontWeight: "bold" },
      headerTooltip: "Libellé de la voie",
      tooltipField: "voieLibelle",
      tooltipShowDelay: 1,
    },
  ];

  const parameterColumns = Array.from({ length: 10 }, (_, i) => {
    const paramName = traitement[`traitementParametre${i + 1}`];
    if (!paramName) return null;
    return {
      headerName: `P${i + 1}) ${paramName}`,
      field: `voieParametre${i + 1}`,
      flex: 1,
      minWidth: 120,
      wrapText: true,
      autoHeight: true,
      headerTooltip: paramName,
      tooltipField: `voieParametre${i + 1}`,
      tooltipShowDelay: 1,
      valueFormatter: (params) => (params.value === "0" ? "" : params.value),
    };
  }).filter(Boolean);

  return [...baseColumns, ...parameterColumns];
};

export const processVoiesTraitements = (voiesTraitements) => {
  const traitementMap = new Map();
  const voiesSansTraitement = [];

  voiesTraitements.forEach((voie) => {
    if (voie.traitementId) {
      if (!traitementMap.has(voie.traitementId)) {
        traitementMap.set(voie.traitementId, {
          nom: voie.traitementNom,
          voies: [],
          parametres: Object.fromEntries(
            Array.from({ length: 10 }, (_, i) => [
              `traitementParametre${i + 1}`,
              voie[`traitementParametre${i + 1}`],
            ])
          ),
        });
      }
      traitementMap.get(voie.traitementId).voies.push(voie);
    } else {
      voiesSansTraitement.push(voie);
    }
  });

  return { traitementMap, voiesSansTraitement };
};