import { useState, useEffect } from "react";

const defCaptDataUsage = () => {
  const [rowData, setRowData] = useState([]);

  useEffect(() => {
    setRowData([
      {
        heure: "00:20:00",
        grp_rc: false,
        grp_ip: false,
      },
      {
        heure: "01:20:00",
        grp_rc: false,
        grp_ip: false,
      },
      {
        heure: "02:20:00",
        grp_rc: false,
        grp_ip: false,
      },
    ]);
  }, []);

  return rowData;
};

export default defCaptDataUsage;
