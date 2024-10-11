package migrationdb.utils;

import java.sql.Date;
import java.sql.Time;
import java.sql.Timestamp;

public final class ConversionUtils {

	private ConversionUtils() {
	}

	@SuppressWarnings("deprecation")
	public static Timestamp getTimestamp(Date date, Time time) {
		Timestamp timestamp = null;

		if (date != null && time != null) {
			timestamp = new Timestamp(
					date.getYear(),
					date.getMonth(), 
					date.getDate(), 
					time.getHours(),
					time.getMinutes(), 
					time.getSeconds(), 
					0);
			int offset = Math.abs(timestamp.getTimezoneOffset()/60);
			timestamp.setHours(timestamp.getHours() - offset); // Local to UTC
		}
		return timestamp;
	}
	
	@SuppressWarnings("deprecation")
	public static Timestamp getTimeStampUTC(Date date, Time time) {
		Timestamp timestamp = null;

		if (date != null && time != null) {
			timestamp = new Timestamp(
					date.getYear(), 
					date.getMonth(), 
					date.getDate(), 
					time.getHours(),
					time.getMinutes(), 
					time.getSeconds(), 
					0); // Already UTC
		}
		return timestamp;
	}
	
	public static boolean stringToBoolean(String value){
		if (value.equalsIgnoreCase("oui")) {
			return true;
		} else {
			return false;
		}
	}
	
	public static long getAbonnements(int oldAbonnements) {
		// ---- Old value ----
		// Groupe -> bit index
		// ADMIN 		-> 0
		// Hydrologie 	-> 1
		// Télétrans 	-> 2
		// Qualité 		-> 3
		// Pluviométrie -> 4
		// Baghera 		-> 5
		// VEM 			-> 6
		
		// ---- New value ----
		// ADMIN (0) = ADMIN 
		// OPERATEUR (1) = Télétrans 
		// VALIDEUR (2) = Hydrologie + VEM 
		// CONSULTATION (3) = Hydrologie + VEM 
		
		boolean hydroOrVEM = false;
		long newAbonnements = 0;
		for (int i = 31; i >= 0; i--) {
			if (oldAbonnements >= Math.pow(2, i)) {
				oldAbonnements -= Math.pow(2, i);
				switch (i) {
				case 0:  // ADMIN
					newAbonnements += 1;  // ADMIN -> 2^0
					break;
				case 1: // Hydrologie
					if (!hydroOrVEM) {
						newAbonnements += 4; // VALIDEUR -> 2^2
						newAbonnements += 8; // CONSULTATION -> 2^3
						hydroOrVEM = true;
					}
					break;
				case 2: // Télétrans
					newAbonnements += 2;  // OPERATEUR -> 2^1
					break;
				case 3: // Qualité
					break;
				case 4: // Pluviométrie
					break;
				case 5: // Baghera
					break;
				case 6: // VEM
					if (!hydroOrVEM) {
						newAbonnements += 4; // VALIDEUR -> 2^2
						newAbonnements += 8; // CONSULTATION -> 2^3
						hydroOrVEM = true;
					}
					break;
				}
			}
		}
		return newAbonnements;
	}
	
	public static long getPreselections(int oldPreselections)
	{
		// ---- Old value ----
		// Preselection -> bit index
		// Pluvio 		-> 0
		// SSRMN	 	-> 1
		// ITEN LCO 	-> 2
		// ITEN TR 		-> 3
		// ITEN HB		-> 4
		// ITEN CBE		-> 5
		// Vidage IP	-> 6
				
		// ---- New value ----
		// PLUVIOMETRIE (0) = Pluvio 
		// SSRMN (1) = SSRMN 
				
		long newPreselections = 0;
		for (int i = 31; i >= 0; i--) {
			if (oldPreselections >= Math.pow(2, i)) {
				oldPreselections -= Math.pow(2, i);
				switch (i) {
				case 0: // Pluvio
					newPreselections += 1; // PLUVIOMETRIE -> 2^0
					break;
				case 1: // SSRMN
					newPreselections += 2; // SSRMN -> 2^1
					break;
				}
			}
		}
		return newPreselections;	
	}
}
