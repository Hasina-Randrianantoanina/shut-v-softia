package migrationdb.reseau;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;

import migrationdb.utils.EnumUtils;

public class VoieTOR implements Cloneable{

	private static int idSequence = 1;
	private static List<VoieTOR> voiesTOR = new ArrayList<VoieTOR>();

	// Postgres fields
	private int id;
	private boolean active; // true
	private int adresseBES;
	private int info;
	private String libelle;
	private int numero;
	private int ordre;
	private int stationId; // Not set in constructor
	private String typeVoie;

	// Temp fields
	private String initialesStation;

	public static void selectVoiesTORAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM ES_TOR ORDER BY ini, num";
			System.out.println("Retrieving ES_TOR...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				boolean active = true;
				int adresseBES = resultSet.getInt("adrBES");
				int info = resultSet.getInt("info");
				String libelle = resultSet.getString("libel") == null ? null : resultSet.getString("libel").trim().replace(' ', '_');
				int numero = resultSet.getInt("num");
				int ordre = resultSet.getInt("ordre");
				String typeVoie = EnumUtils.getType(resultSet.getInt("type"));

				// Temps fields
				String initialesStation = resultSet.getString("ini") == null ? null : resultSet.getString("ini").trim();

				VoieTOR voie = new VoieTOR(active, adresseBES, info, libelle, numero, ordre, typeVoie, initialesStation);
				voiesTOR.add(voie);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void majWithStations(List<Station> stations) {
		for (VoieTOR voieTOR : voiesTOR) {
			for (Station station : stations) {
				if (voieTOR.getInitialesStation().equalsIgnoreCase(station.getInitiales())) {
					voieTOR.setStationId(station.getId());
					break;
				}
			}
		}
	}
	
	public static void addVoiesTORTest(List<Station> stationsTests) throws CloneNotSupportedException{
		
		List<VoieTOR> voiesTORTests = new ArrayList<VoieTOR>();
		
		for (Station station : stationsTests) {
			for (VoieTOR voie : voiesTOR) {
				if (voie.getStationId() != station.getTwinStationId()) { continue;}
				
				VoieTOR voieTest = (VoieTOR) voie.clone();
				voieTest.setId(idSequence++);
				voieTest.setStationId(station.getId());
				voieTest.setLibelle(voie.getLibelle()+ "_test");
				
				voiesTORTests.add(voieTest);
			}
		}

		voiesTOR.addAll(voiesTORTests);
	}
	
	public static void insertVoiesPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO reseau.voies_tor (id, actif, adresse_bes, info, libelle, numero, ordre, station_id, type) VALUES (?,?,?,?,?,?,?,?,?);";
			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (VoieTOR voieTOR : voiesTOR) {

				insertStmt.setInt(1, voieTOR.getId());
				insertStmt.setBoolean(2, voieTOR.isActive());
				insertStmt.setInt(3, voieTOR.getAdresseBES());
				insertStmt.setInt(4, voieTOR.getInfo());
				insertStmt.setString(5, voieTOR.getLibelle());
				insertStmt.setInt(6, voieTOR.getNumero());
				insertStmt.setInt(7, voieTOR.getOrdre());
				if (voieTOR.getStationId() == 0) {insertStmt.setNull(8, Types.INTEGER);} else {insertStmt.setInt(8, voieTOR.getStationId());}
				insertStmt.setString(9, voieTOR.getTypeVoie());
				insertStmt.addBatch();
			}

			System.out.println("Inserting voies_tor...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.voies_tor_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public VoieTOR() {
		this.id = idSequence++;
	}

	public VoieTOR(boolean active, int adresseBES, int info, String libelle, int numero, int ordre, String typeVoie, String initialesStation) {
		this.id = idSequence++;
		this.active = active;
		this.adresseBES = adresseBES;
		this.info = info;
		this.libelle = libelle;
		this.numero = numero;
		this.ordre = ordre;
		this.typeVoie = typeVoie;
		this.initialesStation = initialesStation;
	}

	public static List<VoieTOR> getVoiesTOR() {
		return voiesTOR;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getAdresseBES() {
		return adresseBES;
	}

	public void setAdresseBES(int adresseBES) {
		this.adresseBES = adresseBES;
	}

	public int getInfo() {
		return info;
	}

	public void setInfo(int info) {
		this.info = info;
	}

	public String getLibelle() {
		return libelle;
	}

	public void setLibelle(String libelle) {
		this.libelle = libelle;
	}

	public int getNumero() {
		return numero;
	}

	public void setNumero(int numero) {
		this.numero = numero;
	}

	public int getOrdre() {
		return ordre;
	}

	public void setOrdre(int ordre) {
		this.ordre = ordre;
	}

	public int getStationId() {
		return stationId;
	}

	public void setStationId(int stationId) {
		this.stationId = stationId;
	}

	public String getTypeVoie() {
		return typeVoie;
	}

	public void setTypeVoie(String typeVoie) {
		this.typeVoie = typeVoie;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}

	public boolean isActive() {
		return active;
	}

	public void setActive(boolean active) {
		this.active = active;
	}

}
