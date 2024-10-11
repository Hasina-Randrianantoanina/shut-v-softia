import CommentaireRenderer from "../../ChampEditable/CommentaireRenderer";
import CommentaireEditor from "../../ChampEditable/CommentaireEditor";
import { formatDateForDisplay } from "@/utils/dateUtils";

const AlerteColumns = (onSaveCommentaireAlerte) => {
  return [
    {
      headerName: "Date",
      field: "dateAlerte",
      width: 180,
      minWidth: 150,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay(params.value),
    },
    // {
    //   headerName: "Initiales",
    //   field: "debug_initiales",
    //   width: 120,
    //   minWidth: 100,
    //   maxWidth: 120,
    //   cellStyle: { fontWeight: "bold" },
    // },
    {
      headerName: "Libellé alerte",
      field: "descriptionAlerte",
      width: 300,
      minWidth: 100,
      maxWidth: 350,
      cellStyle: { fontWeight: "bold" },
    },
    // {
    //   headerName: "Action",
    //   field: "action",
    //   width: 120,
    //   minWidth: 80,
    //   maxWidth: 190,
    // },
    {
      headerName: "Commentaire",
      field: "commentaire",
      flex: 1,
      minWidth: 200,
      editable: true,
      cellRenderer: CommentaireRenderer,
      cellEditor: CommentaireEditor,
      cellEditorParams: { 
        onSave: (commentaire, params) => onSaveCommentaireAlerte(params.data.id, commentaire)
      },
      autoHeight: true,
    }
  ];
};

export default AlerteColumns;
