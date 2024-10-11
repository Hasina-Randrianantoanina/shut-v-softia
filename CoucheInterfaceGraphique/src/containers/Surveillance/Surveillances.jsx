import React, { useState, useCallback } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import DefDataSurveillance from "@/components/TableData/Surveillance/DefDataSurveillance";
import DefColumnSurveillance from "@/components/TableColumnDefinition/Surveillance/DefColumnSurveillance";

const Surveillances = () => {
  const columnDefs = DefColumnSurveillance();
  const rowData = DefDataSurveillance();

  return (
    <div className="flex flex-col p-3">
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

export default Surveillances;