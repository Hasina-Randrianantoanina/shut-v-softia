"use client";
import React, { useState } from 'react';
import { useRouter } from 'next/router';
import { FaUserCircle, FaChevronDown, FaChevronUp } from 'react-icons/fa'; 
import { useConnectedUser } from '@/hooks/useConnectedUser';
import { logout } from '@/utils/authUtils';
import { useAuth } from '@/contexts/AuthContext';

const TopBar = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const username = useConnectedUser();
  const router = useRouter();
  const { logout: authLogout } = useAuth();

  const toggleMenu = () => {
    setMenuOpen(!menuOpen);
  };

  const handleLogout = () => {
    logout();
    authLogout();
    router.push('/login');
  };


  return (
    <div className="flex items-center justify-between p-3 bg-white shadow-md">
      <div className="flex items-center">
        <img src="/logo/logo93.png" alt="Logo" className="h-20" />
        <span className="ml-1 text-7xl text-atoli_blue font-jockey-one">SHUT</span>
      </div>
      <div className="relative flex items-center p-2">
        <div className="absolute inset-0 rounded-xl bg-atoli_blue opacity-40"></div>
        <FaUserCircle className="relative w-8 h-8 bg-white rounded-3xl text-atoli_blue" />
        <button
          className="relative flex items-center p-1 px-2 ml-2 font-extrabold bg-white rounded-lg text-atoli_blue focus:outline-none"
          onClick={toggleMenu}
        >
          <span className="mr-2 text-2xl">{username}</span>
          {menuOpen ? <FaChevronUp /> : <FaChevronDown />}
        </button>
        {menuOpen && (
          <div className="absolute right-0 z-10 w-48 bg-white border rounded-lg shadow-lg mt-28">
            <button 
              className="w-full px-4 py-2 font-bold text-left text-atoli_blue hover:bg-gray-100"
              onClick={handleLogout}
            >
              Déconnecter
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default TopBar;