import React, { useState, useCallback } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import DefDataVoieStc from "@/components/TableData/VoieStc/DefDataVoieStc";
import DefColVoieStc from "@/components/TableColumnDefinition/VoieStc/DefColVoieStc";

const VoieStc = () => {
  const columnDefs = DefColVoieStc();
  const rowData = DefDataVoieStc();

  return (
    <div className="flex flex-col p-4">
      <div className="ag-theme-quartz" style={{ flex: "1 1 auto" }}>
        <AgGridReact
          columnDefs={columnDefs}
          rowData={rowData}
          domLayout="autoHeight"
          suppressRowTransform={true}
          stopEditingWhenCellsLoseFocus={false}
          suppressClickEdit={true}
          rowHeight={80}
          suppressCellFocus={true}
          enableCellTextSelection={true}
        />
      </div>
    </div>
  );
};

export default VoieStc;