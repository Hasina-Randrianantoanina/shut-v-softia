"use client";
import Head from "next/head";
import { useState, useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import { FaGear } from "react-icons/fa6";
import UsageNetwork from "@/containers/Stations/ResUsageStations";
import ObservationNetwork from "@/containers/Stations/ResObsStations";
import ColorLegend from "@/components/ColorLegend/ColorLegend";

const Stations= () => {
  const [activeTab, setActiveTab] = useState("usage");
  const { user, loading } = useAuth();
  const router = useRouter();

  const authorizedRoles = ["ADMIN", "OPERATEUR"];

  useEffect(() => {
    if (!loading && (!user || !authorizedRoles.includes(user.role))) {
      router.push("/unauthorized");
    }
  }, [user, loading, router]);

  if (loading) {
    return <div>Chargement...</div>;
  }

  if (!user || !authorizedRoles.includes(user.role)) {
    return null;
  }

  return (
    <>
      <Head>
        <title>Gestion des stations</title>
      </Head>
      <div className="flex flex-col h-full p-4">
      <div className="flex items-center justify-between text-4xl font-bold">
          <div className="flex items-center">
            <div className="p-3 mr-6 rounded-full bg-atoli_blue">
              <FaGear className="text-white" />
            </div>
          Gestion des stations
          </div>
          <ColorLegend />
        </div>
        <hr className="my-2 mt-6 mb-6 border-t-2 border-atoli_blue opacity-40" />

        <div className="flex p-2 mx-4 mb-6 space-x-3 shadow-sm bg-slate-200 opacity-80 rounded-xl">
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

        <div className="flex-grow overflow-hidden border border-atoli_blue rounded-xl">
          {activeTab === "usage" ? <UsageNetwork /> : <ObservationNetwork />}
        </div>
      </div>
    </>
  );
};

export default Stations;
