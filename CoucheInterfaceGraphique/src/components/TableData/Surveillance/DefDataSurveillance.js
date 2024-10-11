import { useState, useEffect } from "react";

const DefDataSurveillance = () => {
  const [rowData, setRowData] = useState([]);

  useEffect(() => {
    // Ici fetch ou simulation de données
    setRowData([
      {
        ini: "EN",
        liaison: "AP",
        fin: "17/05/2024 09:51:09",
        etat: "lect tps réel",
        nbcnx: "Gestionnaire",
        cfg: "17/05/2024 09:36:06",
        derOp: "17/05/2024 09:36:06",
      },
      
    ]);
  }, []);

  return rowData;
};

export default DefDataSurveillance;
