import Head from 'next/head';
import Link from 'next/link';
import { useRouter } from 'next/router';
import { useEffect, useState } from 'react';

const Unauthorized = () => {
  const router = useRouter();
  const [countdown, setCountdown] = useState(5);

  useEffect(() => {
    const timer = setInterval(() => {
      setCountdown((prevCount) => prevCount - 1);
    }, 1000);

    const redirect = setTimeout(() => {
      router.push('/surveillance/etat-station');
    }, 5000);

    return () => {
      clearInterval(timer);
      clearTimeout(redirect);
    };
  }, [router]);

  return (
    <>
      <Head>
        <title>Accès non autorisé</title>
      </Head>
      <div className="flex flex-col items-center justify-center min-h-screen py-2">
        <h1 className="mb-4 text-4xl font-bold">Accès non autorisé</h1>
        <p className="mb-4">Vous n'avez pas les droits nécessaires pour accéder à cette page.</p>
        <p className="mb-4">Redirection automatique dans {countdown} secondes...</p>
        <Link href="/surveillance/etat-station" className="text-blue-500 hover:underline">
          Aller immédiatement à l'état des stations
        </Link>
      </div>
    </>
  );
};

export default Unauthorized;