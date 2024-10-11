

export const getVersionColor = (version, liaison, initiales) => {
  if (liaison === "IP") {
    if (version && version.includes('Y')) return "bg-sky-400"; // STEN IP
    else if (version && version.includes('Z')) return "bg-sky-500"; // STEN2
    else if (version && version.includes('W')) return "bg-sky-600"; // STEN1
    else return "bg-sky-300"; // Autre STEN
  } 
  else if (liaison === "AP") {
    if (initiales === "NU") return "bg-emerald-500"; // Automate M340
    else if (version === "MANQUE" || (version && version.includes('D13A53Aa01'))) return "bg-emerald-400"; // Automate Premium
    else if (version && version.includes('A')) return "bg-emerald-600"; // Automate M580
    else return "bg-emerald-300"; // Autre Automate ?
  }
  else if (liaison === "XX" || liaison === "RC") {
    return "bg-amber-300"; // ISODAQ
  }
  
  return "bg-slate-300"; // Pas de version
};
  
  export const capitalizeFirstTwo = (str) => {
    if (str.length <= 2) {
      return str.toUpperCase();
    }
    return str.slice(0, 2).toUpperCase() + str.slice(2).toLowerCase();
  };