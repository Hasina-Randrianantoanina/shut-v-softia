import ModifyButton from "@/components/CustomButton/ModifyButton";
import EditButtons from "@/components/CustomButton/EditButtons";
import { getVersionColor } from "@/utils/colorStaUtils";

const centeredColumnStyle = {
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
};

export const mockData = [
  {
    initiales: "AB",
    nom: "APPROF BLANC_MESNIL",
    admin: false,
    operateur: false,
    valideur: true,
    consultation: true,
  },
  {
    initiales: "AC",
    nom: "ALBERT CAMUS",
    admin: false,
    operateur: true,
    valideur: false,
    consultation: true,
  },
  {
    initiales: "AF",
    nom: "ANATOLE FRANCE",
    admin: false,
    operateur: false,
    valideur: true,
    consultation: true,
  },
  {
    initiales: "AN",
    nom: "ALIM COLLECTEUR DU NORD",
    admin: false,
    operateur: true,
    valideur: false,
    consultation: true,
  },
];
const PreselectionColumns = () => {
  return [
    {
      headerName: "Initiales",
      field: "initiales",
      cellStyle: { fontWeight: "bold" },
      cellEditor: "agTextCellEditor",
      editable: true,
      width: 100,
      cellStyle: { textAlign: "center", fontWeight: "bold" },
    },
    {
      headerName: "Nom",
      field: "nom",
      cellEditor: "agTextCellEditor",
      editable: true,
    },
    {
      headerName: "PLUVIO",
      field: "pluvio",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      cellStyle: centeredColumnStyle,
      headerClass: "ag-center-header",
      cellRendererParams: {
        checkbox: true,
      },
      cellEditorParams: {
        useFormatter: true,
      },
      flex: 1,
    },
    {
      headerName: "SSRMN",
      field: "ssrmn",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      cellStyle: centeredColumnStyle,
      headerClass: "ag-center-header",

      cellRendererParams: {
        checkbox: true,
      },
      cellEditorParams: {
        useFormatter: true,
      },
      flex: 1,
    },
  ];
};

export default PreselectionColumns;
