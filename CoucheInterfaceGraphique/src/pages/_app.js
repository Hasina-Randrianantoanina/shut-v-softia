import "../app/globals.css";
import { Inter } from "next/font/google";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";
import { useRouter } from "next/router";
import { useEffect } from "react";
import MainLayout from "@/Layouts/MainLayout";
import Modal from "react-modal";
import Head from 'next/head'; 
import { useSessionTimeout } from '@/hooks/useSessionTimeout';
import { AuthProvider } from '@/contexts/AuthContext';
import { isAuthenticated } from '@/utils/authUtils';

const inter = Inter({ subsets: ["latin"] });
const queryClient = new QueryClient();

if (typeof window !== "undefined") {
  Modal.setAppElement("#__next");
}

function MyApp({ Component, pageProps }) {
  const router = useRouter();
  useSessionTimeout();

  useEffect(() => {
    if (!isAuthenticated() && router.pathname !== "/login") {
      router.push("/login");
    }
  }, [router]);

  const getLayout = Component.getLayout || ((page) => <MainLayout>{page}</MainLayout>);

  return (
    <AuthProvider>
      <QueryClientProvider client={queryClient}>
        <Head>
          <title>SHUT</title>
          <link rel="icon"  href="\logo\logo93.png" />
          <meta name="application-name" content="SHUT" />
        </Head>
        <ReactQueryDevtools initialIsOpen={false} />
        {getLayout(<Component {...pageProps} />)}
      </QueryClientProvider>
    </AuthProvider>
  );
}

export default MyApp;