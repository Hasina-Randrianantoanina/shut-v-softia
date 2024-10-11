"use client";
import Head from "next/head";
import { useState } from "react";
import { FaEye } from "react-icons/fa";
import UsageNetwork from "@/containers/DefautCapteur/ResUsageDefautCapteur";
import ObservationNetwork from "@/containers/DefautCapteur/ResObsDefautCapteur";
import ColorLegend from "@/components/ColorLegend/ColorLegend";

const DefautsCapteurs = () => {
  const [activeTab, setActiveTab] = useState("usage");
  return (
    <>
      <Head>
        <title>Défauts capteurs</title>
      </Head>
      <div className="flex flex-col h-full p-4">
        <div className="flex items-center justify-between text-4xl font-bold">
          <div className="flex items-center">
            <div className="p-3 mr-6 rounded-full bg-atoli_blue">
              <FaEye className="text-white" />
            </div>
            Défauts capteurs
          </div>
          <ColorLegend />
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />

        <div className="flex p-2 mx-4 mb-6 space-x-3 shadow-sm bg-slate-200 opacity-80 rounded-xl ">
          <button
            className={`px-3 py-2 ${
              activeTab === "usage"
                ? "bg-shamrock_green "
                : "bg-gray-200 text-gray-400"
            } rounded-xl font-bold shadow-md`}
            onClick={() => setActiveTab("usage")}
          >
            Réseau d'usage
          </button>
          <button
            className={`px-3 py-2 ${
              activeTab === "observation"
                ? "bg-shamrock_green"
                : "bg-gray-200 text-gray-400"
            } rounded-xl font-bold shadow-md`}
            onClick={() => setActiveTab("observation")}
          >
            Réseau d'observation
          </button>
        </div>

        <div className="">
          {activeTab === "usage" ? <UsageNetwork /> : <ObservationNetwork />}
        </div>
      </div>
    </>
  );
};

export default DefautsCapteurs;
