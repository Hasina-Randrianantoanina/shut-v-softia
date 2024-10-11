import CommentaireRenderer from "../../ChampEditable/CommentaireRenderer";
import HeureEditor from "../../ChampEditable/HeureEditor";

const CyclesAppelsUsageColumnDefinition = () => {
  return [
    {
      headerName: "Heure",
      field: "heure",
      width: 140,
      minWidth: 100,
      maxWidth: 200,
      editable: true,
      cellRenderer: CommentaireRenderer,
      cellEditor: HeureEditor,
      autoHeight: true,
      cellStyle: { fontWeight: "bold" },
    },

    {
      headerName: "Liaison RC",
      field: "grp_rc",
      width: 150,
      minWidth: 100,
      maxWidth: 200,
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
    },

    {
      headerName: "Liaison IP",
      field: "grp_ip",
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      flex: 1,
    },
  ];
};

export default CyclesAppelsUsageColumnDefinition;
