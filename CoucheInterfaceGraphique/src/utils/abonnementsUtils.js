export const userHasAccessToStation = (userProfilCode, stationAbonnements) => {
  return (stationAbonnements & (1 << userProfilCode)) !== 0;
};

export const userHasAccessToVoie = (userProfilCode, voieAbonnements) => {
  return (voieAbonnements & (1 << userProfilCode)) !== 0;
};

export const decodeAbonnements = (value) => {
  const abonnements = {
    ADMIN: false,
    OPERATEUR: false,
    VALIDEUR: false,
    CONSULTATION: false,
  };

  for (let i = 3; i >= 0; i--) {
    if (value >= Math.pow(2, i)) {
      value -= Math.pow(2, i);
      switch (i) {
        case 3:
          abonnements.CONSULTATION = true;
          break;
        case 2:
          abonnements.VALIDEUR = true;
          break;
        case 1:
          abonnements.OPERATEUR = true;
          break;
        case 0:
          abonnements.ADMIN = true;
          break;
      }
    }
    if (value === 0) break;
  }
  return abonnements;
};

export const encodeAbonnements = (abonnements) => {
  let value = 0;
  if (abonnements.ADMIN) value += Math.pow(2, 0);
  if (abonnements.OPERATEUR) value += Math.pow(2, 1);
  if (abonnements.VALIDEUR) value += Math.pow(2, 2);
  if (abonnements.CONSULTATION) value += Math.pow(2, 3);
  return value;
};
