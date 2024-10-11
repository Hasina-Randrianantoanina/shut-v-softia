import React, { useState, useCallback } from "react";
import StationSelectorContainerVisu from "@/containers/BandeauSelectionStations/StationSelectorContainerVisu";
import DateRangeSelector from "@/components/TimeSeries/DateRangeSelector";
import VoiesSelector from "@/components/TimeSeries/VoiesSelector";
import DataVisualizer from "@/components/TimeSeries/DataVisualizer";
import LoadingSpinner from "@/components/Loading/LoadingSpinner";
import useTimeSeries from "@/hooks/Visualisation/useTimeSeries";

const VisualisationContainer = () => {
  const [selectedStations, setSelectedStations] = useState([]);
  const [dateRange, setDateRange] = useState([new Date(), new Date()]);
  const [selectedAnas, setSelectedAnas] = useState([]);
  const [selectedTors, setSelectedTors] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const {
    chartData,
    filteredRowData,
    visibleSeries,
    fetchData,
    updateTableData,
    updateYAxisRange,
  } = useTimeSeries(selectedStations.map(s => s.initiales), dateRange, selectedTors, selectedAnas);

  const handleStationSelect = useCallback((stations) => {
    setSelectedStations(stations);
    setSelectedAnas([]);
    setSelectedTors([]);
  }, []);

  const handleDateRangeChange = useCallback((newDateRange) => {
    setDateRange(newDateRange);
  }, []);

  const handleFetchData = useCallback(async () => {
    if (
      selectedStations.length > 0 &&
      (selectedTors.length > 0 || selectedAnas.length > 0)
    ) {
      setIsLoading(true);
      try {
        await fetchData();
      } catch (error) {
        console.error("Error fetching data:", error);
        alert(error.message);
      } finally {
        setIsLoading(false);
      }
    } else {
      alert(
        "Veuillez sélectionner au moins une station et au moins une voie ANA ou TOR"
      );
    }
  }, [selectedStations, selectedTors, selectedAnas, fetchData]);

  return (
    <div className="flex flex-col p-4">
      <StationSelectorContainerVisu onStationSelect={handleStationSelect} reseau="USAGE" multiSelect={true} />
      <div className="p-4 border border-atoli_blue rounded-xl">
        <div className="flex flex-col gap-4 lg:flex-row">
          <DateRangeSelector
            dateRange={dateRange}
            onDateRangeChange={handleDateRangeChange}
            selectedStations={selectedStations.map(s => s.initiales)}
          />
          <VoiesSelector
            selectedStations={selectedStations.map(s => s.initiales)}
            selectedTors={selectedTors}
            setSelectedTors={setSelectedTors}
            selectedAnas={selectedAnas}
            setSelectedAnas={setSelectedAnas}
            onFetchData={handleFetchData}
          />
        </div>
      </div>
      {isLoading ? (
        <LoadingSpinner />
      ) : (
        chartData && (
          <DataVisualizer
            chartData={chartData}
            filteredRowData={filteredRowData}
            updateTableData={updateTableData}
            visibleSeries={visibleSeries}
            selectedTors={selectedTors}
            updateYAxisRange={updateYAxisRange}
          />
        )
      )}
    </div>
  );
};

export default VisualisationContainer;