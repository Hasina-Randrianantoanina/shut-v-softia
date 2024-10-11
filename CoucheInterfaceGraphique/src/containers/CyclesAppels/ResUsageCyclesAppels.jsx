import React from "react";
import CyclesAppelsGrid from "@/containers/CyclesAppels/CyclesAppelsGrid";
import defCycleDataUsage from "@/components/TableData/CyclesAppels/cyclesAppelsDataUsage";
import CyclesAppelsUsageColumns from "@/components/TableColumnDefinition/CyclesAppels/CyclesAppelsUsageColumns";

const ResUsageCyclesAppels = () => {
  const columnDefs = CyclesAppelsUsageColumns();
  const initialData = defCycleDataUsage();
  const newRowTemplate = {
    heure: "00:00:00",
    grp_ls: false,
    grp_rc: false,
    grp_pl: false,
    grp_ip: false,
  };

  return (
    <CyclesAppelsGrid
      initialData={initialData}
      columnDefs={columnDefs}
      newRowTemplate={newRowTemplate}
    />
  );
};

export default ResUsageCyclesAppels;