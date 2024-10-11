import Head from "next/head";
import { useEffect } from "react";
import { useRouter } from "next/router";
import { useAuth } from "@/contexts/AuthContext";
import Utilisateur from "@/containers/Utilisateurs/Utilisateur";
import { FaSuitcase } from "react-icons/fa6";

const Utilisateurs = () => {
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
        <title>Gestion des utilisateur</title>
      </Head>
      <div className="flex flex-col h-full p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaSuitcase className="text-white" />
          </div>
          Gestion des utilisateurs
        </div>
        <hr className="my-2 mt-6 mb-6 border-t-2 border-atoli_blue opacity-40" />
        <div className="flex-grow overflow-hidden border border-atoli_blue rounded-xl">
          <Utilisateur />
        </div>
      </div>
    </>
  );
};

export default Utilisateurs;
