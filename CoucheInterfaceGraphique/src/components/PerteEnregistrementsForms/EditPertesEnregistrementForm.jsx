import React from "react";
import CustomButton from "@/components/CustomButton/CustomButton";
import { formatDateForDateTimeLocal } from "@/utils/dateUtils";

export const EditPertesEnregistrementForm = ({
  formData,
  handleChange,
  handleSubmit,
  isAddMode,
  onClose,
  enumOption,
}) => {
  return (
    <div className="p-6 rounded-xl bg-slate-100">
      <h1 className="mb-6 text-3xl font-bold text-atoli_blue">
        {isAddMode
          ? "Création d'une perte d'enregistrement"
          : "Édition perte d'enregistrement"}
      </h1>
      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="grid grid-cols-3 gap-6">
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Début
            </label>
            <input
              type="datetime-local"
              name="debut"
              value={formatDateForDateTimeLocal(formData.debut)}
              onChange={(e) => {
                handleChange({
                  target: {
                    name: "debut",
                    value: new Date(e.target.value).toISOString(),
                  },
                });
              }}
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Fin
            </label>
            <input
              type="datetime-local"
              name="fin"
              value={formatDateForDateTimeLocal(formData.fin)}
              onChange={(e) => {
                handleChange({
                  target: {
                    name: "fin",
                    value: new Date(e.target.value).toISOString(),
                  },
                });
              }}
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Durée
            </label>
            <input
              type="text"
              name="duree"
              value={formData.durationDisplay}
              readOnly
              className="w-full p-2 bg-gray-100 border border-gray-300 rounded-md shadow-sm"
            />
          </div>
        </div>
        <div className="grid grid-cols-3 gap-6">
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Défaut
            </label>
            <select
              name="defaut"
              value={formData.defaut}
              onChange={handleChange}
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            >
              {enumOption.defaut.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Cause
            </label>
            <select
              name="cause"
              value={formData.cause}
              onChange={handleChange}
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            >
              {enumOption.cause.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Remède
            </label>
            <select
              name="remede"
              value={formData.remede}
              onChange={handleChange}
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            >
              {enumOption.remede.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </div>
        </div>
        <div className="flex items-center space-x-6">
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Evènement en cours
            </label>
            <div className="w-full p-2 bg-gray-100 border border-gray-300 rounded-md shadow-sm">
              {enumOption.evt[0]}
            </div>
          </div>
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Perte critique
            </label>
            <div className="w-full p-2 bg-gray-100 border border-gray-300 rounded-md shadow-sm">
              {enumOption.perteCrit[0]}
            </div>
          </div>
          <div>
            <label className="block mb-1 text-sm font-medium text-gray-700">
              Meteo
            </label>
            <div className="w-full p-2 bg-gray-100 border border-gray-300 rounded-md shadow-sm">
              {enumOption.meteo[0]}
            </div>
          </div>
        </div>
        <div>
          <label className="block mb-1 text-sm font-medium text-gray-700">
            Commentaire
          </label>
          <textarea
            name="commentaire"
            value={formData.commentaire}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            rows="3"
          />
        </div>
        {/* <div className="flex items-center">
          <input
            type="checkbox"
            name="changerDateDernierTransfert"
            checked={formData.changerDateDernierTransfert}
            onChange={handleChange}
            className="w-5 h-5 text-gray-300 border-gray-300 rounded focus:gray-300"
          />
          <label className="block ml-2 text-sm text-gray-900">
            Changer la date de dernier Transfert dans la base de données
          </label>
        </div> */}
        <div>
          <label className="block mb-1 text-sm font-medium text-gray-700">
            Traité par
          </label>
          <input
            type="text"
            name="traitePar"
            value={formData.traitePar}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
        <div className="flex justify-center mt-8 space-x-4">
          <CustomButton onClick={onClose}>Retour</CustomButton>
          <CustomButton type="submit" variant="green">
            {isAddMode ? "Ajouter" : "Enregistrer les modifications"}
          </CustomButton>
        </div>
      </form>
    </div>
  );
};
