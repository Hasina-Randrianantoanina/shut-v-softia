import { useState, useCallback, useMemo } from "react";
import { format } from "date-fns";
import {
  useHistoricalDataMultiple,
  useHistoricalData,
} from "@/services/visualisationAPI";

const useTimeSeriesDataMultiple = (
  selectedStation,
  dateRange,
  selectedTors,
  selectedAnas
) => {
  // États pour stocker les données du graphique et du tableau
  const [chartData, setChartData] = useState(null);
  const [rowData, setRowData] = useState([]);
  const [filteredRowData, setFilteredRowData] = useState([]);
  const [visibleSeries, setVisibleSeries] = useState([]);
  // Hooks persos pour récupérer les données historiques
  const getHistoricalDataMultiple = useHistoricalDataMultiple();
  const getHistoricalData = useHistoricalData();

  // Fonction pour obtenir une couleur en fonction de l'index
  const getColor = useMemo(() => {
    const colors = [
      "#5470C6",
      "#91CC75",
      "#FAC858",
      "#EE6666",
      "#73C0DE",
      "#3BA272",
      "#FC8452",
      "#9A60B4",
      "#EA7CCC",
    ];
    return (index) => colors[index % colors.length];
  }, []);

  // Fonction pour déterminer le type de série en fonction du libellé
  const getSeriesType = (libelle) => {
    const upperLibelle = libelle.toUpperCase();

    const typeChecks = [
      { type: "TOR", check: (l) => l.includes("ETOR") || l.includes("STOR") },
      {
        type: "NIVEAU",
        check: (l) =>
          l.startsWith("Y") || l.includes("CUMUL") || l.includes("POV"),
      },
      { type: "DEBIT", check: (l) => l.startsWith("Q") },
      { type: "TEMPERATURE", check: (l) => l.includes("TEMP") },
      {
        type: "TURBIDITE",
        check: (l) => l.startsWith("T") && !l.startsWith("TEMP"),
      },
      {
        type: "VITESSE",
        check: (l) => l.startsWith("V") && !l.startsWith("VOL"),
      },
      { type: "VOLUME", check: (l) => l.startsWith("VOL") },
      { type: "CONCENTRATION", check: (l) => l.includes("GAZ") },
    ];

    for (const { type, check } of typeChecks) {
      if (check(upperLibelle)) {
        return type;
      }
    }

    return "OTHER";
  };

  // Fonction pour récupérer et formater les données
  const fetchData = useCallback(async () => {
    if (selectedTors.length === 0 && selectedAnas.length === 0) {
      alert("Veuillez sélectionner au moins une voie ANA ou TOR");
      return;
    }

    const startDateTime = format(dateRange[0], "yyyy-MM-dd HH:mm:ss");
    const endDateTime = format(dateRange[1], "yyyy-MM-dd HH:mm:ss");

    try {
      const selectedVoies = [...selectedTors, ...selectedAnas];
      console.log("Fetching data for:", selectedVoies);
      let historicalData = await getHistoricalDataMultiple(
        selectedStation,
        selectedVoies,
        startDateTime,
        endDateTime
      );
      console.log("Received data count:", historicalData.length);

      const dataMap = new Map();
      const libelleSet = new Set();
      const typeMinMaxMap = new Map();
      const typeSet = new Set();

      // Traitement des données en une seule passe
      for (let i = 0; i < historicalData.length; i++) {
        const item = historicalData[i];
        const seriesType = getSeriesType(item.libelle);
        if (!dataMap.has(item.libelle)) {
          dataMap.set(item.libelle, []);
          libelleSet.add(item.libelle);
          typeSet.add(seriesType);
          if (!typeMinMaxMap.has(seriesType)) {
            typeMinMaxMap.set(seriesType, { min: Infinity, max: -Infinity });
          }
        }
        if (item.valeur != null && item.valeur !== "") {
          const value = parseFloat(item.valeur);
          dataMap.get(item.libelle).push({
            name: item.dateTime,
            value: [new Date(item.dateTime).getTime(), value],
          });
          const { min, max } = typeMinMaxMap.get(seriesType);
          typeMinMaxMap.set(seriesType, {
            min: Math.min(min, value),
            max: Math.max(max, value),
          });
        }
      }

      const libelleData = Array.from(libelleSet);
      setVisibleSeries(libelleData);

      const typeArray = Array.from(typeSet);
      const yAxis = typeArray.map((type, index) => {
        const { min, max } = typeMinMaxMap.get(type);
        return {
          type: "value",
          name: type,
          nameLocation: "middle",
          nameGap: 30,
          nameRotate: 90,
          position: index % 2 === 0 ? "left" : "right",
          offset: Math.floor(index / 2) * 80,
          axisLine: { show: true, lineStyle: { color: "#000000" } },
          axisLabel: {
            formatter: (value) => value.toFixed(2),
            color: "#000000",
          },
          min: type === "TOR" ? 0 : min,
          max: type === "TOR" ? 1 : max,
        };
      });

      const seriesData = [];
      for (let i = 0; i < libelleData.length; i++) {
        const libelle = libelleData[i];
        const seriesType = getSeriesType(libelle);
        const yAxisIndex = typeArray.indexOf(seriesType);
        seriesData.push({
          type: seriesType === "TOR" ? "line" : "scatter",
          smooth: false,
          symbol: "circle",
          symbolSize: 3,
          name: libelle,
          yAxisIndex: yAxisIndex,
          itemStyle: { color: getColor(i) },
          seriesColor: getColor(i),
          step: seriesType === "TOR" ? "start" : false,
          data: dataMap.get(libelle).sort((a, b) => a.value[0] - b.value[0]),
          markPoint: {
            data: [
              { type: "min", name: "Min" },
              { type: "max", name: "Max" },
            ],
          },
          progressive: 400, // Rendu progressif
          progressiveThreshold: 3000,
          large: true,
          largeThreshold: 5000,
        });
      }

      // Configuration des options du graphique
      const option = {
        grid: { left: "3%", right: "4%", bottom: "10%", containLabel: true },
        tooltip: { trigger: "axis", axisPointer: { type: "cross" } },
        legend: {
          data: libelleData,
          selected: Object.fromEntries(
            libelleData.map((libelle) => [libelle, true])
          ),
        },
        toolbox: {
          feature: {
            dataZoom: { yAxisIndex: "all" },
            restore: {},
            saveAsImage: {},
          },
        },
        xAxis: {
          type: "time",
          boundaryGap: false,
          axisLabel: {
            formatter: (value) => {
              const date = new Date(value);
              return [
                format(date, "dd/MM/yyyy"),
                format(date, "HH:mm:ss"),
              ].join("\n");
            },
            rotate: 0,
            interval: 0,
            align: "center",
            verticalAlign: "top",
            lineHeight: 16,
            padding: [8, 0, 0, 0],
            rich: {
              date: { lineHeight: 20, align: "center" },
              time: { lineHeight: 20, align: "center" },
            },
          },
          splitLine: { show: true },
          axisTick: { alignWithLabel: true, interval: "auto" },
        },
        yAxis: yAxis,
        dataZoom: [
          {
            type: "slider",
            start: 0,
            end: 100,
            xAxisIndex: [0],
            filterMode: "filter",
            bottom: 20,
            height: 30,
          },
          {
            type: "slider",
            start: 0,
            end: 100,
            yAxisIndex: yAxis.map((_, index) => index),
            filterMode: "empty",
          },
        ],
        series: seriesData,
      };

      setChartData(option);
      setRowData(historicalData);
      setFilteredRowData(historicalData);
    } catch (error) {
      console.error("Error in fetchData:", error);
      alert("Une erreur est survenue lors de la récupération des données.");
    }
  }, [
    dateRange,
    selectedTors,
    selectedAnas,
    selectedStation,
    getHistoricalDataMultiple,
    getColor,
  ]);

  // Fonction pour mettre à jour les données du tableau en fonction du zoom et de la sélection
  const updateTableData = useCallback(
    (chartInstance) => {
      if (chartInstance) {
        const option = chartInstance.getOption();
        const dataZoom = option.dataZoom[0];
        const xAxis = option.xAxis[0];

        let startValue = dataZoom.startValue || xAxis.data[0];
        let endValue = dataZoom.endValue || xAxis.data[xAxis.data.length - 1];

        // Convertir les valeurs en timestamps si nécessaire
        startValue =
          typeof startValue === "string"
            ? new Date(startValue).getTime()
            : startValue;
        endValue =
          typeof endValue === "string"
            ? new Date(endValue).getTime()
            : endValue;

        const newFilteredData = rowData.filter((row) => {
          const rowDate = new Date(row.dateTime).getTime();
          return (
            rowDate >= startValue &&
            rowDate <= endValue &&
            option.legend[0].selected[row.libelle]
          );
        });

        console.log(`Filtered data count: ${newFilteredData.length}`);
        setFilteredRowData(newFilteredData);
      }
    },
    [rowData]
  );

  // Fonction pour mettre à jour la plage de l'axe Y en fonction des séries sélectionnées
  const updateYAxisRange = useCallback((chart, selectedSeries) => {
    if (!chart) return;
    const option = chart.getOption();
    const updatedYAxis = option.yAxis.map((axis, index) => {
      // Calcul des nouvelles valeurs min et max pour chaque axe Y
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
    chartData,
    filteredRowData,
    visibleSeries,
    fetchData,
    updateTableData,
    updateYAxisRange,
  };
};

export default useTimeSeriesDataMultiple;
