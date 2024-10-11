package migrationdb;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

import migrationdb.administration.*;
import migrationdb.appel.*;
import migrationdb.defaut.*;
import migrationdb.reseau.*;

public class Main {

	public static void main(String[] args) {

		// Access
		final String PATH_TO_DB_ACCESS = "C:\\Users\\yann\\Desktop\\DataSHUTProd";
		
		//Postgres
		final String DB_NAME_POSTGRES = "shutweb_prod";
		final int PORT_POSTGRES = 5432;
		final String USER_POSTGRES = "postgres";
		final String PASSWORD_POSTGRES = "rrrrr";
		// Debug mode
		Defaut.setDebugMode(true); // Adding field : debug_validation, debug_initiales
		Alerte.setDebugMode(true); // Adding field : debug_initiales
		Perte.setDebugMode(true); // Adding field : debug_initiales, debug_nom_voie
		boolean forceProductionStationsToInactive = true;
		
		System.out.println("--------------- START ---------------");

		// ---------------------------------------
		// ------------ Select Access ------------
		// ---------------------------------------
		
		System.out.println("\n--- Connecting to access db pertes...\n");
		
		try {
			String dbpath = PATH_TO_DB_ACCESS + "\\BdPertes.mdb";
			String url = "jdbc:ucanaccess://" + dbpath;
			Connection connexionAccess = DriverManager.getConnection(url);
			
			// Defaut
			Defaut.selectDefautsAccess(connexionAccess);
			Perte.selectPertesAccess(connexionAccess);

			connexionAccess.close();

		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		System.out.println("\n--- Connecting to access db vb6...\n");

		try {
			String dbpath = PATH_TO_DB_ACCESS + "\\BdShut_vb6.mdb";
			String url = "jdbc:ucanaccess://" + dbpath;
			Connection connexionAccess = DriverManager.getConnection(url);
			
			// Reseau
			EvenementPluvieux.selectEvenementsPluvieuxAccess(connexionAccess);
			Enregistreur.selectEnregistreursAccess(connexionAccess, Reseau.USAGE);
			Enregistreur.selectEnregistreursAccess(connexionAccess, Reseau.OBSERVATION);
			Station.selectStationsAccess(connexionAccess, Reseau.USAGE);
			Station.selectStationsAccess(connexionAccess, Reseau.OBSERVATION);
			VoieTelemesuree.selectVoiesTelemesureesAccess(connexionAccess, Reseau.USAGE);
			VoieTelemesuree.selectVoiesTelemesureesAccess(connexionAccess, Reseau.OBSERVATION);
			VoieTOR.selectVoiesTORAccess(connexionAccess);
			Traitement.selectTraitementsAccess(connexionAccess);
			
			// Defaut
			DefautActif.selectDefautsActifsAccess(connexionAccess);
			Alerte.selectAlertesAccess(connexionAccess);
			
			// Appel
			Appel.selectAppelsAccess(connexionAccess);

			connexionAccess.close();

		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		// ---------------------------------------
		// ------------ Modifications ------------
		// ---------------------------------------
		System.out.println("\n--- Modifications...");
		
		// Reseau
		Enregistreur.addEnregisteursTest();
		Station.addStationsTest();
		// Enregistreur -> adresseIp, dernierAppel, dernierTransfert, dernierEnregistrement, liaison, pourcentageMemoire, version. Station -> numero, enregistreurId
		Enregistreur.majWithStations(Station.getStations());
		Station.majNumeroStationsRO(); // Station -> numero if Reseau.OBSERVATION using adresse_ip[3]
		VoieTelemesuree.majWithStations(Station.getStations()); // VoieTelemesuree -> stationId
		VoieTOR.majWithStations(Station.getStations()); // VoieTOR -> stationId
		try {
			VoieTelemesuree.addVoiesTelemesureesTest(Station.getStationsTests());
			VoieTOR.addVoiesTORTest(Station.getStationsTests());
		} catch (Exception e) {
			e.printStackTrace();
		}
		
		// Defauts
		// TODO : Defauts => maj active (fin = null, active = true)
		DefautActif.majWithStationsAndVoies(Station.getStations(), VoieTelemesuree.getVoiesTelemesurees(), VoieTOR.getVoiesTOR());
		Defaut.majWithStationsAndVoies(Station.getStations(), VoieTelemesuree.getVoiesTelemesurees(), VoieTOR.getVoiesTOR()); // Defaut -> enregistreurId, voieTelemesureeId, voieTORId, libelleVoie
		Defaut.fixOldLibelleVoieEtat();
		Perte.majWithStationsAndUtilisateurs(Station.getStations(), Utilisateur.getUtilisateurs()); // Perte -> stationId, utilisateurId
		Alerte.majWithStations(Station.getStations()); // Alerte -> stationId
		
		// Appel
		Appel.majWithStations(Station.getStations()); // Appel -> stationId, statut.
		
		if (forceProductionStationsToInactive) {Station.forceProductionStationsToInactive();}
		
		System.out.println("\tmodifications done !");
		
		// ---------------------------------------
		// ----------- Insert Postgres -----------
		// ---------------------------------------
		
		System.out.println("\n--- Connecting to postgres db " + DB_NAME_POSTGRES + "...\n");
		
		try {
			String urlPG = "jdbc:postgresql://localhost:" + PORT_POSTGRES + "/" + DB_NAME_POSTGRES;
			Connection connexionPG = DriverManager.getConnection(urlPG, USER_POSTGRES, PASSWORD_POSTGRES);

			// Administration
			Profil.insertProfilsPG(connexionPG);
			Utilisateur.insertUtilisateursPG(connexionPG);
			
			// Reseau
			EvenementPluvieux.insertEvenementsPluvieuxPG(connexionPG);
			Preselection.insertPreselectionsPG(connexionPG);
			Traitement.insertTraitementsPG(connexionPG);
			Enregistreur.insertEnregistreursPG(connexionPG);
			Station.insertStationsPG(connexionPG);
			VoieTelemesuree.insertVoiesPG(connexionPG);
			VoieTOR.insertVoiesPG(connexionPG);

			// Defaut
			DefautActif.insertDefautsActifsPG(connexionPG);
			Defaut.insertDefautsPG(connexionPG);
			Perte.insertPertesPG(connexionPG);
			Alerte.insertAlertesPG(connexionPG);
			
			// Appel
			Appel.insertAppelsPG(connexionPG);
			
			connexionPG.close();

		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		System.out.println("\n---------------- END ----------------");

	} // End main

}
