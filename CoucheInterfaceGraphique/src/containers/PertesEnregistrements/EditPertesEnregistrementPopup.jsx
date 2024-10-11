import React, { useEffect } from "react";
import Modal from "react-modal";
import { useFormData } from "@/hooks/useFormData";
import { useConnectedUser } from "@/hooks/useConnectedUser";
import {
  formatDurationForDisplay,
  formatDateWithTimezone,
  calculateDuration,
} from "@/utils/dateUtils";
import { EtatEnregistreurSection } from "@/components/PerteEnregistrementsForms/EtatEnregistreurSection";
import { EditPertesEnregistrementForm } from "@/components/PerteEnregistrementsForms/EditPertesEnregistrementForm";
import {
  DefautOptions,
  CauseOptions,
  RemedeOptions,
  Meteo,
  Evt,
  PerteCrit,
} from "@/components/PerteEnregistrementsForms/EnumCauseDefautRemedeTache";

const EditPertesEnregistrementPopup = ({
  isOpen,
  onClose,
  onSave,
  initialData,
  isAddMode = false,
  etatEnregistreur,
  selectedPerte,
}) => {
  const { formData, setFormData, handleChange } = useFormData(
    initialData,
    isAddMode
  );
  const connectedUser = useConnectedUser();

  useEffect(() => {
    if (isOpen) {
      initializeFormData();
    }
  }, [isOpen, initialData, isAddMode]);

  const initializeFormData = () => {
    if (isAddMode) {
      setFormData({
        debut: "",
        fin: "",
        duree: 0,
        durationDisplay: "",
        defaut: "",
        cause: "",
        remede: "",
        commentaire: "",
        changerDateDernierTransfert: false,
        traitePar: connectedUser,
      });
    } else if (initialData) {
      setFormData((prevData) => ({
        ...prevData,
        ...initialData,
        durationDisplay: formatDurationForDisplay(initialData.duree),
        commentaire: initialData.commentaire || "",
        changerDateDernierTransfert:
          initialData.changerDateDernierTransfert || false,
        traitePar: initialData.traitePar || connectedUser,
      }));
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const dataToSave = {
      ...initialData,
      ...formData,
      debut: formData.debut ? formatDateWithTimezone(new Date(formData.debut)) : null,
      fin: formData.fin ? formatDateWithTimezone(new Date(formData.fin)) : null,
      duree: calculateDuration(formData.debut, formData.fin),
    };
    onSave(dataToSave);
  };

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={onClose}
      contentLabel="Édition de Perte d'enregistrement"
      className="z-50 max-w-6xl p-8 mx-auto mt-10 bg-white shadow-xl rounded-2xl"
      overlayClassName="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-start overflow-y-auto pt-10"
    >
      <div className="space-y-8">
        {etatEnregistreur && (
          <EtatEnregistreurSection
            etatEnregistreur={etatEnregistreur}
            selectedPerte={selectedPerte}
          />
        )}
        <EditPertesEnregistrementForm
          formData={formData}
          handleChange={handleChange}
          handleSubmit={handleSubmit}
          isAddMode={isAddMode}
          onClose={onClose}
          enumOption={{
            defaut: DefautOptions,
            cause: CauseOptions,
            remede: RemedeOptions,
            meteo: Meteo,
            evt: Evt,
            perteCrit: PerteCrit,
          }}
        />
      </div>
    </Modal>
  );
};

export default EditPertesEnregistrementPopup;
