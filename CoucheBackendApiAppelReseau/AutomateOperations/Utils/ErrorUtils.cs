namespace AutomateOperations.Utils
{
    // 1 - 99 => FILESYSTEM
        // 1 =>
    // 100 - 199 => PING
        // 100 => mauvais format adresse ip
        // 110 => échec ping serveur
        // 111 => exception globale ping serveur
        // 120 => échec ping routeur
        // 121 => exception globale ping routeur
    // 200 - 299 => TCP
        // 200 => exception création serveur TCP
        // 201 => exception lecture du flux TCP
        // 202 => exception écriture du flux TCP
    // 300 - 399 => FTP
        // ---- Telechargement ----
        // 300 => host, login, mdp non définis
        // 301 => échec de la connexion FTP
        // 302 => dossier distant introuvable
        // 303 => exception globale LECT DONNEES PAR FTP
    // ---- Televersement ----
        // 400 - 499 => LECTURE_STATUS_VERSION_CONFIGs
        // ---- Status donnees ----
        // 400 => exception globale LECT DISPONIBILITE DONNEES
        // ---- Version enregistreur ----
        // 410 => exception globale LECT VERSION ENREGISTREUR
        // 411 => version non prise en charge
        // 412 => type d'herue inconnu
        // ---- Config voies telemesurees ----
        // 420 => exception globale LECT CONFIG VOIES ANALOGIQUES
        // ---- Config voies tor ----
        // 430 => ?
        // ---- Discordance config ----
        // 440 => mesure sans voie
    // 500 - 599 => ECRITURE_BINAIRE
        // 500 => exception création fichier binaire
        // 501 => exception interprétation status
        // 502 => exception calcul question suivante
        // 503 => exception écriture du fichier binaire
        // 504 => exception globale LECT DONNEES PAR TCP
    // 600 - 699 => LECTURE_BINAIRE
        // 600 => dossier source introuvable
        // 601 => fichiers sources introuvables
        // 602 => exception recherche fichiers sources
        // 603 => exception copie fichiers sources
        // 604 => fichier à lire introuvable
        // 605 => version automate non prise en charge
        // 606 => mesure incomplète
        // 607 => type mesure inconnu
        // 608 => exception interprétation mesure
        // 609 => exception globale LECT MESURE
    // 700 - 799 => LECTURE XDQ
    // 800 - 899 => DATABASE
        // 800 => station avec enregistreur non trouvée
        // 801 => exception récupération station
        // 802 => exception récupération voies telemesurees
        // 803 => exception récupération voies tor
        // 804 => exception création schema dans db mesures
        // 805 => exception création table dans db mesures
        // 806 => exception récupération timestamp dans table db mesures
        // 807 => aucune mesure insérée dans table de db mesures
        // 808 => exception globale INSERTION MESURES
        // 809 => exception récupération defauts
        // 810 => défaut actif non supprimé dans table defauts_actifs
        // 811 => défaut actif non inséré dans table defauts_actifs
        // 812 => exception insertion/suppression defauts_actifs
        // 813 => défaut non mis à jour dans table defauts
        // 814 => défauts non insérés dans table defauts
        // 815 => exception insertion defauts dans table defauts
        // 816 => mise à jour de la enregistreur non effectué
        // 817 => exception mise à jour de la station dans reseau.stations
    // 900 - 999 => GESTION DEFAUTS
        // 900 => exception globale GESTION DEFAUTS 
    // 1000 - 1099  => INCONNU

    public static class ErrorUtils
    {
        /// <summary>
        /// 100 => mauvais format adresse ip <br/>
        /// ---- Serveur ---- <br/>
        /// 110 => échec ping serveur <br/>
        /// 111 => exception globale ping serveur <br/>
        /// ---- Routeur --- <br/>
        /// 120 => échec ping routeur <br/>
        /// 121 => exception globale ping routeur <br/>
        /// </summary>
        public static Erreur HandlePingError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "PING");

            switch (code)
            {
                // Format
                case 100:
                    erreur.Description =
                        $"Annulation appel, adresse IP {incorrectValue} n'a pas le bon format";
                    break;
                // Serveur
                case 110:
                    erreur.Description =
                        $"Annulation appel, échec du ping serveur {incorrectValue}";
                    break;
                case 111:
                    erreur.Description =
                        $"Annulation appel, exception globale levée lors du ping serveur {incorrectValue} : {exception!.GetType().Name}";
                    break;
                // Routeur
                case 120:
                    erreur.Description =
                        $"Annulation appel, échec du ping routeur {incorrectValue}";
                    break;
                case 121:
                    erreur.Description =
                        $"Annulation appel, exception globale levée lors du ping routeur {incorrectValue} : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// 200 => exception création serveur TCP <br/>
        /// 201 => exception lecture du flux TCP <br/>
        /// 202 => exception écriture du flux TCP <br/>
        /// </summary>
        public static Erreur HandleTCPError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "TCP");

            switch (code)
            {
                case 200:
                    erreur.Description =
                        $"Annulation appel, exception levée lors de la création du serveur TCP {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 201:
                    erreur.Description =
                        $"Arrêt appel, exception levée lors de la lecture du flux TCP : {exception!.GetType().Name}";
                    break;
                case 202:
                    erreur.Description =
                        $"Arrêt appel, exception levée lors de l'écriture dans le flux TCP : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// --- Telechargement --- <br/>
        /// 300 => host, login, mdp non définis <br/>
        /// 301 => échec de la connexion FTP <br/>
        /// 302 => dossier distant introuvable <br/>
        /// 303 => exception globale LECT DONNEES PAR FTP<br/>
        /// --- Televersement --- <br/>
        /// </summary>
        public static Erreur HandleFTPError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "FTP");

            switch (code)
            {
                // Telechargement
                case 300:
                    erreur.Description =
                        $"Annulation téléchargements FTP et arrêt appel, host, login ou mot de passe FTP non définis";
                    break;
                case 301:
                    erreur.Description =
                        $"Annulation téléchargements FTP et arrêt appel, échec de la connexion FTP";
                    break;
                case 302:
                    erreur.Description =
                        $"Annulation téléchargements FTP et arrêt appel, dossier distant n'existe pas";
                    break;
                case 303:
                    erreur.Description =
                        $"Arrêt téléchargements FTP et arrêt appel, exception globale levée lors de l'étape LECT DONNEES PAR FTP : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// ---- Status données ---- <br/>
        /// 400 => exception globale LECT DISPONIBILITE DONNEES <br/>
        /// ---- Version enregistreur ---- <br/>
        /// 410 => exception globale LECT VERSION ENREGISTREUR <br/>
        /// 411 => version non prise en charge <br/>
        /// 412 => type d'herue inconnu <br/>
        /// ---- Config voies telemesurees ---- <br/>
        /// 420 => exception globale LECT CONFIG VOIES ANALOGIQUES <br/>
        /// ---- Config voies tor ---- <br/>
        /// 430 => ? <br/>
        /// ---- Discordance config ---- <br/>
        /// 440 => mesure sans voie <br/>
        /// </summary>
        public static Erreur HandleStatusError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "LECTURE_STATUS_VERSION_CONFIG");

            switch (code)
            {
                // Status données
                case 400:
                    erreur.Description =
                        $"Arrêt appel, exception globale levée lors de l'étape LECT DISPONIBILITE DONNEES : {exception!.GetType().Name}";
                    break;
                // Version enregistreur
                case 410:
                    erreur.Description =
                        $"Arrêt appel, exception globale levée lors de l'étape LECT VERSION ENREGISTREUR : {exception!.GetType().Name}";
                    break;
                case 411:
                    erreur.Description =
                        $"Arrêt appel : version {incorrectValue} non prise en charge par le protocole automate";
                    break;
                case 412:
                    erreur.Description =
                        $"Arrêt appel : type d'heure {incorrectValue} inconnu";
                    break;
                // Config voies telemesurees
                case 420:
                    erreur.Description =
                        $"Arrêt appel, exception globale levée lors de l'étape LECT CONFIG VOIES ANALOGIQUES : {exception!.GetType().Name}";
                    break;
                // Config voie tor
                // Discordance config
                case 440:
                    erreur.Description =
                        $"Annulation insertion mesures en base, une mesure n'a pas pu être associée à une voie {incorrectValue}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// 500 => exception création fichier binaire <br/>
        /// 501 => exception interprétation status <br/>
        /// 502 => exception calcul question suivante <br/>
        /// 503 => exception écriture du fichier binaire <br/>
        /// 504 => exception globale LECT DONNEES PAR TCP <br/>
        /// </summary>
        public static Erreur HandleWritingError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "ECRITURE_BINAIRE");

            switch (code)
            {
                // Creation fichier
                case 500:
                    erreur.Description =
                        $"Annulation écriture du binaire et arrêt appel, exception levée lors de la création du fichier avant écriture : {exception!.GetType().Name}";
                    break;
                // Interprétation
                case 501:
                    erreur.Description =
                        $"Arrêt écriture du binaire et arrêt appel, exception levée lors de l'interprétation du status : {exception!.GetType().Name}";
                    break;
                case 502:
                    erreur.Description =
                        $"Arrêt écriture du binaire et arrêt appel, exception levée lors du calcul de la question suivante : {exception!.GetType().Name}";
                    break;
                // Ecriture fichier
                case 503:
                    erreur.Description =
                        $"Arrêt écriture du binaire et arrêt appel, exception levée lors de l'écriture dans le fichier binaire : {exception!.GetType().Name}";
                    break;
                // Globale
                case 504:
                    erreur.Description =
                        $"Arrêt écriture du binaire et arrêt appel, exception globale levée lors de l'étape LECT DONNEES PAR TCP : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// 600 => dossier source introuvable <br/>
        /// 601 => fichiers sources introuvables <br/>
        /// 602 => exception recherche fichiers sources <br/>
        /// 603 => exception copie fichiers sources <br/>
        /// 604 => fichier à lire introuvable <br/>
        /// 605 => version automate non prise en charge <br/>
        /// 606 => mesure incomplète <br/>
        /// 607 => type mesure inconnu <br/>
        /// 608 => exception interprétation mesure <br/>
        /// 609 => exception globale LECT MESURES <br/>
        /// </summary>
        public static Erreur HandleReadingError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "LECTURE_BINAIRE");

            switch (code)
            {
                // Dossier
                case 600:
                    erreur.Description =
                        $"Annulation lecture binaire, le dossier de fichiers sources n'existe pas";
                    break;
                // Recherche fichier sources
                case 601:
                    erreur.Description =
                        $"Annulation lecture binaire, les fichiers sources sont introuvables";
                    break;
                case 602:
                    erreur.Description =
                        $"Annulation lecture binaire, exception levée lors de la recherche des fichiers sources  : {exception!.GetType().Name}";
                    break;
                // Copie
                case 603:
                    erreur.Description =
                        $"Annulation lecture binaire, exception levée lors de la copie des fichiers sources  : {exception!.GetType().Name}";
                    break;
                case 604:
                    erreur.Description =
                        $"Annulation lecture binaire, fichier à lire {incorrectValue} introuvable ";
                    break;
                // M580 version inutilisable
                case 605:
                    erreur.Description =
                        $"Annulation lecture binaire, version de l'automate {incorrectValue} non prise en charge";
                    break;
                // Mesure non interprétable
                case 606:
                    erreur.Description =
                        $"Arrêt lecture binaire, mesure incomplète : {incorrectValue} octets manquants";
                    break;
                case 607:
                    erreur.Description =
                        $"Arrêt lecture binaire, type de mesure inconnu : info = {incorrectValue}";
                    break;
                case 608:
                    erreur.Description =
                        $"Arrêt lecture binaire, exception levée lors de l'interprétation d'une mesure de {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 609:
                    erreur.Description =
                        $"Arrêt lecture binaire, exception globale levée lors de l'étape LECT MESURES : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// 800 => station avec enregistreur non trouvée <br/>
        /// 801 => exception récupération station <br/>
        /// 802 => exception récupération voies telemesurees <br/>
        /// 803 => exception récupération voies tor <br/>
        /// 804 => exception création schema dans db mesures <br/>
        /// 805 => exception création table dans db mesures <br/>
        /// 806 => exception récupération timestamp dans table db mesures <br/>
        /// 807 => aucune mesure insérée dans table de db mesures <br/>
        /// 808 => exception globale INSERTION MESURES <br/>
        /// 809 => exception récupération defauts <br/>
        /// 810 => défaut actif non supprimé dans table defauts_actifs <br/>
        /// 811 => défaut actif non inséré dans table defauts_actifs <br/>
        /// 812 => exception insertion/suppression defauts_actifs <br/>
        /// 813 => défaut non mis à jour dans table defauts <br/>
        /// 814 => défauts non insérés dans table defauts <br/>
        /// 815 => exception insertion defauts dans table defauts<br/>
        /// 816 => mise à jour de la enregistreur non effectué <br/>
        /// 817 => exception mise à jour de la station dans reseau.stations <br/>
        /// </summary>
        public static Erreur HandleDatabaseError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "DATABASE");

            switch (code)
            {
                case 800:
                    erreur.Description =
                        $"Annulation appel, aucune station avec enregistreur trouvée en base pour l'id {incorrectValue}";
                    break;
                case 801:
                    erreur.Description =
                        $"Annulation appel, exception levée lors de la récupération de la station id {incorrectValue} en base : {exception!.GetType().Name}";
                    break;
                case 802:
                    erreur.Description =
                        $"Annulation insertion mesures en base, exception levée lors de la récupération des voies telemesurees de la station id {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 803:
                    erreur.Description =
                        $"Annulation insertion mesures en base, exception levée lors de la récupération des voies tor de la station id {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 804:
                    erreur.Description =
                        $"Annulation insertion mesures en base, exception levée lors de la création du schema {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 805:
                    erreur.Description =
                        $"Annulation insertion mesures en base, exception levée lors de la création de la table {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 806:
                    erreur.Description =
                        $"Annulation insertion mesures en base, exception levée lors de la récupération des timestamps de la table {incorrectValue} : {exception!.GetType().Name}";
                    break;
                case 807:
                    erreur.Description =
                        $"Arrêt insertion mesures en base, aucune mesure n'a été insérée dans la table {incorrectValue}";
                    break;
                case 808:
                    erreur.Description =
                        $"Arrêt insertion mesures en base, exception globale levée lors l'étape INSERTION MESURES : {exception!.GetType().Name}";
                    break;
                case 809:
                    erreur.Description =
                        $"Annulation gestion defauts, exception levée lors de la récupération des défauts de la table defaut.defauts : {exception!.GetType().Name}";
                    break;
                case 810:
                    erreur.Description =
                        $"Arrêt gestion defauts, le défaut actif {incorrectValue} n'a pas été supprimé de la table defaut.defauts_actifs";
                    break;
                case 811:
                    erreur.Description =
                        $"Arrêt gestion defauts, le défaut actif {incorrectValue} n'a pas été inséré dans la table defaut.defauts_actifs";
                    break;
                case 812:
                    erreur.Description =
                        $"Arrêt gestion defauts, exception levée lors de l'insertion ou suppression de défauts actifs dans la table defaut.defauts_actifs : {exception!.GetType().Name}";
                    break;
                case 813:
                    erreur.Description =
                        $"Arrêt gestion defauts, le défaut {incorrectValue} n'a pas été mis à jour dans la table defaut.defauts";
                    break;
                case 814:
                    erreur.Description =
                        $"Arrêt gestion defauts, les défauts de la voie id {incorrectValue} n'ont pas été insérés dans la table defaut.defauts";
                    break;
                case 815:
                    erreur.Description =
                        $"Arrêt gestion defauts, exception levée lors de l'insertion des defauts dans la table defaut.defauts : {exception!.GetType().Name}";
                    break;
                case 816:
                    erreur.Description =
                        $"Arrêt traitement, l'enregistreur n'as pas été mis à jour dans la table reseau.enregistreurs";
                    break;
                case 817:
                    erreur.Description =
                        $"Arrêt traitement, exception levée lors de la mise à jour de la station dans la table reseau.enregistreurs : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        /// <summary>
        /// 900 => exception globale GESTION DEFAUTS<br/>
        /// </summary>
        public static Erreur HandleDefautsHandlingError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "GESTION_DEFAUTS");

            switch (code)
            {
                case 900:
                    erreur.Description =
                        $"Arrêt gestion defauts, exception globale levée lors l'étape GESTION DEFAUTS : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        public static Erreur HandleUnknowError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "INCONNU");

            switch (code)
            {
                case 1000:
                    erreur.Description =
                        $"Annulation ou arrêt appel ou traitement, exception inconnue levée dans Automate.Execute() : {exception!.GetType().Name}";
                    break;
            }

            return erreur;
        }

        public static Erreur HandleFileSystemError(
            int code,
            Exception? exception = null,
            string? incorrectValue = null
        )
        {
            Erreur erreur = new Erreur(code, "FILE_SYSTEM");
            switch (code) 
            {
                case 1:
                    erreur.Description = $"Erreur du système de fichiers : {exception.Message}";
                    break;
            }
            return erreur;
        }
    }
}
