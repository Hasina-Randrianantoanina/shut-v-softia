import React, { useState, useEffect } from "react";
import Modal from "react-modal";
import {
  useAddStationWithEnregistreur,
  useAvailableStationNumbers,
  useStation,
  useCreateStationFromModel,
} from "@/services/stationsAPI";
import { useEnregistreurs, useEnregistreurVersions } from "@/services/enregistreursAPI";
import CustomButton from "@/components/CustomButton/CustomButton";
import Alert from "@/components/Alert/Alert";

const AddStationPopup = ({ isOpen, onClose, reseauType }) => {
  const initialFormState = {
    initiales: "",
    nom: "",
    numero: "",
    adresseIp: "",
    liaison: "",
    actif: true,
    enregistreurVersion: "",
    modelStationId: "",
    reseau: reseauType,
  };

  const [formData, setFormData] = useState(initialFormState);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);

  const addStationMutation = useAddStationWithEnregistreur();
  const createStationFromModelMutation = useCreateStationFromModel();
  const { data: availableNumbers } = useAvailableStationNumbers();
  const { data: stations } = useStation(reseauType);
  const { data: enregistreurs } = useEnregistreurs();
  const { data: enregistreurVersions } = useEnregistreurVersions(
    formData.liaison
  );

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prevData) => {
      const newData = {
        ...prevData,
        [name]: type === "checkbox" ? checked : value,
      };

      if (name === "numero" || name === "liaison") {
        const ipEnd =
          name === "liaison"
            ? value === "IP"
              ? "9"
              : "1"
            : prevData.liaison === "IP"
            ? "9"
            : "1";
        newData.adresseIp = `192.168.${newData.numero}.${ipEnd}`;
      }

      return newData;
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    try {
      let result;
      if (formData.modelStationId) {
        result = await createStationFromModelMutation.mutateAsync({
          modelStationId: parseInt(formData.modelStationId),
          newStation: {
            ...formData,
            numero: parseInt(formData.numero),
          },
        });
      } else {
        result = await addStationMutation.mutateAsync(formData);
      }
      setIsSubmitted(true);
      setSuccess("Station ajoutée avec succès.");
    } catch (error) {
      setError(`Erreur: ${error.message}`);
    }
  };

  const handleClose = () => {
    setFormData(initialFormState);
    setError("");
    setSuccess("");
    setIsSubmitted(false);
    onClose();
  };

  useEffect(() => {
    if (!isOpen) {
      setFormData(initialFormState);
      setError("");
      setSuccess("");
      setIsSubmitted(false);
    }
  }, [isOpen]);

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={handleClose}
      contentLabel="Ajout d'une Station"
      className="z-50 max-w-3xl p-8 mx-auto mt-10 bg-white shadow-xl rounded-2xl"
      overlayClassName="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-start overflow-y-auto pt-10"
    >
      <div className="p-6 bg-slate-100 rounded-xl">
      <h2 className="mb-6 text-2xl font-bold text-atoli_blue">
        Ajouter une Station
      </h2>

      {error && <Alert variant="error">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      {!isSubmitted ? (
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Initiales
              </label>
              <input
                type="text"
                name="initiales"
                value={formData.initiales}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                required
              />
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Nom
              </label>
              <input
                type="text"
                name="nom"
                value={formData.nom}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                required
              />
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Numéro
              </label>
              <select
                name="numero"
                value={formData.numero}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                required
              >
                <option value="">Sélectionnez un numéro</option>
                {availableNumbers?.map((num) => (
                  <option key={num} value={num}>
                    {num}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Adresse IP
              </label>
              <input
                type="text"
                name="adresseIp"
                value={formData.adresseIp}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                required
              />
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Liaison
              </label>
              <select
                name="liaison"
                value={formData.liaison}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                required
              >
                <option value="">Sélectionnez une liaison</option>
                <option value="IP">IP</option>
                <option value="AP">AP</option>
              </select>
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Version Enregistreur
              </label>
              <select
                name="enregistreurVersion"
                value={formData.enregistreurVersion}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                required
                disabled={!formData.liaison}
              >
                <option value="">Sélectionnez une version</option>
                {enregistreurVersions?.map((version) => (
                  <option key={version} value={version}>
                    {version}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Réseau
              </label>
              <input
                type="text"
                name="reseau"
                value={formData.reseau}
                className="w-full p-2 text-xs bg-gray-100 border border-gray-300 rounded-xl"
                readOnly
              />
            </div>
            <div>
              <label className="block mb-1 text-sm font-medium text-gray-700">
                Modèle de Station
              </label>
              <select
                name="modelStationId"
                value={formData.modelStationId}
                onChange={handleChange}
                className="w-full p-2 text-xs border border-gray-300 rounded-xl"
              >
                <option value="">Sélectionnez un modèle (optionnel)</option>
                {stations?.map((station) => (
                  <option key={station.id} value={station.id}>
                    {station.initiales} - {station.nom}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="flex items-center">
            <input
              type="checkbox"
              name="actif"
              checked={formData.actif}
              onChange={handleChange}
              className="w-5 h-5 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
            />
            <label className="ml-2 text-sm text-gray-900">Actif</label>
          </div>
          <div className="flex justify-center mt-6 space-x-4">
            <CustomButton onClick={handleClose} variant="secondary">
              Annuler
            </CustomButton>
            <CustomButton type="submit" variant="primary">
              {formData.modelStationId
                ? "Créer à partir du modèle"
                : "Ajouter la Station"}
            </CustomButton>
          </div>
        </form>
      ) : (
        <div className="flex justify-end mt-6">
          <CustomButton onClick={handleClose} variant="primary">
            OK
          </CustomButton>
        </div>
      )}
      </div>
    </Modal>
  );
};

export default AddStationPopup;
