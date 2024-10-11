import { format } from 'date-fns';

export const getChartOptions = (seriesData, yAxis, libelleData) => {
  // Vérifier si les données sont vides
  if (!seriesData || seriesData.length === 0 || !yAxis || yAxis.length === 0) {
    return getEmptyChartOptions();
  }

  return {
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
};

// Fonction pour créer des options de graphique vides
const getEmptyChartOptions = () => ({
  title: {
    text: 'Aucune donnée disponible pour la période sélectionnée',
    left: 'center',
    top: 'middle'
  },
  grid: {},
  tooltip: {},
  legend: {},
  toolbox: {},
  xAxis: { type: 'time', data: [] },
  yAxis: [{ type: 'value', name: 'ANA' }, { type: 'value', name: 'TOR' }],
  series: []
});