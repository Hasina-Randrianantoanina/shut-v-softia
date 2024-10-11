package migrationdb.administration;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

public class Profil {
	
	private static int idSequence = 1;
	private static List<Profil> profils = createProfils();

	// Postgres fields
	private int id;
	private int code;
	private String profil;
	
	private static List<Profil> createProfils()
	{
		List<Profil> profils = new ArrayList<Profil>();
		
		profils.add(new Profil(0, "ADMIN"));
		profils.add(new Profil(1, "OPERATEUR"));
		profils.add(new Profil(2, "VALIDEUR"));
		profils.add(new Profil(3, "CONSULTATION"));
		
		return profils;
	}
	
	public static void insertProfilsPG(Connection connexion)
	{
		try {
			// Insert into table
			String query = "INSERT INTO administration.profils (id, code, profil) VALUES (?,?,?)";
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Profil profil: profils) {
				
				insertStmt.setInt(1, profil.getId());
				insertStmt.setInt(2, profil.getCode());
				insertStmt.setString(3, profil.getProfil());
				insertStmt.addBatch();
			}

			System.out.println("Inserting profils...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE administration.profils_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	} 
	
	public Profil(int code, String profil) {
		this.id = idSequence++;
		this.code = code;
		this.profil = profil;
	}

	public Profil() {
		this.id = idSequence++;
	}
	
	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getCode() {
		return code;
	}

	public void setCode(int code) {
		this.code = code;
	}

	public String getProfil() {
		return profil;
	}

	public void setProfil(String profil) {
		this.profil = profil;
	}

	public static List<Profil> getProfils() {
		return profils;
	}
	
}
