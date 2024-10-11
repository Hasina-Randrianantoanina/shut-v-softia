package migrationdb.administration;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

public class Utilisateur {
	
	private static int idSequence = 1;
	private static List<Utilisateur> utilisateurs = createUtilisateurs();

	// Postgres fields
	private int id;
	private boolean actif;
	private String email;
	private String nom;
	private int profilId;

	private static List<Utilisateur> createUtilisateurs() {
		List<Utilisateur> utilisateurs = new ArrayList<Utilisateur>();

		utilisateurs.add(new Utilisateur(true, "jleroy@seinesaintdenis.fr", "leroy", 1));
		utilisateurs.add(new Utilisateur(true, "fvandelannoote@seinesaintdenis.fr", "vandelannoote", 3));
		utilisateurs.add(new Utilisateur(true, "sbarone@seinesaintdenis.fr", "barone", 3));
		utilisateurs.add(new Utilisateur(true, "abovero@seinesaintdenis.fr", "bovero", 3));
		utilisateurs.add(new Utilisateur(true, "fdesetables@seinesaintdenis.fr", "desetables", 2));
		utilisateurs.add(new Utilisateur(true, "jcantarinha@seinesaintdenis.fr", "cantarinha", 3));
		utilisateurs.add(new Utilisateur(true, "lbensalem@seinesaintdenis.fr", "bensalem", 1));
		utilisateurs.add(new Utilisateur(false, "flecardonnel@seinesaintdenis.fr", "lecardonnel", 3));
		utilisateurs.add(new Utilisateur(true, "fchaumeau@seinesaintdenis.fr", "chaumeau", 3));
		utilisateurs.add(new Utilisateur(true, "vlanier@seinesaintdenis.fr", "lanier", 3));
		utilisateurs.add(new Utilisateur(true, "goudin@seinesaintdenis.fr", "oudin", 2));
		utilisateurs.add(new Utilisateur(true, "hbenzouaoui@seinesaintdenis.fr", "benzouaoui", 0));
		utilisateurs.add(new Utilisateur(true, "lcelestine@seinesaintdenis.fr", "celestine", 3));
		utilisateurs.add(new Utilisateur(true, "ypresson@seinesaintdenis.fr", "presson", 2));
		utilisateurs.add(new Utilisateur(true, "jmisset@seinesaintdenis.fr", "misset", 3));
		utilisateurs.add(new Utilisateur(true, "jembry@seinesaintdenis.fr", "embry", 3));
		utilisateurs.add(new Utilisateur(true, "sreichardt@seinesaintdenis.fr", "reichardt", 3));
		utilisateurs.add(new Utilisateur(false, "eazaiez@seinesaintdenis.fr", "azaiez", 3));
		utilisateurs.add(new Utilisateur(true, "foirdizahir@seinesaintdenis.fr", "oirdizahir", 3));
		utilisateurs.add(new Utilisateur(true, "iisrael@seinesaintdenis.fr", "israel", 1));
		utilisateurs.add(new Utilisateur(true, "ykaloussi@seinesaintdenis.fr", "kaloussi", 3));
		utilisateurs.add(new Utilisateur(true, "jlesueur@softia.fr", "softia", 0));
		utilisateurs.add(new Utilisateur(true, "jcmilon@seinesaintdenis.fr", "milon", 0));
		utilisateurs.add(new Utilisateur(true, "lcopin@seinesaintdenis.fr", "copin", 1));
		utilisateurs.add(new Utilisateur(true, "aamrous@seinesaintdenis.fr", "amerous", 3));
		utilisateurs.add(new Utilisateur(true, "alkaci@seinesaintdenis.fr", "kaci", 3));
		utilisateurs.add(new Utilisateur(true, "nalfima@seinesaintdenis.fr", "alfima", 3));
		utilisateurs.add(new Utilisateur(true, "clmercier@seinesaintdenis.fr", "mercier", 2));
		utilisateurs.add(new Utilisateur(true, "klouber@seinesaintdenis.fr", "louber", 3));
		
		return utilisateurs;
	}

	public static void insertUtilisateursPG(Connection connexion)
	{
		try {
			// Insert into table
			String query = "INSERT INTO administration.utilisateurs (id, actif, email, nom, profil_id) VALUES (?,?,?,?,?)";
			PreparedStatement insertStmt = connexion.prepareStatement(query);
			
			for (Utilisateur utilisateur : utilisateurs) {
				
				insertStmt.setInt(1, utilisateur.getId());
				insertStmt.setBoolean(2, utilisateur.isActif());
				insertStmt.setString(3, utilisateur.getEmail());
				insertStmt.setString(4, utilisateur.getNom());
				insertStmt.setInt(5, utilisateur.getProfilId() +1);
				insertStmt.addBatch();
			}

			System.out.println("Inserting utilisateurs...");
			int[] affectedRows = insertStmt.executeBatch();
			insertStmt.close();
			System.out.println("\tdone - Affected rows : " + affectedRows.length + ", next id : " + idSequence);
			
			// Update id sequence
			String seqQuery = "ALTER SEQUENCE administration.utilisateurs_id_seq RESTART WITH " + idSequence + ";";
			Statement seqStmt = connexion.createStatement();
			seqStmt.execute(seqQuery);
			seqStmt.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public Utilisateur() {
		this.id=idSequence++;
	}

	public Utilisateur(boolean actif, String email, String nom, int profilId) {
		this.id=idSequence++;
		this.actif = actif;
		this.email = email;
		this.nom = nom;
		this.profilId = profilId;
	}
	
	public static List<Utilisateur> getUtilisateurs() {
		return utilisateurs;
	}

	public boolean isActif() {
		return actif;
	}

	public void setActif(boolean actif) {
		this.actif = actif;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public String getNom() {
		return nom;
	}

	public void setNom(String nom) {
		this.nom = nom;
	}

	public int getProfilId() {
		return profilId;
	}

	public void setProfilId(int profilId) {
		this.profilId = profilId;
	}

}
