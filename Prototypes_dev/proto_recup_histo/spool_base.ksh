if [ $# -ne 1 ]
then
    echo "Erreur : Entrer le nom du fichier à spooler !!!"
    exit 1
fi
>nohup.out

station_to_treate=$1
fichier_entete_to_treate=$2
fichier_donnes_to_treate=$3

cp -p $fichier_entete_to_treate fivoieall
cp -p $fichier_donnes_to_treate fiall

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF
\copy select * from $1 TO 'fispool';
EOF