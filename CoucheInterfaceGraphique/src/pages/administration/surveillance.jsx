"use client";
import Head from "next/head";
import { useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import { FaSuitcase } from "react-icons/fa6";
import Surveillances from "@/containers/Surveillance/Surveillances";

const Surveillance = () => {
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
        <title>Surveillance</title>
      </Head>
      <div className="p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaSuitcase className="text-white" />
          </div>
          Enregistreur en temps réel
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
        <div className="border border-atoli_blue rounded-xl">
          <div className="flex items-center p-3 font-bold">
            Connexions VisuNet en cours
          </div>
          <hr className="mt-6 mr-2 border-atoli_blue opacity-40" />
          <div className="mb-0 space-x-3 shadow-sm bg-slate-200 opacity-70 ">
            <div className="flex flex-col p-2">
              <div className="p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
                <div className="flex items-center space-x-3">
                  Port série utilisé pour la connexion locale :
                  <input
                    type="text"
                    placeholder=""
                    className="px-3 py-2 font-semibold text-black border-2 rounded-lg border-atoli_blue"
                  />
                  <button
                    type="submit"
                    className="px-3 py-2 font-semibold text-white bg-green-700 shadow-md rounded-xl"
                  >
                    Valider
                  </button>
                </div>
                <div className="flex items-center space-x-3">
                  VisuNet n'est pas démarré
                </div>
              </div>
            </div>
          </div>
        </div>
        <div className="border border-atoli_blue rounded-xl">
          <Surveillances />
        </div>
      </div>
    </>
  );
};

export default Surveillance;
