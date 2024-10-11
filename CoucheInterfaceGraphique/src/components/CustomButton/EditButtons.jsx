// components/EditButtons.js
import React from "react";
import { FaCheck, FaTimes } from "react-icons/fa";

const EditButtons = ({ onSave, onCancel }) => (
  <div className="flex mt-3 space-x-2">
    <button
      onClick={onSave}
      className="p-1 bg-green-400 rounded-md shadow-sm hover:bg-green-600"
    >
      <FaCheck size={20} className="text-white" />
    </button>
    <button
      onClick={onCancel}
      className="p-1 bg-red-400 rounded-md shadow-sm hover:bg-red-600"
    >
      <FaTimes size={20} className="text-white" />
    </button>
  </div>
);

export default EditButtons;
