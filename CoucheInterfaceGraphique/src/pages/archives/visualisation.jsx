"use client";
import VisualisationContainer from "@/containers/Visualisation/VisualisationContainer";
import Head from "next/head";
import { FaBoxArchive } from "react-icons/fa6";
import ColorLegend from "@/components/ColorLegend/ColorLegend";

const Visualisation = () => {
  return (
    <>
      <Head>
        <title>Visualisation des données</title>
      </Head>
      <div className="p-4">
      <div className="flex items-center justify-between text-4xl font-bold">
          <div className="flex items-center">
            <div className="p-3 mr-6 rounded-full bg-atoli_blue">
              <FaBoxArchive className="text-white" />
            </div>
            Visualisation des données
          </div>
          <ColorLegend />
        </div>
        <hr className="mt-6 mb-4 ml-2 mr-2 border-t-2 border-atoli_blue opacity-40" />
        <VisualisationContainer />
      </div>
    </>
  );
};

export default Visualisation;