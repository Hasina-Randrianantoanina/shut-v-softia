import React from "react";
import { getVersionColor } from "@/utils/colorStaUtils";

const StationSelector = ({ stations, selectedStation, onSelect }) => {
  return (
    <div className="p-4 rounded-lg">
      <h2 className="mb-4 text-xl font-semibold">Sélectionner une station :</h2>
      <div className="max-h-[60vh] overflow-y-auto">
        <div className="flex flex-wrap gap-1 p-3 rounded-xl bg-slate-100">
          {stations.map((station) => (
            <button
              key={station.id}
              className={`px-2 py-1 text-xs font-bold rounded 
                w-[calc(20%-0.2rem)] 
                sm:w-[calc(16.666%-0.2rem)] 
                md:w-[calc(5%-0.2rem)] 
                lg:w-[calc(4%-0.2rem)] 
                xl:w-[calc(3.333%-0.2rem)] 
                2xl:w-[calc(2.857%-0.2rem)]
                ${
                selectedStation === station.initiales
                  ? "bg-atoli_blue text-white"
                  : `${getVersionColor(station.enregistreurVersion, station.liaison, station.initiales)} text-white hover:opacity-80`
              }`}
              onClick={() => onSelect(station)}
              title={`${station.nom} - Version: ${station.enregistreurVersion || 'Non spécifiée'} - Liaison: ${station.liaison}`}
            >
              {station.initiales}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default StationSelector;

