import React, { useState, useRef, useMemo, useEffect } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import Alert from "@/components/Alert/Alert";
import { useStation, useUpdatePreselections } from "@/services/stationsAPI";
import { useGetPreselections } from "@/services/preselectionsAPI";
import { getVersionColor } from "@/utils/colorStaUtils";
import { getDetailedEquipmentType } from "@/utils/stationTypes";

const PreselectionContainer = () => {
  const { data: stations, isLoading: isLoadingStations } = useStation();
  const { data: preselections, isLoading: isLoadingPreselections } =
    useGetPreselections();
  const [localRowData, setLocalRowData] = useState([]);
  const [errorMessage, setErrorMessage] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const gridRef = useRef(null);
  const updatePreselectionsMutation = useUpdatePreselections();



  const decodePreselections = (value, preselections) => {
    const decoded = {};
    preselections.forEach((preselection) => {
      decoded[preselection.name] = (value & (1 << preselection.code)) !== 0;
    });
    return decoded;
  };

  const encodePreselections = (decodedPreselections, allPreselections) => {
    let value = 0;
    allPreselections.forEach((preselection) => {
      if (decodedPreselections[preselection.name]) {
        value |= 1 << preselection.code;
      }
    });
    return value;
  };

  useEffect(() => {
    if (stations && preselections) {
      setLocalRowData(
        stations.map((station) => ({
          ...station,
          ...decodePreselections(station.preselections, preselections),
        }))
      );
    }
  }, [stations, preselections]);

  const columnDefs = useMemo(() => {
    if (!preselections) return [];

    const baseColumns = [
      {
        headerName: "Initiales",
        field: "initiales",
        cellStyle: { fontWeight: "bold" },
        width: 100,
        cellClass: (params) => {
          const colorClass = getVersionColor(
            params.data.enregistreurVersion,
            params.data.liaison,
            params.data.initiales
          );
          return [
            "font-bold",
            colorClass,
            "text-black",
            "flex",
            "items-center",
            "justify-center",
            "h-full",
            "mr-2",
          ];
        },
      },
      {
        headerName: "Nom",
        field: "nom",
        width: 220,
      },
      {
        headerName: "Type d'équipement",
        cellStyle: { fontWeight: "bold" },
        field: "equipmentType",
        width: 200,
        valueGetter: (params) => getDetailedEquipmentType(params.data),
      },
    ];

    const preselectionColumns = preselections.map((preselection) => ({
      headerName: preselection.name,
      field: preselection.name,
      cellRenderer: "agCheckboxCellRenderer",
      cellEditor: "agCheckboxCellEditor",
      editable: true,
      width: 120,
      cellStyle: { textAlign: "center" },
      flex: 1,
    }));

    return [...baseColumns, ...preselectionColumns];
  }, [preselections]);

  const onCellValueChanged = async (params) => {
    const { data, colDef, newValue } = params;
    const preselection = preselections.find((p) => p.name === colDef.field);

    if (preselection) {
      const updatedPreselections = {
        ...decodePreselections(data.preselections, preselections),
        [colDef.field]: newValue,
      };

      const encodedPreselections = encodePreselections(
        updatedPreselections,
        preselections
      );

      try {
        await updatePreselectionsMutation.mutateAsync({
          stationId: data.id,
          preselections: encodedPreselections,
        });

        setSuccessMessage("Présélections mises à jour avec succès");
        setTimeout(() => setSuccessMessage(null), 5000);
      } catch (error) {
        console.error(error);
        setErrorMessage("Erreur lors de la mise à jour des présélections");
        setTimeout(() => setErrorMessage(null), 5000);

        setLocalRowData((prevData) =>
          prevData.map((row) =>
            row.id === data.id ? { ...row, [colDef.field]: !newValue } : row
          )
        );
      }
    }
  };

  if (isLoadingStations || isLoadingPreselections)
    return <div>Chargement...</div>;

  return (
    <div className="flex flex-col h-full p-4">
      {errorMessage && <Alert variant="error">{errorMessage}</Alert>}
      {successMessage && <Alert variant="success">{successMessage}</Alert>}

      <style jsx>{`
        :global(.ag-center-header) {
          display: flex;
          align-items: center;
          justify-content: center;
        }
        :global(.ag-center-header .ag-header-cell-label) {
          justify-content: center;
        }
      `}</style>

      <div className="flex-grow ag-theme-quartz">
        <AgGridReact
          ref={gridRef}
          columnDefs={columnDefs}
          rowData={localRowData}
          onCellValueChanged={onCellValueChanged}
          getRowId={(params) => params.data.id.toString()}
          suppressClickEdit={true}
          domLayout="normal"
          defaultColDef={{
            resizable: true,
          }}
        />
      </div>
    </div>
  );
};

export default PreselectionContainer;
