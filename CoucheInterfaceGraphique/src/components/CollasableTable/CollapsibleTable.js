import React, { useState, useRef, useEffect } from 'react';
import { AgGridReact } from "ag-grid-react";
import { FaChevronDown, FaChevronUp } from 'react-icons/fa';
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";

const CollapsibleTable = ({ title, columnDefs, rowData, extraContent, gridProps = {} }) => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const gridRef = useRef(null);

  const toggleCollapse = () => {
    setIsCollapsed(!isCollapsed);
  };

  useEffect(() => {
    if (gridRef.current && gridRef.current.api) {
      gridRef.current.api.sizeColumnsToFit();
    }
  }, [rowData, isCollapsed]);

  return (
    <div className="flex flex-col h-full p-4 border border-atoli_blue rounded-xl">
      <div 
        className="flex items-center justify-between mb-4 cursor-pointer" 
        onClick={toggleCollapse}
      >
        <h1 className="text-2xl font-bold">{title}</h1>
        {isCollapsed ? <FaChevronDown /> : <FaChevronUp />}
      </div>

      {!isCollapsed && (
        <>
          {extraContent}
          <div className="flex-grow ag-theme-quartz" style={{ height: '600px', width: '100%', overflow: 'hidden' }}>
            <AgGridReact
              ref={gridRef}
              columnDefs={columnDefs}
              rowData={rowData}
              domLayout='normal'
              suppressRowTransform={true}
              stopEditingWhenCellsLoseFocus={false}
              suppressClickEdit={true}
              rowHeight={80}
              suppressCellFocus={true}
              enableCellTextSelection={true}
              overlayNoRowsTemplate="<span class='text-gray-500'>Aucune donnée disponible</span>"
              headerHeight={60}
              {...gridProps}
            />
          </div>
        </>
      )}
    </div>
  );
};

export default CollapsibleTable;