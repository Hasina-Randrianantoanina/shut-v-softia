import React from "react";
import CustomButton from "@/components/CustomButton/CustomButton";

const WarningPopup = ({ isOpen, onClose, onConfirm, message }) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div className="p-4">
      <div className="p-6 rounded-xl bg-slate-200">
        <p className="mb-4">{message}</p>
        <div className="flex justify-center space-x-2">
          <CustomButton onClick={onClose} variant="gray">
            Annuler
          </CustomButton>
          <CustomButton onClick={onConfirm} variant="red">
            Confirmer
          </CustomButton >
        </div>
      </div>
      </div>
    </div>
  );
};

export default WarningPopup;