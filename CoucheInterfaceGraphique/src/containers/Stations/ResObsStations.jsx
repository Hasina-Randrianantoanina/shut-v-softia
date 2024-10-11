import React, {
  useState,
  useCallback,
  useRef,
  useEffect,
  useMemo,
} from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import { FaSync } from "react-icons/fa";
import {
  useStation,
  useUpdateStation,
  useAvailableStationNumbers,
} from "@/services/stationsAPI";
import StationsObsColumns from "@/components/TableColumnDefinition/Stations/StationsColumns";
import CustomButton from "@/components/CustomButton/CustomButton";
import AddStationPopup from "@/containers/Stations/AddStationPopup";
import Alert from "@/components/Alert/Alert";

const ResObsStations = () => {
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const { data: stations, isLoading, isError, refetch } = useStation();
  const [editingRowId, setEditingRowId] = useState(null);
  const [errorMessage, setErrorMessage] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const [localRowData, setLocalRowData] = useState([]);
  const gridRef = useRef(null);
  const updateStationMutation = useUpdateStation();
  const { data: availableNumbers } = useAvailableStationNumbers();

  useEffect(() => {
    if (stations) {
      setLocalRowData(
        stations.filter((station) => station.reseau === "OBSERVATION")
      );
    }
  }, [stations]);

  const onModify = useCallback((id) => {
    setEditingRowId(id);
    if (gridRef.current && gridRef.current.api) {
      const rowNode = gridRef.current.api.getRowNode(id);
      if (rowNode) {
        gridRef.current.api.startEditingCell({
          rowIndex: rowNode.rowIndex,
          colKey: "initiales",
        });
      }
    }
  }, []);

  const onSave = useCallback(
    async (id) => {
      if (gridRef.current && gridRef.current.api) {
        gridRef.current.api.stopEditing();
        const updatedRow = localRowData.find(
          (row) => row.id.toString() === id.toString()
        );
        if (updatedRow) {
          try {
            await updateStationMutation.mutateAsync({
              station: {
                id: updatedRow.id,
                initiales: updatedRow.initiales,
                nom: updatedRow.nom,
                numero: updatedRow.numero,
                bassinVersant: updatedRow.bassinVersant,
                actif: updatedRow.actif,
                enregistreurId: updatedRow.enregistreurId,
                reseau: updatedRow.reseau,
              },
              enregistreur: {
                id: updatedRow.enregistreurId,
                adresseIp: updatedRow.adresseIp,
                liaison: updatedRow.liaison,
                version: updatedRow.enregistreurVersion,
              },
            });

            setEditingRowId(null);
            setErrorMessage(null);
            setSuccessMessage("Station mise à jour avec succès");

            setLocalRowData((prevData) =>
              prevData.map((row) =>
                row.id === updatedRow.id ? { ...updatedRow } : row
              )
            );
            gridRef.current.api.refreshCells({ force: true });
          } catch (error) {
            console.error("Erreur lors de la mise à jour:", error);
            setErrorMessage(
              error.message ||
                "Une erreur s'est produite lors de la mise à jour"
            );

            const freshStations = await refetch();
            setLocalRowData(
              freshStations.data.filter(
                (station) => station.reseau === "OBSERVATION"
              )
            );
            gridRef.current.api.setRowData(
              freshStations.data.filter(
                (station) => station.reseau === "OBSERVATION"
              )
            );

            setTimeout(() => setErrorMessage(null), 5000);
            setTimeout(() => setSuccessMessage(null), 5000);
          }
        }
      }
    },
    [localRowData, updateStationMutation, refetch]
  );

  const onCancel = useCallback(() => {
    if (gridRef.current && gridRef.current.api) {
      gridRef.current.api.stopEditing(true);
      setEditingRowId(null);
      setErrorMessage(null);
      setSuccessMessage(null);

      if (editingRowId) {
        const originalRow = stations.find(
          (station) => station.id.toString() === editingRowId
        );
        if (originalRow) {
          setLocalRowData((prevData) =>
            prevData.map((row) =>
              row.id.toString() === editingRowId ? { ...originalRow } : row
            )
          );
          gridRef.current.api.applyTransaction({
            update: [{ ...originalRow }],
          });
        }
      }
    }
  }, [stations, editingRowId]);

  const columnDefs = useMemo(
    () =>
      StationsObsColumns(
        onModify,
        onSave,
        onCancel,
        editingRowId,
        availableNumbers,
        gridRef.current?.api
      ),
    [
      onModify,
      onSave,
      onCancel,
      editingRowId,
      availableNumbers,
      gridRef.current?.api,
    ]
  );

  const forceRefresh = useCallback(async () => {
    try {
      const freshStations = await refetch();
      setLocalRowData(
        freshStations.data.filter((station) => station.reseau === "OBSERVATION")
      );
      if (gridRef.current && gridRef.current.api) {
        gridRef.current.api.setRowData(
          freshStations.data.filter(
            (station) => station.reseau === "OBSERVATION"
          )
        );
      }
    } catch (error) {
      console.error("Erreur lors de l'actualisation des données:", error);
      setErrorMessage(
        "Erreur lors de l'actualisation des données. Veuillez réessayer."
      );
    }
  }, [refetch]);

  if (isLoading) return <div>Chargement...</div>;
  if (isError) return <div>Une erreur s'est produite</div>;

  return (
    <div className="flex flex-col h-full p-4">
      <div className="flex-shrink-0 p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
        <div className="flex items-center space-x-3">
          <CustomButton onClick={() => setIsAddModalOpen(true)}>
            Ajouter une station
          </CustomButton>
          <CustomButton onClick={forceRefresh} variant="green">
            <FaSync />
          </CustomButton>
          <AddStationPopup
            isOpen={isAddModalOpen}
            onClose={() => setIsAddModalOpen(false)}
            reseauType="OBSERVATION"
          />
        </div>
      </div>

      {errorMessage && <Alert variant="error">{errorMessage}</Alert>}
      {successMessage && <Alert variant="success">{successMessage}</Alert>}

      <div className="flex-grow ag-theme-quartz">
        <AgGridReact
          ref={gridRef}
          columnDefs={columnDefs}
          rowData={localRowData}
          rowSelection="multiple"
          getRowId={(params) => params.data.id.toString()}
          editType="fullRow"
          suppressClickEdit={true}
          onRowEditingStarted={(params) => setEditingRowId(params.node.id)}
          onRowEditingStopped={() => setEditingRowId(null)}
          domLayout="normal"
          overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
        />
      </div>
    </div>
  );
};

export default ResObsStations;
