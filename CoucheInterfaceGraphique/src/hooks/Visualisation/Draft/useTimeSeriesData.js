import { useState, useCallback, useMemo } from 'react';
import { format } from "date-fns";
import { useHistoricalData } from "@/services/visualisationAPI";

const useTimeSeriesData = (selectedStation, dateRange, selectedTors, selectedAnas) => {
  const [chartData, setChartData] = useState(null);
  const [rowData, setRowData] = useState([]);
  const [filteredRowData, setFilteredRowData] = useState([]);
  const [visibleSeries, setVisibleSeries] = useState([]);

  const getHistoricalData = useHistoricalData();

  const getColor = useMemo(() => {
    const colors = ["#5470C6", "#91CC75", "#FAC858", "#EE6666", "#73C0DE", "#3BA272", "#FC8452", "#9A60B4", "#EA7CCC"];
    return (index) => colors[index % colors.length];
  }, []);

  const fetchData = useCallback(async () => {
    if (selectedTors.length === 0 && selectedAnas.length === 0) {
      alert("Veuillez sélectionner au moins une voie ANA ou TOR");
      return;
    }

    const startDateTime = format(dateRange[0], "yyyy-MM-dd HH:mm:ss");
    const endDateTime = format(dateRange[1], "yyyy-MM-dd HH:mm:ss");

    try {
      const selectedVoies = [...selectedTors, ...selectedAnas];
      const historicalData = await getHistoricalData(
        selectedStation,
        selectedVoies,
        startDateTime,
        endDateTime
      );
  
      console.log('Historical data:', historicalData);
      console.log('TOR data:', historicalData.filter(item => selectedTors.includes(item.libelle)));
  
      const libelleData = [...new Set(historicalData.map((t) => t.libelle))];
      setVisibleSeries(libelleData);
  
      const dataMap = new Map();
      historicalData.forEach(item => {
        if (!dataMap.has(item.libelle)) {
          dataMap.set(item.libelle, []);
        }
        dataMap.get(item.libelle).push({
          name: item.dateTime,
          value: [new Date(item.dateTime).getTime(), item.valeur]
        });
      });
  
      const yAxis = libelleData.map((libelle, index) => {
        const isTOR = selectedTors.includes(libelle);
        return {
          type: "value",
          name: libelle,
          position: index % 2 === 0 ? "left" : "right",
          offset: Math.floor(index / 2) * 50,
          axisLine: {
            show: true,
            lineStyle: {
              color: getColor(index),
            },
          },
          axisLabel: {
            formatter: isTOR ? "{value}" : "{value}",
          },
          min: isTOR ? 0 : undefined,
          max: isTOR ? 1 : undefined,
        };
      });
  
      const seriesData = libelleData.map((libelle, index) => {
        const isTOR = selectedTors.includes(libelle);
        const data = dataMap.get(libelle);
        if (!data || data.length === 0) {
          console.warn(`No data for series ${libelle}`);
          return null;
        }
        return {
          type: "line",
          smooth: !isTOR,
          symbol: "circle",
          symbolSize: 3,
          name: libelle,
          yAxisIndex: index,
          itemStyle: {
            color: getColor(index),
          },
          step: isTOR ? 'start' : false,
          data: data.sort((a, b) => a.value[0] - b.value[0]),
        };
      }).filter(Boolean);

      const option = {
        grid: {
          left: '3%',
          right: '4%',
          bottom: '3%',
          containLabel: true
        },
        tooltip: {
          trigger: "axis",
          axisPointer: {
            type: "cross",
          },
        },
        legend: {
          data: libelleData,
          selected: libelleData.reduce((acc, libelle) => {
            acc[libelle] = true;
            return acc;
          }, {}),
        },
        toolbox: {
          feature: {
            dataZoom: { yAxisIndex: "none" },
            restore: {},
            saveAsImage: {},
          },
        },
        xAxis: {
          type: "time",
          boundaryGap: false,
        },
        yAxis: yAxis,
        dataZoom: [
          {
            type: 'inside',
            start: 0,
            end: 100
          },
          {
            start: 0,
            end: 100
          }
        ],
        series: seriesData,
      };

      setChartData(option);
      setRowData(historicalData);
      setFilteredRowData(historicalData);
    } catch (error) {
      console.error("Erreur lors de la récupération des données:", error);
      alert("Une erreur est survenue lors de la récupération des données.");
    }
  }, [dateRange, selectedTors, selectedAnas, selectedStation, getHistoricalData, getColor]);

  
  
  const updateTableData = useCallback((chartInstance) => {
    if (chartInstance) {
      const option = chartInstance.getOption();
      const dataZoom = option.dataZoom[0];
      const xAxis = option.xAxis[0];
      
      let startValue, endValue;
  
      if (dataZoom.startValue && dataZoom.endValue) {
        startValue = dataZoom.startValue;
        endValue = dataZoom.endValue;
      } else {
        const start = dataZoom.start / 100;
        const end = dataZoom.end / 100;
        const data = xAxis.data;
        startValue = data[Math.floor(start * (data.length - 1))];
        endValue = data[Math.ceil(end * (data.length - 1))];
      }
  
      console.log("Zoom range:", startValue, endValue);
  
      const newFilteredData = rowData.filter((row) => {
        const rowDate = new Date(row.dateTime);
        return (
          rowDate >= new Date(startValue) &&
          rowDate <= new Date(endValue) &&
          option.legend[0].selected[row.libelle]
        );
      });
      setFilteredRowData(newFilteredData);
    }
  }, [rowData]);

  return {
    chartData,
    filteredRowData,
    visibleSeries,
    fetchData,
    updateTableData,
  };
};

export default useTimeSeriesData;
