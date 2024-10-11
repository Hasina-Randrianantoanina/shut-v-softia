"use client";
import React, { useState } from "react";
import { useRouter } from "next/router";
import {
  FaUser,
  FaLock,
  FaEye,
  FaEyeSlash,
  FaChevronDown,
} from "react-icons/fa";
import { useSessionTimeout } from '@/hooks/useSessionTimeout';
import { useUsers, useLogin } from '@/services/usersAPI';
import { setToken, getUserInfo } from '@/utils/authUtils';

const LoginComponent = ({ login }) => {
  const [mail, setMail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const router = useRouter();
  const resetTimeout = useSessionTimeout();
  const { data: users, isLoading: usersLoading, isError: usersError } = useUsers();
  const loginMutation = useLogin();

  const handleLogin = async (event) => {
    event.preventDefault();
    loginMutation.mutate(
      { Mail: mail, Password: password },
      {
        onSuccess: (data) => {
          setToken(data.token);
          const userInfo = getUserInfo();
          if (userInfo) {
            login(userInfo);
          }
          resetTimeout();
          window.dispatchEvent(new Event('userLoggedIn'));
          router.push("/surveillance/defauts-stations");
        },
        onError: (error) => {
          console.error("Erreur lors de la connexion:", error);
          alert(error.message || "Erreur lors de la connexion");
        }
      }
    );
  };

  
  return (
    <div className="grid w-screen h-screen grid-cols-1 md:grid-cols-2">
      <div className="flex flex-col items-center justify-center p-6 bg-white md:p-0">
        <div className="flex flex-col mt-auto mb-auto text-center">
          <span className="text-atoli_blue font-jockey-one" style={{ fontSize: '12rem' }}>SHUT</span>
          <div className="mx-auto mt-4" style={{ width: "fit-content" }}>
            <span className="text-xl font-bold md:text-2xl text-atoli_blue">
              Système d'Historisation Urbain et Télévidable
            </span>
          </div>
        </div>

        <div className="flex items-center justify-between w-full px-4 mt-auto mb-4 text-center">
          <img
            src="/logo/logo93_horizontal.jpg"
            alt="Logo"
            width={400}
            height={100}
            className="mr-2"
          />
          <span className="text-gray-500 text-m">
            © 2024 Direction de l'Eau et de l'Assainissement (DEA)
          </span>
        </div>
      </div>
      <div className="flex items-center justify-center p-6 bg-atoli_blue md:p-0">
        <div className="flex flex-col w-full max-w-md p-4">
          <h1 className="mb-6 text-xl font-semibold text-white md:text-3xl">
            Entrez vos identifiants
          </h1>

          <form onSubmit={handleLogin} className="flex flex-col space-y-4">
            <div>
              <label className="block p-2 font-bold text-white">
                Sélectionner un opérateur
              </label>
              <div className="relative">
                <FaUser className="absolute transform -translate-y-1/2 left-3 top-1/2 text-atoli_blue" />
                <select
                  value={mail} 
                  onChange={(e) => setMail(e.target.value)} 
                  className="w-full py-2 pl-10 pr-10 font-bold bg-white border-2 border-gray-300 appearance-none rounded-xl focus:outline-none focus:border-blue-500 text-atoli_blue">
                  <option className="font-bold text-atoli_blue" value="">
                    Sélectionner un opérateur
                  </option>
                  {usersLoading ? (
                    <option>Chargement...</option>
                  ) : usersError ? (
                    <option>Erreur de chargement</option>
                  ) : (
                    users?.map((user) => (
                      <option key={user.id} value={user.email}>
                        {user.nom}
                      </option>
                    ))
                  )}
                </select>
                <FaChevronDown className="absolute transform -translate-y-1/2 pointer-events-none right-3 top-1/2 text-atoli_blue" />
              </div>
            </div>

            <div className="">
              <label className="block p-2 font-bold text-white">
                Mot de passe
              </label>
              <div className="relative">
                <FaLock className="absolute transform -translate-y-1/2 left-3 top-1/2 text-atoli_blue" />
                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="Mot de passe"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="w-full py-2 pl-10 pr-10 font-bold bg-white border-2 border-gray-300 rounded-xl focus:outline-none focus:border-blue-500 text-atoli_blue"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute transform -translate-y-1/2 right-3 top-1/2 text-atoli_blue">
                  {showPassword ? <FaEyeSlash /> : <FaEye />}
                </button>
              </div>
            </div>

            <div className="mt-12">
              <button
                type="submit"
                className="w-full py-2 mt-6 mb-6 font-bold bg-white border-2 text-atoli_blue border-atoli_blue rounded-xl hover:bg-gray-100"
                disabled={loginMutation.isLoading}>
                {loginMutation.isLoading ? 'Connexion...' : 'Valider'}
              </button>
            </div>

            <hr className="text-white" />

            <div className="relative">
              <div className="absolute inset-0 mt-5 bg-white bg-opacity-50 rounded-xl"></div>
              <div className="relative p-4">
                <label className="block my-4 font-bold text-center text-atoli_blue">
                  ou accéder à l'archive en
                </label>
                <button
                  type="button"
                  onClick={() => router.push("/archive-libre")}
                  className="w-full py-2 font-bold bg-white shadow-md text-atoli_blue rounded-xl hover:bg-gray-100">
                  Accès libre
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default LoginComponent;