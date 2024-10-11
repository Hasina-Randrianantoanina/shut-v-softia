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

public class DefautActif {

	private static int idSequence = 1;
	private static List<DefautActif> defautsActifs = new ArrayList<DefautActif>();

	// Postgres fields
	private int id;
	private Timestamp appel;
	private String commentaire;
	private String descriptionDefaut;
	private String etatDesTaches; // Not set in constructor
	private int stationId; // Not set in constructor
	private String typeDefautActif;
	private int voieTelemesureeId; // Not set in constructor
	private int voieTORId; // Not set in constructor
	private int utilisateurId; // Null

	// Temp fields
	private String initiales;
	
	public static void selectDefautsActifsAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM Defauts";
			System.out.println("Retrieving Defauts...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				Timestamp appel = ConversionUtils.getTimestamp(resultSet.getDate("DateHeure"), resultSet.getTime("DateHeure"));
				String commentaire = resultSet.getString("Comment") == null ? null : resultSet.getString("Comment").trim();
				String typeDefautActif = EnumUtils.getTypeDefautActif(resultSet.getInt("NumDefaut"));
				String initiales = resultSet.getString("Initiales") == null ? null : resultSet.getString("Initiales").trim();
				String descriptionDefaut = resultSet.getString("Defaut") == null ? null : resultSet.getString("Defaut").trim();
				
				DefautActif defautActif = new DefautActif(appel, commentaire, typeDefautActif, initiales, descriptionDefaut);
				defautsActifs.add(defautActif);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void majWithStationsAndVoies(List<Station> stations, List<VoieTelemesuree> voiesTelemesurees, List<VoieTOR> voiesTOR) {
		for (DefautActif defautActif : defautsActifs) {
			if (defautActif.getTypeDefautActif() == null
					|| (!defautActif.getTypeDefautActif().equalsIgnoreCase("CAPTEUR") && !defautActif.getTypeDefautActif().equalsIgnoreCase("ETAT"))) {
				for (Station station : stations) {
					if (station.getInitiales().equalsIgnoreCase(defautActif.getInitiales())) {
						defautActif.setStationId(station.getId());
						break;
					}
				}
			} else if (defautActif.getTypeDefautActif().equalsIgnoreCase("CAPTEUR")) {
				defautActif.setDescriptionDefaut(defautActif.getDescriptionDefaut().replace(' ', '_'));
				for (VoieTelemesuree voieTelemesuree : voiesTelemesurees) {
					if (voieTelemesuree.getInitialesStation().equalsIgnoreCase(defautActif.getInitiales()) 
							&& voieTelemesuree.getLibelle().equalsIgnoreCase(defautActif.getDescriptionDefaut())) {
						defautActif.setVoieTelemesureeId(voieTelemesuree.getId());
						defautActif.setDescriptionDefaut(null);
						break;
					}
				}
			} else {
				defautActif.setDescriptionDefaut(defautActif.getDescriptionDefaut().replace(' ', '_'));
				for (VoieTOR voieTOR : voiesTOR) {
					if (voieTOR.getInitialesStation().equalsIgnoreCase(defautActif.getInitiales())
							&& voieTOR.getLibelle().equalsIgnoreCase(defautActif.getDescriptionDefaut())) {
						defautActif.setVoieTORId(voieTOR.getId());
						defautActif.setDescriptionDefaut(null);
						break;
					}
				}
			}
		}
	}
	
	public static void insertDefautsActifsPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO defaut.defauts_actifs (id, appel, commentaire, description_defaut, station_id, type, voie_telemesuree_id, voie_tor_id, utilisateur_id) VALUES (?,?,?,?,?,?,?,?,?);";
			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (DefautActif defautActif : defautsActifs) {
				insertStmt.setInt(1, defautActif.getId());
				insertStmt.setTimestamp(2, defautActif.getAppel());
				insertStmt.setString(3, defautActif.getCommentaire());
				insertStmt.setString(4, defautActif.getDescriptionDefaut());
				if (defautActif.getStationId() == 0) {insertStmt.setNull(5, Types.INTEGER);} else {insertStmt.setInt(5, defautActif.getStationId());}
				insertStmt.setString(6, defautActif.getTypeDefautActif());
				if (defautActif.getVoieTelemesureeId() == 0) {insertStmt.setNull(7, Types.INTEGER);} else {insertStmt.setInt(7, defautActif.getVoieTelemesureeId());}
				if (defautActif.getVoieTORId() == 0) {insertStmt.setNull(8, Types.INTEGER);} else {insertStmt.setInt(8, defautActif.getVoieTORId());}
				if (defautActif.getUtilisateurId() == 0) {insertStmt.setNull(9, Types.INTEGER);} else {insertStmt.setInt(9, defautActif.getUtilisateurId());}
				insertStmt.addBatch();
			}

			System.out.println("Inserting defauts_actifs...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence
			String seqQuery = "ALTER SEQUENCE defaut.defauts_actifs_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public DefautActif(Timestamp appel, String commentaire, String typeDefautActif, String initiales, String descriptionDefaut) {
		this.id = idSequence++;
		this.appel = appel;
		this.commentaire = commentaire;
		this.typeDefautActif = typeDefautActif;
		this.initiales = initiales;
		this.descriptionDefaut = descriptionDefaut;
	}

	public DefautActif() {
		this.id = idSequence++;
	}
	
	public int getUtilisateurId() {
		return utilisateurId;
	}

	public String getEtatDesTaches() {
		return etatDesTaches;
	}

	public void setEtatDesTaches(String etatDesTaches) {
		this.etatDesTaches = etatDesTaches;
	}

	public void setUtilisateurId(int utilisateurId) {
		this.utilisateurId = utilisateurId;
	}

	public String getInitiales() {
		return initiales;
	}

	public void setInitiales(String initiales) {
		this.initiales = initiales;
	}

	public Timestamp getAppel() {
		return appel;
	}

	public void setAppel(Timestamp appel) {
		this.appel = appel;
	}

	public String getTypeDefautActif() {
		return typeDefautActif;
	}

	public void setTypeDefautActif(String typeDefautActif) {
		this.typeDefautActif = typeDefautActif;
	}

	public String getDescriptionDefaut() {
		return descriptionDefaut;
	}

	public void setDescriptionDefaut(String descriptionDefaut) {
		this.descriptionDefaut = descriptionDefaut;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getCommentaire() {
		return commentaire;
	}

	public void setCommentaire(String commentaire) {
		this.commentaire = commentaire;
	}


	public int getStationId() {
		return stationId;
	}

	public void setStationId(int stationId) {
		this.stationId = stationId;
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

	public static List<DefautActif> getDefautsActifs() {
		return defautsActifs;
	}

}
