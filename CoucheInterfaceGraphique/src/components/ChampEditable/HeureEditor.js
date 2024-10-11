import React, {
  useState,
  useEffect,
  forwardRef,
  useImperativeHandle,
} from "react";
import { FaCheck, FaTimes } from "react-icons/fa";

const isValidTime = (time) => {
  return /^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$/.test(time);
};

const HeureEditor = forwardRef((props, ref) => {
  const [value, setValue] = useState(props.value);
  const [isValid, setIsValid] = useState(true);

  useEffect(() => {
    if (inputRef.current) {
      inputRef.current.focus();
    }
  }, []);

  const inputRef = React.useRef(null);

  const onChange = (event) => {
    const newValue = event.target.value;
    setValue(newValue);
    setIsValid(isValidTime(newValue));
  };

  const onKeyDown = (event) => {
    if (event.key === "Enter") {
      props.api.stopEditing();
    } else if (event.key === "Escape") {
      props.api.stopEditing(true);
    }
  };

  useImperativeHandle(ref, () => ({
    getValue() {
      return isValid ? value : props.value;
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
          onClick={() => props.api.stopEditing()}
          className="p-1 ml-2 bg-green-400 rounded-md shadow-sm hover:bg-green-600"
          disabled={!isValid}
        >
          <FaCheck size={20} className="text-white" />
        </button>
        <button
          onClick={() => props.api.stopEditing(true)}
          className="p-1 ml-2 bg-red-400 rounded-md shadow-sm hover:bg-red-600"
        >
          <FaTimes size={20} className="text-white" />
        </button>
      </div>
      <input
        type="time"
        step="1"
        ref={inputRef}
        value={value}
        onChange={onChange}
        onKeyDown={onKeyDown}
        className={`flex-grow p-1 mr-2 border rounded ${
          isValid ? "border-atoli_blue" : "border-red-500"
        }`}
      />
    </div>
  );
});

export default HeureEditor;
