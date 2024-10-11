import { useState, useEffect } from "react";

const DefDataVoieStc = () => {
  const [rowData, setRowData] = useState([]);

  useEffect(() => {
    
    setRowData([
      {
        voie: "In",
        type: "BlockLogique",
        nom: "ACT_SUIVI_SPECIAL",
        source: "-",
        voice: "-",
        valeur: "0",
        mdf_v: '-'
      },
      
    ]);
  }, []);

  return rowData;
};

export default DefDataVoieStc;
