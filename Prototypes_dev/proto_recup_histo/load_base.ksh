if [ $# -ne 3 ]
then
    echo "Erreur : Entrer et dans cet ordre, la station, le nom du fichier entete et des donnees à traiter !!!"
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
truncate table sh_$station_to_treate.tmp_load_file_jour_releves;
truncate table sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana;
truncate table sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana_archive;
EOF

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF
\copy sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana(fichier,lettre_voie,nom_voie,precision) FROM 'fivoieall' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(ordre,fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fiall' DELIMITER ';';
ANALYSE sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana;
ANALYSE sh_$station_to_treate.tmp_load_file_jour_releves;
EOF
