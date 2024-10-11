import { useReducer, useCallback, useMemo } from "react";
import { useDataFetching } from "@/hooks/Visualisation/useDataFetching";
import { processData } from "@/utils/Visualisation/dataProcessing";
import { getChartOptions } from "@/utils/Visualisation/chartOptions";

const initialState = {
  chartData: null,
  rowData: [],
  filteredRowData: [],
  visibleSeries: [],
  processedData: null,
};

function reducer(state, action) {
  switch (action.type) {
    case "SET_CHART_DATA":
      return { ...state, chartData: action.payload };
    case "SET_ROW_DATA":
      return {
        ...state,
        rowData: action.payload,
        filteredRowData: action.payload,
      };
    case "SET_VISIBLE_SERIES":
      return { ...state, visibleSeries: action.payload };
    case "UPDATE_FILTERED_DATA":
      return { ...state, filteredRowData: action.payload };
    case "SET_PROCESSED_DATA":
      return { ...state, processedData: action.payload };
    default:
      throw new Error();
  }
}

const useTimeSeries = (
  selectedStation,
  dateRange,
  selectedTors,
  selectedAnas
) => {
  const [state, dispatch] = useReducer(reducer, initialState);
  const fetchData = useDataFetching(
    selectedStation,
    dateRange,
    selectedTors,
    selectedAnas
  );

  const handleFetchData = useCallback(async () => {
    try {
      const historicalData = await fetchData();
  
      // Filtrer les données en fonction de la plage de dates sélectionnée
      const filteredData = historicalData.filter((data) => {
        const dataDate = new Date(data.dateTime).getTime();
        return (
          dataDate >= dateRange[0].getTime() &&
          dataDate <= dateRange[1].getTime()
        );
      });
  
      const processedData = processData(filteredData);
      dispatch({ type: "SET_PROCESSED_DATA", payload: processedData });
  
      const { seriesData, yAxis, libelleData } = processedData;
      const chartOptions = getChartOptions(seriesData, yAxis, libelleData);
  
      dispatch({ type: "SET_CHART_DATA", payload: chartOptions });
      dispatch({ type: "SET_ROW_DATA", payload: filteredData });
      dispatch({ type: "SET_VISIBLE_SERIES", payload: libelleData });
    } catch (error) {
      console.error("Error in fetchData:", error);
      dispatch({ type: "SET_CHART_DATA", payload: getEmptyChartOptions() });
      dispatch({ type: "SET_ROW_DATA", payload: [] });
      dispatch({ type: "SET_VISIBLE_SERIES", payload: [] });
      alert(error.message || "Une erreur est survenue lors de la récupération des données.");
    }
  }, [fetchData, dateRange]);

  // Fonction pour créer des options de graphique vides
  const getEmptyChartOptions = () => ({
    grid: {},
    tooltip: {},
    legend: {},
    toolbox: {},
    xAxis: { type: "time", data: [] },
    yAxis: [
      { type: "value", name: "ANA" },
      { type: "value", name: "TOR" },
    ],
    series: [],
  });

  // Memo processedData
  const memoProcessedData = useMemo(
    () => state.processedData,
    [state.processedData]
  );

  const updateTableData = useCallback(
    (chartInstance) => {
      if (chartInstance) {
        const option = chartInstance.getOption();
        const dataZoom = option.dataZoom[0];
        const xAxis = option.xAxis[0];

        let startValue = dataZoom.startValue || xAxis.data[0];
        let endValue = dataZoom.endValue || xAxis.data[xAxis.data.length - 1];

        startValue =
          typeof startValue === "string"
            ? new Date(startValue).getTime()
            : startValue;
        endValue =
          typeof endValue === "string"
            ? new Date(endValue).getTime()
            : endValue;

        const newFilteredData = state.rowData.filter((row) => {
          const rowDate = new Date(row.dateTime).getTime();
          return (
            rowDate >= startValue &&
            rowDate <= endValue &&
            option.legend[0].selected[row.libelle]
          );
        });

        console.log(`Filtered data count: ${newFilteredData.length}`);
        dispatch({ type: "UPDATE_FILTERED_DATA", payload: newFilteredData });
      }
    },
    [state.rowData]
  );

  const updateYAxisRange = useCallback((chart, selectedSeries) => {
    if (!chart) return;
    const option = chart.getOption();
    const updatedYAxis = option.yAxis.map((axis, index) => {
      const seriesOfType = selectedSeries.filter((s) => s.yAxisIndex === index);
      if (seriesOfType.length === 0) return axis;

      let min = Infinity;
      let max = -Infinity;

      for (const series of seriesOfType) {
        for (const point of series.data) {
          const value = point.value[1];
          if (value < min) min = value;
          if (value > max) max = value;
        }
      }

      return {
        ...axis,
        min: axis.name === "TOR" ? 0 : min,
        max: axis.name === "TOR" ? 1 : max,
      };
    });

    chart.setOption({ yAxis: updatedYAxis });
  }, []);

  return {
    ...state,
    processedData: memoProcessedData,
    fetchData: handleFetchData,
    updateTableData,
    updateYAxisRange,
  };
};

export default useTimeSeries;
