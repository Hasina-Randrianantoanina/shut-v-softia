import { getVersionColor } from "@/utils/colorStaUtils";
import { formatDateForDisplay } from "@/utils/dateUtils";

const DefEtatColumns = () => {
  return [
    {
      headerName: "Initiales",
      field: "stationInitiales",
      width: 100,
      minWidth: 100,
      maxWidth: 120,
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
      minWidth: 100,
      maxWidth: 200,
      valueFormatter: (params) => formatDateForDisplay(params.value),
      sort: "desc",
    },
    {
      headerName: "Libellé défaut (Voie)",
      field: "voieLibelle",
      width: 190,
      minWidth: 100,
      maxWidth: 220,
      cellStyle: { fontWeight: "bold" },
    },
    {
      headerName: "Dernier Transfert",
      field: "stationDernierTransfert",
      flex : 1,
      // width: 180,
      // minWidth: 150,
      // maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
  ];
};

export default DefEtatColumns;
