import React, { useState, useEffect } from "react";
import Modal from "react-modal";
import { useAddUser } from "@/services/usersAPI";
import CustomButton from "@/components/CustomButton/CustomButton";
import Alert from "@/components/Alert/Alert";

const AddUserPopup = ({ isOpen, onClose }) => {
  const initialFormState = {
    nom: "",
    email: "",
    profil: "",
    actif: true,
  };

  const PROFIL_MAPPING = {
    'ADMIN': 1,
    'OPERATEUR': 2,
    'VALIDEUR': 3,
    'CONSULTATION': 4
  };
  
  const PROFIL_MAPPING_REVERSE = Object.fromEntries(
    Object.entries(PROFIL_MAPPING).map(([key, value]) => [value, key])
  );
  

  const [formData, setFormData] = useState(initialFormState);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);

  const addUserMutation = useAddUser();

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    try {
      const userDataWithProfilId = {
        ...formData,
        profilId: PROFIL_MAPPING[formData.profil],
      };
      await addUserMutation.mutateAsync(userDataWithProfilId);
      setIsSubmitted(true);
      setSuccess("Utilisateur ajouté avec succès.");
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
      contentLabel="Ajout d'un Utilisateur"
      className="z-50 max-w-3xl p-8 mx-auto mt-10 bg-white shadow-xl rounded-2xl"
      overlayClassName="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-start overflow-y-auto pt-10"
    >
      <div className="p-6 bg-slate-100 rounded-xl">
        <h2 className="mb-6 text-2xl font-bold text-atoli_blue">
          Ajouter un Utilisateur
        </h2>

        {error && <Alert variant="error">{error}</Alert>}
        {success && <Alert variant="success">{success}</Alert>}

        {!isSubmitted ? (
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
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
                  Email
                </label>
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                  required
                />
              </div>
              <div>
                <label className="block mb-1 text-sm font-medium text-gray-700">
                  Profil
                </label>
                <select
                  name="profil"
                  value={formData.profil}
                  onChange={handleChange}
                  className="w-full p-2 text-xs border border-gray-300 rounded-xl"
                  required
                >
                  <option value="">Sélectionnez un profil</option>
                  {Object.keys(PROFIL_MAPPING).map((profil) => (
                    <option key={profil} value={profil}>
                      {profil}
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
                Ajouter l'Utilisateur
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

export default AddUserPopup;
