import { useState, useEffect } from "react";
import {
  formatDateForInput,
  calculateDuration,
  formatDurationForDisplay,
} from "@/utils/dateUtils";

export const useFormData = (initialData, isAddMode) => {
  const [formData, setFormData] = useState({
    debut: "",
    fin: "",
    duree: "",
    durationDisplay: "",
    defaut: "",
    cause: "",
    remede: "",
    commentaire: "",
    changerDateDernierTransfert: false,
    traitePar: "",
  });

  useEffect(() => {
    if (isAddMode) {
      setFormData({
        debut: "",
        fin: "",
        duree: "",
        durationDisplay: "",
        defaut: "",
        cause: "",
        remede: "",
        commentaire: "",
        changerDateDernierTransfert: false,
        traitePar: "",
      });
    } else if (initialData) {
      const calculatedDuration = calculateDuration(
        initialData.debut,
        initialData.fin
      );
      setFormData({
        ...initialData,
        debut: formatDateForInput(initialData.debut),
        fin: formatDateForInput(initialData.fin),
        duree: calculatedDuration,
        durationDisplay: formatDurationForDisplay(calculatedDuration),
      });
    }
  }, [initialData, isAddMode]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prevData) => {
      const newData = {
        ...prevData,
        [name]: type === "checkbox" ? checked : value,
      };

      if (name === "debut" || name === "fin") {
        const duration = calculateDuration(newData.debut, newData.fin);
        newData.duree = duration;
        newData.durationDisplay = formatDurationForDisplay(duration);
      }

      return newData;
    });
  };

  return { formData, setFormData, handleChange };
};
