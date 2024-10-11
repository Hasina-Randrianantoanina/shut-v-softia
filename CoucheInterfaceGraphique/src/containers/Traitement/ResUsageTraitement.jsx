import React, { useState, useMemo, useCallback, useEffect } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import StationSelectorContainer from "@/containers/BandeauSelectionStations/StationSelectorContainerMultiple";
import { useVoiesTraitementsByStations } from "@/services/traitementsAPI";
import { useGetPreselections } from "@/services/preselectionsAPI";
import { useStation } from "@/services/stationsAPI";
import { FaChevronDown, FaChevronUp } from "react-icons/fa";
import { getStationTypes, TABLE_HEIGHT, TABLE_MARGIN } from "@/utils/stationTypes";
import { getColumnDefs, processVoiesTraitements } from "@/utils/traitementUtils";

const ResUsageTraitement = () => {
  const [selectedStations, setSelectedStations] = useState([]);
  const [expandedTables, setExpandedTables] = useState({});
  const [selectedOption, setSelectedOption] = useState("");
  const { data: preselections, isLoading: isLoadingPreselections } = useGetPreselections();
  const { data: allStations, isLoading: isLoadingStations } = useStation("USAGE");
  const {
    data: voiesTraitements,
    isLoading,
    error,
  } = useVoiesTraitementsByStations(
    selectedStations.map((station) => station.initiales)
  );

  const stationTypes = useMemo(() => getStationTypes(allStations), [allStations]);

  const handleOptionChange = useCallback((e) => {
    const optionValue = e.target.value;
    setSelectedOption(optionValue);

    if (optionValue.startsWith("type_")) {
      const selectedType = stationTypes.find(type => type.value === optionValue);
      if (selectedType) {
        setSelectedStations(selectedType.stations);
      }
    } else if (optionValue && preselections && allStations) {
      const selectedPreselectionObj = preselections.find(p => p.id.toString() === optionValue);
      if (selectedPreselectionObj) {
        const stationsInPreselection = allStations.filter(station => 
          (station.preselections & (1 << selectedPreselectionObj.code)) !== 0
        );
        setSelectedStations(stationsInPreselection);
      }
    } else {
      setSelectedStations([]);
    }
  }, [preselections, allStations, stationTypes]);

  useEffect(() => {
    if (voiesTraitements) {
      const initialExpandedState = {
        "sans-traitement": true,
        ...voiesTraitements.reduce((acc, voie) => {
          if (voie.traitementId) {
            acc[voie.traitementId] = true;
          }
          return acc;
        }, {}),
      };
      setExpandedTables(initialExpandedState);
    }
  }, [voiesTraitements]);

  const handleStationSelect = useCallback((stations) => {
    setSelectedStations(stations);
    setSelectedOption("");
  }, []);

  const toggleTable = useCallback((tableId) => {
    setExpandedTables((prev) => ({
      ...prev,
      [tableId]: !prev[tableId],
    }));
  }, []);

  const traitementsTables = useMemo(() => {
    if (!voiesTraitements) return null;

    const { traitementMap, voiesSansTraitement } = processVoiesTraitements(voiesTraitements);

    return (
      <>
        {voiesSansTraitement.length > 0 && (
          <div key="sans-traitement" className={TABLE_MARGIN}>
            <h2
              className="flex items-center mb-4 text-xl font-bold cursor-pointer"
              onClick={() => toggleTable("sans-traitement")}
            >
              {expandedTables["sans-traitement"] ? (
                <FaChevronUp className="mr-2" />
              ) : (
                <FaChevronDown className="mr-2" />
              )}
              Voies sans traitement
            </h2>
            {expandedTables["sans-traitement"] && (
              <div
                className="ag-theme-quartz"
                style={{ height: `${TABLE_HEIGHT}px`, width: "100%" }}
              >
                <AgGridReact
                  columnDefs={getColumnDefs(null, true)}
                  rowData={voiesSansTraitement}
                  headerHeight={60}
                  suppressCellFocus={true}
                  overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
                  defaultColDef={{
                    resizable: true,
                    sortable: true,
                  }}
                />
              </div>
            )}
          </div>
        )}

        {Array.from(traitementMap.entries()).map(
          ([traitementId, traitement]) => (
            <div key={traitementId} className={TABLE_MARGIN}>
              <h2
                className="flex items-center mb-4 text-xl font-bold cursor-pointer"
                onClick={() => toggleTable(traitementId)}
              >
                {expandedTables[traitementId] ? (
                  <FaChevronUp className="mr-2" />
                ) : (
                  <FaChevronDown className="mr-2" />
                )}
                {traitement.nom}
              </h2>
              {expandedTables[traitementId] && (
                <div
                  className="ag-theme-quartz"
                  style={{ height: `${TABLE_HEIGHT}px`, width: "100%" }}
                >
                  <AgGridReact
                    columnDefs={getColumnDefs(traitement.parametres)}
                    rowData={traitement.voies}
                    headerHeight={60}
                    overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
                    suppressCellFocus={true}
                    defaultColDef={{
                      resizable: true,
                      sortable: true,
                    }}
                  />
                </div>
              )}
            </div>
          )
        )}
      </>
    );
  }, [voiesTraitements, expandedTables, toggleTable]);

  return (
    <div className="flex flex-col p-4">
      <div className="flex flex-col mb-4">
        <StationSelectorContainer
          onStationSelect={handleStationSelect}
          reseau="USAGE"
          multiSelect={true}
          selectedStations={selectedStations}
        />
        <div className="self-end mt-2">
          <select 
            value={selectedOption} 
            onChange={handleOptionChange}
            className="px-2 py-1 text-sm font-semibold border rounded-lg border-atoli_blue"
          >
            <option value="">Filtre stations</option>
            <optgroup label="Types d'enregistreur">
              {stationTypes.map(type => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </optgroup>
            <optgroup label="Présélections">
              {preselections && preselections.map(preselection => (
                <option key={preselection.id} value={preselection.id.toString()}>
                  {preselection.name}
                </option>
              ))}
            </optgroup>
          </select>
        </div>
      </div>
      <div className="p-4 border border-atoli_blue rounded-xl">
        <h1 className="mb-4 text-2xl font-bold">Traitements</h1>
        {isLoading && <p>Chargement des traitements...</p>}
        {error && <p className="text-red-500">Erreur: {error.message}</p>}
        {traitementsTables}
      </div>
    </div>
  );
};

export default ResUsageTraitement;