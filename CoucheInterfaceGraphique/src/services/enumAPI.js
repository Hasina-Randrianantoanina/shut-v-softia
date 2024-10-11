import { useQuery } from '@tanstack/react-query';

const BASE_URL = '/api';

export const useEnumValues = (enumType) => {
  return useQuery({
    queryKey: ['enumValues', enumType],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Enums/${enumType}`, {
        credentials: 'include',
      });
      if (!response.ok) {
        throw new Error(`Erreur lors de la récupération des valeurs de l'enum ${enumType}`);
      }
      return response.json();
    }
  });
};