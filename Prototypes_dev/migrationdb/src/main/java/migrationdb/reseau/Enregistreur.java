package migrationdb.reseau;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.List;

import migrationdb.utils.ConversionUtils;

public class Enregistreur {

	private static int idSequence = 1;
	private static List<Enregistreur> enregistreurs = new ArrayList<Enregistreur>();

	// Postgres fields
	private int id;
	private String adresseIP; // Not set in constructor
	private Timestamp maj;
	private Timestamp dernierAppel; // Not set in constructor
	private Timestamp dernierTransfert; // Not set in constructor
	private Timestamp dernierEnregistrement; // Not set in constructor
	private String liaison; // Not set in constructor
	private int pourcentageMemoire; // Not set in constructor
	private String typeHeure; // Not set in constructor
	private String version; // Not set in constructor
	
	// Temp fields
	private String initialesStation; 
	private int numeroStation; // Station -> numero
	
	public static void selectEnregistreursAccess(Connection connexion, Reseau reseau) {
		String tableName = "";
		switch(reseau)
		{
		case USAGE:
			tableName = "enregistreurs";
			break;
		case OBSERVATION:
			tableName = "enregistreurs_resobs";
			break;
		}
		
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM " + tableName + " ORDER BY ini";
			System.out.println("Retrieving " + tableName + "...");
			ResultSet resultSet = stmt.executeQuery(query);
			
			while (resultSet.next()) {
				// Postgres fields
				Timestamp maj = ConversionUtils.getTimestamp(resultSet.getDate("DateMaj"), resultSet.getTime("DateMaj"));
				
				// Temp fields
				String initialesStation = resultSet.getString("ini") == null ? null : resultSet.getString("ini").trim();
				int numeroStation = resultSet.getInt("n_sta");
				
				Enregistreur enregistreur = new Enregistreur(maj, initialesStation, numeroStation);
				enregistreurs.add(enregistreur);
			}
			
			resultSet.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	@SuppressWarnings("deprecation")
	public static void addEnregisteursTest()
	{
		enregistreurs.add(new Enregistreur (new Timestamp(0, 0, 1, 1, 0, 0, 0), "EN_test" ,252));
		enregistreurs.add(new Enregistreur (new Timestamp(0, 0, 1, 1, 0, 0, 0), "GP_test" ,253));
		enregistreurs.add(new Enregistreur (new Timestamp(0, 0, 1, 1, 0, 0, 0), "PM_test" ,254));
	}
	
	public static void majWithStations(List<Station> stations)
	{
		for (Enregistreur enregistreur : enregistreurs)
		{
			for (Station station : stations)
			{
				if (enregistreur.getInitialesStation().equalsIgnoreCase(station.getInitiales())) {
					enregistreur.setAdresseIP(station.getAdresseIP());
					enregistreur.setDernierAppel(station.getDernierAppel());
					enregistreur.setDernierTransfert(station.getDernierTransfert());
					if (station.getLiaison().equalsIgnoreCase("AP") || station.getLiaison().equalsIgnoreCase("XX")){ enregistreur.setDernierEnregistrement(station.getDernierTransfert());}
					enregistreur.setLiaison(station.getLiaison());
					enregistreur.setPourcentageMemoire(station.getPourcentageMemoire());
					enregistreur.setTypeHeure(station.getTypeHeure());
					enregistreur.setVersion(station.getVersionEnregistreur());
					station.setNumero(enregistreur.getNumeroStation());
					station.setEnregistreurId(enregistreur.getId());
					break;
				}
			}
		}
	}
	
	public static void insertEnregistreursPG(Connection connexion)
	{
		try {
			// Insert into table
			String query = "INSERT INTO reseau.enregistreurs (id, adresse_ip, dernier_appel, dernier_transfert, dernier_enregistrement, liaison, mise_a_jour, pourcentage_memoire, type_heure, version)";
			query += " VALUES (?,?,?,?,?,?,?,?,?,?)";
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Enregistreur enregistreur : enregistreurs) {
				
				insertStmt.setInt(1, enregistreur.getId());
				insertStmt.setString(2, enregistreur.getAdresseIP());
				insertStmt.setTimestamp(3, enregistreur.getDernierAppel());
				insertStmt.setTimestamp(4, enregistreur.getDernierTransfert());
				insertStmt.setTimestamp(5, enregistreur.getDernierEnregistrement());
				insertStmt.setString(6, enregistreur.getLiaison());
				insertStmt.setTimestamp(7, enregistreur.getMaj());
				insertStmt.setInt(8, enregistreur.getPourcentageMemoire());
				insertStmt.setString(9, enregistreur.getTypeHeure());
				insertStmt.setString(10, enregistreur.getVersion());
				insertStmt.addBatch();
			}

			System.out.println("Inserting enregistreurs...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.enregistreurs_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public Enregistreur() {
		this.id = idSequence++;
	}
	
	public Enregistreur(Timestamp maj, String initialesStation, int numeroStation) {
		this.id = idSequence++;
		this.maj = maj;
		this.initialesStation = initialesStation;
		this.numeroStation = numeroStation;
	}

	public static List<Enregistreur> getEnregistreurs() {
		return enregistreurs;
	}

	public String getAdresseIP() {
		return adresseIP;
	}

	public void setAdresseIP(String adresseIP) {
		this.adresseIP = adresseIP;
	}

	public String getLiaison() {
		return liaison;
	}

	public void setLiaison(String liaison) {
		this.liaison = liaison;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public Timestamp getDernierAppel() {
		return dernierAppel;
	}

	public void setDernierAppel(Timestamp dernierAppel) {
		this.dernierAppel = dernierAppel;
	}

	public Timestamp getDernierEnregistrement() {
		return dernierEnregistrement;
	}

	public void setDernierEnregistrement(Timestamp dernierEnregistrement) {
		this.dernierEnregistrement = dernierEnregistrement;
	}

	public int getPourcentageMemoire() {
		return pourcentageMemoire;
	}

	public void setPourcentageMemoire(int pourcentageMemoire) {
		this.pourcentageMemoire = pourcentageMemoire;
	}

	public Timestamp getDernierTransfert() {
		return dernierTransfert;
	}

	public void setDernierTransfert(Timestamp dernierTransfert) {
		this.dernierTransfert = dernierTransfert;
	}

	public int getNumeroStation() {
		return numeroStation;
	}

	public void setNumeroStation(int numeroStation) {
		this.numeroStation = numeroStation;
	}

	public Timestamp getMaj() {
		return maj;
	}

	public void setMaj(Timestamp maj) {
		this.maj = maj;
	}

	public String getVersion() {
		return version;
	}

	public void setVersion(String version) {
		this.version = version;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}

	public String getTypeHeure() {
		return typeHeure;
	}

	public void setTypeHeure(String typeHeure) {
		this.typeHeure = typeHeure;
	}

}
