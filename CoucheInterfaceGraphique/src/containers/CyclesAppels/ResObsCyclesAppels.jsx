import React from "react";
import CyclesAppelsGrid from "@/containers/CyclesAppels/CyclesAppelsGrid";
import defCycleDataObs from "@/components/TableData/CyclesAppels/cyclesAppelsDataObs";
import CyclesAppelsObsColumns from "@/components/TableColumnDefinition/CyclesAppels/CyclesAppelsObsColumns";

const ResObsCyclesAppels = () => {
  const columnDefs = CyclesAppelsObsColumns();
  const initialData = defCycleDataObs();
  const newRowTemplate = {
    heure: "00:00:00",
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
  };

  return (
    <CyclesAppelsGrid
      initialData={initialData}
      columnDefs={columnDefs}
      newRowTemplate={newRowTemplate}
    />
  );
};

export default ResObsCyclesAppels;