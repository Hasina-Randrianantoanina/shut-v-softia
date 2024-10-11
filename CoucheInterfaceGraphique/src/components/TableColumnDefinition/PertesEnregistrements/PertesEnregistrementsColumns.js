import React from "react";
import ActionButtons from "@/components/CustomButton/ActionButtons";
import { formatDateForDisplay, formatDurationForDisplay } from "@/utils/dateUtils";

const PertesEnregistrementsColumns = ({ onDelete, onEdit }) => {
  return [
    {
      headerName: "ID",
      field: "id",
      hide: true,
    },
    {
      headerName: "Actions",
      width: 100,
      cellRenderer: (params) => (
        <ActionButtons
          onDelete={() => onDelete(params.data)}
          onEdit={() => onEdit(params.data)}
        />
      ),
    },
    {
      headerName: "Début",
      field: "debut",
      width: 180,
      minWidth: 120,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay (params.value),
    },
    {
      headerName: "Fin",
      field: "fin",
      width: 180,
      minWidth: 120,
      maxWidth: 220,
      valueFormatter: (params) => formatDateForDisplay (params.value),
      sort: "desc",
    },
    {
      headerName: "Cause",
      field: "cause",
      width: 250,
      minWidth: 100,
      maxWidth: 350,
      cellStyle: { fontWeight: "bold" },
    },
    {
      headerName: "Défaut",
      field: "defaut",
      width: 170,
      minWidth: 150,
      maxWidth: 300,
    },
    {
      headerName: "Remède",
      field: "remede",
      width: 200,
      minWidth: 150,
      maxWidth: 300,
    },
    {
      headerName: "Durée",
      field: "duree",
      width: 150,
      valueFormatter: (params) => formatDurationForDisplay(params.value),
    },
    {
      headerName: "Critique",
      field: "critique",
      width: 120,
      cellRenderer: (params) => (params.value ? "Oui" : "Non"),
    },
    {
      headerName: "Commentaire",
      field: "commentaire",
      flex: 1,
    },
  ];
};

export default PertesEnregistrementsColumns;
