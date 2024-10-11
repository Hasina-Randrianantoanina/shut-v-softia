"use client";
import Head from "next/head";
import { useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import { FaGear } from "react-icons/fa6";
import PreselectionContainer from "@/containers/AbonnementGroupePreselection/PreselectionContainer";
import ColorLegend from "@/components/ColorLegend/ColorLegend";

const Preselection = () => {
  const { user, loading } = useAuth();
  const router = useRouter();
  const authorizedRoles = ["ADMIN", "OPERATEUR", "VALIDEUR"];

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
        <title>Gestion des présélections de stations</title>
      </Head>
      <div className="flex flex-col h-full p-4">
      <div className="flex items-center justify-between text-4xl font-bold">
          <div className="flex items-center">
            <div className="p-3 mr-6 rounded-full bg-atoli_blue">
              <FaGear className="text-white" />
            </div>
            Gestion des présélections de stations
          </div>
          <ColorLegend />
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />

        <div className="flex-grow overflow-hidden border border-atoli_blue rounded-xl">
          <PreselectionContainer />
        </div>
      </div>
    </>
  );
};

export default Preselection;
