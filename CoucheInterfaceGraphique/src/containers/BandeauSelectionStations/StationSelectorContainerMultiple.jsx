import React, { useState, useMemo, useCallback, useEffect } from 'react';
import StationSelector from "@/components/StationBandeauSelection/StationSelectorMultiple";
import { useStation } from '@/services/stationsAPI';

const StationSelectorContainer = ({ onStationSelect, reseau, multiSelect = false, selectedStations = [] }) => {
  const [internalSelectedStations, setInternalSelectedStations] = useState(selectedStations);
  const { data: stations, isLoading, error } = useStation(reseau);

  const activeStations = useMemo(() => {
    if (!stations) return [];
    return stations.filter(station => station.actif && station.reseau === reseau);
  }, [stations, reseau]);

  const handleStationSelect = useCallback((station) => {
    setInternalSelectedStations(prev => {
      let newSelection;
      if (multiSelect) {
        const isAlreadySelected = prev.some(s => s.id === station.id);
        if (isAlreadySelected) {
          newSelection = prev.filter(s => s.id !== station.id);
        } else {
          newSelection = [...prev, station];
        }
      } else {
        newSelection = [station];
      }
      return newSelection;
    });
  }, [multiSelect]);

  useEffect(() => {
    onStationSelect(internalSelectedStations);
  }, [internalSelectedStations, onStationSelect]);

  useEffect(() => {
    setInternalSelectedStations(selectedStations);
  }, [selectedStations]);

  if (isLoading) return <div>Chargement des stations...</div>;
  if (error) return <div>Erreur: {error.message}</div>;

  return (
    <StationSelector
      stations={activeStations}
      selectedStations={internalSelectedStations}
      onSelect={handleStationSelect}
      multiSelect={multiSelect}
    />
  );
};

export default StationSelectorContainer;