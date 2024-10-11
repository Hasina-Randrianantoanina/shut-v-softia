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
import migrationdb.utils.ConversionUtils;

public class Alerte {

	private static int idSequence = 1;
	private static List<Alerte> alertes = new ArrayList<Alerte>();
	private static boolean debugMode;
	
	// Postgres fields
	private int id;
	private boolean acquitter;
	private String commentaire;
	private Timestamp dateAlerte;
	private String descriptionAlerte;
	private String parametreAction; // PS1
	private String parametreAlerte; // PE1
	private int stationId; // Not set in constructor 
	private int type;
	private int utilisateurId; // Null
	
	// Old fields
	private int numero;

	// Debug fields
	private String initialesStation;

	public static void selectAlertesAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM Alertes ORDER BY Initiales";
			System.out.println("Retrieving Alertes...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				boolean acquitter = resultSet.getBoolean("Acq");
				String commentaire = resultSet.getString("Comment") == null ? null : resultSet.getString("Comment").trim();
				Timestamp dateAlerte = ConversionUtils.getTimestamp(resultSet.getDate("DateHeure"), resultSet.getTime("DateHeure"));
				String descriptionAlerte = resultSet.getString("Texte") == null ? null : resultSet.getString("Texte").trim();
				int numero = resultSet.getInt("Num");
				String parametreAction = resultSet.getString("PS1") == null ? null : resultSet.getString("PS1").trim();
				String parametreAlerte = resultSet.getString("PE1") == null ? null : resultSet.getString("PE1").trim();
				int type = resultSet.getInt("Type");
				
				// Debug fields
				String initialesStation = resultSet.getString("Initiales") == null ? null : resultSet.getString("Initiales").trim();

				Alerte alerte = new Alerte(acquitter, commentaire, dateAlerte, descriptionAlerte, parametreAction, parametreAlerte, type, numero, initialesStation);
						
				alertes.add(alerte);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void majWithStations(List<Station> stations) {
		for (Alerte alerte : alertes) {
			for (Station station : stations) {
				if (alerte.getInitialesStation().equalsIgnoreCase(station.getInitiales())) {
					alerte.setStationId(station.getId());
					alerte.setInitialesStation(null);
					break;
				}
			}
		}
	}
	
	public static void insertAlertesPG(Connection connexion)
	{
		try {
			// Insert into table
			String query = "INSERT INTO defaut.alertes (id, acquitter, commentaire, date_alerte, description_alerte, parametre_action, parametre_alerte, station_id, type, utilisateur_id, old_numero) VALUES (?,?,?,?,?,?,?,?,?,?,?)";
			if (debugMode) {query = "INSERT INTO defaut.alertes (id, acquitter, commentaire, date_alerte, description_alerte, parametre_action, parametre_alerte, station_id, type, utilisateur_id, old_numero, debug_initiales) VALUES (?,?,?,?,?,?,?,?,?,?,?,?)";}
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Alerte alerte : alertes) {
				insertStmt.setInt(1, alerte.getId());
				insertStmt.setBoolean(2, alerte.isAcquitter());
				insertStmt.setString(3, alerte.getCommentaire());
				insertStmt.setTimestamp(4, alerte.getDateAlerte());
				insertStmt.setString(5, alerte.getDescriptionAlerte());
				insertStmt.setString(6, alerte.getParametreAction());
				insertStmt.setString(7, alerte.getParametreAlerte());
				if (alerte.getStationId() == 0){insertStmt.setNull(8, Types.INTEGER);} else {insertStmt.setInt(8, alerte.getStationId());}
				insertStmt.setInt(9, alerte.getType());
				if (alerte.getUtilisateurId() == 0){insertStmt.setNull(10, Types.INTEGER);} else {insertStmt.setInt(10, alerte.getUtilisateurId());}
				insertStmt.setInt(11, alerte.getNumero());
				if (debugMode) {insertStmt.setString(12, alerte.getInitialesStation());}
				insertStmt.addBatch();
			}

			System.out.println("Inserting alertes...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE defaut.alertes_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}	
	
	public Alerte() {
		this.id = idSequence++;
	}

	public Alerte(boolean acquitter, String commentaire, Timestamp dateAlerte, String descriptionAlerte, String parametreAction,
			String parametreAlerte, int type, int numero, String initialesStation) {
		this.id = idSequence++;
		this.acquitter = acquitter;
		this.commentaire = commentaire;
		this.dateAlerte = dateAlerte;
		this.descriptionAlerte = descriptionAlerte;
		this.parametreAction = parametreAction;
		this.parametreAlerte = parametreAlerte;
		this.type = type;
		this.numero = numero;
		this.initialesStation = initialesStation;
	}

	public boolean isDebugMode() {
		return debugMode;
	}

	public int getUtilisateurId() {
		return utilisateurId;
	}

	public void setUtilisateurId(int utilisateurId) {
		this.utilisateurId = utilisateurId;
	}

	public static void setDebugMode(boolean debugMode) {
		Alerte.debugMode = debugMode;
	}

	public static List<Alerte> getAlertes() {
		return alertes;
	}

	public String getDescriptionAlerte() {
		return descriptionAlerte;
	}

	public void setDescriptionAlerte(String descriptionAlerte) {
		this.descriptionAlerte = descriptionAlerte;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public boolean isAcquitter() {
		return acquitter;
	}

	public void setAcquitter(boolean acquitter) {
		this.acquitter = acquitter;
	}

	public String getCommentaire() {
		return commentaire;
	}

	public void setCommentaire(String commentaire) {
		this.commentaire = commentaire;
	}

	public Timestamp getDateAlerte() {
		return dateAlerte;
	}

	public void setDateAlerte(Timestamp dateAlerte) {
		this.dateAlerte = dateAlerte;
	}

	public int getNumero() {
		return numero;
	}

	public void setNumero(int numero) {
		this.numero = numero;
	}

	public String getParametreAction() {
		return parametreAction;
	}

	public void setParametreAction(String parametreAction) {
		this.parametreAction = parametreAction;
	}

	public String getParametreAlerte() {
		return parametreAlerte;
	}

	public void setParametreAlerte(String parametreAlerte) {
		this.parametreAlerte = parametreAlerte;
	}

	public int getStationId() {
		return stationId;
	}

	public void setStationId(int stationId) {
		this.stationId = stationId;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}

}
