
const DefColumnSurveillance = () => {
    return [
      {
        headerName: "Ini",
        field: "ini",
        width: 100,
        minWidth: 100,
        maxWidth: 120,
        cellStyle: { fontWeight: "bold" },
      },
      {
        headerName: "Liaison",
        field: "liaison",
        width: 150,
        minWidth: 100,
        maxWidth: 200,
      },
      {
        headerName: "Fin",
        field: "fin",
        width: 150,
        minWidth: 100,
        maxWidth: 200,
        cellStyle: { fontWeight: "bold" },
      },
      {
        headerName: "Etat",
        field: "etat",
        width: 100,
        minWidth: 80,
        maxWidth: 120,
      },
      {
        headerName: "Nbcnx",
        field: "nbcnx",
        width: 180,
        minWidth: 150,
        maxWidth: 220,
      },
      {
        headerName: "Cfg",
        field: "cfg",
        flex: 1,
        width: 180,
        minWidth: 150,
        maxWidth: 220,
      },
      {
        headerName: "DerOp",
        field: "derOp",
        flex: 1,
        /* width: 180,
        minWidth: 150,
        maxWidth: 220, */
      }
    ];
  };
  
  export default DefColumnSurveillance;