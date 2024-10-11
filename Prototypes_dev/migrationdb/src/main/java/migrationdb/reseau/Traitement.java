package migrationdb.reseau;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

public class Traitement {

	private static int idSequence = 1;
	private static List<Traitement> traitements = new ArrayList<Traitement>();
	
	
	// Postgres fields
	private int id;
	private String nom;
	private String parametre1;
	private String parametre2;
	private String parametre3;
	private String parametre4;
	private String parametre5;
	private String parametre6;
	private String parametre7;
	private String parametre8;
	private String parametre9;
	private String parametre10;
	
	public static void selectTraitementsAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM TraitSpec ORDER BY RéfTraitement ASC";
			System.out.println("Retrieving TraitSpec...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				String nom = resultSet.getString("Nom") == null ? null : resultSet.getString("Nom").trim();
				String[] parametres = new String[10];
				for (int i = 1; i <= 10; i++) {
					parametres[i-1] = resultSet.getString("p" + i) == null ? null : resultSet.getString("p" + i).trim();
				}
				
				Traitement traitement = new Traitement(nom, parametres);
				traitements.add(traitement);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void insertTraitementsPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO reseau.traitements (id, nom, parametre1, parametre2, parametre3, parametre4, parametre5, parametre6, parametre7, parametre8, parametre9, parametre10)";
			query += " VALUES (?,?,?,?,?,?,?,?,?,?,?,?)";
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Traitement traitement : traitements) {
				
				insertStmt.setInt(1, traitement.getId());
				insertStmt.setString(2, traitement.getNom());
				insertStmt.setString(3, traitement.getParametre1());
				insertStmt.setString(4, traitement.getParametre2());
				insertStmt.setString(5, traitement.getParametre3());
				insertStmt.setString(6, traitement.getParametre4());
				insertStmt.setString(7, traitement.getParametre5());
				insertStmt.setString(8, traitement.getParametre6());
				insertStmt.setString(9, traitement.getParametre7());
				insertStmt.setString(10, traitement.getParametre8());
				insertStmt.setString(11, traitement.getParametre9());
				insertStmt.setString(12, traitement.getParametre10());
				insertStmt.addBatch();
			}

			System.out.println("Inserting traitements...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.traitements_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public Traitement() {
		this.id = idSequence++;
	}

	public Traitement(String nom, String[] parametres) {
		this.id = idSequence++;
		this.nom = nom;
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
	}
	
	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public static List<Traitement> getTraitements() {
		return traitements;
	}

	public String getParametre1() {
		return parametre1;
	}

	public void setParametre1(String parametre1) {
		this.parametre1 = parametre1;
	}

	public String getParametre2() {
		return parametre2;
	}

	public void setParametre2(String parametre2) {
		this.parametre2 = parametre2;
	}

	public String getParametre3() {
		return parametre3;
	}

	public void setParametre3(String parametre3) {
		this.parametre3 = parametre3;
	}

	public String getParametre4() {
		return parametre4;
	}

	public void setParametre4(String parametre4) {
		this.parametre4 = parametre4;
	}

	public String getParametre5() {
		return parametre5;
	}

	public void setParametre5(String parametre5) {
		this.parametre5 = parametre5;
	}

	public String getParametre6() {
		return parametre6;
	}

	public void setParametre6(String parametre6) {
		this.parametre6 = parametre6;
	}

	public String getParametre7() {
		return parametre7;
	}

	public void setParametre7(String parametre7) {
		this.parametre7 = parametre7;
	}

	public String getParametre8() {
		return parametre8;
	}

	public void setParametre8(String parametre8) {
		this.parametre8 = parametre8;
	}

	public String getParametre9() {
		return parametre9;
	}

	public void setParametre9(String parametre9) {
		this.parametre9 = parametre9;
	}

	public String getParametre10() {
		return parametre10;
	}

	public void setParametre10(String parametre10) {
		this.parametre10 = parametre10;
	}

	public String getNom() {
		return nom;
	}

	public void setNom(String nom) {
		this.nom = nom;
	}
	
}
