"use client";
import React from "react";
import { useAuth } from "@/contexts/AuthContext";
import {
  FaEye,
  FaClock,
  FaBoxArchive,
  FaSuitcase,
  FaGear,
  FaArrowLeft,
  FaArrowRight,
} from "react-icons/fa6";
import Menu from "./Menu.jsx";

const Navbar = ({ isNavbarCollapsed, toggleNavbarCollapse }) => {
  const { user } = useAuth();
  const menu = [
    {
      name: "surveillance",
      label: "Surveillance",
      icon: <FaEye className="text-white" />,
      submenus: [
        {
          name: "etats_stations",
          label: "Etat Stations",
          href: "/surveillance/etat-station",
        },
        {
          name: "defauts_stations",
          label: "Défauts Stations",
          href: "/surveillance/defauts-stations",
        },
        {
          name: "defauts_capteurs",
          label: "Défauts Capteurs",
          href: "/surveillance/defauts-capteurs",
        },
        {
          name: "pertes_enreg",
          label: "Pertes enregistrements",
          href: "/surveillance/pertes-enregistrements",
        },
      ],
    },
    {
      name: "temps_reel",
      label: "Temps réel",
      icon: <FaClock className="text-white" />,
      submenus: [
        {
          name: "connexions",
          label: "Connexions",
          href: "/tempsReel/connexion",
          disabled: true,
        },
      ],
    },
    {
      name: "archives",
      label: "Archives",
      icon: <FaBoxArchive className="text-white" />,
      submenus: [
        {
          name: "visualisation",
          label: "Visualisation",
          href: "/archives/visualisation",
        },
        {
          name: "transfert",
          label: "Transfert",
          href: "/archives/transfert",
          disabled: true,
        },
        {
          name: "integration_manuelle",
          label: "Intégration manuelle",
          href: "/archives/integration",
          disabled: true,
        },
      ],
    },
    {
      name: "administration",
      label: "Administration",
      icon: <FaSuitcase className="text-white" />,
      submenus: [
        {
          name: "utilisateurs",
          label: "Utilisateurs",
          href: "/administration/utilisateurs",
          roles: ["ADMIN"],
        },
        {
          name: "cycles_appels",
          label: "Cycles d'appels",
          href: "/administration/cycles-appel",
          roles: ["ADMIN"],
        },
        {
          name: "fichier_trace",
          label: "Fichier trace",
          href: "/administration/fichiers-trace",
          disabled: true,
        },
        {
          name: "surveillance",
          label: "Surveillance",
          href: "/administration/surveillance",
          disabled: true,
        },
        {
          name: "voies_stc",
          label: "Voies STC",
          href: "/administration/voies-stc",
          disabled: true,
        },
      ],
      roles: ["ADMIN", "OPERATEUR"],
    },
    {
      name: "configuration",
      label: "Configuration",
      icon: <FaGear className="text-white" />,
      submenus: [
        {
          name: "stations",
          label: "Stations",
          href: "/configuration/stations",
          roles: ["ADMIN", "OPERATEUR"],
        },
        {
          name: "enregistreurs",
          label: "Enregistreurs",
          href: "/configuration/enregistreurs",
          disabled: true,
        },
        {
          name: "traitements",
          label: "Traitements",
          href: "/configuration/traitements",
          roles: ["ADMIN", "OPERATEUR", "VALIDEUR"],
        },
        {
          name: "abonnement_voies",
          label: "Abonnement voies",
          href: "/configuration/abonnements-voies",
          roles: ["ADMIN"],
          // disabled: true,
        },
        {
          name: "abonnement_stations",
          label: "Abonnement stations",
          href: "/configuration/abonnements-stations",
          roles: ["ADMIN"],
        },
        {
          name: "groupe_voies",
          label: "Groupe voies",
          href: "/configuration/groupes-voies",
          roles: ["ADMIN", "OPERATEUR", "VALIDEUR"],
        },
        {
          name: "preselection",
          label: "Préselection",
          href: "/configuration/preselections",
          roles: ["ADMIN", "OPERATEUR", "VALIDEUR"],
        },
      ],
    },
  ];

  const filterMenuItems = (menuItems) => {
    return menuItems.filter(item => 
      !item.roles || (user && user.role && item.roles.includes(user.role))
    ).map(item => ({
      ...item,
      submenus: item.submenus ? filterMenuItems(item.submenus) : undefined
    }));
  };

  const filteredMenu = filterMenuItems(menu);

  const collapseNavbar = () => {
    if (!isNavbarCollapsed) {
      toggleNavbarCollapse();
    }
  };

  return (
    <div
      className={`flex flex-col top-0 left-0 h-full p-4 pt-10 overflow-y-auto text-white ${
        isNavbarCollapsed ? "w-20" : "w-72"
      } bg-atoli_blue transition-all duration-300 relative`}
    >
      <button
        onClick={toggleNavbarCollapse}
        className={`absolute right-4 transform translate-x-1/2 p-1 bg-white rounded-full shadow-md top-3 text-atoli_blue transition-all duration-300`}
      >
        {isNavbarCollapsed ? <FaArrowRight /> : <FaArrowLeft />}
      </button>

      {filteredMenu.map((mi, idx) => (
        <div key={idx} className={`${isNavbarCollapsed ? "mb-6" : ""}`}>
          <Menu
            isNavbarCollapsed={isNavbarCollapsed}
            menu={mi}
            topClassName={idx === 0 ? "" : "mt-6"}
            toggleNavbarCollapse={toggleNavbarCollapse}
            collapseNavbar={collapseNavbar}
          />
        </div>
      ))}
    </div>
  );
};

export default Navbar;
