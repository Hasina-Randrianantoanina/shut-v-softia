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
import migrationdb.utils.EnumUtils;

public class EvenementPluvieux {

	private static int idSequence = 1;
	private static List<EvenementPluvieux> evenementsPluvieux = new ArrayList<EvenementPluvieux>();

	// Postgres fields
	private int id;
	private String bassinVersant;
	private Timestamp debut;
	private Timestamp fin;

	public static void selectEvenementsPluvieuxAccess(Connection connexion) {
		try {
			Statement stmt = connexion.createStatement();
			String query = "SELECT * FROM EvtsPluvieux ORDER BY Début";
			System.out.println("Retrieving EvtsPluvieux...");
			ResultSet resultSet = stmt.executeQuery(query);

			while (resultSet.next()) {
				// Postgres fields
				String bassinVersant = resultSet.getString("NomVoie") == null ? null : EnumUtils.getBassinVersant(resultSet.getString("NomVoie").trim());
				Timestamp debut = ConversionUtils.getTimestamp(resultSet.getDate("Début"), resultSet.getTime("Début"));
				Timestamp fin = ConversionUtils.getTimestamp(resultSet.getDate("Fin"), resultSet.getTime("Fin"));

				EvenementPluvieux evenement = new EvenementPluvieux(bassinVersant, debut, fin);
				evenementsPluvieux.add(evenement);
			}

			resultSet.close();
			stmt.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}

	public static void insertEvenementsPluvieuxPG(Connection connexion) {
		try {
			// Insert into table
			String query = "INSERT INTO reseau.evenements_pluvieux (id, bassin_versant, debut, fin) VALUES (?,?,?,?);";
			PreparedStatement insertStmt = connexion.prepareStatement(query);

			for (EvenementPluvieux evenement : evenementsPluvieux) {

				insertStmt.setInt(1, evenement.getId());
				insertStmt.setString(2, evenement.getBassinVersant());
				insertStmt.setTimestamp(3, evenement.getDebut());
				insertStmt.setTimestamp(4, evenement.getFin());
				insertStmt.addBatch();
			}

			System.out.println("Inserting evenements_pluvieux...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);

			// Update id sequence
			String seqQuery = "ALTER SEQUENCE reseau.evenements_pluvieux_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public EvenementPluvieux() {
		this.id = idSequence++;
	}

	public EvenementPluvieux(String bassinVersant, Timestamp debut, Timestamp fin) {
		this.id = idSequence++;
		this.bassinVersant = bassinVersant;
		this.debut = debut;
		this.fin = fin;
	}

	public static List<EvenementPluvieux> getEvenementsPluvieux() {
		return evenementsPluvieux;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getBassinVersant() {
		return bassinVersant;
	}

	public void setBassinVersant(String bassinVersant) {
		this.bassinVersant = bassinVersant;
	}

	public Timestamp getDebut() {
		return debut;
	}

	public void setDebut(Timestamp debut) {
		this.debut = debut;
	}

	public Timestamp getFin() {
		return fin;
	}

	public void setFin(Timestamp fin) {
		this.fin = fin;
	}
}
