import CommentaireRenderer from "../../ChampEditable/CommentaireRenderer";
import CommentaireEditor from "../../ChampEditable/CommentaireEditor";
import { getVersionColor } from "@/utils/colorStaUtils";
import { formatDateForDisplay } from "@/utils/dateUtils";

const DefCaptColumnDefinition = (onSaveCommentaire) => {
  return [
    {
      headerName: "Initiales",
      field: "stationInitiales",
      width: 100,
      minWidth: 100,
      maxWidth: 100,
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
      minWidth: 180,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay (params.value),
      sort: "desc",
    },
    {
      headerName: "Libellé défaut (Voie)",
      field: "voieLibelle",
      width: 190,
      minWidth: 190,
      maxWidth: 220,
      cellStyle: { fontWeight: "bold" },
    },
    {
      headerName: "Priorité",
      field: "voiePriorite",
      width: 100,
      minWidth: 100,
      maxWidth: 120,
    },
    {
      headerName: "Type",
      field: "type",
      width: 120,
      minWidth: 120,
      maxWidth: 150,
    },
    {
      headerName: "Dernier Transfert",
      field: "stationDernierTransfert",
      width: 180,
      minWidth: 180,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay (params.value),
    },
    {
      headerName: "Commentaire",
      field: "commentaire",
      flex: 1,
      minWidth: 200,
      editable: true,
      cellRenderer: CommentaireRenderer,
      cellEditor: CommentaireEditor,
      cellEditorParams: {
        onSave: (commentaire, params) =>
          onSaveCommentaire(params.data.id, commentaire),
      },
      autoHeight: true,
    },
  ];
};

export default DefCaptColumnDefinition;
