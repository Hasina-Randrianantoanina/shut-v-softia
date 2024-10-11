import { useState, useEffect } from "react";

const defCaptDataObs = () => {
  const [rowData, setRowData] = useState([]);

  useEffect(() => {
    setRowData([
      {
        heure: "00:20:00",
        grp_1: false,
        grp_2: false,
        grp_3: false,
        grp_4: false,
        grp_5: false,
        grp_6: false,
        grp_7: false,
        grp_8: false,
        grp_9: false,
        grp_10: false,
        grp_11: false,
        grp_sigma: false,
      },

    ]);
  }, []);

  return rowData;
};

export default defCaptDataObs;
