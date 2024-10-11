import React, { useState, useCallback, useMemo } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import { useStationDetailsObs } from "@/services/stationsAPI";
import { useGetPreselections } from "@/services/preselectionsAPI";
import EtatStationColumns from "@/components/TableColumnDefinition/EtatStation/EtatStationColumns";
import CustomButton from "@/components/CustomButton/CustomButton";
import { getStationTypesByDetails } from "@/utils/stationTypes";

const ResObsEtatStation = () => {
  const columnDefs = EtatStationColumns();
  const { data: stationDetails, isLoading, error } = useStationDetailsObs();
  const { data: preselections } = useGetPreselections();
  const [selectedOption, setSelectedOption] = useState("");
  const [gridApi, setGridApi] = useState(null);

  const stationTypes = useMemo(() => getStationTypesByDetails(stationDetails), [stationDetails]);

  const onGridReady = (params) => {
    setGridApi(params.api);
  };

  const handleOptionChange = useCallback((e) => {
    const optionValue = e.target.value;
    setSelectedOption(optionValue);

    if (gridApi) {
      gridApi.deselectAll();

      if (optionValue.startsWith("type_")) {
        const selectedType = stationTypes.find(type => type.value === optionValue);
        if (selectedType) {
          gridApi.forEachNode(node => {
            if (selectedType.stations.some(station => station.initiales === node.data.initiales)) {
              node.setSelected(true);
            }
          });
        }
      } else if (optionValue && preselections) {
        const selectedPreselectionObj = preselections.find(p => p.id.toString() === optionValue);
        if (selectedPreselectionObj) {
          gridApi.forEachNode(node => {
            if ((node.data.preselections & (1 << selectedPreselectionObj.code)) !== 0) {
              node.setSelected(true);
            }
          });
        }
      }
    }
  }, [gridApi, stationTypes, preselections]);

  if (isLoading) return <div>Chargement...</div>;
  if (error) return <div>Une erreur est survenue: {error.message}</div>;

  return (
    <div className="flex flex-col h-full p-4">
      <div className="p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
        <div className="flex items-center space-x-3">
          <CustomButton
            onClick={() => console.log("Appeler les stations présélectionnées")}
          >
            Appeler les stations sélectionnées
          </CustomButton>
          <select 
            value={selectedOption} 
            onChange={handleOptionChange}
            className="px-3 py-2 font-semibold text-black border-2 border-atoli_blue rounded-xl"
          >
            <option value="">Présélections</option>
            <optgroup label="Types d'enregistreur">
              {stationTypes.map(type => (
                <option key={type.value} value={type.value}>
                  {type.label} ({type.stations.length})
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
          <CustomButton onClick={() => console.log("Appeler la station")}>
            Appeler la station
          </CustomButton>
          <input
            type="text"
            placeholder="Initiales"
            className="px-3 py-2 font-semibold text-black border-2 border-atoli_blue rounded-xl"
          />
        </div>
      </div>

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

export default ResObsEtatStation;