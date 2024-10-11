import React, {
  useCallback,
  useMemo,
  useRef,
  useEffect,
  useState,
} from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { useVoies } from "@/services/visualisationAPI";
import CustomButton from "@/components/CustomButton/CustomButton";
import { capitalizeFirstTwo } from "@/utils/colorStaUtils";

const VoiesSelector = ({
  selectedStations,
  selectedTors,
  setSelectedTors,
  selectedAnas,
  setSelectedAnas,
  onFetchData,
}) => {
  const {
    data: voiesData,
    isLoading: voiesLoading,
    error,
  } = useVoies(selectedStations);
  const [selectedPreset, setSelectedPreset] = useState("aucun");
  const anaGridRef = useRef(null);
  const torGridRef = useRef(null);

  const presets = [
    { value: "aucun", label: "" },
    { value: "preset1", label: "Présélection 1" },
    { value: "preset2", label: "Présélection 2" },
    { value: "preset3", label: "Présélection 3" },
    { value: "preset4", label: "Présélection 4" },
    { value: "preset5", label: "Présélection 5" },
    { value: "preset6", label: "Présélection 6" },
  ];

  const columnDefs = useMemo(
    () => [
      {
        field: "station",
        headerName: "Station",
        flex: 1,
        cellRenderer: (params) => capitalizeFirstTwo(params.value),
        cellStyle: { fontWeight: "bold" },
        width: 50,
      },
      {
        field: "libelle",
        headerName: "Libellé",
        checkboxSelection: true,
        flex: 3,
      },
    ],
    []
  );

  const { anaRowData, torRowData } = useMemo(() => {
    if (!voiesData) return { anaRowData: [], torRowData: [] };
    const ana = [];
    const tor = [];
    voiesData.forEach((stationData) => {
      stationData.analogVoies.forEach((voie) =>
        ana.push({ station: stationData.name, libelle: voie })
      );
      stationData.torVoies.forEach((voie) =>
        tor.push({ station: stationData.name, libelle: voie })
      );
    });
    return { anaRowData: ana, torRowData: tor };
  }, [voiesData]);

  const onAnaRowSelected = useCallback(
    (event) => {
      const selectedRows = event.api.getSelectedRows();
      setSelectedAnas(
        selectedRows.map((row) => ({
          station: row.station,
          libelle: row.libelle,
        }))
      );
    },
    [setSelectedAnas]
  );

  const onTorRowSelected = useCallback(
    (event) => {
      const selectedRows = event.api.getSelectedRows();
      setSelectedTors(
        selectedRows.map((row) => ({
          station: row.station,
          libelle: row.libelle,
        }))
      );
    },
    [setSelectedTors]
  );

  useEffect(() => {
    if (anaGridRef.current && anaGridRef.current.api) {
      anaGridRef.current.api.forEachNode((node) => {
        const isSelected = selectedAnas.some(
          (item) =>
            item.station === node.data.station &&
            item.libelle === node.data.libelle
        );
        node.setSelected(isSelected);
      });
    }
  }, [selectedAnas]);

  useEffect(() => {
    if (torGridRef.current && torGridRef.current.api) {
      torGridRef.current.api.forEachNode((node) => {
        const isSelected = selectedTors.some(
          (item) =>
            item.station === node.data.station &&
            item.libelle === node.data.libelle
        );
        node.setSelected(isSelected);
      });
    }
  }, [selectedTors]);

  if (error) {
    console.error("Error fetching voies:", error);
    return <div>Erreur lors du chargement des voies: {error.message}</div>;
  }

  return (
    <div className="mr-4 lg:w-3/5">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-bold">Sélection des voies</h2>
        <div className="flex items-center">
          <label htmlFor="preselection" className="mr-2 text-sm font-bold">
            Choisir une présélection :
          </label>
          <select
            id="preselection"
            value={selectedPreset}
            onChange={(e) => setSelectedPreset(e.target.value)}
            className="p-2 text-sm font-semibold border rounded-xl border-atoli_blue"
          >
            {presets.map((preset) => (
              <option key={preset.value} value={preset.value}>
                {preset.label}
              </option>
            ))}
          </select>
        </div>
      </div>
      <div className="flex flex-col lg:flex-row lg:space-x-4">
        <div className="mb-4 lg:w-1/2">
          <h3 className="mb-2 text-lg font-semibold">Voies analogiques</h3>
          {voiesLoading ? (
            <p>Chargement des ANAs...</p>
          ) : (
            <div className="ag-theme-quartz" style={{ height: 475 }}>
              <AgGridReact
                ref={anaGridRef}
                rowData={anaRowData}
                columnDefs={columnDefs}
                rowSelection="multiple"
                onSelectionChanged={onAnaRowSelected}
                suppressRowClickSelection={false}
                rowMultiSelectWithClick={true}
              />
            </div>
          )}
        </div>
        <div className="mb-4 lg:w-1/2">
          <h3 className="mb-2 text-lg font-semibold">Voies TOR</h3>
          {voiesLoading ? (
            <p>Chargement des TORs...</p>
          ) : (
            <div className="ag-theme-quartz" style={{ height: 475 }}>
              <AgGridReact
                ref={torGridRef}
                rowData={torRowData}
                columnDefs={columnDefs}
                rowSelection="multiple"
                onSelectionChanged={onTorRowSelected}
                suppressRowClickSelection={false}
                rowMultiSelectWithClick={true}
              />
            </div>
          )}
        </div>
      </div>
      <div className="flex justify-center mt-2">
        <CustomButton variant="green" onClick={onFetchData}>
          Visualiser
        </CustomButton>
      </div>
    </div>
  );
};

export default VoiesSelector;
