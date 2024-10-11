import { getVersionColor } from "@/utils/colorStaUtils";
import { formatDateForDisplay } from "@/utils/dateUtils";

const StationColumnDefinition = () => {
  return [
    {
      headerName: "Initiales",
      field: "initiales",
      width: 140,
      minWidth: 100,
      maxWidth: 170,
      cellStyle: { fontWeight: "bold" },
      checkboxSelection: true,
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
      headerName: "Nom Station",
      field: "nom",
      width: 200,
      minWidth: 150,
      maxWidth: 250,
      cellStyle: { fontWeight: "bold" },
    },
    {
      headerName: "Dernier appel",
      field: "dernierAppelDate",
      width: 180,
      minWidth: 100,
      maxWidth: 200,
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
    {
      headerName: "Dernier Enregistrement",
      field: "dernierEnregistrement",
      width: 210,
      minWidth: 100,
      maxWidth: 250,
      valueFormatter: (params) => formatDateForDisplay(params.value),
      
    },
    {
      headerName: "Transfert",
      field: "dernierTransfert",
      width: 180,
      minWidth: 100,
      maxWidth: 200,
      cellStyle: (params) => {
        return params.data.defautPing ? { color: 'red' } : null;
      },
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
    {
      headerName: "Défaut État",
      field: "defautEtat",
      width: 130,
      minWidth: 120,
      maxWidth: 150,
      cellRenderer: (params) => 
        params.value ? 
          <div style={{display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%'}}>
            <span style={{color: 'red', fontSize: '1.2em'}}>✓</span>
          </div> : 
          null,
    },
    {
      headerName: "Défaut Capteur",
      field: "defautCapteur",
      width: 150,
      minWidth: 120,
      maxWidth: 200,
      cellRenderer: (params) => 
        params.value ? 
          <div style={{display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%'}}>
            <span style={{color: 'red', fontSize: '1.2em'}}>✓</span>
          </div> : 
          null,
    },
    // {
    //   headerName: "Défaut Ping",
    //   field: "defautPing",
    //   width: 130,
    //   minWidth: 120,
    //   maxWidth: 150,
    //   cellRenderer: (params) => params.value ? '✓' : '✗',
    // },
    // {
    //   headerName: "Autre Défaut",
    //   field: "defautAutre",
    //   width: 130,
    //   minWidth: 120,
    //   maxWidth: 150,
    //   cellRenderer: (params) => params.value ? '✓' : '✗',
    // },
    {
      headerName: "Appel",
      field: "statut",
      width: 150,
      minWidth: 100,
      maxWidth: 200,
    },
    {
      headerName: "Mise à jour",
      field: "dateMaj",
      width: 180,
      minWidth: 100,
      maxWidth: 200,
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
    {
      headerName: "Sauv.",
      field: "sauvegarde",
      // width: 120,
      minWidth: 50,
      // maxWidth: 140,
    },
    { headerName: "%Mém.", field: "pourcentageMemoire", flex: 1, minWidth: 100 },
  ];


};

export default StationColumnDefinition;