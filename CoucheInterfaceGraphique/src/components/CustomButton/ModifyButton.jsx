// components/ModifyButton.js
import React from "react";
import { FaEdit } from "react-icons/fa";

const ModifyButton = ({ onClick }) => (
  <button
    onClick={onClick}
    className="p-1 mt-3 bg-green-400 rounded-md shadow-sm hover:bg-green-600"
  >
    <FaEdit size={20} className="text-white" />
  </button>
);

export default ModifyButton;
