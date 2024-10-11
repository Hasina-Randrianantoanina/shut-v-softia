package migrationdb.reseau;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;

import migrationdb.utils.ConversionUtils;
import migrationdb.utils.EnumUtils;

public class VoieTelemesuree implements Cloneable {

	private static int idSequence = 1;
	private static List<VoieTelemesuree> voiesTelemesurees = new ArrayList<VoieTelemesuree>();
	
	// Postgres fields
	private int id;
	private long abonnements;
	private boolean active; // true
	private int adresseBES;
	private float delta;
	private int groupe;
	private int info;
	private String libelle;
	private int numero;
	private int ordre;
	private int parametre1;
	private int parametre2;
	private int parametre3;
	private int parametre4;
	private int parametre5;
	private int parametre6;
	private int parametre7;
	private int parametre8;
	private int parametre9;
	private int parametre10;
	private String priorite;
	private float seuilBas;
	private float seuilHaut;
	private int stationId; // Not set in constructor
	private int traitementId;
	private String unite;
	private int virgule;
	private int voieEnregistree;
	private int voieStockee;

	// Temp fields
	private String initialesStation;

	public static void selectVoiesTelemesureesAccess(Connection connexion, Reseau reseau) {
		String tableName = "";
		switch(reseau)
		{
		case USAGE:
			tableName = "voies_internes";
			break;
		case OBSERVATION:
			tableName = "voies_resobs";
			break;
		}
		
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM " + tableName + " ORDER BY ini, num";
			System.out.println("Retrieving " + tableName + "...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				long abonnements = ConversionUtils.getAbonnements(resultSet.getInt("abts"));
				boolean active = true;
				int adresseBES = resultSet.getInt("adrBES");
				float delta = resultSet.getFloat("delta");
				int groupe = resultSet.getInt("groupe");
				int info = resultSet.getInt("info");
				String libelle = resultSet.getString("libel") == null ? null : resultSet.getString("libel").trim().replace(' ', '_');
				int numero = resultSet.getInt("num");
				int ordre = resultSet.getInt("ordre");
				int[] parametres = new int[10];
				for (int i = 1; i <= 10; i++) {
					parametres[i-1] = resultSet.getInt("para" + i);
				}
				String priorite = EnumUtils.getPriorite(resultSet.getInt("priorité"));
				float seuilBas = resultSet.getFloat("seuilB");
				float seuilHaut = resultSet.getFloat("seuilH");
				int traitementId = resultSet.getInt("trait") + 1;
				String unite = EnumUtils.getUnite(resultSet.getInt("unites"));
				int virgule = resultSet.getInt("virgule");
				int voieEnregistree = resultSet.getInt("ve");
				int voieStockee = resultSet.getInt("vs");
				
				// Temps fields
				String initialesStation = resultSet.getString("ini") == null ? null : resultSet.getString("ini").trim();

				VoieTelemesuree voie = new VoieTelemesuree(abonnements, active, adresseBES, delta, groupe, info, libelle, numero, ordre, parametres, priorite, seuilBas,
						seuilHaut, traitementId, unite, virgule, voieEnregistree, voieStockee, initialesStation);
				voiesTelemesurees.add(voie);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}

	public static void addVoiesTelemesureesTest(List<Station> stationsTests) throws CloneNotSupportedException{
		List<VoieTelemesuree> voiesTelemesureesTests = new ArrayList<VoieTelemesuree>();
		
		for (Station station : stationsTests) {
			for (VoieTelemesuree voie : voiesTelemesurees) {
				if (voie.getStationId() != station.getTwinStationId()) { continue;}
				
				VoieTelemesuree voieTest = (VoieTelemesuree) voie.clone();
				voieTest.setId(idSequence++);
				voieTest.setStationId(station.getId());
				voieTest.setLibelle(voie.getLibelle()+ "_test");
				
				voiesTelemesureesTests.add(voieTest);
			}
		}

		voiesTelemesurees.addAll(voiesTelemesureesTests);
	}
	
	public static void majWithStations(List<Station> stations) {
		for (VoieTelemesuree voieTelemesuree : voiesTelemesurees) {
			for (Station station : stations) {
				if (voieTelemesuree.getInitialesStation().equalsIgnoreCase(station.getInitiales())) {
					voieTelemesuree.setStationId(station.getId());
					break;
				}
			}
		}
	}
	
	public static void insertVoiesPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO reseau.voies_telemesurees (id, abonnements, actif, adresse_bes, delta, groupe, info, libelle, numero, ordre,";
			query += " parametre1, parametre2, parametre3, parametre4, parametre5, parametre6, parametre7, parametre8, parametre9, parametre10,";
			query += " priorite, seuil_bas, seuil_haut, station_id, traitement_id, unite, virgule, voie_enregistree, voie_stockee) ";
			query += " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";
			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (VoieTelemesuree voieTelemesuree : voiesTelemesurees) {

				insertStmt.setInt(1, voieTelemesuree.getId());
				insertStmt.setLong(2, voieTelemesuree.getAbonnements());
				insertStmt.setBoolean(3, voieTelemesuree.isActive());
				insertStmt.setInt(4, voieTelemesuree.getAdresseBES());
				insertStmt.setFloat(5, voieTelemesuree.getDelta());
				insertStmt.setInt(6, voieTelemesuree.getGroupe());
				insertStmt.setInt(7, voieTelemesuree.getInfo());
				insertStmt.setString(8, voieTelemesuree.getLibelle());
				insertStmt.setInt(9, voieTelemesuree.getNumero());
				insertStmt.setInt(10, voieTelemesuree.getOrdre());
				insertStmt.setInt(11, voieTelemesuree.getParametre1());
				insertStmt.setInt(12, voieTelemesuree.getParametre2());
				insertStmt.setInt(13, voieTelemesuree.getParametre3());
				insertStmt.setInt(14, voieTelemesuree.getParametre4());
				insertStmt.setInt(15, voieTelemesuree.getParametre5());
				insertStmt.setInt(16, voieTelemesuree.getParametre6());
				insertStmt.setInt(17, voieTelemesuree.getParametre7());
				insertStmt.setInt(18, voieTelemesuree.getParametre8());
				insertStmt.setInt(19, voieTelemesuree.getParametre9());
				insertStmt.setInt(20, voieTelemesuree.getParametre10());
				insertStmt.setString(21, voieTelemesuree.getPriorite());
				insertStmt.setFloat(22, voieTelemesuree.getSeuilBas());
				insertStmt.setFloat(23, voieTelemesuree.getSeuilHaut());
				if (voieTelemesuree.getStationId() == 0) {insertStmt.setNull(24, Types.INTEGER);} else {insertStmt.setInt(24, voieTelemesuree.getStationId());}
				if (voieTelemesuree.getTraitementId() == 0) {insertStmt.setNull(25, Types.INTEGER);} else {insertStmt.setInt(25, voieTelemesuree.getTraitementId());}
				insertStmt.setString(26, voieTelemesuree.getUnite());
				insertStmt.setInt(27, voieTelemesuree.getVirgule());
				insertStmt.setInt(28, voieTelemesuree.getVoieEnregistree());
				insertStmt.setInt(29, voieTelemesuree.getVoieStockee());
				insertStmt.addBatch();
			}

			System.out.println("Inserting voies_telemesurees...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.voies_telemesurees_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}


	public VoieTelemesuree(long abonnements, boolean active, int adresseBES, float delta, int groupe, int info, String libelle, int numero, int ordre, int[] parametres,
			String priorite, float seuilBas, float seuilHaut, int traitementId, String unite, int virgule, int voieEnregistree, int voieStockee,   String initialesStation) {
		this.id = idSequence++;
		this.abonnements = abonnements;
		this.active = active;
		this.adresseBES = adresseBES;
		this.delta = delta;
		this.groupe = groupe;
		this.info = info;
		this.libelle = libelle;
		this.numero = numero;
		this.ordre = ordre;
		this.parametre1 = parametres[0];
		this.parametre2 = parametres[1];
		this.parametre3 = parametres[2];
		this.parametre4 = parametres[3];
		this.parametre5 = parametres[4];
		this.parametre6 = parametres[5];
		this.parametre7 = parametres[6];
		this.parametre8 = parametres[7];
		this.parametre9 = parametres[8];
		this.parametre10 = parametres[9];		
		this.priorite = priorite;
		this.seuilBas = seuilBas;
		this.seuilHaut = seuilHaut;
		this.traitementId = traitementId;
		this.unite = unite;
		this.virgule = virgule;
		this.voieEnregistree = voieEnregistree;
		this.voieStockee = voieStockee;
		this.initialesStation = initialesStation;
	}

	public VoieTelemesuree() {
		this.id = idSequence++;
	}
	
	public static List<VoieTelemesuree> getVoiesTelemesurees() {
		return voiesTelemesurees;
	}

	public int getVirgule() {
		return virgule;
	}

	public void setVirgule(int virgule) {
		this.virgule = virgule;
	}

	public String getInitialesStation() {
		return initialesStation;
	}

	public void setInitialesStation(String initialesStation) {
		this.initialesStation = initialesStation;
	}

	public int getStationId() {
		return stationId;
	}

	public void setStationId(int stationId) {
		this.stationId = stationId;
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

	public long getAbonnements() {
		return abonnements;
	}

	public void setAbonnements(long abonnements) {
		this.abonnements = abonnements;
	}

	public float getDelta() {
		return delta;
	}

	public void setDelta(float delta) {
		this.delta = delta;
	}

	public int getGroupe() {
		return groupe;
	}

	public void setGroupe(int groupe) {
		this.groupe = groupe;
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

	public float getSeuilBas() {
		return seuilBas;
	}

	public void setSeuilBas(float seuilBas) {
		this.seuilBas = seuilBas;
	}

	public float getSeuilHaut() {
		return seuilHaut;
	}

	public void setSeuilHaut(float seuilHaut) {
		this.seuilHaut = seuilHaut;
	}

	public String getPriorite() {
		return priorite;
	}

	public void setPriorite(String priorite) {
		this.priorite = priorite;
	}

	public String getUnite() {
		return unite;
	}

	public void setUnite(String unite) {
		this.unite = unite;
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

	public int getVoieEnregistree() {
		return voieEnregistree;
	}

	public void setVoieEnregistree(int voieEnregistree) {
		this.voieEnregistree = voieEnregistree;
	}

	public int getVoieStockee() {
		return voieStockee;
	}

	public void setVoieStockee(int voieStockee) {
		this.voieStockee = voieStockee;
	}

	public boolean isActive() {
		return active;
	}

	public void setActive(boolean active) {
		this.active = active;
	}

	public int getParametre1() {
		return parametre1;
	}

	public void setParametre1(int parametre1) {
		this.parametre1 = parametre1;
	}

	public int getParametre2() {
		return parametre2;
	}

	public void setParametre2(int parametre2) {
		this.parametre2 = parametre2;
	}

	public int getParametre3() {
		return parametre3;
	}

	public void setParametre3(int parametre3) {
		this.parametre3 = parametre3;
	}

	public int getParametre4() {
		return parametre4;
	}

	public void setParametre4(int parametre4) {
		this.parametre4 = parametre4;
	}

	public int getParametre5() {
		return parametre5;
	}

	public void setParametre5(int parametre5) {
		this.parametre5 = parametre5;
	}

	public int getParametre6() {
		return parametre6;
	}

	public void setParametre6(int parametre6) {
		this.parametre6 = parametre6;
	}

	public int getParametre7() {
		return parametre7;
	}

	public void setParametre7(int parametre7) {
		this.parametre7 = parametre7;
	}

	public int getParametre8() {
		return parametre8;
	}

	public int getTraitementId() {
		return traitementId;
	}

	public void setTraitementId(int traitementId) {
		this.traitementId = traitementId;
	}

	public void setParametre8(int parametre8) {
		this.parametre8 = parametre8;
	}

	public int getParametre9() {
		return parametre9;
	}

	public void setParametre9(int parametre9) {
		this.parametre9 = parametre9;
	}

	public int getParametre10() {
		return parametre10;
	}

	public void setParametre10(int parametre10) {
		this.parametre10 = parametre10;
	}

}
