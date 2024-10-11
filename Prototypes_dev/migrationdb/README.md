# Migration des db access vers postgres

## Préparer la db postgres
1. Créer une database dans pgadmin ('shutweb' par défaut)
2. Restaurer la structure de la base avec le fichier `**\shut-refonte\Prototypes_dev\migrationdb\backup_ddMMyyyy`

## Préparer l'environnement Java
1. Créer un compte oracle `https://signon.oracle.com/signin`
2. Télécharger et unzipper la jdk-11.0.18 au lien suivant `https://www.oracle.com/webapps/redirect/signon?nexturl=https://download.oracle.com/otn/java/jdk/11.0.23%2B7/9bd8d305c900ee4fa3e613b59e6f42de/jdk-11.0.23_windows-x64_bin.zip`
3. Télécharger et unzipper maven au lien suivant `https://dlcdn.apache.org/maven/maven-3/3.8.8/binaries/apache-maven-3.8.8-bin.zip`
4. Ajouter à la variable d'environnement PATH (système) la jdk `**\Java\jdk-11\bin`
5. Ajouter à la variable d'environnement PATH maven `(**\apache-maven-3.8.8\bin)`

## Modifier le projet java
1. Ouvrir le fichier `**\shut-refonte\Prototypes_dev\migrationdb\src\main\java\migrationdb\Main.java` et modifier les valeurs des constantes des premières lignes pour correspondre à votre environnement
```
// Access
final String PATH_TO_DB_ACCESS = "C:\\Users\\yann\\Desktop\\DataSHUTProd";
//Postgres
final String DB_NAME_POSTGRES = "shutweb_prod";
final int PORT_POSTGRES = 5432;
final String USER_POSTGRES = "postgres";
final String PASSWORD_POSTGRES = "rrrrr";
// Debug mode
Station.setDebugMode(true); // Adding fields : debug_dernier_appel, debug_dernier_transfert
Alerte.setDebugMode(true); // Adding field : debug_initiales_station
```

## Build & run
1. Lancer un terminal dans `**\shut-refonte\Prototypes_dev\migrationdb`
2. Exécuter la commande `mvn clean install`
3. Ouvrir et lancer le script `**\shut-refonte\Prototypes_dev\migrationdb\truncatePG.sql`
3. Exécuter la commande `mvn exec:java`
4. Attendre que la console affiche `---------------- END ----------------`
