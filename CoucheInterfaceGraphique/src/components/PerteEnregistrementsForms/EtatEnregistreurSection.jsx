import React from "react";
import { formatDateForDisplay } from "@/utils/dateUtils";

export const EtatEnregistreurSection = ({
  etatEnregistreur,
  selectedPerte,
}) => {
  if (!etatEnregistreur || !selectedPerte) return null;

  const dateFields = [
    { key: "stop", label: "stop" },
    { key: "derTrf", label: "derTrf" },
    { key: "horl", label: "horl" },
    { key: "diffHorl", label: "diffHorl" },
    { key: "go", label: "go" },
    { key: "init", label: "init" },
    { key: "acq", label: "acq" },
    { key: "enrg", label: "enrg" },
  ];

  const formatDiffHorloge = (diff) => {
    const hours = Math.floor(Math.abs(diff) * 24);
    const minutes = Math.floor((Math.abs(diff) * 24 * 60) % 60);
    const seconds = Math.floor((Math.abs(diff) * 24 * 60 * 60) % 60);
    return `${hours.toString().padStart(2, "0")}h ${minutes
      .toString()
      .padStart(2, "0")}m ${seconds.toString().padStart(2, "0")}s`;
  };

  const getDateColor = (key) => {
    const typedef = selectedPerte.type;
    switch (typedef) {
      case "PB_DATE_GO":
        return key === "go" ? "text-red-500" : "";
      case "PB_DATE_INIT":
        return key === "init" ? "text-red-500" : "";
      case "PB_DATE_ACQ":
        return key === "acq" ? "text-red-500" : "";
      case "PB_DATE_ENREGISTREMENT":
        return key === "enrg" ? "text-red-500" : "";
      case "PB_ECART_HORLOGE":
        return key === "diffHorl" ? "text-red-500" : "";
      case "PB_DATE_GO_ET_DATE_INIT":
        return key === "go" || key === "init" ? "text-red-500" : "";
      default:
        return "";
    }
  };

  const getTaskColor = (index, etatDesTaches) => {
    if (!etatDesTaches) return "bg-slate-400 text-white";
    const taskState = etatDesTaches[index] || "";
    if (taskState === "5" || taskState === "7") {
      return "bg-shamrock_green text-white";
    } else if (taskState === "") {
      return "bg-slate-400 text-white";
    } else {
      return "bg-red-500 text-white";
    }
  };

  const reorganizedTasks = [
    "T2 Démarrage & Init",
    "T3 ModBus esclave",
    "T4 Gestion pile TCP",
    "T5 Gestion pile TCP(10ms)",
    "T6 Gestion des tempos",
    "T13 Enregistrement",
    "T16 Acq & traits spé",
    "T17 Dialogue Shut",
    "T36 Gestion appel ext",
    "T37 Surveil appel RC",
    "T39 Gestion vidage piles",
    "T40 Serveur FTP",
    "T41 Gestion sockets",
    "T42 Serveur Modbus",
    "T43 ModBus TCP",
    "T63 Gestion mémoire flash",
  ];

  return (
    <div className="p-6 mb-8 bg-slate-100 rounded-xl">
      <h1 className="mb-6 text-3xl font-bold text-atoli_blue">
        Etat enregistreur
      </h1>
      <p className="mb-3 text-lg font-medium">
        Version: {etatEnregistreur.version}
      </p>
      <div className="grid grid-cols-4 gap-4 mb-4">
        {dateFields.map(({ key, label }) => {
          const colorClass = getDateColor(key);
          return (
            <div key={key} className="p-3 bg-white rounded shadow">
              <p className={`font-semibold ${colorClass || "text-atoli_blue"}`}>
                {label}
              </p>
              <p className={colorClass || "text-gray-600"}>
                {key === "diffHorl"
                  ? formatDiffHorloge(etatEnregistreur.dates[key])
                  : formatDateForDisplay(etatEnregistreur.dates[key])}
              </p>
            </div>
          );
        })}
      </div>
      <div className="p-4 bg-white rounded shadow">
        <h4 className="mb-2 font-semibold text-gray-700">Tâches</h4>
        <div className="grid grid-cols-4 gap-2">
          {reorganizedTasks.map((tache, index) => (
            <div
              key={`tache-${index}`}
              className={`p-2 text-sm rounded ${getTaskColor(
                index,
                selectedPerte.etatDesTaches
              )}`}
            >
              {tache}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
