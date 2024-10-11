export const STATION_TYPES = {
  STEN_IP: "STEN IP",
  STEN2: "STEN2",
  AUTOMATE_M340_M580: "Automate M340 et M580",
  AUTOMATE_PREMIUM: "Automate Premium",
  ISODAQ: "ISODAQ",
};

export const getStationTypes = (allStations) => {
  if (!allStations) return [];
  return Object.entries(STATION_TYPES).map(([key, label]) => ({
    value: `type_${key}`,
    label,
    stations: allStations.filter((station) => {
      const { enregistreurVersion, liaison, initiales } = station;
      switch (key) {
        case "STEN_IP":
          return (
            liaison === "IP" &&
            enregistreurVersion &&
            enregistreurVersion.includes("Y")
          );
        case "STEN2":
          return (
            liaison === "IP" &&
            enregistreurVersion &&
            enregistreurVersion.includes("Z")
          );
        case "AUTOMATE_M340_M580":
          return (
            liaison === "AP" &&
            (initiales === "NU" ||
              (enregistreurVersion && enregistreurVersion.includes("A")))
          );
        case "AUTOMATE_PREMIUM":
          return (
            liaison === "AP" &&
            (enregistreurVersion === "MANQUE" ||
              (enregistreurVersion &&
                enregistreurVersion.includes("D13A53Aa01")))
          );
        case "ISODAQ":
          return liaison === "XX" || liaison === "RC";
        default:
          return false;
      }
    }),
  }));
};

export const getStationTypesByDetails = (stationDetails) => {
  if (!stationDetails) return [];
  return Object.entries(STATION_TYPES).map(([key, label]) => ({
    value: `type_${key}`,
    label,
    stations: stationDetails.filter((station) => {
      const { enregistreurVersion, enregistreurLiaison, initiales } = station;
      switch (key) {
        case "STEN_IP":
          return (
            enregistreurLiaison === "IP" &&
            enregistreurVersion &&
            enregistreurVersion.includes("Y")
          );
        case "STEN2":
          return (
            enregistreurLiaison === "IP" &&
            enregistreurVersion &&
            enregistreurVersion.includes("Z")
          );
        case "AUTOMATE_M340_M580":
          return (
            enregistreurLiaison === "AP" &&
            (initiales === "NU" ||
              (enregistreurVersion && enregistreurVersion.includes("A")))
          );
        case "AUTOMATE_PREMIUM":
          return (
            enregistreurLiaison === "AP" &&
            (enregistreurVersion === "MANQUE" ||
              (enregistreurVersion &&
                enregistreurVersion.includes("D13A53Aa01")))
          );
        case "ISODAQ":
          return enregistreurLiaison === "XX" || enregistreurLiaison === "RC";
        default:
          return false;
      }
    }),
  }));
};

export const getDetailedEquipmentType = (station) => {
  const { enregistreurVersion, liaison, initiales } = station;
  if (liaison === "IP") {
    if (enregistreurVersion && enregistreurVersion.includes("Y"))
      return "STEN IP";
    if (enregistreurVersion && enregistreurVersion.includes("Z"))
      return "STEN2";
    if (enregistreurVersion && enregistreurVersion.includes("W"))
      return "STEN1";
    return "Autre STEN";
  }
  if (liaison === "AP") {
    if (initiales === "NU") return "Automate M340";
    if (
      enregistreurVersion === "MANQUE" ||
      (enregistreurVersion && enregistreurVersion.includes("D13A53Aa01"))
    )
      return "Automate Premium";
    if (enregistreurVersion && enregistreurVersion.includes("A"))
      return "Automate M580";
    return "Autre Automate";
  }
  if (liaison === "XX" || liaison === "RC") {
    return "ISODAQ";
  }
  return "Inconnu";
};

export const TABLE_HEIGHT = 400;
export const TABLE_MARGIN = "mb-8";
