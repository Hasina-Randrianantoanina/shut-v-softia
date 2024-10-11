import React from 'react';
import { FaEdit, FaTrash, FaCheck, FaTimes } from 'react-icons/fa';

const UserActionButtons = ({ isEditing, onEdit, onDelete, onSave, onCancel }) => {
  if (isEditing) {
    return (
      <div className="flex space-x-2">
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
  }

  return (
    <div className="flex space-x-2">
      <button
        onClick={onEdit}
        className="p-1 bg-green-400 rounded-md shadow-sm hover:bg-green-600"
      >
        <FaEdit size={20} className="text-white" />
      </button>
      <button
        onClick={onDelete}
        className="p-1 bg-red-400 rounded-md shadow-sm hover:bg-red-600"
      >
        <FaTrash size={20} className="text-white" />
      </button>
    </div>
  );
};

export default UserActionButtons;