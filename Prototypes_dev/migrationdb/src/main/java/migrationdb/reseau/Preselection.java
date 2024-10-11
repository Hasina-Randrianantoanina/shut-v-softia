package migrationdb.reseau;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

public class Preselection {
	
	private static int idSequence = 1;
	private static List<Preselection> preselections = createPreselections();

	// Postgres fields
	private int id;
	private int code;
	private String preselection;

	private static List<Preselection> createPreselections()
	{
		List<Preselection> preselections = new ArrayList<Preselection>();
		
		preselections.add(new Preselection(0, "PLUVIOMETRIE"));
		preselections.add(new Preselection(1, "SSRMN"));
		
		return preselections;
	}
	
	public static void insertPreselectionsPG(Connection connexion)
	{
		try {
			// Insert into table
			String query = "INSERT INTO reseau.preselections (id, code, preselection) VALUES (?,?,?)";
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Preselection preseletion: preselections) {
				
				insertStmt.setInt(1, preseletion.getId());
				insertStmt.setInt(2, preseletion.getCode());
				insertStmt.setString(3, preseletion.getPreselection());
				insertStmt.addBatch();
			}

			System.out.println("Inserting preselections...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.preselections_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	} 
	
	public Preselection(int code, String preselection) {
		this.id = idSequence++;
		this.code = code;
		this.preselection = preselection;
	}

	public Preselection() {
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

	public String getPreselection() {
		return preselection;
	}

	public void setPreselection(String preselection) {
		this.preselection = preselection;
	}

	public static List<Preselection> getPreselections() {
		return preselections;
	}

}
