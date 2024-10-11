import { getVersionColor } from "@/utils/colorStaUtils";

const AbtsStationColumns = () => {
  return [
    {
      headerName: "Initiales",
      field: "initiales",
      cellStyle: { fontWeight: "bold" },
      width: 100,
      cellStyle: { textAlign: "center", fontWeight: "bold" },
      cellClass: (params) => {
        const colorClass = getVersionColor(
          params.data.version,
          params.data.liaison,
          params.data.initiales
        );
        return ['font-bold', colorClass, 'text-black', 'flex', 'items-center', 'justify-center', 'h-full', 'mr-2'];
      },
      cellRenderer: (params) => {
        return params.value;
      },
    },
    {
      headerName: "Nom",
      field: "nom",
    },
    {
      headerName: "ADMIN",
      field: "ADMIN",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      width: 100,
      cellStyle: { textAlign: "center" },
      headerClass: "ag-center-header",
      flex :1,
    },
    {
      headerName: "OPERATEUR",
      field: "OPERATEUR",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      width: 100,
      cellStyle: { textAlign: "center" },
      headerClass: "ag-center-header",
      flex :1,
    },
    {
      headerName: "VALIDEUR",
      field: "VALIDEUR",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      width: 100,
      cellStyle: { textAlign: "center" },
      headerClass: "ag-center-header",
      flex :1,
    },
    {
      headerName: "CONSULTATION",
      field: "CONSULTATION",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      width: 100,
      cellStyle: { textAlign: "center" },
      headerClass: "ag-center-header",
      flex :1,
    },
  ];
};

export default AbtsStationColumns;