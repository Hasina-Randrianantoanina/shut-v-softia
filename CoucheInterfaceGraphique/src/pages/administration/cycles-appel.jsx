"use client";
import Head from "next/head";
import { FaSuitcase } from "react-icons/fa6";
import { useState, useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import UsageNetwork from "@/containers/CyclesAppels/ResUsageCyclesAppels";
import ObservationNetwork from "@/containers/CyclesAppels/ResObsCyclesAppels";

const CyclesAppel = () => {
  const [activeTab, setActiveTab] = useState("USAGE");
  const { user, loading } = useAuth();
  const router = useRouter();

  const authorizedRoles = ["ADMIN"];

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
        <title>Gestion des cycles d'appels</title>
      </Head>
      <div className="p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaSuitcase className="text-white" />
          </div>
          Gestion des cycles d'appel
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
        <div className="flex p-2 mx-4 mb-6 space-x-3 shadow-sm bg-slate-200 opacity-80 rounded-xl ">
          <button
            className={`px-3 py-2 ${
              activeTab === "USAGE"
                ? "bg-shamrock_green "
                : "bg-gray-200 text-gray-400"
            } rounded-xl font-bold shadow-md`}
            onClick={() => setActiveTab("USAGE")}
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

        <div className="border border-atoli_blue rounded-xl">
          {activeTab === "USAGE" ? <UsageNetwork /> : <ObservationNetwork />}
        </div>
      </div>
    </>
  );
};

export default CyclesAppel;
