"use client";
export default function LoginLayout({ children }) {
    return (
      <div className="flex flex-col items-center justify-center h-screen bg-gray-100">
        {children}
      </div>
    );
  }