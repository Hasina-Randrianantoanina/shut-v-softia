import React from "react";
import {
  useDefautsActifsCapteurUsage,
  useUpdateDefautActif,
  useDefautsCapteurLast24Hours,
} from "@/services/defautsAPI";
import DefCaptColumns from "@/components/TableColumnDefinition/DefautsCapteurs/DefCaptColumns";
import { useConnectedUser } from "@/hooks/useConnectedUser";
import CollapsibleTable from "@/components/CollasableTable/CollapsibleTable";
import { getVersionColor } from "@/utils/colorStaUtils";
import { useAuth } from '@/contexts/AuthContext';

const ResUsageDefCapteur = () => {
  const { user } = useAuth();
  const username = useConnectedUser();
  const {
    data: rowData,
    isLoading,
    error,
  } = useDefautsActifsCapteurUsage();
  const {
    data: last24HoursData,
    isLoading: isLast24HoursLoading,
    error: last24HoursError,
  } = useDefautsCapteurLast24Hours();
  
  const updateDefautActif = useUpdateDefautActif();


  const onSaveCommentaire = (id, commentaire) => {
    updateDefautActif.mutate({
      id,
      commentaire,
      utilisateur: username,
    });
  };

  const columnDefs = DefCaptColumns(onSaveCommentaire);

  const last24HoursColumnDefs = [
    {
      headerName: "Initiales",
      field: "stationInitiales",
      sortable: true,
      width: 100,
      minWidth: 70,
      maxWidth: 120,
      cellClass: (params) => {
        const colorClass = getVersionColor(
          params.data.enregistreurVersion,
          params.data.enregistreurLiaison,
          params.data.stationInitiales
        );
        return [
          "font-bold",
          colorClass,
          "text-black",
          "flex",
          "items-center",
          "justify-center",
          "h-full",
        ];
      },
      cellRenderer: (params) => {
        return params.value;
      },
    },
    { headerName: "Voie", field: "voieLibelle", sortable: true, filter: true, cellStyle: { fontWeight: "bold" }, },
    {
      headerName: "Occurrences",
      field: "nombreOccurrences",
      sortable: true,
      filter: true,
      flex: 1,
    },
  ];

  if (isLoading || isLast24HoursLoading) return <div>Chargement...</div>;
  if (error || last24HoursError)
    return (
      <div>
        Une erreur est survenue: {error?.message || last24HoursError?.message}
      </div>
    );

  return (
    <div className="flex flex-col h-full gap-4 p-4 lg:flex-row">
      <div className="flex-grow lg:w-2/3" style={{ height: "800px" }}>
        <CollapsibleTable
          title="Défauts Actifs"
          columnDefs={columnDefs}
          rowData={rowData}
          gridProps={{
            domLayout: "normal",
            suppressRowTransform: true,
            stopEditingWhenCellsLoseFocus: false,
            suppressClickEdit: true,
            rowHeight: 80,
            suppressCellFocus: true,
            enableCellTextSelection: true,
          }}
          overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
        />
      </div>
      <div className="w-full lg:w-1/3" style={{ height: "800px" }}>
        <CollapsibleTable
          title="Apparitions / Disparitions (24h)"
          columnDefs={last24HoursColumnDefs}
          rowData={last24HoursData}
          gridProps={{
            domLayout: "normal",
            suppressRowTransform: true,
            stopEditingWhenCellsLoseFocus: false,
            suppressClickEdit: true,
            rowHeight: 40,
            suppressCellFocus: true,
            enableCellTextSelection: true,
          }}
        />
      </div>
    </div>
  );
};

export default ResUsageDefCapteur;
