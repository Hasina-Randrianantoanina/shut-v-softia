// components/CyclesAppelsGrid.jsx
import React, { useState, useEffect, useCallback } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import { FaPlus } from "react-icons/fa";
import CustomButton from "@/components/CustomButton/CustomButton";

const CyclesAppelsGrid = ({ initialData, columnDefs, newRowTemplate }) => {
  // États pour les données des lignes et les API de la grille
  const [rowData, setRowData] = useState([]);
  const [gridApi, setGridApi] = useState(null);
  const [gridColumnApi, setGridColumnApi] = useState(null);

  // Initialisation des données au chargement du composant
  useEffect(() => {
    if (initialData.length > 0) {
      setRowData(initialData);
    }
  }, [initialData]);

  // Callback exécuté lorsque la grille est prête
  const onGridReady = useCallback((params) => {
    setGridApi(params.api);
    setGridColumnApi(params.columnApi);
  }, []);

  // Fonction pour trier les données par heure
  const sortRowData = useCallback((data) => {
    return [...data].sort((a, b) => a.heure.localeCompare(b.heure));
  }, []);

  // Gestion de la mise à jour des données
  const handleUpdate = useCallback(() => {
    if (gridApi) {
      const updatedData = [];
      gridApi.forEachNode((node) => {
        updatedData.push(node.data);
      });
      const sortedData = sortRowData(updatedData);
      console.log("Données à envoyer via API:", sortedData);
      // Ici, envoi des données via API
    }
  }, [gridApi, sortRowData]);

  // Gestion de l'annulation des modifications
  const handleCancel = useCallback(() => {
    if (gridApi) {
      gridApi.setGridOption("rowData", initialData);
    }
  }, [gridApi, initialData]);

  // Ajout d'une nouvelle ligne
  const handleAddRow = useCallback(() => {
    const updatedRowData = sortRowData([...rowData, newRowTemplate]);
    setRowData(updatedRowData);
    if (gridApi) {
      gridApi.setGridOption("rowData", updatedRowData);
    }
  }, [rowData, sortRowData, gridApi, newRowTemplate]);

  // Gestion du changement de valeur d'une cellule
  const onCellValueChanged = useCallback(
    (params) => {
      if (params.column.getColId() === "heure") {
        const updatedRowData = rowData.map((row) =>
          row === params.data ? { ...row, heure: params.newValue } : row
        );
        const sortedData = sortRowData(updatedRowData);
        setRowData(sortedData);
        if (gridApi) {
          gridApi.setGridOption("rowData", sortedData);
        }
      }
    },
    [rowData, sortRowData, gridApi]
  );

  return (
    <div className="flex flex-col p-4">
      <div className="p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <CustomButton onClick={handleUpdate}>Mettre à jour</CustomButton>
            <CustomButton onClick={handleCancel} variant="red">
              Annuler
            </CustomButton>
          </div>
          <CustomButton
            onClick={handleAddRow}
            className="flex items-center"
            variant="gray"
          >
            <FaPlus className="mr-2" /> Ajouter un cycle
          </CustomButton>
        </div>
      </div>

      <div className="ag-theme-quartz" style={{ flex: "1 1 auto" }}>
        <AgGridReact
          columnDefs={columnDefs}
          rowData={rowData}
          onGridReady={onGridReady}
          domLayout="autoHeight"
          onCellValueChanged={onCellValueChanged}
          rowHeight={80}
          autoSizeColumns={true}
        />
      </div>
    </div>
  );
};

export default CyclesAppelsGrid;
