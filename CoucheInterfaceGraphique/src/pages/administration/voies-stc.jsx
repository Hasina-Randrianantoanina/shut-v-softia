"use client";
import Head from "next/head";
import { FaSuitcase } from "react-icons/fa6";
import { useState, useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import { FaChevronDown } from "react-icons/fa";

import VoieStc from "@/containers/VoieStc/VoieStc";

const VoiesStc = () => {
  const [isVisible, setIsVisible] = useState(false);
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

  const showFormNewVoie = () => {
    setIsVisible(!isVisible);
  };

  return (
    <>
      <Head>
        <title>Gestion des stations</title>
      </Head>
      <div className="p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaSuitcase className="text-white" />
          </div>
          Gestion des Stations
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
        <div className="border border-atoli_blue rounded-xl">
          <div className="flex items-center p-3 font-bold">Stations</div>
          <hr className="mt-6 mr-2 border-atoli_blue opacity-40" />
          <div className="mb-0 space-x-3 shadow-sm bg-slate-200 opacity-70 ">
            <div className="py-6 px-9">
              {!isVisible ? (
                <button
                  onClick={showFormNewVoie}
                  className="px-3 py-2 font-semibold text-white shadow-md bg-atoli_blue rounded-xl"
                >
                  Nouvelle voie
                </button>
              ) : (
                <div>
                  <button
                    onClick={showFormNewVoie}
                    className="px-3 py-2 font-semibold text-white bg-red-700 shadow-md rounded-xl" //style={{ display: "none" }}
                    aria-hidden="true"
                  >
                    <p>Annuler</p>
                  </button>
                </div>
              )}
            </div>

            {isVisible && (
              <form className="flex px-4" action="#" method="POST">
                <div className="py-4 rounded-md shadow-sm -space-y-px-12">
                  <label className="block p-2 font-bold text-black">Nom</label>
                  <input
                    type="text"
                    placeholder="Nom"
                    className="w-full py-2 pl-10 pr-10 font-bold bg-white border-2 border-gray-300 rounded-xl focus:outline-none focus:border-blue-500 text-atoli_blue "
                  />
                  <div>
                    <button
                      type="submit"
                      className="w-full py-2 mt-6 mb-6 font-bold text-white bg-green-600 border-2 rounded-xl"
                    >
                      Valider
                    </button>
                  </div>
                </div>
                <div className="px-12 py-4 -space-y-px rounded-md shadow-sm">
                  <label className="block p-2 font-bold text-black">Voie</label>
                  <div className="relative">
                    <select
                      value=""
                      className="w-full py-2 pl-10 pr-10 font-bold bg-white border-2 border-gray-300 appearance-none rounded-xl focus:outline-none focus:border-blue-500 text-atoli_blue"
                    >
                      <option className="font-bold text-atoli_blue" value="">
                        Out
                      </option>
                      <option value="Voie">Voie</option>
                      {/* fetch des user ici */}
                    </select>
                    <FaChevronDown className="absolute transform -translate-y-1/2 pointer-events-none right-3 top-1/2 text-atoli_blue" />
                  </div>
                </div>
                <div className="px-12 py-4 -space-y-px rounded-md shadow-sm">
                  <label className="block p-2 font-bold text-black">Type</label>
                  <div className="relative">
                    <select
                      value=""
                      className="w-full py-2 pl-10 pr-10 font-bold bg-white border-2 border-gray-300 appearance-none rounded-xl focus:outline-none focus:border-blue-500 text-atoli_blue"
                    >
                      <option className="font-bold text-atoli_blue" value="">
                        VI
                      </option>
                      <option value="type">type</option>
                      {/* fetch des user ici */}
                    </select>
                    <FaChevronDown className="absolute transform -translate-y-1/2 pointer-events-none right-3 top-1/2 text-atoli_blue" />
                  </div>
                </div>
              </form>
            )}
          </div>
        </div>

        <div className="border border-atoli_blue rounded-xl">
          <VoieStc />
        </div>
      </div>
    </>
  );
};

export default VoiesStc;
