"use client";
import Head from "next/head";
import { FaBoxArchive } from "react-icons/fa6";

const Integration = () => {
  return (
    <>
      <Head>
        <title>Intégration de données issues de fichiers locaux</title>
      </Head>
      <div className="p-4">
        <div className="flex items-center text-4xl font-bold">
          <div className="p-3 mr-6 rounded-full bg-atoli_blue">
            <FaBoxArchive className="text-white" />
          </div>
          Intégration de données issues de fichiers locaux
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
      </div>
    </>
  );
};

export default Integration;
