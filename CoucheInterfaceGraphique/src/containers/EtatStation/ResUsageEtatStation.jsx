import React, { useState, useCallback, useMemo, useEffect } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import { useStationDetails, useAppelerStations } from "@/services/stationsAPI";
import { useGetPreselections } from "@/services/preselectionsAPI";
import EtatStationColumns from "@/components/TableColumnDefinition/EtatStation/EtatStationColumns";
import CustomButton from "@/components/CustomButton/CustomButton";
import { getStationTypesByDetails } from "@/utils/stationTypes";
import Alert from "@/components/Alert/Alert";
import LoadingSpinner from "@/components/Loading/LoadingSpinner";

const ResUsageEtatStation = () => {
  const columnDefs = EtatStationColumns();
  const {
    data: stationDetails,
    isLoading: isLoadingStations,
    error: stationError,
  } = useStationDetails();
  const { data: preselections } = useGetPreselections();
  const [selectedOption, setSelectedOption] = useState("");
  const [gridApi, setGridApi] = useState(null);
  const [logs, setLogs] = useState([]);
  const [alertMessage, setAlertMessage] = useState(null);
  const [alertVariant, setAlertVariant] = useState("info");
  const appelerStationsMutation = useAppelerStations();
  const [stationInitiales, setStationInitiales] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [loadingDots, setLoadingDots] = useState("");

  useEffect(() => {
    let interval;
    if (isLoading) {
      interval = setInterval(() => {
        setLoadingDots((dots) => (dots.length >= 3 ? "" : dots + "."));
      }, 500);
    }
    return () => clearInterval(interval);
  }, [isLoading]);

  const stationTypes = useMemo(
    () => getStationTypesByDetails(stationDetails),
    [stationDetails]
  );

  const onGridReady = (params) => {
    setGridApi(params.api);
  };

  const handleOptionChange = useCallback(
    (e) => {
      const optionValue = e.target.value;
      setSelectedOption(optionValue);

      if (gridApi) {
        gridApi.deselectAll();

        if (optionValue.startsWith("type_")) {
          const selectedType = stationTypes.find(
            (type) => type.value === optionValue
          );
          if (selectedType) {
            gridApi.forEachNode((node) => {
              if (
                selectedType.stations.some(
                  (station) => station.initiales === node.data.initiales
                )
              ) {
                node.setSelected(true);
              }
            });
          }
        } else if (optionValue && preselections) {
          const selectedPreselectionObj = preselections.find(
            (p) => p.id.toString() === optionValue
          );
          if (selectedPreselectionObj) {
            gridApi.forEachNode((node) => {
              if (
                (node.data.preselections &
                  (1 << selectedPreselectionObj.code)) !==
                0
              ) {
                node.setSelected(true);
              }
            });
          }
        }
      }
    },
    [gridApi, stationTypes, preselections]
  );

  const handleAppelerStations = useCallback(
    async (selectedStationIds) => {
      if (selectedStationIds.length === 0) {
        setAlertMessage("Veuillez sélectionner au moins une station.");
        setAlertVariant("warning");
        return;
      }

      setIsLoading(true);
      setAlertMessage("Appel en cours");
      setAlertVariant("info");

      try {
        console.log("IDs des stations sélectionnées:", selectedStationIds);
        const result = await appelerStationsMutation.mutateAsync(
          selectedStationIds
        );

        console.log("Résultat de l'appel des stations:", result);
        setLogs((prevLogs) => [...prevLogs, JSON.stringify(result, null, 2)]);

        setAlertMessage(result.message);
        setAlertVariant(
          result.message.includes("réussi") ? "success" : "error"
        );
        if (result.details) {
          setLogs((prevLogs) => [...prevLogs, `Détails : ${result.details}`]);
        }
      } catch (error) {
        console.error("Erreur complète:", error);
        let errorMessage;
        try {
          const parsedError = JSON.parse(error.message);
          errorMessage = parsedError.message || parsedError.details || error.message;
        } catch {
          errorMessage = error.message || "Une erreur est survenue lors de l'appel des stations.";
        }
      
        // Extraire la partie pertinente de l'erreur
        const relevantError = extractRelevantError(errorMessage);
      
        setAlertMessage(relevantError);
        setAlertVariant("error");
        setLogs((prevLogs) => [...prevLogs, `Erreur : ${errorMessage}`]);
      } finally {
        setIsLoading(false);
      }
    },
    [appelerStationsMutation]
  );

  const handleAppelerStationUnique = useCallback(() => {
    if (!gridApi || !stationInitiales) return;

    const stationInitialesLower = stationInitiales.toLowerCase();
    let foundStation = null;

    gridApi.forEachNode((node) => {
      if (node.data.initiales.toLowerCase() === stationInitialesLower) {
        foundStation = node;
      }
    });

    if (foundStation) {
      gridApi.deselectAll();
      foundStation.setSelected(true);
      handleAppelerStations([foundStation.data.stationId]);
    } else {
      console.log(
        "Stations disponibles:",
        gridApi
          .getModel()
          .getRowsAfterFilter()
          .map((node) => node.data.initiales)
      );
      setAlertMessage("Aucune station trouvée avec ces initiales.");
      setAlertVariant("error");
    }
  }, [gridApi, stationInitiales, handleAppelerStations]);

  const handleAppelerStationsMultiples = useCallback(() => {
    if (!gridApi) return;
    const selectedNodes = gridApi.getSelectedNodes();
    const selectedStationIds = selectedNodes.map((node) => node.data.stationId);
    handleAppelerStations(selectedStationIds);
  }, [gridApi, handleAppelerStations]);

  const closeAlert = () => {
    setAlertMessage(null);
  };


  const extractRelevantError = (errorMessage) => {
    if (!errorMessage) return "";
    const match = errorMessage.match(/Échec pour la station .+?:.+?\.txt/);
    
    if (match) {
      // Remplace "System.Exception:" par un espace
      return match[0].replace("System.Exception:", "");
    }
    const firstLine = errorMessage.split('\n')[0].replace("System.Exception:", "");
    return firstLine.trim();
  };

  
  
  if (isLoadingStations) return <LoadingSpinner />;
  if (stationError)
    return (
      <Alert variant="error">
        Une erreur est survenue: {stationError.message}
      </Alert>
    );

  return (
    <div className="flex flex-col h-full p-4">
      <div className="p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
        <div className="flex items-center space-x-3">
          <CustomButton onClick={handleAppelerStationsMultiples}>
            Appeler les stations sélectionnées
          </CustomButton>
          <select
            value={selectedOption}
            onChange={handleOptionChange}
            className="px-3 py-2 font-semibold text-black border-2 border-atoli_blue rounded-xl"
          >
            <option value="">Présélections</option>
            <optgroup label="Types d'enregistreur">
              {stationTypes.map((type) => (
                <option key={type.value} value={type.value}>
                  {type.label} ({type.stations.length})
                </option>
              ))}
            </optgroup>
            <optgroup label="Présélections">
              {preselections &&
                preselections.map((preselection) => (
                  <option
                    key={preselection.id}
                    value={preselection.id.toString()}
                  >
                    {preselection.name}
                  </option>
                ))}
            </optgroup>
          </select>
          <CustomButton onClick={handleAppelerStationUnique}>
            Appeler la station
          </CustomButton>
          <input
            type="text"
            placeholder="Initiales"
            className="px-3 py-2 font-semibold text-black border-2 border-atoli_blue rounded-xl"
            value={stationInitiales}
            onChange={(e) => setStationInitiales(e.target.value)}
          />
        </div>
      </div>

      {(alertMessage || isLoading) && (
        <Alert variant={isLoading ? "info" : alertVariant}>
          {isLoading ? `Appel en cours${loadingDots}` : alertMessage}
          {!isLoading && (
            <button onClick={closeAlert} className="float-right font-bold">
              &times;
            </button>
          )}
        </Alert>
      )}

      <div className="flex-grow ag-theme-quartz">
        <AgGridReact
          columnDefs={columnDefs}
          rowData={stationDetails}
          rowSelection="multiple"
          suppressCellFocus={true}
          domLayout="normal"
          onGridReady={onGridReady}
          overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
        />
      </div>
    </div>
  );
};

export default ResUsageEtatStation;
