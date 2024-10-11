import ModifyButton from "@/components/CustomButton/ModifyButton";
import EditButtons from "@/components/CustomButton/EditButtons";
import { getVersionColor } from "@/utils/colorStaUtils";

const StationsUsageColumns = (
  onModify,
  onSave,
  onCancel,
  editingRowId,
  availableNumbers,
  gridApi
) => {
  const liaisonOptions = ["IP", "AP"];
  const bassinVersantOptions = ["", "MOREE", "SEINE", "UNITAIRE"];
  const meteoOptions = ["", "temps de pluie", "temps sec", "mixte"];

  const isEditing = (params) => params.data.id.toString() === editingRowId;

  return [
    {
      headerName: "Actions",
      field: "actions",
      width: 120,
      cellRenderer: (params) => {
        const isEditing = params.data.id.toString() === editingRowId;
        if (isEditing) {
          return (
            <EditButtons
              onSave={() => onSave(params.data.id)}
              onCancel={() => onCancel(params.data.id)}
            />
          );
        }
        return <ModifyButton onClick={() => onModify(params.data.id)} />;
      },
      pinned: "center",
    },
    {
      headerName: "Initiales",
      field: "initiales",
      width: 100,
      minWidth: 80,
      maxWidth: 120,
      cellStyle: { fontWeight: "bold" },
      cellEditor: "agTextCellEditor",
      editable: true,
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
      width: 200,
      minWidth: 100,
      maxWidth: 220,
      cellEditor: "agTextCellEditor",
      editable: true,
    },
    {
      headerName: "N°",
      field: "numero",
      width: 100,
      minWidth: 80,
      maxWidth: 120,
      editable: true,
      cellEditor: "agSelectCellEditor",
      cellEditorParams: (params) => {
        // Inclure le numéro actuel de la station dans la liste des options
        const currentNumber = params.data.numero;
        const allNumbers = [
          ...new Set([currentNumber, ...availableNumbers]),
        ].sort((a, b) => a - b);
        return { values: allNumbers };
      },
    },
    {
      headerName: "Active",
      field: "actif",
      width: 100,
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: isEditing,
      cellStyle: { textAlign: "center" },
      cellRendererParams: {
        checkbox: true,
      },
      cellEditorParams: {
        useFormatter: true,
      },
      valueGetter: (params) => !!params.data.actif,
      valueSetter: (params) => {
        params.data.actif = params.newValue;
        return true;
      },
    },
    {
      headerName: "Liaison",
      field: "liaison",
      width: 100,
      minWidth: 80,
      maxWidth: 120,
      cellEditor: "agSelectCellEditor",
      cellEditorParams: {
        values: liaisonOptions,
      },
    },
    {
      headerName: "Vidage IP",
      field: "vidageip",
      width: 130,
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      cellStyle: { textAlign: "center" },
      cellRendererParams: {
        checkbox: true,
      },
      cellEditorParams: {
        useFormatter: true,
      },
    },
    {
      headerName: "Version Enregistreur",
      field: "enregistreurVersion",
      width: 190,
      minWidth: 80,
      maxWidth: 220,
      cellEditor: "agTextCellEditor",
      editable: true,
    },
    {
      headerName: "Adresse IP",
      field: "adresseIp",
      width: 140,
      minWidth: 80,
      maxWidth: 220,
      editable: true,
      cellEditor: "agTextCellEditor",
    },
    {
      headerName: "% Mémoire",
      field: "pourcentageMemoire",
      width: 140,
      minWidth: 80,
      maxWidth: 220,
      editable: false,
      cellEditor: "agTextCellEditor",
    },
    {
      headerName: "Type Heure",
      field: "typeHeure",
      width: 140,
      minWidth: 80,
      maxWidth: 220,
      editable: false,
      cellEditor: "agTextCellEditor",
    },
    {
      headerName: "Bassin Versant",
      field: "bassinVersant",
      width: 150,
      minWidth: 80,
      maxWidth: 170,
      cellEditor: "agSelectCellEditor",
      cellEditorParams: {
        values: bassinVersantOptions,
      },
      editable: true,
    },
    {
      headerName: "Météo",
      field: "meteo",
      cellEditor: "agSelectCellEditor",
      cellEditorParams: {
        values: meteoOptions,
      },
      editable: true,
      flex: 1,
    },
  ];
};

export default StationsUsageColumns;
