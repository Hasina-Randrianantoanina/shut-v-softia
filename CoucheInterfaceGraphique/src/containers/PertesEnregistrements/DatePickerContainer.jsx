import React, { useState, useEffect } from "react";
import CustomDatePicker from "@/components/DatePicker/CustomDatePicker";
import CustomButton from "@/components/CustomButton/CustomButton";
import { useUpdateEnregistreurDernierTransfert } from "@/services/enregistreursAPI";
import { formatDateWithTimezone } from "@/utils/dateUtils";
import Alert from "@/components/Alert/Alert";

const DatePickerContainer = ({
  selectedStation,
  onAddClick,
  showLast12Months,
  onToggleLast12Months,
  enregistreurInfo,
}) => {
  const [lastTransferDate, setLastTransferDate] = useState(null);
  const [transferDate, setTransferDate] = useState(null);
  const [showAlert, setShowAlert] = useState(false);

  const updateDernierTransfert = useUpdateEnregistreurDernierTransfert();

  useEffect(() => {
    if (enregistreurInfo && enregistreurInfo.dernierTransfert) {
      setLastTransferDate(new Date(enregistreurInfo.dernierTransfert));
    }
  }, [enregistreurInfo]);

  const handleModifierClick = async () => {
    if (lastTransferDate && enregistreurInfo) {
      try {
        const localISOString = formatDateWithTimezone(lastTransferDate);

        await updateDernierTransfert.mutateAsync({
          enregistreurId: enregistreurInfo.id,
          dernierTransfert: localISOString,
        });

        setShowAlert(true);
        setTimeout(() => setShowAlert(false), 5000);
      } catch (error) {
        console.error(
          "Erreur lors de la mise à jour de la date de dernier transfert:",
          error
        );
      }
    }
  };


  return (
    <div className="p-4 mb-4 border-y-[0.5px] bg-slate-100 border-atoli_blue">
      {showAlert && (
        <Alert variant="success">
          La date de dernier transfert a été modifiée avec succès.
        </Alert>
      )}
      {selectedStation && (
        <div className="flex justify-start mb-4 text-2xl font-bold item-center text-atoli_blue">
          Station {selectedStation}
        </div>
      )}
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center">
          <input
            type="checkbox"
            checked={showLast12Months}
            onChange={(e) => onToggleLast12Months(e.target.checked)}
            className="w-4 h-4 border-gray-300 rounded text-atoli_blue focus:ring-atoli_blue"
          />
          <label className="ml-2 text-sm font-medium text-gray-700">
            Les 12 derniers mois
          </label>
        </div>
      </div>

      <div className="flex justify-start space-x-4">
        <div className="w-auto">
          <CustomDatePicker
            selectedDate={lastTransferDate}
            onChange={(date) => setLastTransferDate(date)}
            label="Date de dernier transfert"
            placeholder="jj/mm/aa hh:mm"
            showTime={true}
          />
          <CustomButton
            className="mt-2"
            onClick={handleModifierClick}
            disabled={!enregistreurInfo}
          >
            Modifier
          </CustomButton>
        </div>
        <div className="w-auto">
          <CustomDatePicker
            selectedDate={transferDate}
            onChange={(date) => setTransferDate(date)}
            label="Transfert jour (jj/mm/aa)"
            placeholder="jj/mm/aa"
          />
          <CustomButton
            className="mt-2"
            onClick={() => console.log("Transférer")}
          >
            Transférer
          </CustomButton>
        </div>
        <div className="mt-7">
          <CustomButton onClick={onAddClick}>Ajouter</CustomButton>
        </div>
      </div>
    </div>
  );
};

export default DatePickerContainer;
