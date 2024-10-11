const AbtsVoieColumns = () => {
  return [
    {
      headerName: "Voie",
      field: "voieLibelle",
      width: 190,
      minWidth: 100,
      maxWidth: 220,
      cellStyle: { fontWeight: "bold" },
      sort: 'desc',
      sortingOrder: ['asc', 'desc'] 
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
      flex : 1
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
      flex : 1
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
      flex : 1
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
      flex : 1
    },
  ];
};

export default AbtsVoieColumns;