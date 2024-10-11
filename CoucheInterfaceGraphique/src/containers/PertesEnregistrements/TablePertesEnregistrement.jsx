import React from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import PertesEnregistrementsColumns from "@/components/TableColumnDefinition/PertesEnregistrements/PertesEnregistrementsColumns";

const TablePertesEnregistrement = ({ rowData, onEditRow, onDeleteRow }) => {
  const columnDefs = PertesEnregistrementsColumns({
    onDelete: onDeleteRow,
    onEdit: onEditRow,
  });

  return (
    <div className="flex flex-col p-4">
      <div className="ag-theme-quartz" style={{ flex: "1 1 auto" }}>
        <AgGridReact
          columnDefs={columnDefs}
          rowData={rowData}
          rowSelection="multiple"
          domLayout="autoHeight"
          suppressCellFocus={true}
          overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
        />
      </div>
    </div>
  );
};

export default TablePertesEnregistrement;
