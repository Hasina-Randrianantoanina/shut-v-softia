import React, { useState } from "react";
import Link from "next/link";
import { VscArrowUp, VscArrowDown } from "react-icons/vsc";

export default function Menu({
  isNavbarCollapsed,
  menu,
  topClassName,
  toggleNavbarCollapse,
  collapseNavbar,
}) {
  const [isMenuCollapsed, setIsMenuCollapsed] = useState(true);

  const handleIconClick = () => {
    if (isNavbarCollapsed) {
      toggleNavbarCollapse();
      setIsMenuCollapsed(false);
    } else {
      setIsMenuCollapsed((prevState) => !prevState);
    }
  };

  const handleSubmenuClick = () => {
    collapseNavbar();
  };

  const MenuItem = ({ item }) => {
    if (item.disabled) {
      return (
        <div className="flex items-center justify-between p-2 ml-2 text-lg font-bold text-gray-400 text-opacity-50 cursor-not-allowed">
          <span>{item.label}</span>
        </div>
      );
    }
    return (
      <Link href={item.href}>
        <div
          className="flex items-center justify-between p-2 ml-2 text-lg font-bold text-opacity-50 cursor-pointer text-atoli_blue hover:bg-navbar hover:text-atoli_blue"
          onClick={handleSubmenuClick}
        >
          <span>{item.label}</span>
        </div>
      </Link>
    );
  };

  return (
    <div className={`${topClassName}`}>
      <div
        className={`group flex flex-col ${
          isMenuCollapsed ? "bg-white bg-opacity-50" : "bg-white"
        } rounded-2xl`}
      >
        <div
          className="relative flex items-center justify-between m-3 text-xl font-bold cursor-pointer"
          onClick={handleIconClick}
        >
          <div className="flex items-center">
            <div className="relative p-1 rounded-full bg-atoli_blue">
              {isNavbarCollapsed ? (
                <div className="peer">
                  {menu.icon}
                </div>
              ) : (
                menu.icon
              )}
              {isNavbarCollapsed && (
                <div className="fixed z-10 px-2 py-1 ml-2 text-lg font-bold transition-all duration-150 bg-white rounded-lg shadow-lg opacity-0 -translate-y-3/4 text-atoli_blue left-16 peer-hover:opacity-100 whitespace-nowrap">
                  {menu.label}
                </div>
              )}
            </div>
            {!isNavbarCollapsed && (
              <span className="ml-2 text-black">{menu.label}</span>
            )}
          </div>
          {!isNavbarCollapsed && (
            <div className="flex items-center">
              <div className="p-1 rounded-full bg-atoli_blue">
                <div className="font-light text-white">
                  {isMenuCollapsed ? <VscArrowDown /> : <VscArrowUp />}
                </div>
              </div>
            </div>
          )}
        </div>
        {!isMenuCollapsed && !isNavbarCollapsed && (
          <div>
            {menu.submenus.map((mi, idx) => (
              <MenuItem 
                key={idx} 
                item={mi} 
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}