import React, {
  useState,
  useEffect,
  forwardRef,
  useImperativeHandle,
} from "react";
import { FaCheck, FaTimes } from "react-icons/fa";

const CommentaireEditor = forwardRef((props, ref) => {
  const [value, setValue] = useState(props.value);
  const inputRef = React.useRef(null);

  useEffect(() => {
    if (inputRef.current) {
      inputRef.current.focus();
    }
  }, []);

  const onChange = (event) => {
    setValue(event.target.value);
  };

  const onSave = () => {
    if (props.onSave) {
      props.onSave(value, props);
      props.api.stopEditing();
    } else {
      console.error("onSave n'est pas défini");
    }
  };

  const onCancel = () => {
    props.api.stopEditing(true);
  };

  useImperativeHandle(ref, () => ({
    getValue() {
      return value;
    },
    isCancelBeforeStart() {
      return false;
    },
    isCancelAfterEnd() {
      return false;
    },
  }));

  return (
    <div className="flex items-center w-full">
      <div className="flex flex-col flex-shrink-0 mr-2 space-y-2">
        <button
          onClick={onSave}
          className="p-1 ml-2 bg-green-400 rounded-md shadow-sm hover:bg-green-600"
        >
          <FaCheck size={20} className="text-white" />
        </button>
        <button
          onClick={onCancel}
          className="p-1 ml-2 bg-red-400 rounded-md shadow-sm hover:bg-red-600"
        >
          <FaTimes size={20} className="text-white" />
        </button>
      </div>
      <textarea
        ref={inputRef}
        value={value}
        onChange={onChange}
        className="flex-grow p-1 mr-2 border rounded border-atoli_blue"
        style={{ height: "40px" }}
      />
    </div>
  );
});

export default CommentaireEditor;
