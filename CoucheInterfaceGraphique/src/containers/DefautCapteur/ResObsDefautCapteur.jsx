import React from "react";
import {
  useDefautsActifsCapteurObs,
  useUpdateDefautActif,
} from "@/services/defautsAPI";
import DefCaptColumns from "@/components/TableColumnDefinition/DefautsCapteurs/DefCaptColumns";
import { useConnectedUser } from "@/hooks/useConnectedUser";
import CollapsibleTable from "@/components/CollasableTable/CollapsibleTable";
import { getVersionColor } from "@/utils/colorStaUtils";
import { useAuth } from '@/contexts/AuthContext';

const ResObsDefCapteur = () => {
  const { user } = useAuth();
  const username = useConnectedUser();
  const {
    data: rowData,
    isLoading,
    error,
  } = useDefautsActifsCapteurObs();

  
  const updateDefautActif = useUpdateDefautActif();


  const onSaveCommentaire = (id, commentaire) => {
    updateDefautActif.mutate({
      id,
      commentaire,
      utilisateur: username,
    });
  };

  const columnDefs = DefCaptColumns(onSaveCommentaire);


  return (
    <div className="flex flex-col h-full gap-4 p-4 lg:flex-row">
      <div className="flex-grow" style={{ height: "800px" }}>
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
        />
      </div>
    </div>
  );
};

export default ResObsDefCapteur;
