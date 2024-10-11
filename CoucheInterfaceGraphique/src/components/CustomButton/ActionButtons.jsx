// components/ActionButtons.js
import React from "react";
import { FaTrash, FaEdit } from "react-icons/fa";

const ActionButtons = ({ onDelete, onEdit }) => {
  return (
    <div className="flex space-x-2">
      <button
        onClick={onDelete}
        className="p-1 mt-3 bg-red-400 rounded-md shadow-sm hover:bg-red-600"
      >
        <FaTrash size={20} className="text-white" />
      </button>
      <button
        onClick={onEdit}
        className="p-1 mt-3 bg-green-400 rounded-md shadow-sm hover:bg-green-600"
      >
        <FaEdit size={20} className="text-white" />
      </button>
    </div>
  );
};

export default ActionButtons;
