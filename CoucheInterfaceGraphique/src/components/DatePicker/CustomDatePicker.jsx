import React from 'react';

const CustomDatePicker = ({ selectedDate, onChange, label, placeholder, showTime = false }) => {
  const formatDateForInput = (date) => {
    if (!date) return '';
    return date.toLocaleString('sv-SE').slice(0, 16).replace(' ', 'T');
  };

  const handleChange = (e) => {
    const inputDate = new Date(e.target.value);
    onChange(inputDate);
  };

  

  return (
    <div className="flex flex-col">
      <label className="mb-2 text-sm font-medium text-gray-700">{label}</label>
      <input
        type="datetime-local"
        value={formatDateForInput(selectedDate)}
        onChange={handleChange}
        className="w-full px-3 py-2 pr-4 text-sm border rounded-lg border-atoli_blue focus:outline-none focus:ring-2 focus:ring-atoli_blue"
        placeholder={placeholder}
      />
    </div>
  );
};

export default CustomDatePicker;