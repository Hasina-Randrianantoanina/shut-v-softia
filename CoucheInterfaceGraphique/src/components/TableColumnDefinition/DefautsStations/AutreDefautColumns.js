import { getVersionColor } from "@/utils/colorStaUtils";
import { formatDateForDisplay } from "@/utils/dateUtils";

const AutreDefautColumns = () => {
  return [
    {
      headerName: "Initiales",
      field: "stationInitiales",
      width: 100,
      minWidth: 100,
      maxWidth: 100,
      cellClass: (params) => {
        const colorClass = getVersionColor(
          params.data.enregistreurVersion,
          params.data.enregistreurLiaison,
          params.data.stationInitiales
        );
        return ['font-bold', colorClass, 'text-black', 'flex', 'items-center', 'justify-center', 'h-full'];
      },
      cellRenderer: (params) => {
        return params.value;
      },
    },
    {
      headerName: "Date défaut",
      field: "appel",
      width: 180,
      minWidth: 180,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay(params.value),
      sort: "desc",
    },
    {
        headerName: "Libellé",
        field: "descriptionDefaut",
        width: 350,
        minWidth: 190,
        maxWidth: 400,
        cellStyle: { fontWeight: "bold" },
      },
  
    {
      headerName: "Statut",
      field: "dernierAppelStatut",
      width: 150,
      minWidth: 150,
      maxWidth: 220,
    },

    {
      headerName: "Maj.",
      field: "stationDateMaj",
      width: 180,
      minWidth: 180,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
    {
      headerName: "Sauv.",
      field: "sauvegarde",
      width: 190,
      minWidth: 100,
      // maxWidth: 220,
      flex: 1,
    },
  ];
};

export default AutreDefautColumns;
