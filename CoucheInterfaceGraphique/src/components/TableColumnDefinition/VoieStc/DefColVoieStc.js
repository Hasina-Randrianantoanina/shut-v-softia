import { FaEdit, FaTrash } from 'react-icons/fa';
import { confirmAlert } from "react-confirm-alert";
// import Swal from 'sweetalert2';

const submit = () => {
  //alert("Voulez-vous supprimer")
  // Swal.fire({
  //   title: "Suppression de voie",
  //   text: "Vous voulez supprimer cette voie?",
  //   icon: "warning",
  //   showCancelButton: true,
  //   confirmButtonColor: "#3085d6",
  //   cancelButtonColor: "#d33",
  //   confirmButtonText: "Supprimer",
  //   cancelButtonText: "Annuler",
  // }).then((result) => {
  //   if (result.isConfirmed) {
  //     Swal.fire({
  //       title: "Suppression!",
  //       text: "Cette voie a été bien supprimé.",
  //       icon: "success"
  //     });
  //   }
  // });
    
};

const CustomButtonComponent = (props) => {
  return (
      <div className="flex space-x-2">
        <button onClick={submit} className="p-1 mt-3 bg-red-400 rounded-md shadow-sm hover:bg-red-600" >
            <FaTrash className="text-white cursor-pointer hover:text-white" />
        </button>
        <button className="p-1 mt-3 bg-green-400 rounded-md shadow-sm hover:bg-green-600" >
            <FaEdit className="text-white cursor-pointer hover:text-white"/>
        </button>
      </div>
  );
};

const DefColVoieStc = () => {
    return [
      {
        headerName: "Action",
        field: "",
        cellRenderer: CustomButtonComponent,
        width: 100,
        minWidth: 100,
        maxWidth: 120,
        cellStyle: { fontWeight: "bold" },
      },
      {
        headerName: "Voie",
        field: "voie",
        width: 100,
        minWidth: 100,
        maxWidth: 120,
        cellStyle: { fontWeight: "bold" },
      },
      {
        headerName: "Type",
        field: "type",
        width: 150,
        minWidth: 100,
        maxWidth: 200,
      },
      {
        headerName: "Nom",
        field: "nom",
        width: 150,
        minWidth: 100,
        maxWidth: 200,
        cellStyle: { fontWeight: "bold" },
      },
      {
        headerName: "Source",
        field: "source",
        width: 100,
        minWidth: 80,
        maxWidth: 120,
      },
      {
        headerName: "Voice",
        field: "voice",
        width: 180,
        minWidth: 150,
        maxWidth: 220,
      },
      {
        headerName: "Valeur",
        field: "valeur",
        flex: 1,
        width: 180,
        minWidth: 150,
        maxWidth: 220,
      },
      {
        headerName: "mdf V",
        field: "mdf_v",
        cellRenderer: "agCheckboxCellRenderer",
        cellEditor: "agCheckboxCellEditor",
        editable:true,
        flex: 1,
        /* width: 180,
        minWidth: 150,
        maxWidth: 220, */
      }
    ];
  };
  
  export default DefColVoieStc;