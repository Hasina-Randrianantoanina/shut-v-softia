package migrationdb.utils;

import java.sql.Timestamp;
import java.util.Set;

import migrationdb.reseau.Reseau;

public final class EnumUtils {

	private EnumUtils() {
	}

	// ---------------- Appel ----------------
	public static String getStatutAppel(int code) {
		switch (code) {
		case 0:
			return "TERMINE";
		case 1:
			return "EN_ATTENTE";
		case 2:
			return "EN_COURS";
		case 3:
			return "SPECIAL";
		case 4:
			return "DIFFERE";
		case 5:
			return "ARRET";
		default:
			return null;
		}
	}

	public static String getDefautAppel(int code) {
		switch (code) {
		default:
			return null;
		}
	}

	// --------------- Defaut ----------------
	public static String getTypeDefaut(int code) {
		switch (code) {
		case 10:
			return "ETAT";
		case 20:
			return "CAPTEUR";
		default:
			return null;
		}
	}

	// ------------- DefautActif -------------
	public static String getTypeDefautActif(int code) {
		switch (code) {
		case 27:
			return "ETAT";
		case 26:
			return "CAPTEUR";
		case 2011:
			return "PING";
		default:
			return null;
		}
	}	
	
	// ---------- EvenementPluvieux ----------
	public static String getBassinVersant(String nomVoie) {
		int underscoreIndex = nomVoie.indexOf('_');
		return nomVoie.substring(underscoreIndex + 1);
	}

	// ---------------- Perte ----------------
	public static String getCausePerte(String cause) {
		switch (cause.toLowerCase()) {
		case "?":
			return "?";
		case "arrêt enregistreur":
			return "Arrêt enregistreur";
		case "battement capteur":
			return "Battement capteur";
		case "battement jbus":
			return "Battement Jbus";
		case "changement heure été hiver":
			return "Changement heure été hiver";
		case "défaut paramétrage":
			return "Défaut paramétrage";
		case "incident autoshut":
			return "Incident AutoShut";
		case "maintenance station":
			return "Maintenance station";
		case "mise à jour distante du logiciel":
			return "Mise à jour distante du logiciel";
		case "mise à jour du matériel":
			return "Mise à jour du matériel";
		case "mise à jour locale du logiciel":
			return "Mise à jour locale du logiciel";
		case "non déterminée":
			return "Non déterminée";
		case "panne alimentation":
			return "Panne alimentation";
		case "panne automate":
			return "Panne automate";
		case "panne capteur pluviométrique":
			return "Panne capteur pluviométrique";
		case "panne ligne adsl":
			return "Panne ligne ADSL";
		case "panne ligne rc":
			return "Panne ligne RC";
		case "panne logiciel enregistreur fonctionnement partiel":
			return "Panne logiciel enregistreur fonctionnement partiel";
		case "panne matériel enregistreur":
			return "Panne matériel enregistreur";
		case "panne routeur":
			return "Panne routeur";
		case "panne voie d'acquisition":
			return "Panne voie d'acquisition";
		case "plantage enregistreur":
			return "Plantage enregistreur";
		case "préventif":
			return "Préventif";
		case "problème transmission":
			return "Problème transmission";
		case "tâche 13 (enregistrement) bloquée":
			return "Tâche 13 (enregistrement) bloquée";
		case "tâche 16 (acquisition) bloquée":
			return "Tâche 16 (acquisition) bloquée";
		case "tâche 17 (dialogue visu, shut) bloquée":
			return "Tâche 17 (dialogue Visu, shut) bloquée";
		case "tâche 40 (ftp, vidage) bloquée":
			return "Tâche 40 (FTP, vidage) bloquée";
		case "tâche 41 (dialogue visu en tcp) bloquée":
			return "Tâche 41 (dialogue Visu en TCP) bloquée";
		case "tâche autre bloquée":
			return "Tâche autre bloquée";
		case "tâches 42, 43 (dialogue modbustcp) bloquée":
			return "Tâches 42, 43 (dialogue ModbusTCP) bloquée";
		case "travaux":
			return "Travaux";
		// Old case with near identical current value
		case "mise à jour locale":
			return "Mise à jour locale du logiciel";
		case "mise à jour distante":
			return "Mise à jour distante du logiciel";
		// Old case unknown
		case "mise à jour logiciel": 
			// Pertes n°310, 344, 348, 366, 517, 692, 693, 695, 698, 743, 748, 749, 1779, 1980, 4711, 4759, 4783, 4790, 4909, 5070, 5077, 5080,
			// 5251, 5254, 5855, 9939, 11106, 11202, 12309, 14583, 14586, 15065, 15070, 15100, 15101, 15686, 15779, 15780, 15781, 15891, 16009, 
			// 16010, 16738, 18660, 18664, 18665, 18666, 18667, 18668, 20302, 20303, 20304, 20328, 20329, 20330, 20331, 20332, 20336, 20337,
			// 20338, 20339, 20340, 20341, 20343, 20344, 20345, 20346, 20351, 20352, 20353, 20354, 20368, 20369, 20370, 20371, 20372, 20373, 
			// 20374, 20377, 20398, 20399, 20405, 20449, 20450, 20451, 20460, 20593, 21198, 21881, 21908, 21909, 21949, 21952, 21953, 21963,
			// 21964, 21983, 21984, 22006, 22023, 22055, 22114, 22119, 22120, 22121, 22122, 22217, 22218, 22219, 22437, 22438, 22471, 22487, 
			// 22488, 24355, 24397, 25632, 26639, 26640, 27441, 27490, 28603, 28608, 29179, 29232, 30178, 30184, 30904, 31351, 31362, 31912, 
			// 32694, 39074, 42429, 42445, 44850, 45042, 49158, 49399, 50313, 153968, 206959, 231301, 232818, 237605
			return "Mise à jour logiciel";	
		case "panne aquaval": // Pertes n°30089, 44091, 62718
			return "Panne Aquaval";
		case "panne modules e/s": // Perte n°24677
			return "Panne modules E/S";
		case "tâche 2 (démarrage + init) bloquée": // Pertes n°74731, 155348, 162093, 317596
			return "Tâche 2 (démarrage + init) bloquée";
		case "tâche 3 (dial trans, stc) bloquée": // Perte n°334755
			return "Tâche 3 (dial Trans, STC) bloquée";
		case "tâche 5 (dial visu, shut)) bloquée": // Pertes n°153245, 189750, 189822, 198930, 213333, 245268, 335320, 349853
			return "Tâche 5 (dial Visu, shut)) bloquée";
		case "trou les 31 et 01 aout alors que dtrans tjrs 7h58": // Perte n°26749
			return "trou les 31 et 01 aout alors que dtrans tjrs 7h58";
		default:
			return "ERROR_" + cause;
		}
	}
 
	public static String getDefautPerte(String defaut) {
		switch (defaut.toLowerCase()) {
		case "?":
			return "?";
		case "alerte remplissage mémoire":
			return "Alerte remplissage mémoire";
		case "arrêt acquisition":
			return "Arrêt acquisition";
		case "arrêt enregistrement":
			return "Arrêt enregistrement";
		case "arrêt enregistreur":
			return "Arrêt enregistreur";
		case "arrêt transfert shut":
			return "Arrêt Transfert SHUT";
		case "défaut acquisition jbus":
			return "Défaut acquisition Jbus";
		case "défaut acquisition physique":
			return "Défaut acquisition physique";
		case "défaut capteur":
			return "Défaut capteur";
		case "défaut comptage pluviométrique":
			return "Défaut comptage pluviométrique";
		case "défaut date initialisation":
			return "Défaut date initialisation";
		case "défaut dialogue adsl":
			return "Défaut dialogue ADSL";
		case "défaut dialogue série":
			return "Défaut dialogue série";
		case "dérive horloge":
			return "Dérive horloge";
		case "dysf configuration visushut":
			return "Dysf configuration Visushut";
		case "dysfonctionnement enregistreur":
			return "Dysfonctionnement enregistreur";
		case "dysfonctionnement téléchargement programme":
			return "Dysfonctionnement téléchargement programme";
		case "ftp hs et taches ok":
			return "FTP HS et taches ok";
		case "ne répond pas au ping":
			return "Ne répond pas au ping";
		case "paramétrage shut":
			return "Paramétrage Shut";
		case "plus d'enregistrements à 15 s":
			return "Plus d'enregistrements à 15 s";
		case "reset":
			return "Reset";
		// Old case unknown	
		case "défaut dialogue":
			// Pertes n°22079, 22080, 25519, 27748, 27477, 25632, 44188, 45042, 58765, 83470, 83752, 84197, 55172, 44186, 44166, 86693, 81472, 97211,
			// 98298, 103489, 104321, 46479, 108338, 44192, 109846, 111040, 85848, 112813, 114457, 114941, 115600, 116562, 138585, 151957, 153571,
			// 154434, 154662, 155222, 155419, 162725, 162740, 163327, 167840, 167917, 168223, 168451, 168998, 169915, 180909, 180976, 180992, 181154,
			// 188689, 189751, 189822, 195767, 198930, 198933, 198934, 200408, 206912, 203159, 203160, 212925, 213187, 222501, 206916, 227603, 207696,
			// 232764, 223257, 224967, 323665, 349853
			return "Défaut dialogue";
		default:
			return "ERROR_" + defaut;
		}
	}

	public static String getRemedePerte(String remede) {
		switch (remede.toLowerCase()) {
		case "?":
			return "?";
		case "arrêt / marche logiciel (@bloquer, @go)":
			return "Arrêt / marche logiciel";
		case "arrêt / marche matériel (interrupteur)":
			return "Arrêt / marche matériel (interrupteur)";
		case "echange standard enregistreur":
			return "Echange standard enregistreur";
		case "effacer mémoire d'enregistrement":
			return "Effacer mémoire d'enregistrement";
		case "fin intervention iten sur enregistreur":
			return "Fin intervention ITEN sur enregistreur";
		case "fin maintenance sur station":
			return "Fin maintenance sur station";
		case "fin mise à jour du logiciel":
			return "Fin mise à jour du logiciel";
		case "fin travaux":
			return "Fin travaux";
		case "initialiser l'enregistreur (@init)":
			return "Initialiser l'enregistreur (@init)";
		case "mise à jour paramétrage":
			return "Mise à jour paramétrage";
		case "mise à l’heure enregistreur":
			return "Mise à l’heure enregistreur";
		case "plus de défaut":
			return "Plus de défaut";
		case "rechargement config (visushut)":
			return "Rechargement config (VisuShut)";
		case "rechargement distant programme enregistreur":
			return "Rechargement distant programme enregistreur";
		case "rechargement local programme enregistreur":
			return "Rechargement local programme enregistreur";
		case "relance enregistreur (@go)":
			return "Relance enregistreur (@go)";
		case "retour alimentation":
			return "Retour alimentation";
		// Old case with near identical current value
		case "disparition du défaut":
			return "Plus de défaut";
		case "etour alimentation":
			return "Retour alimentation";
		case "initialisation enregistreur":
			return "Initialiser l'enregistreur (@init)";
		case "rechargement local programme":
			return "Rechargement local programme enregistreur";
		case "relance enregistreur":
			return "Relance enregistreur (@go)";
		// Old case unknown	
		case "echange poste 24 volts": // Perte n°28884
			return "Echange poste 24 volts";
		case "mise à jour locale du logiciel": // Perte n°56327
			return "Mise à jour locale du logiciel";
		case "rechargement configuration enregistreur": // Perte n°26446
			return "Rechargement configuration enregistreur";
		case "relance automate": // Pertes n°27640, 197711
			return "Relance automate";
		case "relance module e/s": // Perte n°24677
			return "Relance module E/S";
		case "téléchargement ksp distant": // Pertes n°107910, 228873, 232764, 223257, 224967, 349853
			return "Téléchargement KSP distant";
		case "téléchargement ksp local": // Perte n°232432
			return "Téléchargement KSP local";
		default:
			return "ERROR_" + remede;
		}
	}
	
	public static String getTypePerte(int code) {
		switch (code) {
		case 1:
			return "PB_DATE_GO";
		case 2:
			return "PB_DATE_INIT";
		case 3:
			return "PB_DATE_ACQ";
		case 4:
			return "PB_DATE_ENREGISTREMENT";
		case 5:
			return "PB_ECART_HORLOGE";
		case 7:
			return "PB_DATE_GO_ET_DATE_INIT";
		default:
			return null;
		}
	}
	
	// --------------- Station ---------------
	public static String getTypeHeure(Timestamp dernierTransfert, String initiales, String versionEnregistreur, Reseau reseau) {
		Set<String> m580InUTC = Set.of("ba", "bbp", "bd", "bgp", "bm", "ch", "clp", "cv", "drp", "du", "fb", "fh", "ga",
				"gm", "gp", "gyp", "he", "je", "jm", "kk", "lgp", "lr", "lvp", "mo", "ms", "mtp", "ncp", "nep", "nmp",
				"npp", "oup", "pm", "pnp", "po", "ps", "qm", "rl", "ru", "rv", "sl", "tfp", "th", "vip", "vs");
		Set<String> m580InLocale = Set.of("nu", "xy");

		// NB : BRp and GR new Automate, typeHeure not know for now

		String typeHeure = "";

		if (reseau == Reseau.OBSERVATION) {
			typeHeure = "UNIVERSELLE";
		} else if (versionEnregistreur != null && !versionEnregistreur.isEmpty()) {
			// RU
			if (versionEnregistreur.equalsIgnoreCase("D16A56Ca01")) {
				// M580
				if (m580InUTC.contains(initiales.toLowerCase())) {
					typeHeure = "UNIVERSELLE";
				} else if (m580InLocale.contains(initiales.toLowerCase())) {
					typeHeure = "LOCALE";
				} else {
					typeHeure = "INCONNU";
					System.out.println("--- TypeHeure unknown (station = " + initiales + ", version = M580) in EnumUtils.getTypeHeure()");
				}
			} else {
				// STEN and Premium
				typeHeure = "LOCALE";
			}
		} else {
			System.out.println("--- TypeHeure unknown (station = " + initiales + ", version = null / empty) in EnumUtils.getTypeHeure()");
			typeHeure = "INCONNU";
		}

		return typeHeure;
	}
	
	// ------------ VoieTelemesuree-----------
	public static String getPriorite(int code) {
		switch (code) {
		case 0:
			return "-";
		case 1:
			return "A";
		case 2:
			return "B";
		case 3:
			return "C";
		case 4:
			return "B/A";
		case 5:
			return "C/A";
		default:
			return null;
		}
	}

	public static String getUnite(int code) {
		switch (code) {
		case 0:
			return "-";
		case 1:
			return "m";
		case 2:
			return "cm";
		case 3:
			return "m/s";
		case 4:
			return "m3/s";
		case 5:
			return "FAU";
		case 6:
			return "mm";
		case 7:
			return "m3";
		case 8:
			return "%";
		case 9:
			return "db";
		default:
			return null;
		}
	}

	// --------------- VoieTOR ---------------
	public static String getType(int code) {
		switch (code) {
		case 1:
			return "TS";
		case 2:
			return "TC";
		case 3:
			return "ETAT";
		default:
			return null;
		}
	}
	
	// ------------- Utilisateur -------------
	public static String getNomUtilisateur(String nom) {
		switch (nom.toLowerCase()) {
		// Current users
		case "hb":
			return "benzouaoui";
		case "hbenzouaoui":
			return "benzouaoui";
		case "cbensalem":
			return "bensalem";
		case "jleroy":
			return "leroy";
		case "jcmilon":
			return "milon";
		case "lcopin":
			return "copin";
		// Old users
		case "tr":
			return "rollando";
		case "trollando":
			return "rollando";
		default:
			return "ERROR_" + nom;
		}
	}
	
}
