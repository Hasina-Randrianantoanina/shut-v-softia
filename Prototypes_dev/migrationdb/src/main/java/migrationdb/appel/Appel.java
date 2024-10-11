package migrationdb.appel;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import migrationdb.reseau.Station;
import migrationdb.utils.ConversionUtils;

public class Appel {
	
	private static int idSequence = 1;
	private static List<Appel> appels = new ArrayList<Appel>();

	// Postgres fields
	private int id;
	private Timestamp dateAppel;
	private int stationId; // Not set in constructor 
	private String statut; // Not set in constructor 
	
	// Temp fields
	private String initialesStation;

	public static void selectAppelsAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM Appels ORDER BY HeureAppel DESC";
			System.out.println("Retrieving Appels...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				Timestamp dateAppel = ConversionUtils.getTimestamp(resultSet.getDate("HeureAppel"), resultSet.getTime("HeureAppel"));

				// Temp fields
				String initialesStation = resultSet.getString("Initiales") == null ? null : resultSet.getString("Initiales").trim();

				Appel appel = new Appel(dateAppel, initialesStation);
				appels.add(appel);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void majWithStations(List<Station> stations) {
		for (Station station : stations) {
			List<Appel> appelsOfThisStation = appels
					.stream()
					.filter(a -> a.getInitialesStation().equalsIgnoreCase(station.getInitiales()))
					.collect(Collectors.toList());
			
			for (int i = 0; i < appelsOfThisStation.size(); i++ ) {
				appelsOfThisStation.get(i).setStationId(station.getId());
				if (i == 0) {appelsOfThisStation.get(i).setStatut(station.getStatutAppel());}
			}
		}
	}
	
	public static void insertAppelsPG(Connection connexion)
	{
		try {
			// Insert into table
			String query = "INSERT INTO appel.appels (id, date_appel, station_id, statut) VALUES (?,?,?,?)";
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Appel appel : appels) {
				
				insertStmt.setInt(1, appel.getId());
				insertStmt.setTimestamp(2, appel.getDateAppel());
				if (appel.getStationId() == 0){insertStmt.setNull(3, Types.INTEGER);} else {insertStmt.setInt(3, appel.getStationId());}
				insertStmt.setString(4, appel.getStatut());
				insertStmt.addBatch();
			}

			System.out.println("Inserting appels...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE appel.appels_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}	
	
	public static List<Appel> getAppels() {
		return appels;
	}

	public Appel() {
		this.id = idSequence++;
	}
	
	public Appel(Timestamp dateAppel, String initialesStation) {
		this.id = idSequence++;
		this.dateAppel = dateAppel;
		this.initialesStation = initialesStation;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public Timestamp getDateAppel() {
		return dateAppel;
	}

	public void setDateAppel(Timestamp dateAppel) {
		this.dateAppel = dateAppel;
	}

	public int getStationId() {
		return stationId;
	}

	public void setStationId(int stationId) {
		this.stationId = stationId;
	}

	public String getStatut() {
		return statut;
	}

	public void setStatut(String statut) {
		this.statut = statut;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}
}
