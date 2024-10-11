import { getVersionColor } from "@/utils/colorStaUtils";
import { formatDateForDisplay } from "@/utils/dateUtils";

const DefStationColumns = () => {
  return [
    {
      headerName: "Initiales",
      field: "stationInitiales",
      checkboxSelection: true,
      width: 100,
      minWidth: 100,
      maxWidth: 120,
      cellClass: (params) => {
        const colorClass = getVersionColor(
          params.data.enregistreurVersion,
          params.data.enregistreurLiaison,
          params.data.stationInitiales
        );
        return ['font-bold', colorClass, 'text-black', 'flex', 'items-center', 'justify-center', 'h-full', 'mr-2'];
      },
      cellRenderer: (params) => {
        return params.value;
      },
    },
    {
      headerName: "Dernier Transfert",
      field: "stationDernierTransfert",
      width: 180,
      minWidth: 180,
      maxWidth: 200,
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
    {
      headerName: "Libellé défaut",
      field: "descriptionDefaut",
      width: 300,
      minWidth: 200,
      maxWidth: 350,
      cellStyle: { fontWeight: "bold" },
    },
    {
      headerName: "Dernier Appel",
      field: "dernierAppelDate",
      width: 180,
      minWidth: 180,
      valueFormatter: (params) => formatDateForDisplay(params.value),
      flex:1
    },
  ];
};

export default DefStationColumns;
