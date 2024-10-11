import { getSeriesType } from "@/utils/Visualisation/seriesUtils";
import { getColor } from "@/utils/Visualisation/colorUtils";

export const processData = (historicalData) => {
  const dataMap = new Map();
  const libelleSet = new Set();
  const typeMinMaxMap = new Map();
  const typeSet = new Set();
  let minDate = Infinity;
  let maxDate = -Infinity;

  // Traitement des données en une seule passe
  historicalData.forEach(item => {
    const seriesType = getSeriesType(item.libelle);
    if (!dataMap.has(item.libelle)) {
      dataMap.set(item.libelle, []);
      libelleSet.add(item.libelle);
      typeSet.add(seriesType);
      if (!typeMinMaxMap.has(seriesType)) {
        typeMinMaxMap.set(seriesType, { min: Infinity, max: -Infinity });
      }
    }
    const timestamp = new Date(item.dateTime).getTime();
    minDate = Math.min(minDate, timestamp);
    maxDate = Math.max(maxDate, timestamp);
    if (item.valeur != null && item.valeur !== "") {
      const value = parseFloat(item.valeur);
      dataMap.get(item.libelle).push({
        name: item.dateTime,
        value: [timestamp, value],
      });
      const { min, max } = typeMinMaxMap.get(seriesType);
      typeMinMaxMap.set(seriesType, {
        min: Math.min(min, value),
        max: Math.max(max, value),
      });
    }
  });

  const libelleData = Array.from(libelleSet);
  const typeArray = Array.from(typeSet);

  // Fonction pour remplir les données manquantes
  const fillMissingData = (data) => {
    const filledData = [];
    let lastTimestamp = minDate;
    data.forEach(point => {
      if (point.value[0] > lastTimestamp) {
        filledData.push({
          name: new Date(lastTimestamp).toISOString(),
          value: [lastTimestamp, null]
        });
      }
      filledData.push(point);
      lastTimestamp = point.value[0];
    });
    if (lastTimestamp < maxDate) {
      filledData.push({
        name: new Date(maxDate).toISOString(),
        value: [maxDate, null]
      });
    }
    return filledData;
  };

  const yAxis = typeArray.map((type, index) => {
    const { min, max } = typeMinMaxMap.get(type);
    return {
      type: "value",
      name: type,
      nameLocation: "middle",
      nameGap: 50,
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

  const seriesData = libelleData.map((libelle, index) => {
    const seriesType = getSeriesType(libelle);
    const yAxisIndex = typeArray.indexOf(seriesType);
    const rawData = dataMap.get(libelle).sort((a, b) => a.value[0] - b.value[0]);
    const filledData = fillMissingData(rawData);
    return {
      type: seriesType === "TOR" ? "line" : "scatter",
      smooth: false,
      symbol: "circle",
      symbolSize: 3,
      name: libelle,
      yAxisIndex: yAxisIndex,
      itemStyle: { color: getColor(index) },
      seriesColor: getColor(index),
      step: seriesType === "TOR" ? "start" : false,
      data: filledData,
      markPoint: {
        data: [
          { type: "min", name: "Min" },
          { type: "max", name: "Max" },
        ],
      },
      progressive: 400,
      progressiveThreshold: 3000,
      large: true,
      largeThreshold: 5000,
      connectNulls: false,  // Ne pas connecter les points nuls
    };
  });

  return {
    seriesData,
    yAxis,
    libelleData
  };
};