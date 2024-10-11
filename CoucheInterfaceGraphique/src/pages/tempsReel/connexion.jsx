"use client";
import Head from "next/head";
import { FaEye } from "react-icons/fa";

const Connexion = () => {
  return (
    <>

      <Head>
        <title>Connexion temps réel</title>
      </Head>
      <div className="p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaEye className="text-white" />
          </div>
          Connexion en temps réel aux enregistreurs
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
      </div>

    </>
  );
};

export default Connexion;
