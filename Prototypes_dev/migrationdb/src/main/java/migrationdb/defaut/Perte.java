package migrationdb.defaut;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;

import migrationdb.administration.Utilisateur;
import migrationdb.reseau.Station;
import migrationdb.utils.ConversionUtils;
import migrationdb.utils.EnumUtils;

public class Perte {
	
	private static int idSequence = 1;
	private static List<Perte> pertes = new ArrayList<Perte>();
	private static boolean debugMode;

	// Postgres fields
	private int id;
	private Timestamp appel;
	private String cause;
	private String commentaire;
	private String critique;
	private Timestamp dateAcquisition;
	private Timestamp dateEnregistrement;
	private Timestamp dateGo;
	private Timestamp dateInit;
	private Timestamp dateStop;
	private Timestamp debut;
	private String defaut;
	private float diffHorloge; // secondes ?
	private int duree; // minutes
	private String etatDesTaches;
	private Timestamp fin;
	private Timestamp horloge;
	private String remede;
	private String type;
	private int stationId; // Not set in constructor
	private int utilisateurId; // Not set in constructor
	private String versionEnregistreur;
	
	// Old fields
	private String nomUtilisateur; // Used when utilisateurId could not be found
	private int numero;	// old PK NumDéf access database
	
	// Temp fields
	private String initialesStation; 
	private String nomVoie; // Pertes < 2006
	
	public static void selectPertesAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM PertesEnrg WHERE Type > 0 AND Type < 10 ORDER BY NumDéf";
			System.out.println("Retrieving PertesEnrg (pertes)...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				// TODO : appel OK pour 10 && 20
				Timestamp appel = ConversionUtils.getTimestamp(resultSet.getDate("DateDéfaut"), resultSet.getTime("DateDéfaut")); 
				String cause = resultSet.getString("Cause") == null ? null : EnumUtils.getCausePerte(resultSet.getString("Cause").trim());
				String commentaire = resultSet.getString("Commentaire") == null ? null : resultSet.getString("Commentaire").trim();
				String critique = resultSet.getString("Critique") == null ? null : resultSet.getString("Critique").trim();
				Timestamp dateAcquisition = ConversionUtils.getTimestamp(resultSet.getDate("Acq"), resultSet.getTime("Acq"));
				Timestamp dateEnregistrement = ConversionUtils.getTimestamp(resultSet.getDate("Enrg"), resultSet.getTime("Enrg"));
				Timestamp dateGo = ConversionUtils.getTimestamp(resultSet.getDate("Go"), resultSet.getTime("Go"));
				Timestamp dateInit = ConversionUtils.getTimestamp(resultSet.getDate("Init"), resultSet.getTime("Init"));
				Timestamp dateStop = ConversionUtils.getTimestamp(resultSet.getDate("Stop"), resultSet.getTime("Stop"));
				Timestamp debut = ConversionUtils.getTimestamp(resultSet.getDate("Début"), resultSet.getTime("Début"));
				String defaut = resultSet.getString("Défaut") == null ? null : EnumUtils.getDefautPerte(resultSet.getString("Défaut").trim());
				float diffHorloge = resultSet.getFloat("DiffHorl");
				Timestamp fin = ConversionUtils.getTimestamp(resultSet.getDate("Fin"), resultSet.getTime("Fin"));
				int duree = (int) Math.floor((fin.getTime() - debut.getTime())/60000d);
				String etatDesTaches = resultSet.getString("EtatdesTaches") == null ? null : resultSet.getString("EtatdesTaches").trim();
				if (etatDesTaches != null) {while (etatDesTaches.length() < 16) {etatDesTaches += "0";}}
				Timestamp horloge = ConversionUtils.getTimestamp(resultSet.getDate("HorlCible"), resultSet.getTime("HorlCible"));
				String remede = resultSet.getString("Remède") == null ? null : EnumUtils.getRemedePerte(resultSet.getString("Remède").trim());
				String type = EnumUtils.getTypePerte(resultSet.getInt("Type"));
				String versionEnregistreur = resultSet.getString("VersionEnrg") == null ? null : resultSet.getString("VersionEnrg").trim();
				
				// Old fields
				String nomUtilisateur = resultSet.getString("TraitéPar") == null ? null : EnumUtils.getNomUtilisateur(resultSet.getString("TraitéPar").trim());
				int numero = resultSet.getInt("NumDéf");

				// Temp fields
				String initialesStation = resultSet.getString("Initiales") == null ? null : resultSet.getString("Initiales").trim();
				if (initialesStation.equalsIgnoreCase("jep")) {initialesStation = "JE";}
				else if (initialesStation.equalsIgnoreCase("lop")) {initialesStation = "LO";}
				String nomVoie =  resultSet.getString("NomVoie") == null ? null : resultSet.getString("NomVoie").trim();
				if (nomVoie != null && !nomVoie.isEmpty()) {commentaire = "Possible defaut voie lié à une perte enregistreur vu en historique, non traité : " +nomVoie;}
				
				Perte perte = new Perte(appel, cause, commentaire, critique, dateAcquisition, dateEnregistrement, dateGo, dateInit, dateStop, debut, defaut, 
						diffHorloge, duree, etatDesTaches, fin, horloge, remede, type, versionEnregistreur, nomUtilisateur, numero,  initialesStation, nomVoie);
				pertes.add(perte);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void majWithStationsAndUtilisateurs(List<Station> stations, List<Utilisateur> utilisateurs) {
		for (Perte perte : pertes) {
			for (Station station : stations) {
				if (perte.getInitialesStation().equalsIgnoreCase(station.getInitiales())) {
					perte.setStationId(station.getId());
					break;
				}
			}
			if (perte.getNomUtilisateur() == null || perte.getNomUtilisateur().isEmpty()) { continue; }
			for (Utilisateur utilisateur : utilisateurs) {
				if (perte.getNomUtilisateur().equalsIgnoreCase(utilisateur.getNom())) {
					perte.setUtilisateurId(utilisateur.getId());
					perte.setNomUtilisateur(null);
					break;
				}
			}
		}
	}

	public static void insertPertesPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO defaut.pertes (id, appel, cause, commentaire, critique, date_acquisition, date_enregistrement, date_go, date_init, date_stop, debut, defaut, diff_horloge, duree,";
			query += " etat_des_taches, fin, horloge, remede, station_id, type, utilisateur_id, version_enregistreur, old_numero, old_nom_utilisateur)";
			query += " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";
			if (debugMode) {
				query = "INSERT INTO defaut.pertes (id, appel, cause, commentaire, critique, date_acquisition, date_enregistrement, date_go, date_init, date_stop, debut, defaut, diff_horloge, duree,";
				query += " etat_des_taches, fin, horloge, remede, station_id, type, utilisateur_id, version_enregistreur, old_numero, old_nom_utilisateur, debug_initiales, debug_nom_voie)";
				query += " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";
			}
			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (Perte perte : pertes) {
				
				if (perte.getInitialesStation().equalsIgnoreCase("XV") || perte.getInitialesStation().equalsIgnoreCase("LO")) { continue; }
				
				insertStmt.setInt(1, perte.getId());
				insertStmt.setTimestamp(2, perte.getAppel());
				insertStmt.setString(3, perte.getCause());
				insertStmt.setString(4, perte.getCommentaire());
				if (perte.getCritique() == null) {insertStmt.setNull(5, Types.BOOLEAN);} else {insertStmt.setBoolean(5, ConversionUtils.stringToBoolean(perte.getCritique()));}
				insertStmt.setTimestamp(6, perte.getDateAcquisition());
				insertStmt.setTimestamp(7, perte.getDateEnregistrement());
				insertStmt.setTimestamp(8, perte.getDateGo());
				insertStmt.setTimestamp(9, perte.getDateInit());
				insertStmt.setTimestamp(10, perte.getDateStop());
				insertStmt.setTimestamp(11, perte.getDebut());
				insertStmt.setString(12, perte.getDefaut());
				insertStmt.setFloat(13, perte.getDiffHorloge());
				insertStmt.setInt(14, perte.getDuree());
				insertStmt.setString(15, perte.getEtatDesTaches());
				insertStmt.setTimestamp(16, perte.getFin());
				insertStmt.setTimestamp(17, perte.getHorloge());
				insertStmt.setString(18, perte.getRemede());
				if (perte.getStationId() == 0) {insertStmt.setNull(19, Types.INTEGER);} else {insertStmt.setInt(19, perte.getStationId());}
				insertStmt.setString(20, perte.getType());
				if (perte.getUtilisateurId() == 0) {insertStmt.setNull(21, Types.INTEGER);} else {insertStmt.setInt(21, perte.getUtilisateurId());}
				insertStmt.setString(22, perte.getVersionEnregistreur());
				insertStmt.setInt(23, perte.getNumero());
				insertStmt.setString(24, perte.getNomUtilisateur());
				if (debugMode) {
					insertStmt.setString(25, perte.getInitialesStation());
					insertStmt.setString(26, perte.getNomVoie());
				}
				insertStmt.addBatch();
			}

			System.out.println("Inserting pertes...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence
			String seqQuery = "ALTER SEQUENCE defaut.pertes_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public Perte() {
		this.id = idSequence++;
		}
	
	public Perte(Timestamp appel, String cause, String commentaire, String critique, Timestamp dateAcquisition, Timestamp dateEnregistrement, Timestamp dateGo,
			 Timestamp dateInit, Timestamp dateStop, Timestamp debut, String defaut, float diffHorloge, int duree, String etatDesTaches, Timestamp fin, Timestamp horloge, 
			 String remede, String type, String versionEnregistreur, String nomUtilisateur, int numero, String initialesStation, String nomVoie) {
		this.id = idSequence++;
		this.appel = appel;
		this.cause = cause;
		this.commentaire = commentaire;
		this.critique = critique;
		this.dateAcquisition = dateAcquisition;
		this.dateEnregistrement = dateEnregistrement;
		this.dateGo = dateGo;
		this.dateInit = dateInit;
		this.dateStop = dateStop;
		this.debut = debut;
		this.defaut = defaut;
		this.diffHorloge = diffHorloge;
		this.duree = duree;
		this.etatDesTaches = etatDesTaches;
		this.fin = fin;
		this.horloge = horloge;
		this.remede = remede;
		this.type = type;
		this.versionEnregistreur = versionEnregistreur;
		this.nomUtilisateur = nomUtilisateur;
		this.numero = numero;
		this.initialesStation = initialesStation;
		this.nomVoie = nomVoie;
	}
	
	public static List<Perte> getPertes() {
		return pertes;
	}

	public String getNomVoie() {
		return nomVoie;
	}

	public void setNomVoie(String nomVoie) {
		this.nomVoie = nomVoie;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}

	public String getEtatDesTaches() {
		return etatDesTaches;
	}

	public void setEtatDesTaches(String etatDesTaches) {
		this.etatDesTaches = etatDesTaches;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public Timestamp getAppel() {
		return appel;
	}

	public void setAppel(Timestamp appel) {
		this.appel = appel;
	}

	public String getCause() {
		return cause;
	}

	public void setCause(String cause) {
		this.cause = cause;
	}

	public String getCommentaire() {
		return commentaire;
	}

	public void setCommentaire(String commentaire) {
		this.commentaire = commentaire;
	}

	public String getCritique() {
		return critique;
	}

	public void setCritique(String critique) {
		this.critique = critique;
	}

	public Timestamp getDateAcquisition() {
		return dateAcquisition;
	}

	public void setDateAcquisition(Timestamp dateAcquisition) {
		this.dateAcquisition = dateAcquisition;
	}

	public Timestamp getDateEnregistrement() {
		return dateEnregistrement;
	}

	public void setDateEnregistrement(Timestamp dateEnregistrement) {
		this.dateEnregistrement = dateEnregistrement;
	}

	public Timestamp getDateGo() {
		return dateGo;
	}

	public void setDateGo(Timestamp dateGo) {
		this.dateGo = dateGo;
	}

	public Timestamp getDateInit() {
		return dateInit;
	}

	public void setDateInit(Timestamp dateInit) {
		this.dateInit = dateInit;
	}

	public Timestamp getDebut() {
		return debut;
	}

	public void setDebut(Timestamp debut) {
		this.debut = debut;
	}

	public String getDefaut() {
		return defaut;
	}

	public void setDefaut(String defaut) {
		this.defaut = defaut;
	}

	public float getDiffHorloge() {
		return diffHorloge;
	}

	public void setDiffHorloge(float diffHorloge) {
		this.diffHorloge = diffHorloge;
	}

	public int getDuree() {
		return duree;
	}

	public Timestamp getDateStop() {
		return dateStop;
	}

	public void setDateStop(Timestamp dateStop) {
		this.dateStop = dateStop;
	}

	public void setDuree(int duree) {
		this.duree = duree;
	}

	public Timestamp getFin() {
		return fin;
	}

	public void setFin(Timestamp fin) {
		this.fin = fin;
	}

	public Timestamp getHorloge() {
		return horloge;
	}

	public void setHorloge(Timestamp horloge) {
		this.horloge = horloge;
	}

	public String getType() {
		return type;
	}

	public void setType(String type) {
		this.type = type;
	}

	public String getNomUtilisateur() {
		return nomUtilisateur;
	}

	public static boolean isDebugMode() {
		return debugMode;
	}

	public static void setDebugMode(boolean debugMode) {
		Perte.debugMode = debugMode;
	}

	public String getVersionEnregistreur() {
		return versionEnregistreur;
	}

	public void setVersionEnregistreur(String versionEnregistreur) {
		this.versionEnregistreur = versionEnregistreur;
	}

	public void setNomUtilisateur(String nomUtilisateur) {
		this.nomUtilisateur = nomUtilisateur;
	}

	public int getNumero() {
		return numero;
	}

	public void setNumero(int numero) {
		this.numero = numero;
	}

	public String getRemede() {
		return remede;
	}

	public void setRemede(String remede) {
		this.remede = remede;
	}

	public int getStationId() {
		return stationId;
	}

	public void setStationId(int stationId) {
		this.stationId = stationId;
	}

	public int getUtilisateurId() {
		return utilisateurId;
	}

	public void setUtilisateurId(int utilisateurId) {
		this.utilisateurId = utilisateurId;
	}
	
}


