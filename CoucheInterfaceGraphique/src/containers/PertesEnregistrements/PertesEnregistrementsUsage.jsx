import React, { useState, useMemo, useCallback } from "react";
import StationSelectorContainer from "@/containers/BandeauSelectionStations/StationSelectorContainer";
import DatePickerContainer from "@/containers/PertesEnregistrements/DatePickerContainer";
import TablePertesEnregistrements from "@/containers/PertesEnregistrements/TablePertesEnregistrement";
import EditPertesEnregistrementPopup from "@/containers/PertesEnregistrements/EditPertesEnregistrementPopup";
import WarningPopup from "@/components/Alert/WarningPopup";
import {
  usePertesByStation,
  useUpdatePerte,
  useInsertPerte,
  useDeletePerte,
} from "@/services/pertesAPI";
import { Taches } from "@/components/PerteEnregistrementsForms/EnumCauseDefautRemedeTache";


const PertesEnregistrementsUsage = () => {
  const [selectedStation, setSelectedStation] = useState(null);
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [editingPerte, setEditingPerte] = useState(null);
  const [showLast12Months, setShowLast12Months] = useState(false);
  const [isWarningPopupOpen, setIsWarningPopupOpen] = useState(false);
  const [perteToDelete, setPerteToDelete] = useState(null);

  const { data, isLoading, error } = usePertesByStation(selectedStation?.id);
  const pertes = data?.pertes || [];
  const enregistreurInfo = data?.enregistreurInfo;

  const filteredPertes = useMemo(() => {
    if (!showLast12Months) return pertes;
    const twelveMonthsAgo = new Date();
    twelveMonthsAgo.setMonth(twelveMonthsAgo.getMonth() - 12);
    return pertes.filter((perte) => new Date(perte.debut) >= twelveMonthsAgo);
  }, [pertes, showLast12Months]);

  const combinedEtatEnregistreur = useMemo(() => {
    if (!enregistreurInfo || !editingPerte) {
      return null;
    }
    return {
      version: enregistreurInfo.version,
      dates: {
        stop: editingPerte.stop,
        derTrf: enregistreurInfo.dernierTransfert,
        horl: editingPerte.horloge,
        diffHorl: editingPerte.diffHorloge,
        go: editingPerte.dateGo,
        stop: editingPerte.dateStop,
        init: editingPerte.dateInit,
        acq: editingPerte.dateAcquisition,
        enrg: editingPerte.dateEnregistrement,
      },
      taches: Taches.taches,
      liaison: enregistreurInfo.liaison,
      dernierAppel: enregistreurInfo.dernierAppel,
      dernierEnregistrement: enregistreurInfo.dernierEnregistrement,
      miseAJour: enregistreurInfo.miseAJour,
    };
  }, [enregistreurInfo, editingPerte]);

  const updatePerteMutation = useUpdatePerte();
  const insertPerteMutation = useInsertPerte();

  const handleStationSelect = useCallback((station) => {
    setSelectedStation(station);
  }, []);

  const handleAddClick = useCallback(() => {
    setEditingPerte(null);
    setIsPopupOpen(true);
  }, []);

  const handleClosePopup = useCallback(() => {
    setIsPopupOpen(false);
    setEditingPerte(null);
  }, []);

  const handleSave = useCallback(
    async (item) => {
      try {
        if (editingPerte) {
          await updatePerteMutation.mutateAsync({
            ...editingPerte,
            ...item,
            stationId: selectedStation.id,
          });
        } else {
          await insertPerteMutation.mutateAsync({
            ...item,
            stationId: selectedStation.id,
          });
        }
        handleClosePopup();
      } catch (error) {
        console.error("Error saving perte:", error);
      }
    },
    [
      editingPerte,
      selectedStation,
      updatePerteMutation,
      insertPerteMutation,
      handleClosePopup,
    ]
  );

  const handleEditClick = useCallback((perte) => {
    setEditingPerte(perte);
    setIsPopupOpen(true);
  }, []);

  const handleToggleLast12Months = useCallback((value) => {
    setShowLast12Months(value);
  }, []);

  const deletePerteMutation = useDeletePerte();

  const handleDeleteClick = useCallback((perte) => {
    setPerteToDelete(perte);
    setIsWarningPopupOpen(true);
  }, []);

  const handleConfirmDelete = useCallback(async () => {
    if (perteToDelete) {
      try {
        await deletePerteMutation.mutateAsync(perteToDelete.id);
        setIsWarningPopupOpen(false);
        setPerteToDelete(null);
      } catch (error) {
        console.error("Error deleting perte:", error);
      }
    }
  }, [perteToDelete, deletePerteMutation]);

  return (
    <div className="flex flex-col p-4">
      <StationSelectorContainer
        onStationSelect={handleStationSelect}
        reseau="USAGE"
      />
      <div className="border border-atoli_blue rounded-xl">
        <h1 className="p-4 text-2xl font-bold">Les pertes d'enregistrements</h1>
        <DatePickerContainer
          selectedStation={selectedStation?.initiales}
          onAddClick={handleAddClick}
          showLast12Months={showLast12Months}
          onToggleLast12Months={handleToggleLast12Months}
          enregistreurInfo={enregistreurInfo}
        />
        {isLoading ? (
          <div>Chargement des pertes...</div>
        ) : (
          <TablePertesEnregistrements
            rowData={filteredPertes}
            onEditRow={handleEditClick}
            onDeleteRow={handleDeleteClick}
          />
        )}
      </div>
      <EditPertesEnregistrementPopup
        isOpen={isPopupOpen}
        onClose={handleClosePopup}
        onSave={handleSave}
        initialData={editingPerte || {}}
        isAddMode={!editingPerte}
        etatEnregistreur={combinedEtatEnregistreur}
        selectedPerte={editingPerte}
      />
      <WarningPopup
        isOpen={isWarningPopupOpen}
        onClose={() => setIsWarningPopupOpen(false)}
        onConfirm={handleConfirmDelete}
        message="Êtes-vous sûr de vouloir supprimer cette perte ?"
      />
    </div>
  );
};

export default PertesEnregistrementsUsage;
