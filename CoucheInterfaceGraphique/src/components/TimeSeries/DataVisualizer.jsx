import React, {
  useState,
  useRef,
  useEffect,
  useMemo,
  useCallback,
} from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import * as echarts from "echarts";
import CustomButton from "@/components/CustomButton/CustomButton";
import { FaDownload } from "react-icons/fa";
import { formatDateForDisplay } from "@/utils/dateUtils";

const DataVisualizer = ({
  chartData,
  filteredRowData,
  updateTableData,
  visibleSeries,
  selectedTors = [],
  updateYAxisRange,
}) => {
  const chartRef = useRef(null);
  const gridRef = useRef(null);
  const [chart, setChart] = useState(null);
  const [thresholds, setThresholds] = useState([]);
  const [newThreshold, setNewThreshold] = useState({
    name: "",
    value: "",
    series: "",
  });
  const [availableSeries, setAvailableSeries] = useState([]);

  // Effet pour mettre à jour les séries disponibles lorsque les données du graphique changent
  useEffect(() => {
    if (chartData && chartData.series) {
      const seriesFromChart = chartData.series.map((s) => ({
        name: s.name,
        color: s.itemStyle.color,
      }));
      setAvailableSeries(seriesFromChart);
    }
  }, [chartData]);

  // Fonction pour restructurer les données filtrées pour l'affichage dans le tableau
  const restructureData = useCallback((filteredRowData) => {
    if (!filteredRowData || filteredRowData.length === 0)
      return { data: [], voies: [] };

    const voies = new Set();
    const dataMap = new Map();

    filteredRowData.forEach((row) => {
      voies.add(row.libelle);
      const timestamp = new Date(row.dateTime).getTime();
      if (!dataMap.has(timestamp)) {
        dataMap.set(timestamp, { dateTime: row.dateTime });
      }
      dataMap.get(timestamp)[row.libelle] = row.valeur;
    });

    const data = Array.from(dataMap.values()).sort(
      (a, b) => new Date(a.dateTime) - new Date(b.dateTime)
    );
    return { data, voies: Array.from(voies) };
  }, []);

  // Mémorisation des données restructurées et des voies
  const { data: restructuredData, voies } = useMemo(
    () => restructureData(filteredRowData),
    [filteredRowData, restructureData]
  );

  // Définition des colonnes pour le tableau AG-Grid
  const columnDefs = useMemo(() => {
    if (!voies || voies.length === 0) return [];
    return [
      {
        field: "dateTime",
        headerName: "Date et Heure",
        filter: true,
        sortable: true,
        resizable: true,
        minWidth: 180,
        valueFormatter: (params) => formatDateForDisplay (params.value),
      },
      ...voies.map((voie) => ({
        field: voie,
        headerName: voie,
        filter: "agNumberColumnFilter",
        sortable: true,
        flex: 1,
        resizable: true,
        valueFormatter: (params) =>
          params.value !== undefined ? params.value : "",
      })),
    ];
  }, [voies]);

  // Gestion des événements du graphique (changement de légende, zoom)
  const handleChartEvents = useCallback(() => {
    if (chart && updateTableData) {
      updateTableData(chart);

      const option = chart.getOption();
      const selectedSeries = option.series.filter(
        (s) => option.legend[0].selected[s.name]
      );
      updateYAxisRange(chart, selectedSeries);
    }
  }, [chart, updateTableData, updateYAxisRange]);

  // Fonction pour ajouter un nouveau seuil
  const addThreshold = () => {
    if (newThreshold.name && newThreshold.value && newThreshold.series) {
      const selectedSeries = availableSeries.find(
        (s) => s.name === newThreshold.series
      );
      if (!selectedSeries) return;

      const updatedThresholds = [
        ...thresholds,
        {
          ...newThreshold,
          color: selectedSeries.color,
        },
      ];
      setThresholds(updatedThresholds);
      setNewThreshold({ name: "", value: "", series: "" });
      updateChart(updatedThresholds);
    }
  };

  // Fonction pour mettre à jour le graphique avec les nouveaux seuils
  const updateChart = (updatedThresholds) => {
    if (chart && chartData) {
      const newOption = { ...chartData };
      newOption.series = [
        ...(chartData.series || []),
        ...updatedThresholds
          .map((threshold) => {
            const seriesIndex = chartData.series.findIndex(
              (s) => s.name === threshold.series
            );
            if (seriesIndex === -1) return null;
            return {
              name: threshold.name,
              type: "line",
              yAxisIndex: chartData.series[seriesIndex].yAxisIndex,
              markLine: {
                silent: true,
                symbol: ["none", "none"],
                label: { show: true },
                data: [
                  {
                    yAxis: parseFloat(threshold.value),
                    label: {
                      formatter: threshold.name,
                      position: "insideEndTop",
                    },
                  },
                ],
                lineStyle: {
                  color: threshold.color,
                  type: "dashed",
                  width: 2,
                },
              },
            };
          })
          .filter(Boolean),
      ];
      chart.setOption(newOption);
    }
  };

  // Effet pour initialiser et mettre à jour le graphique
  useEffect(() => {
    if (chartRef.current && chartData) {
      let currentChart = chart;
      if (!currentChart) {
        currentChart = echarts.init(chartRef.current);
        setChart(currentChart);
      }
  
      try {
        if (
          chartData.yAxis &&
          chartData.yAxis.length > 0 &&
          chartData.series &&
          chartData.series.length > 0
        ) {
          currentChart.setOption(chartData, true);
        } else {
          // Afficher un message ou un graphique vide
          currentChart.setOption({
            title: {
              text: 'Aucune donnée disponible pour la période sélectionnée',
              left: 'center',
              top: 'middle'
            }
          });
        }
      } catch (error) {
        console.error("Erreur lors de la configuration du graphique:", error);
      }
  
      // Gestion des événements du graphique
      currentChart.off("legendselectchanged");
      currentChart.off("datazoom");
      currentChart.on("legendselectchanged", handleChartEvents);
      currentChart.on("datazoom", handleChartEvents);
    }
  }, [chartData, chart, handleChartEvents]);

  // Effet pour gérer le redimensionnement de la fenêtre
  useEffect(() => {
    const handleResize = () => {
      if (chart) {
        chart.resize();
      }
    };
    window.addEventListener("resize", handleResize);
    return () => {
      window.removeEventListener("resize", handleResize);
      if (chart) {
        chart.dispose();
      }
    };
  }, [chart]);

  // Fonction pour exporter les données en CSV
  const onExportClick = useCallback(() => {
    if (gridRef.current && gridRef.current.api) {
      gridRef.current.api.exportDataAsCsv();
    }
  }, []);

  useEffect(() => {
    if (chart) {
      chart.on("datazoom", () => {
        updateTableData(chart);
      });

      return () => {
        chart.off("datazoom");
      };
    }
  }, [chart, updateTableData]);

  const hasEmptySeries = useMemo(() => {
    return chartData.series.some(series => series.data.every(point => point[1] === null));
  }, [chartData]);


  return (
    <div className="p-4 mt-8 border border-atoli_blue rounded-xl">
      <h2 className="mt-4 mb-4 text-xl font-bold">Données</h2>
      {hasEmptySeries && (
        <div className="p-2 mb-4 text-yellow-700 bg-yellow-100 border border-yellow-400 rounded">
          Attention : Certaines voies sélectionnées n'ont pas de données pour la période choisie.
        </div>
      )}
      <div className="p-2 mt-4 mb-4 shadow-sm opacity-80 rounded-xl">
        <div className="flex items-center justify-between mb-4"></div>

        {chartData && Object.keys(chartData).length > 0 && (
          <div className="mb-6" ref={chartRef} style={{ height: "700px" }} />
        )}

        <div className="flex justify-center mb-4 space-x-2">
          <input
            type="text"
            placeholder="Nom du seuil"
            value={newThreshold.name}
            onChange={(e) =>
              setNewThreshold({ ...newThreshold, name: e.target.value })
            }
            className="px-2 py-1 border-2 border-atoli_blue rounded-xl"
          />
          <input
            type="number"
            placeholder="Valeur du seuil"
            value={newThreshold.value}
            onChange={(e) =>
              setNewThreshold({ ...newThreshold, value: e.target.value })
            }
            className="px-2 py-1 border-2 border-atoli_blue rounded-xl"
          />
          <select
            value={newThreshold.series}
            onChange={(e) =>
              setNewThreshold({ ...newThreshold, series: e.target.value })
            }
            className="px-2 py-1 border-2 border-atoli_blue rounded-xl"
          >
            <option value="">Sélectionner une série</option>
            {availableSeries.length > 0 ? (
              availableSeries.map((series) => (
                <option key={series.name} value={series.name}>
                  {series.name}
                </option>
              ))
            ) : (
              <option disabled>Aucune série disponible</option>
            )}
          </select>
          <CustomButton onClick={addThreshold}>Ajouter un seuil</CustomButton>
        </div>
        <div className="flex justify-end mt-4">
          <CustomButton onClick={onExportClick} className="flex items-center space-x-2">
            <FaDownload className="w-4 h-4" />
            <span>.CSV</span>
          </CustomButton>
        </div>
        <div
          className="mt-6 ag-theme-quartz"
          style={{ height: 800, width: "100%" }}
        >
          <AgGridReact
            ref={gridRef}
            rowData={restructuredData}
            columnDefs={columnDefs}
            domLayout="normal"
            pagination={true}
            paginationPageSize={100}
            // paginationPageSizeSelector={[100, 500, 1000]}
            // rowBuffer={100}
            // cacheQuickFilter={true}
          />
        </div>
      </div>
    </div>
  );
};

export default DataVisualizer;
