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

import migrationdb.reseau.Station;
import migrationdb.reseau.VoieTOR;
import migrationdb.reseau.VoieTelemesuree;
import migrationdb.utils.ConversionUtils;
import migrationdb.utils.EnumUtils;

public class Defaut {
	
	private static int idSequence = 1;
	private static List<Defaut> defauts = new ArrayList<Defaut>();
	private static boolean debugMode;
	
	// Postgres fields
	private int id;
	private boolean active; // Not set in constructor
	private Timestamp appel;
	private String commentaire;
	private Timestamp debut;
	private Timestamp fin;
	private String typeDefaut;
	private int voieTelemesureeId; // Not set in constructor
	private int voieTORId; // Not set in constructor
	private int utilisateurId; // Not set in constructor
	private String versionEnregistreur;

	// Old fields
	private String libelleVoie; // Used when voieId could not be found
	private int numero;	// Old PK NumDéf access database
	
	// Temp fields	
	private String initialesStation;  // Defaut -> EnregistreurId
	private int numeroVoie; // To fix old libelle voie etat

	// Debug fields
	private boolean validation;
	
	public static void selectDefautsAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM PertesEnrg WHERE Type IN (10, 20) ORDER BY NumDéf ASC";
			System.out.println("Retrieving PertesEnrg (defauts)...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				Timestamp appel = ConversionUtils.getTimestamp(resultSet.getDate("DateDéfaut"), resultSet.getTime("DateDéfaut"));
				String commentaire = resultSet.getString("Commentaire") == null ? null : resultSet.getString("Commentaire").trim();
				if (resultSet.getString("Défaut") != null && resultSet.getString("Défaut").equalsIgnoreCase("Trouvé fin sans début")) {commentaire = "Trouvé fin sans début";}
				Timestamp debut = ConversionUtils.getTimestamp(resultSet.getDate("Début"), resultSet.getTime("Début"));
				Timestamp fin = ConversionUtils.getTimestamp(resultSet.getDate("Fin"), resultSet.getTime("Fin"));
				String typeDefaut = EnumUtils.getTypeDefaut(resultSet.getInt("Type"));
				String versionEnregistreur = resultSet.getString("VersionEnrg") == null ? null : resultSet.getString("VersionEnrg").trim();
				
				// Old fields
				String libelleVoie = resultSet.getString("NomVoie") == null ? null : resultSet.getString("NomVoie").trim().replace(' ', '_');
				int numero = resultSet.getInt("NumDéf");

				// Temp fields
				String initialesStation = resultSet.getString("Initiales") == null ? null : resultSet.getString("Initiales").trim();
				if (initialesStation.equalsIgnoreCase("jep")) {initialesStation = "JE";}
				else if (initialesStation.equalsIgnoreCase("lop")) {initialesStation = "LO";}
				int numeroVoie = resultSet.getInt("Voie");
				
				// Debug fields
				boolean validation = resultSet.getBoolean("Validation");

				Defaut defaut = new Defaut(appel, commentaire, debut, fin, typeDefaut, versionEnregistreur, libelleVoie, numero, initialesStation, numeroVoie, validation);
				defauts.add(defaut);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}

	public static void majWithStationsAndVoies(List<Station> stations, List<VoieTelemesuree> voiesTelemesurees, List<VoieTOR> voiesTOR) {
		for (Defaut defaut : defauts) {
			// voieTelemesureeId
			if (defaut.getTypeDefaut().equalsIgnoreCase("CAPTEUR")) {
				for (VoieTelemesuree voieTelemesuree : voiesTelemesurees) {
					if (voieTelemesuree.getInitialesStation().equalsIgnoreCase(defaut.getInitialesStation()) 
							&& voieTelemesuree.getLibelle().equalsIgnoreCase(defaut.getLibelleVoie())) {
						defaut.setVoieTelemesureeId(voieTelemesuree.getId());
						defaut.setLibelleVoie(null);
						break;
					}
				}

			}
			// voieTORId
			else if (defaut.getTypeDefaut().equalsIgnoreCase("ETAT")) {
				for (VoieTOR voieTOR : voiesTOR) {
					if (voieTOR.getInitialesStation().equalsIgnoreCase(defaut.getInitialesStation()) 
							&&  voieTOR.getLibelle().equalsIgnoreCase(defaut.getLibelleVoie())) {
						defaut.setVoieTORId(voieTOR.getId());
						defaut.setLibelleVoie(null);
						break;
					}
				} 
			} else {
				System.out.println("------------ Error with defaut n°" + defaut.getNumero() + " : type unknown");
			}
		}
	}
	
	public static void fixOldLibelleVoieEtat() {
		for (Defaut defaut : defauts) {
			if (!defaut.getTypeDefaut().equalsIgnoreCase("ETAT") || defaut.getVoieTORId() != 0) {continue;}
			
			if (defaut.getLibelleVoie() == null || defaut.getLibelleVoie().equalsIgnoreCase("Voie_état_??")) {
				defaut.setLibelleVoie("Voie Etat module " + defaut.getNumeroVoie());
			} else if (defaut.getLibelleVoie().startsWith("Voie_Etat_")) {
				if (defaut.getNumeroVoie() != Integer.parseInt(defaut.getLibelleVoie().substring(10))) {
					System.out.println("- No = " + defaut.getNumeroVoie() + ", libelle = " + defaut.getLibelleVoie());
					}
				defaut.setLibelleVoie("Voie Etat module " + defaut.getNumeroVoie());
		}}
	}
	
	public static void insertDefautsPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO defaut.defauts (id, actif, appel, commentaire, debut, fin, type, voie_telemesuree_id, voie_tor_id, utilisateur_id, version_enregistreur, old_libelle_voie, old_numero) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?);";
			if (debugMode) {query = "INSERT INTO defaut.defauts (id, actif, appel, commentaire, debut, fin, type, voie_telemesuree_id, voie_tor_id, utilisateur_id, version_enregistreur, old_libelle_voie, old_numero, debug_validation, debug_initiales) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";}
			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (Defaut defaut : defauts) {
				
				if (defaut.getInitialesStation().equalsIgnoreCase("XV") || defaut.getInitialesStation().equalsIgnoreCase("LO")) { continue; }
				
				insertStmt.setInt(1, defaut.getId());
				insertStmt.setBoolean(2, defaut.isActive());
				insertStmt.setTimestamp(3, defaut.getAppel());
				insertStmt.setString(4, defaut.getCommentaire());
				insertStmt.setTimestamp(5, defaut.getDebut());
				insertStmt.setTimestamp(6, defaut.getFin());
				insertStmt.setString(7, defaut.getTypeDefaut());
				if (defaut.getVoieTelemesureeId() == 0) {insertStmt.setNull(8, Types.INTEGER);} else {insertStmt.setInt(8, defaut.getVoieTelemesureeId());}
				if (defaut.getVoieTORId() == 0) {insertStmt.setNull(9, Types.INTEGER);} else {insertStmt.setInt(9, defaut.getVoieTORId());}
				if (defaut.getUtilisateurId() == 0) {insertStmt.setNull(10, Types.INTEGER);} else {insertStmt.setInt(10, defaut.getUtilisateurId());}
				insertStmt.setString(11, defaut.getVersionEnregistreur());
				insertStmt.setString(12, defaut.getLibelleVoie());
				insertStmt.setInt(13, defaut.getNumero());
				if (debugMode) {
					insertStmt.setBoolean(14, defaut.isValidation());
					insertStmt.setString(15, defaut.getInitialesStation());
				}
				insertStmt.addBatch();
			}

			System.out.println("Inserting defauts...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence 
			String seqQuery = "ALTER SEQUENCE defaut.defauts_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public Defaut() {
		this.id = idSequence++;
	}

	public Defaut(Timestamp appel, String commentaire, Timestamp debut, Timestamp fin, String typeDefaut, String versionEnregistreur, String libelleVoie, int numero, String initialesStation, int numeroVoie, boolean validation) {
		this.id = idSequence++;
		this.appel = appel;
		this.commentaire = commentaire;
		this.debut = debut;
		this.fin = fin;
		this.typeDefaut = typeDefaut;
		this.versionEnregistreur = versionEnregistreur;
		this.libelleVoie = libelleVoie;
		this.numero = numero;
		this.initialesStation = initialesStation;
		this.numeroVoie = numeroVoie;
		this.validation = validation;
	}
	
	public int getUtilisateurId() {
		return utilisateurId;
	}

	public int getNumeroVoie() {
		return numeroVoie;
	}

	public void setNumeroVoie(int numeroVoie) {
		this.numeroVoie = numeroVoie;
	}

	public void setUtilisateurId(int utilisateurId) {
		this.utilisateurId = utilisateurId;
	}

	public boolean isValidation() {
		return validation;
	}

	public void setValidation(boolean validation) {
		this.validation = validation;
	}

	public Timestamp getAppel() {
		return appel;
	}

	public void setAppel(Timestamp appel) {
		this.appel = appel;
	}

	public static List<Defaut> getDefauts() {
		return defauts;
	}

	public boolean isActive() {
		return active;
	}

	public void setActive(boolean active) {
		this.active = active;
	}

	public String getLibelleVoie() {
		return libelleVoie;
	}

	public void setLibelleVoie(String libelleVoie) {
		this.libelleVoie = libelleVoie;
	}

	public String getCommentaire() {
		return commentaire;
	}

	public void setCommentaire(String commentaire) {
		this.commentaire = commentaire;
	}

	public Timestamp getDebut() {
		return debut;
	}

	public void setDebut(Timestamp debut) {
		this.debut = debut;
	}

	public static boolean isDebugMode() {
		return debugMode;
	}

	public static void setDebugMode(boolean debugMode) {
		Defaut.debugMode = debugMode;
	}

	public Timestamp getFin() {
		return fin;
	}

	public void setFin(Timestamp fin) {
		this.fin = fin;
	}

	public int getNumero() {
		return numero;
	}

	public String getVersionEnregistreur() {
		return versionEnregistreur;
	}

	public void setVersionEnregistreur(String versionEnregistreur) {
		this.versionEnregistreur = versionEnregistreur;
	}

	public void setNumero(int numero) {
		this.numero = numero;
	}

	public String getTypeDefaut() {
		return typeDefaut;
	}

	public void setTypeDefaut(String typeDefaut) {
		this.typeDefaut = typeDefaut;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}

	public int getVoieTelemesureeId() {
		return voieTelemesureeId;
	}

	public void setVoieTelemesureeId(int voieTelemesureeId) {
		this.voieTelemesureeId = voieTelemesureeId;
	}

	public int getVoieTORId() {
		return voieTORId;
	}

	public void setVoieTORId(int voieTORId) {
		this.voieTORId = voieTORId;
	}
	
}
