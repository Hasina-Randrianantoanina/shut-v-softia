# Architecture frontend du projet SHUT au 29/07/2014

Pour lancer l'application : **npm run dev** à la source du projet

## 1. Framework et langage

- **Next.js** (version 14.2.3) : Framework React avec rendu côté client et/ou serveur
- **React** (version 18.3.1) : Bibliothèque JavaScript pour construire les interfaces
- **JavaScript/JSX** : Langage principal utilisé avec la syntaxe JSX pour les composants React

## 2. Structure du projet

/src
├── /app             # Configuration globale de l'application Next.js
├── /components      # Composants React réutilisables
├── /containers      # Composants de plus haut niveau, souvent composés de plusieurs composants
├── /hooks           # Hooks React personnalisés
├── /layouts         # Composants de mise en page
├── /pages           # Routes/pages de l'application Next.js
├── /services        # Services pour les appels API
├── /styles          # Fichiers CSS/styles globaux
└── /utils           # Fonctions utilitaires

## 3. Gestion d'état et requêtes

- **React Query (TanStack Query)** : Utilisé pour la gestion de l'état côté serveur et les requêtes API

## 4. Styling

- **Tailwind CSS** : Framework CSS utilitaire pour le styling
- **CSS Modules** : Pour les styles spécifiques aux composants

## 5. Composants UI

- **AG Grid** : Utilisé pour les tableaux de données
- **React Icons** : Bibliothèque d'icônes
- **React DatePicker** : Pour les sélecteurs de date
- **ECharts** : Pour les graphiques et visualisations
- **ChartJS** : Pour les graphiques et visualisations

## 6. Routing

- Géré par Next.js avec son système de fichiers basé sur le routing

## 7. Syntaxe et conventions

- Utilisation de composants fonctionnels React avec Hooks
- Nommage en PascalCase pour les composants
- Nommage en camelCase pour les fonctions et variables

## 8. Gestion des formulaires

- Utilisation de composants personnalisés pour les champs de formulaire
- Validation des formulaires gérée manuellement

## 9. Responsive Design

- Utilisation des classes Tailwind pour la mise en page responsive


### Description des components clés au 29/07/2014

1. **ChampEditable** : Composants pour l'édition inline dans les tableaux AG Grid.
   - `CommentaireEditor.js` : Éditeur pour les champs de commentaire.
   - `CommentaireRenderer.js` : Rendu des champs de commentaire.
   - `HeureEditor.js` : Éditeur pour les champs d'heure.

2. **CollasableTable** : Table pliable/dépliable pour afficher des données de manière compacte.

3. **CustomButton** : Boutons personnalisés réutilisables.
   - `ActionButtons.jsx` : Boutons d'action (éditer, supprimer).
   - `CustomButton.jsx` : Bouton de base personnalisable.
   - `EditButtons.jsx` : Boutons pour l'édition (sauvegarder, annuler).
   - `ModifyButton.jsx` : Bouton de modification.

4. **DatePicker** : Composant personnalisé de sélection de date.

5. **Login** : Composant pour la page de connexion.

6. **MainLayout** : Composants pour la mise en page principale de l'application.
   - `Footer.jsx`, `Menu.jsx`, `Navbar.jsx`, `Topbar.jsx` : Éléments de la mise en page.

7. **PerteEnregistrementsForms** : Formulaires pour la gestion des pertes d'enregistrements.

8. **StationBandeauSelection** : Composants pour la sélection des stations.

9. **TimeSeries** : Composants pour l'affichage des séries temporelles.
   - Utilise différentes bibliothèques (ECharts, Chart.js) pour les visualisations. Bibliothèque principale à définir.

10. **TableData** : Données (mock pour le moment, récupéré des services d'appel API ensuite) des tableaux.

11. **TableColumnDefinition** : Définition et configuration des colonnes des tableaux.

   ### Description des containers clés au 29/07/2014

1. **BandeauSelectionStations**
   - `StationSelectorContainerUsage.jsx` : Gère la sélection des stations pour le réseau d'usage.

2. **CyclesAppels**
   - `CyclesAppelsGrid.jsx` : Grille pour l'affichage et l'édition des cycles d'appels.
   - `ResObsCyclesAppels.jsx` : Container pour les cycles d'appels du réseau d'observation.
   - `ResUsageCyclesAppels.jsx` : Container pour les cycles d'appels du réseau d'usage.

3. **DefautCapteur**
   - Containers pour la gestion des défauts capteurs pour les réseaux d'observation et d'usage.

4. **DefautStation**
   - Containers pour la gestion des défauts stations pour les réseaux d'observation et d'usage.

5. **EtatStation**
   - Containers pour l'affichage de l'état des stations pour les réseaux d'observation et d'usage.

6. **PertesEnregistrements**
   - `DatePickerContainer.jsx` : Gère la sélection de dates pour les pertes d'enregistrements.
   - `EditPertesEnregistrementPopup.jsx` : Popup d'édition pour les pertes d'enregistrements.
   - `PertesEnregistrementsUsage.jsx` : Container principal pour la gestion des pertes d'enregistrements.
   - `ResUsagePertesEnregistrement.jsx` : Affichage des pertes d'enregistrements pour le réseau d'usage.

7. **Stations**
   - Containers pour la gestion des stations pour les réseaux d'observation et d'usage.
