import React from 'react';
import UserActionButtons from '@/components/CustomButton/UserActionButtons';

const PROFIL_MAPPING = {
  1: 'ADMIN',
  2: 'OPERATEUR',
  3: 'VALIDEUR',
  4: 'CONSULTATION'
};

const PROFIL_MAPPING_REVERSE = {
  'ADMIN': 1,
  'OPERATEUR': 2,
  'VALIDEUR': 3,
  'CONSULTATION': 4
};

const DefColUsers = (onModify, onSave, onCancel, editingRowId, onDelete) => {
  const profilOptions = ["1", "2", "3", "4"];
  // const profilOptions = Object.entries(PROFIL_MAPPING).map(([key, value]) => ({
  //   label: value,
  //   value: parseInt(key)
  // }));
  return [
    {
      headerName: "Actions",
      field: "actions",
      width: 120,
      cellRenderer: (params) => {
        const isEditing = params.data.id.toString() === editingRowId;
        return (
          <div className="flex items-center justify-center h-full">
            <UserActionButtons
              isEditing={isEditing}
              onEdit={() => onModify(params.data.id)}
              onDelete={() => onDelete(params.data)}
              onSave={() => onSave(params.data.id)}
              onCancel={() => onCancel()}
            />
          </div>
        );
      },
      cellStyle: { 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'center',
        height: '100%',
        padding: 0 
      },
      pinned: "left",
    },
    
    {
      headerName: "ID",
      field: "id",
      hide: true,
    },
    {
      headerName: "Nom",
      field: "nom",
      editable: true,
      cellStyle: { fontWeight: "bold" },
    },
    {
      headerName: "Email",
      field: "email",
      width: 250,
      minWidth: 120,
      maxWidth: 290,
      editable: true,
    },
    {
      headerName: "Profil",
      field: "profilId",
      editable: true,
      cellStyle: { fontWeight: "bold" },
      cellEditor: 'agSelectCellEditor',
      cellEditorParams: {
        values: profilOptions,
      },
    
      valueFormatter: (params) => {
        return PROFIL_MAPPING[params.value] || 'Unknown';
      },
    
      // Ajout d'une méthode pour s'assurer que la valeur sélectionnée soit bien prise en compte
      valueParser: (params) => {
        return parseInt(params.newValue);  // Convertir la nouvelle valeur en entier
      },
    },
    {
      headerName: "Actif",
      field: "actif",
      editable: true,
      flex: 1,
      cellRenderer: (params) => (
        <input 
          type="checkbox" 
          checked={params.value} 
          disabled={params.data.id.toString() !== editingRowId}
          onChange={() => {}}
        />
      ),
      cellEditor: 'agCheckboxCellEditor',
    },
  ];
};

export default DefColUsers;