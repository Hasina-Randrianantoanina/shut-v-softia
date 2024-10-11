import Navbar from "@/components/MainLayout/Navbar";
import Topbar from "@/components/MainLayout/Topbar";
import Footer from "@/components/MainLayout/Footer";
import { Inter } from "next/font/google";
import { useState, useEffect } from "react";

const inter = Inter({ subsets: ["latin"] });

export default function MainLayout({ children }) {
  const [isNavbarCollapsed, setIsNavbarCollapsed] = useState(true);

  const toggleNavbarCollapse = () => {
    setIsNavbarCollapsed(!isNavbarCollapsed);
  };

  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth <= 768) {
        setIsNavbarCollapsed(true);
      }
    };

    window.addEventListener("resize", handleResize);
    handleResize();

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  return (
    <div className={`${inter.className} flex flex-col h-screen`}>
      <Topbar />
      <div className="flex flex-1 overflow-hidden">
        <Navbar
          isNavbarCollapsed={isNavbarCollapsed}
          toggleNavbarCollapse={toggleNavbarCollapse}
        />
        <div className="flex flex-col flex-1 overflow-hidden">
          <main className="flex-1 overflow-auto">
            {children}
          </main>
          <Footer />
        </div>
      </div>
    </div>
  );
}