import React, { useState, useRef, useEffect } from 'react';
import { FaInfoCircle } from 'react-icons/fa';
import { getVersionColor } from "@/utils/colorStaUtils";

// Hook personnalisé pour détecter les clics en dehors de l'élément
const useOutsideClick = (callback) => {
  const ref = useRef();

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (ref.current && !ref.current.contains(event.target)) {
        callback();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [callback]);

  return ref;
};

const ColorLegend = () => {
  const [isOpen, setIsOpen] = useState(false);
  const ref = useOutsideClick(() => setIsOpen(false));

  const legendItems = [
    { type: "STEN IP", version: "Y", liaison: "IP" },
    { type: "STEN2", version: "Z", liaison: "IP" },
    // { type: "STEN1", version: "W", liaison: "IP" },
    // { type: "Autre STEN", version: "", liaison: "IP" },
    { type: "Automate M340", initiales: "NU", liaison: "AP" },
    { type: "Automate Premium", version: "D13A53Aa01", liaison: "AP" },
    { type: "Automate M580", version: "A", liaison: "AP" },
    // { type: "Autre Automate", version: "", liaison: "AP" },
    { type: "ISODAQ", liaison: "XX" },
    { type: "Autre", liaison: "" }
  ];

  return (
    <div className="relative" ref={ref}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center space-x-2 text-atoli_blue hover:text-atoli_blue-dark focus:outline-none"
      >
        <FaInfoCircle className="text-3xl" />
      </button>
      {isOpen && (
        <div className="absolute z-10 w-64 p-4 mt-2 rounded-lg shadow-2xl right-1 bg-slate-100">
          <h3 className="pb-2 mb-3 text-lg font-bold border-b">Types d'équipement</h3>
          <div className="grid grid-cols-1 gap-2">
            {legendItems.map(item => (
              <div key={item.type} className="flex items-center space-x-2">
                <div className={`w-4 h-4 rounded ${getVersionColor(item.version, item.liaison, item.initiales)}`}></div>
                <span className="text-sm">{item.type}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default ColorLegend;