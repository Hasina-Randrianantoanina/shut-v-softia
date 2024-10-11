import React, {
  useState,
  useRef,
  useMemo,
  useEffect,
  useCallback,
} from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import StationSelectorContainer from "@/containers/BandeauSelectionStations/StationSelectorContainer";
import AbtsVoieColumns from "@/components/TableColumnDefinition/AbonnementGroupePreselection/AbtsVoieColumns";
import Alert from "@/components/Alert/Alert";
import {
  useUpdateAbonnementsVoie,
  useVoiesByStationId,
} from "@/services/voiesAPI";
import { decodeAbonnements, encodeAbonnements } from "@/utils/abonnementsUtils";

const AbtsVoieUsageContainer = () => {
  const [selectedStation, setSelectedStation] = useState(null);
  const gridRef = useRef(null);
  const [errorMessage, setErrorMessage] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);

  const {
    data: voies,
    isLoading,
    isError,
    refetch,
  } = useVoiesByStationId(selectedStation?.id);

  const [localRowData, setLocalRowData] = useState([]);
  const updateVoieMutation = useUpdateAbonnementsVoie();

  const handleStationSelect = useCallback((station) => {
    setSelectedStation(station);
  }, []);

  useEffect(() => {
    if (selectedStation?.id) {
      refetch();
    }
  }, [selectedStation, refetch]);

  const columnDefs = useMemo(() => AbtsVoieColumns(), []);

  useEffect(() => {
    if (voies) {
      let voiesToProcess = voies.voiesTelemesurees || [];
      const decodedVoies = voiesToProcess.map((voie) => ({
        ...voie,
        voieLibelle: voie.libelle,
        ...decodeAbonnements(voie.abonnements),
      }));
      setLocalRowData(decodedVoies);
    }
  }, [voies]);

  const onCellValueChanged = async (params) => {
    const { data, colDef } = params;
    const abonnementFields = ["ADMIN", "OPERATEUR", "VALIDEUR", "CONSULTATION"];

    if (abonnementFields.includes(colDef.field)) {
      const updatedAbonnements = {
        ADMIN: data.ADMIN,
        OPERATEUR: data.OPERATEUR,
        VALIDEUR: data.VALIDEUR,
        CONSULTATION: data.CONSULTATION,
      };

      const encodedAbonnements = encodeAbonnements(updatedAbonnements);

      try {
        await updateVoieMutation.mutateAsync({
          voieId: data.id,
          abonnements: encodedAbonnements,
        });

        setSuccessMessage("Abonnements mis à jour avec succès");
        setTimeout(() => setSuccessMessage(null), 5000);

        // Mettre à jour localRowData sans changer l'ordre
        const updatedRowData = localRowData.map((row) =>
          row.id === data.id
            ? { ...row, ...updatedAbonnements, abonnements: encodedAbonnements }
            : row
        );
        setLocalRowData(updatedRowData);

        // Forcer le tri
        if (gridRef.current && gridRef.current.api) {
          gridRef.current.api.refreshCells();
          gridRef.current.api.redrawRows();
        }
      } catch (error) {
        console.error(error);
        setErrorMessage("Erreur lors de la mise à jour des abonnements");
        setTimeout(() => setErrorMessage(null), 5000);

        setLocalRowData((prevData) =>
          prevData.map((row) =>
            row.id === data.id
              ? { ...row, [colDef.field]: !data[colDef.field] }
              : row
          )
        );
      }
    }
  };

  return (
    <div className="flex flex-col p-4">
      <StationSelectorContainer
        onStationSelect={handleStationSelect}
        reseau="USAGE"
      />
      <div className="mt-4 border border-atoli_blue rounded-xl">
        <h1 className="mt-4 mb-2 ml-4 text-2xl font-bold">Les abonnements de voies</h1>
        {selectedStation && (
          <div className="flex justify-start ml-4 text-2xl font-bold item-center text-atoli_blue">
            Station {selectedStation.initiales}
          </div>
        )}
        {errorMessage && (
          <div style={{ margin: "8px 16px" }}>
            <Alert variant="error">{errorMessage}</Alert>
          </div>
        )}
        {successMessage && (
          <div style={{ margin: "8px 16px" }}>
            <Alert variant="success">{successMessage}</Alert>
          </div>
        )}

        {isLoading && <div className="p-4">Chargement des voies...</div>}
        {isError && (
          <div className="p-4">
            Une erreur s'est produite lors du chargement des voies
          </div>
        )}
        {selectedStation &&
          !isLoading &&
          !isError &&
          localRowData.length > 0 && (
            <div
              className="flex-grow p-4 ag-theme-quartz"
              style={{ height: "600px", width: "100%" }}
            >
              <AgGridReact
                ref={gridRef}
                columnDefs={columnDefs}
                rowData={localRowData}
                onCellValueChanged={onCellValueChanged}
                getRowId={(params) => params.data.id.toString()}
                suppressClickEdit={true}
                domLayout="normal"
                overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
                defaultColDef={{
                  resizable: true,
                  sortable: true,
                }}
                onGridReady={(params) => {
                  params.api.sizeColumnsToFit();
                }}
              />
            </div>
          )}
        {selectedStation &&
          !isLoading &&
          !isError &&
          localRowData.length === 0 && (
            <div>Aucune voie trouvée pour cette station</div>
          )}
      </div>
    </div>
  );
};

export default AbtsVoieUsageContainer;