import React from "react";

const CustomButton = ({
  onClick,
  children,
  className = "",
  variant = "default",
}) => {
  const baseClasses =
    "px-3 py-2 font-semibold text-white shadow-md rounded-xl focus:outline-none focus:ring-2 focus:ring-offset-2";

  const variantClasses = {
    default: "bg-atoli_blue hover:bg-atoli_blue_dark focus:ring-atoli_blue",
    red: "bg-red-600 hover:bg-red-700 focus:ring-red-600",
    gray: "bg-slate-500 hover:bg-slate-600 focus:ring-slate-500",
    green:
      "bg-shamrock_green hover:bg-shamrock_green_dark focus:ring-shamrock_green ",
    // autres variante
  };

  return (
    <button
      onClick={onClick}
      className={`
        ${baseClasses}
        ${variantClasses[variant] || variantClasses.default}
        ${className}
      `}
    >
      {children}
    </button>
  );
};

export default CustomButton;
