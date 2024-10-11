"use client";
import Head from "next/head";
import { useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import { FaSuitcase } from "react-icons/fa6";

const FichiersTraces = () => {
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
        <title>Fichiers Traces</title>
      </Head>
      <div className="p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaSuitcase className="text-white" />
          </div>
          Visualisation et édition des fichiers traces
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
      </div>
    </>
  );
};

export default FichiersTraces;
