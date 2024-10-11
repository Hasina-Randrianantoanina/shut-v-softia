import React, { useState, useRef, useMemo, useEffect } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import AbtsStationColumns from "@/components/TableColumnDefinition/AbonnementGroupePreselection/AbtsStationColumns";
import Alert from "@/components/Alert/Alert";
import { useStation, useUpdateAbonnements } from "@/services/stationsAPI";
import { decodeAbonnements, encodeAbonnements } from "@/utils/abonnementsUtils";

const AbtsStationContainer = () => {
  const { data: stations, isLoading, isError } = useStation();
  const [localRowData, setLocalRowData] = useState([]);
  const [errorMessage, setErrorMessage] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const gridRef = useRef(null);
  const updateStationMutation = useUpdateAbonnements();

  useEffect(() => {
    if (stations) {
      setLocalRowData(
        stations.map(station => ({
          ...station,
          ...decodeAbonnements(station.abonnements),
        }))
      );
    }
  }, [stations]);

  const columnDefs = useMemo(() => AbtsStationColumns(), []);

  const onCellValueChanged = async (params) => {
    const { data, colDef } = params;
    const abonnementFields = ['ADMIN', 'OPERATEUR', 'VALIDEUR', 'CONSULTATION'];
    
    if (abonnementFields.includes(colDef.field)) {
      const updatedAbonnements = {
        ADMIN: data.ADMIN,
        OPERATEUR: data.OPERATEUR,
        VALIDEUR: data.VALIDEUR,
        CONSULTATION: data.CONSULTATION
      };
      
      const encodedAbonnements = encodeAbonnements(updatedAbonnements);
      
      try {
        await updateStationMutation.mutateAsync({
          stationId: data.id,
          abonnements: encodedAbonnements
        });
        
        setSuccessMessage("Abonnements mis à jour avec succès");
        setTimeout(() => setSuccessMessage(null), 5000);
      } catch (error) {
        console.error(error);
        setErrorMessage("Erreur lors de la mise à jour des abonnements");
        setTimeout(() => setErrorMessage(null), 5000);
        
        setLocalRowData(prevData => 
          prevData.map(row => 
            row.id === data.id ? { ...row, [colDef.field]: !data[colDef.field] } : row
          )
        );
      }
    }
  };

  if (isLoading) return <div>Chargement...</div>;
  if (isError) return <div>Une erreur s'est produite</div>;

  return (
    <div className="flex flex-col h-full p-4">
      {errorMessage && <Alert variant="error">{errorMessage}</Alert>}
      {successMessage && <Alert variant="success">{successMessage}</Alert>}

      <div className="flex-grow ag-theme-quartz">
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
          }}
        />
      </div>
    </div>
  );
};

export default AbtsStationContainer;