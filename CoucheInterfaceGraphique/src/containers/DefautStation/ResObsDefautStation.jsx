import React, { useMemo } from "react";
import CollapsibleTable from "@/components/CollasableTable/CollapsibleTable";
import AlerteColumns from "@/components/TableColumnDefinition/DefautsStations/AlerteColumns";
import DefPingColumns from "@/components/TableColumnDefinition/DefautsStations/DefPingColumns";
import DefCaptColumns from "@/components/TableColumnDefinition/DefautsCapteurs/DefCaptColumns";
import DefEtatColumns from "@/components/TableColumnDefinition/DefautsStations/DefEtatColumns";
import DefAutreColumns from "@/components/TableColumnDefinition/DefautsStations/AutreDefautColumns";
import CustomButton from "@/components/CustomButton/CustomButton";
import { useConnectedUser } from "@/hooks/useConnectedUser";
import {
  useDefautsActifsCapteurObs,
  useUpdateDefautActif,
  useDefautsActifsEtatObs,
  useDefautsActifsPingObs,
  useDefautsActifsAutreObs,
} from "@/services/defautsAPI";
import { useAlertesObs, useUpdateAlerteCommentaire } from "@/services/alertesAPI";

const ResObsDefStation = () => {
  const username = useConnectedUser();

  //Param tableau Alerte
  const { data: rowDataAlertes, isLoadingAlerte, errorAlerte } = useAlertesObs();
  const updateAlerteCommentaire = useUpdateAlerteCommentaire();

  const onSaveCommentaireAlerte = (id, commentaire) => {
    updateAlerteCommentaire.mutate({
      id,
      commentaire,
      utilisateur: username,
    });
  };

  const columnAlerte = AlerteColumns(onSaveCommentaireAlerte);

  //Param tableau défauts capteur
  const {
    data: rowDataCapteur,
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

  const columnDefsCapteur = DefCaptColumns(onSaveCommentaire).filter(
    (col) => col.field !== "voiePriorite" && col.field !== "type"
  );

  //Param tableau etats station
  const {
    data: rowDataDefsEtat,
    isLoadingEtat,
    errorStation,
  } = useDefautsActifsEtatObs();
  const columnDefsEtat = DefEtatColumns();

  //Param tableau defauts données
  const {
    data: rowDataDefsPing,
    isLoadingPing,
    errorPing,
  } = useDefautsActifsPingObs();
  const columnDefsPing = DefPingColumns();

  //Param tableau autres défauts
  const {
    data: rowDataDefsAutre,
    isLoadingAutre,
    errorAutre,
  } = useDefautsActifsAutreObs();
  const columnDefsAutre = DefAutreColumns();

  const extraContentDefautDonnees = (
    <div className="p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
      <div className="flex items-center space-x-3">
        <CustomButton
          onClick={() => console.log("Appeler les stations présélectionnées")}
        >
          Appeler les stations sélectionnées
        </CustomButton>
      </div>
    </div>
  );

  return (
    <div className="flex flex-col p-4 space-y-6">
      <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
        <CollapsibleTable
          title="Défaut de données"
          columnDefs={columnDefsPing}
          rowData={rowDataDefsPing}
          extraContent={extraContentDefautDonnees}
        />
        <CollapsibleTable
          title="Alertes"
          columnDefs={columnAlerte}
          rowData={rowDataAlertes}
        />
        <CollapsibleTable
          title="Défauts capteur"
          columnDefs={columnDefsCapteur}
          rowData={rowDataCapteur}
        />

        {/* <CollapsibleTable
          title="Défauts Etat"
          columnDefs={columnDefsEtat}
          rowData={rowDataDefsEtat}
        /> */}

        <CollapsibleTable
          title="Autres Défauts"
          columnDefs={columnDefsAutre}
          rowData={rowDataDefsAutre}
        />
      </div>
    </div>
  );
};

export default ResObsDefStation;
