import React, { useState, useCallback, useRef, useEffect } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "@/styles/ag-theme-quartz.css";
import DefColUsers from "@/components/TableColumnDefinition/Utilisateurs/DefColUsers";
import { useUsers, useUpdateUser, useDeleteUser } from "@/services/usersAPI";
import Alert from "@/components/Alert/Alert";
import WarningPopup from "@/components/Alert/WarningPopup";
import CustomButton from "@/components/CustomButton/CustomButton";
import AddUserPopup from "@/containers/Utilisateurs/AddUserPopup";

const Utilisateur = () => {
  const [editingRowId, setEditingRowId] = useState(null);
  const [errorMessage, setErrorMessage] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);
  const [localRowData, setLocalRowData] = useState([]);
  const [isWarningPopupOpen, setIsWarningPopupOpen] = useState(false);
  const [isAddUserPopupOpen, setIsAddUserPopupOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState(null);
  const gridRef = useRef(null);

  const { data: users, isLoading, isError, refetch } = useUsers();
  const updateUserMutation = useUpdateUser();
  const deleteUserMutation = useDeleteUser();

  useEffect(() => {
    if (users) {
      const processedUsers = users.map((user) => ({
        ...user,
        profilId: parseInt(user.profilId),
      }));
      setLocalRowData(processedUsers);
      console.log("Données initiales:", processedUsers);
    }
  }, [users]);

  const onModify = useCallback((id) => {
    setEditingRowId(id);
    if (gridRef.current && gridRef.current.api) {
      const rowNode = gridRef.current.api.getRowNode(id);
      if (rowNode) {
        gridRef.current.api.startEditingCell({
          rowIndex: rowNode.rowIndex,
          colKey: "nom",
        });
      }
    }
  }, []);

  const onSave = useCallback(
    async (id) => {
      if (gridRef.current && gridRef.current.api) {
        // Forcer la grille à terminer l'édition et capturer les modifications
        gridRef.current.api.stopEditing();
        // Récupérer la ligne modifiée depuis la grille
        const updatedRow = gridRef.current.api.getRowNode(id).data;
        if (updatedRow) {
          const dataToSend = {
            ...updatedRow,
            profilId: parseInt(updatedRow.profilId), // S'assurer que profilId est bien un entier
          };
          try {
            // Appel API pour mettre à jour l'utilisateur
            await updateUserMutation.mutateAsync(dataToSend);
            // Mettre à jour l'état local avec la nouvelle version de la ligne
            setLocalRowData((prevData) =>
              prevData.map((row) =>
                row.id === updatedRow.id ? dataToSend : row
              )
            );
            // Réinitialiser l'état d'édition
            setEditingRowId(null);
            setErrorMessage(null);
            setSuccessMessage("Utilisateur mis à jour avec succès");

            setTimeout(() => setSuccessMessage(null), 5000);
          } catch (error) {
            console.error("Erreur lors de la mise à jour:", error);
            setErrorMessage(
              error.message ||
                "Une erreur s'est produite lors de la mise à jour"
            );
            setTimeout(() => setErrorMessage(null), 5000);
          }
        }
      }
    },
    [localRowData, updateUserMutation]
  );

  const onCancel = useCallback(() => {
    if (gridRef.current && gridRef.current.api) {
      gridRef.current.api.stopEditing(true);
      setEditingRowId(null);
    }
  }, []);

  const onDelete = useCallback((user) => {
    setUserToDelete(user);
    setIsWarningPopupOpen(true);
  }, []);

  const handleConfirmDelete = useCallback(async () => {
    if (userToDelete) {
      try {
        await deleteUserMutation.mutateAsync(userToDelete.id);
        setIsWarningPopupOpen(false);
        setUserToDelete(null);
        setSuccessMessage("Utilisateur supprimé avec succès");
        setTimeout(() => setSuccessMessage(null), 5000);
        setLocalRowData((prev) =>
          prev.filter((user) => user.id !== userToDelete.id)
        );
      } catch (error) {
        console.error("Erreur lors de la suppression:", error);
        setErrorMessage(
          "Une erreur s'est produite lors de la suppression. Veuillez réessayer."
        );
        setTimeout(() => setErrorMessage(null), 5000);
      }
    }
  }, [userToDelete, deleteUserMutation]);

  const columnDefs = DefColUsers(
    onModify,
    onSave,
    onCancel,
    editingRowId,
    onDelete
  );

  if (isLoading) return <div>Chargement...</div>;
  if (isError)
    return <div>Une erreur s'est produite lors du chargement des données</div>;

  return (
    <div className="flex flex-col h-full p-4">
      <div className="p-2 mb-4 shadow-sm bg-slate-200 opacity-80 rounded-xl">
        <div className="flex items-center space-x-3">
          <CustomButton onClick={() => setIsAddUserPopupOpen(true)}>
            Ajouter un utilisateur
          </CustomButton>
        </div>
      </div>
      {errorMessage && <Alert variant="error">{errorMessage}</Alert>}
      {successMessage && <Alert variant="success">{successMessage}</Alert>}
      <div className="flex-grow ag-theme-quartz">
        <AgGridReact
          ref={gridRef}
          columnDefs={columnDefs}
          rowData={localRowData}
          rowSelection="multiple"
          getRowId={(params) => params.data.id.toString()}
          editType="fullRow"
          suppressClickEdit={true}
          onRowEditingStarted={(params) => setEditingRowId(params.node.id)}
          onRowEditingStopped={() => setEditingRowId(null)}
          domLayout="normal"
        />
      </div>
      <WarningPopup
        isOpen={isWarningPopupOpen}
        onClose={() => setIsWarningPopupOpen(false)}
        onConfirm={handleConfirmDelete}
        message="Êtes-vous sûr de vouloir supprimer cet utilisateur ?"
      />
      <AddUserPopup
        isOpen={isAddUserPopupOpen}
        onClose={() => {
          setIsAddUserPopupOpen(false);
          refetch();
        }}
      />
    </div>
  );
};

export default Utilisateur;
