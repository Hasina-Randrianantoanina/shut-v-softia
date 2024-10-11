Bienvenue sur SHUT Refonte

## Intégration de TimescaleDb dans prototype_fullstack_React-Dotnet

1. Pour créer la base TimescaleDb et insérer les données, voir `prototype_series_tempo_TimescaleDB/README.md` section `Prepare database`.
2. Modifier les `ConnectionStrings` dans `prototype_fullstack_React-Dotnet/ProtoBack/appsettings.json`:
```js
  "ConnectionStrings": {
    "dbConnect": "Host=localhost;Port=5432;Database=ProtoShutV1;Username=postgres;Password=admin123;",
    "TimescaleDb": "Host=localhost; Port=5434; Database=timescale; Username=postgres; Password=softia"
  }
```
3. Lance le back-end `prototype_fullstack_React-Dotnet/ProtoBack`
4. Executer le front `prototype_fullstack_React-Dotnet/ProtoFrontShut/proto-front-v2` et naviguer vers le menu `Visu. TimescaleDb`.