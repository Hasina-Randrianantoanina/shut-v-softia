package migrationdb.reseau;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;

import migrationdb.utils.ConversionUtils;
import migrationdb.utils.EnumUtils;

public class Station {

	private static int idSequence = 1;
	private static List<Station> stations = new ArrayList<Station>();
	private static List<Station> stationsTests = new ArrayList<Station>();
	
	// Postgres fields
	private int id;
	private long abonnements;
	private boolean active;
	private String bassinVersant;
	private int enregistreurId; // Not set in constructor
	private String initiales;
	private String nom;
	private int numero; // Not set in constructor
	private long preselections;
	private String reseau; 

	// Temp fields
	private String adresseIP; // Enregistreur -> adresseIp
	private Timestamp dernierAppel; // Enregistreur -> dernierAppel
	private Timestamp dernierTransfert; // Enregistreur -> dernierTransfert
	private String liaison; // Enregistreur -> liaison
	private int pourcentageMemoire; // Enregistreur -> pourcentageMemoire
	private String statutAppel; // Appel -> statut
	private String typeHeure; // Enregistreur -> typeHeure
	private String versionEnregistreur; // Enregistreur -> version
	private int twinStationId; // Tests stations

	public static void selectStationsAccess(Connection connexion, Reseau reseau) {
		String tableName = "";
		switch(reseau)
		{
		case USAGE:
			tableName = "Stations";
			break;
		case OBSERVATION:
			tableName = "Stations_resobs";
			break;
		}
		
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM " + tableName + " ORDER BY Initiales";
			System.out.println("Retrieving " + tableName + "...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				long abonnements = ConversionUtils.getAbonnements(resultSet.getInt("Abts"));
				boolean active = resultSet.getBoolean("TransfertAuto");
				String bassinVersant = resultSet.getString("BassinVersant") == null ? null : resultSet.getString("BassinVersant").trim();
				String initiales = resultSet.getString("Initiales") == null ? null : resultSet.getString("Initiales").trim();
				String nom = resultSet.getString("Nom") == null ? null : resultSet.getString("Nom").trim();
				long preselections = ConversionUtils.getPreselections(resultSet.getInt("TypeSta"));

				// Temp fields
				String adresseIP = resultSet.getString("AdresseIP") == null ? null : resultSet.getString("AdresseIP").trim();
				Timestamp dernierTransfert = ConversionUtils.getTimestamp(resultSet.getDate("DernierTrf"), resultSet.getTime("DernierTrf"));
				Timestamp dernierAppel = ConversionUtils.getTimestamp(resultSet.getDate("DernierAppel"), resultSet.getTime("DernierAppel"));
				String liaison = resultSet.getString("Liaison") == null ? null : resultSet.getString("Liaison").trim();
				int pourcentageMemoire = resultSet.getInt("pcMémoire");
				String statutAppel = EnumUtils.getStatutAppel(resultSet.getInt("Appel"));
				String versionEnregistreur = resultSet.getString("VersionENrg") == null ? null : resultSet.getString("VersionENrg").trim();
				String typeHeure = EnumUtils.getTypeHeure(dernierTransfert, initiales, versionEnregistreur, reseau);
				if (typeHeure.equalsIgnoreCase("UNIVERSELLE")) {dernierTransfert = ConversionUtils.getTimeStampUTC(resultSet.getDate("DernierTrf"), resultSet.getTime("DernierTrf"));}

				if (initiales.equalsIgnoreCase("LQ")) {active = false;} // Special case : LQ with liaison RC (not implemented)
				
				Station station = new Station(abonnements, active, bassinVersant, initiales, nom, preselections, reseau.toString(), adresseIP, dernierAppel, dernierTransfert, liaison, pourcentageMemoire, statutAppel, typeHeure, versionEnregistreur);
				stations.add(station);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	@SuppressWarnings("deprecation")
	public static void addStationsTest() {
		for (Station station : stations) {
			Station stationTest = null;
			
			if (station.getInitiales().equalsIgnoreCase("XY")) 
			{
				station.setDernierTransfert(new Timestamp(124, 9, 10, 0, 0, 0, 0));
				continue;
			}
			
			if (!station.getInitiales().equalsIgnoreCase("EN")
					&& !station.getInitiales().equalsIgnoreCase("GP")
					&& !station.getInitiales().equalsIgnoreCase("PM"))  {
				continue;
			}
			
			stationTest = new Station();
			stationTest.setAbonnements(station.getAbonnements());
			stationTest.setActive(true);
			stationTest.setBassinVersant(station.getBassinVersant());
			stationTest.setDernierAppel(new Timestamp(124, 9, 10, 0, 0, 0, 0));
			stationTest.setDernierTransfert(new Timestamp(124, 9, 10, 0, 0, 0, 0));
			stationTest.setInitiales(station.getInitiales() + "_test");
			stationTest.setNom("TEST " + station.getNom());
			stationTest.setPreselections(station.getPreselections());
			stationTest.setReseau(station.getReseau());
			stationTest.setTwinStationId(station.getId());
			
			if (station.getInitiales().equalsIgnoreCase("EN")) {
				stationTest.setAdresseIP("192.168.20.1");
				stationTest.setLiaison("AP");
				stationTest.setVersionEnregistreur("D13A53Aa01");
				stationTest.setTypeHeure("LOCALE");
			} else if (station.getInitiales().equalsIgnoreCase("GP")) {
				stationTest.setAdresseIP("192.168.229.9");
				stationTest.setLiaison("IP");
				stationTest.setVersionEnregistreur("D12Z52IFc01");
				stationTest.setTypeHeure("LOCALE");
			} else if (station.getInitiales().equalsIgnoreCase("PM")) {
				stationTest.setAdresseIP("192.168.218.9");
				stationTest.setLiaison("IP");
				stationTest.setVersionEnregistreur("D11Y52IFa01");
				stationTest.setTypeHeure("LOCALE");
			}
			stationsTests.add(stationTest);
		}
		stations.addAll(stationsTests);
	}
	
	public static void majNumeroStationsRO() {
		for (Station station : stations) {
			if (station.getReseau().equalsIgnoreCase("OBSERVATION") && station.getAdresseIP() != null) {
				String[] parts = station.getAdresseIP().split("\\.");
				if (parts.length != 4) {
					System.out.println("AdresseIP does not contain 4 parts for the station " + station.getInitiales() + " => " + station.getAdresseIP());}
				else {
					station.setNumero(Integer.parseInt(parts[2]));
				}
			}
		}
	}
	
	public static void forceProductionStationsToInactive() {
		for (Station station : stations) {
			station.setActive(false);
			if (station.getInitiales().equalsIgnoreCase("XY")) {station.setActive(true);}
		}
		for (Station stationTest : stationsTests) {
			stationTest.setActive(true);
		}
	}
	
	public static void insertStationsPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO reseau.stations (id, abonnements, actif, bassin_versant, enregistreur_id, initiales, nom, numero, preselections, reseau)";
			query += " VALUES (?,?,?,?,?,?,?,?,?,?);";

			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (Station station : stations) {

				insertStmt.setInt(1, station.getId());
				insertStmt.setLong(2, station.getAbonnements());
				insertStmt.setBoolean(3, station.isActive());
				insertStmt.setString(4, station.getBassinVersant());
				if (station.getEnregistreurId() == 0){insertStmt.setNull(5, Types.INTEGER);} else {insertStmt.setInt(5, station.getEnregistreurId());}
				insertStmt.setString(6, station.getInitiales());
				insertStmt.setString(7, station.getNom());
				insertStmt.setInt(8, station.getNumero());
				insertStmt.setLong(9, station.getPreselections());
				insertStmt.setString(10, station.getReseau());
				insertStmt.addBatch();
			}

			System.out.println("Inserting stations...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.stations_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public Station() {
		this.id = idSequence++;
	}

	public Station(long abonnements, boolean active, String bassinVersant , String initiales, String nom, long preselections, String reseau, String adresseIP, 
			Timestamp dernierAppel, Timestamp dernierTransfert, String liaison,  int pourcentageMemoire, String statutAppel, String typeHeure, String versionEnregistreur) {
		this.id = idSequence++;
		this.abonnements = abonnements;
		this.active = active;
		this.bassinVersant = bassinVersant;
		this.initiales = initiales;
		this.nom = nom;
		this.preselections = preselections;
		this.reseau = reseau;
		this.adresseIP = adresseIP;
		this.dernierAppel = dernierAppel;
		this.dernierTransfert = dernierTransfert;
		this.liaison = liaison;
		this.pourcentageMemoire = pourcentageMemoire;
		this.statutAppel = statutAppel;
		this.typeHeure = typeHeure;
		this.versionEnregistreur = versionEnregistreur;
	}
	
	public String getReseau() {
		return reseau;
	}

	public void setReseau(String reseau) {
		this.reseau = reseau;
	}

	public long getPreselections() {
		return preselections;
	}

	public static List<Station> getStationsTests() {
		return stationsTests;
	}

	public void setPreselections(long preselections) {
		this.preselections = preselections;
	}

	public static List<Station> getStations() {
		return stations;
	}

	public boolean isActive() {
		return active;
	}

	public void setActive(boolean active) {
		this.active = active;
	}

	public String getStatutAppel() {
		return statutAppel;
	}

	public void setStatutAppel(String statutAppel) {
		this.statutAppel = statutAppel;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getAdresseIP() {
		return adresseIP;
	}

	public void setAdresseIP(String adresseIP) {
		this.adresseIP = adresseIP;
	}

	public String getBassinVersant() {
		return bassinVersant;
	}

	public void setBassinVersant(String bassinVersant) {
		this.bassinVersant = bassinVersant;
	}

	public String getInitiales() {
		return initiales;
	}

	public void setInitiales(String initiales) {
		this.initiales = initiales;
	}

	public String getLiaison() {
		return liaison;
	}

	public void setLiaison(String liaison) {
		this.liaison = liaison;
	}

	public String getNom() {
		return nom;
	}

	public void setNom(String nom) {
		this.nom = nom;
	}

	public int getNumero() {
		return numero;
	}

	public void setNumero(int numero) {
		this.numero = numero;
	}

	public int getEnregistreurId() {
		return enregistreurId;
	}

	public void setEnregistreurId(int enregistreurId) {
		this.enregistreurId = enregistreurId;
	}

	public int getPourcentageMemoire() {
		return pourcentageMemoire;
	}

	public void setPourcentageMemoire(int pourcentageMemoire) {
		this.pourcentageMemoire = pourcentageMemoire;
	}

	public String getVersionEnregistreur() {
		return versionEnregistreur;
	}

	public void setVersionEnregistreur(String versionEnregistreur) {
		this.versionEnregistreur = versionEnregistreur;
	}

	public Timestamp getDernierAppel() {
		return dernierAppel;
	}

	public void setDernierAppel(Timestamp dernierAppel) {
		this.dernierAppel = dernierAppel;
	}

	public Timestamp getDernierTransfert() {
		return dernierTransfert;
	}

	public void setDernierTransfert(Timestamp dernierTransfert) {
		this.dernierTransfert = dernierTransfert;
	}

	public String getTypeHeure() {
		return typeHeure;
	}

	public void setTypeHeure(String typeHeure) {
		this.typeHeure = typeHeure;
	}

	public long getAbonnements() {
		return abonnements;
	}

	public void setAbonnements(long abonnements) {
		this.abonnements = abonnements;
	}

	public int getTwinStationId() {
		return twinStationId;
	}

	public void setTwinStationId(int twinStationId) {
		this.twinStationId = twinStationId;
	}

}
