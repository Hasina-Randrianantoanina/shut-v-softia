import React, { forwardRef } from "react";
import { FaEdit } from "react-icons/fa";

const CommentaireRenderer = forwardRef((props, ref) => {
  const onEditClick = () => {
    props.api.startEditingCell({
      rowIndex: props.rowIndex,
      colKey: props.column.getColId(),
    });
  };

  return (
    <div className="flex items-center w-full h-full">
      <button
        onClick={onEditClick}
        className="p-1 mr-2 bg-green-400 rounded-md shadow-sm hover:bg-green-600 "
      >
        <FaEdit size={20} className="text-white" />
      </button>
      <span className="truncate">{props.value || "Commentaire..."}</span>
    </div>
  );
});

export default CommentaireRenderer;
