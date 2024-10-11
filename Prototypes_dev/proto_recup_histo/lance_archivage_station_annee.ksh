if [ $# -ne 2 ]
then
    echo "Erreur : Entrer le nom de la station, et l'ensemble des années à traiter sur 2 chiffres espacés de virgule!!!"
    exit 1
fi

station_to_treate=$1
annees_to_treate=$2

rm -f ../$station_to_treate/*.traite*
rm -f ../$station_to_treate/*.voie
rm -f ../$station_to_treate/*.tmp
rm -f fi[1-7]
rm -f *.log
rm -f fivoie
>nohup.out

ch_annees_to_treate=`echo ${annees_to_treate} | sed -e "s/,/_/g"`
fichier_nabyl_body=${station_to_treate}_"fichier_nabyl_body_"$ch_annees_to_treate".csv"
>${fichier_nabyl_body}

fichier_nabyl_entete=${station_to_treate}_"fichier_nabyl_entete_"$ch_annees_to_treate".csv"
>${fichier_nabyl_entete}

fic_log=$station_to_treate"_".log

m_station_to_treate=`echo $station_to_treate |tr -s A-Z a-z`

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF
truncate table sh_$m_station_to_treate.tmp_load_file_jour_releves;
truncate table sh_$m_station_to_treate.tmp_load_file_jour_releves_voie_ana;
EOF

ch2_annees_to_treate=$ch_annees_to_treate
arret=1
chaine_commande=""
while [ $arret -eq 1 ]
do
    
	annee=`echo $ch2_annees_to_treate | awk -F"_" '{print $1}'`	
	if [ ${#ch2_annees_to_treate} -gt 2 ]
	then
		chaine_commande=$chaine_commande" ../$station_to_treate/*$annee.S[0-5][0-9]"
		ch2_annees_to_treate=`echo $ch2_annees_to_treate | sed -e "s/${annee}_//"`
	else
		if [ $arret -eq 1 ]
        then
            chaine_commande=$chaine_commande" ../$station_to_treate/*$annee.S[0-5][0-9]"
        fi
        arret=0
	fi
	
done

chaine_commande="ls $chaine_commande"
echo "Date debut lancement ${station_to_treate} annee $ch_annees_to_treate $(date '+%Y-%m-%d %H:%M:%S')"
for fic in `$chaine_commande`
do
    dos2unix $fic
    echo "Date debut lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
    ./work_releves_files_avec_tor_correc.ksh ${station_to_treate} $fic
    cat fi1 fi2 fi3 fi4 fi5 fi6 fi7 >> $fichier_nabyl_body
	if [ -f fi8 ]
	then
		cat fi8 >> $fichier_nabyl_body
	fi
	if [ -f fi9 ]
	then
		cat fi9 >> $fichier_nabyl_body
	fi
	if [ -f fi10 ]
	then
		cat fi10 >> $fichier_nabyl_body
	fi
	if [ -f fi11 ]
	then
		cat fi11 >> $fichier_nabyl_body
	fi
	if [ -f fi12 ]
	then
		cat fi12 >> $fichier_nabyl_body
	fi
	if [ -f fi13 ]
	then
		cat fi13 >> $fichier_nabyl_body
	fi
	if [ -f fi14 ]
	then
		cat fi14 >> $fichier_nabyl_body
	fi
	cat fivoie >> $fichier_nabyl_entete
    echo "Date fin lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
done
cat *.log > $fic_log
echo "Date fin lancement ${station_to_treate} annee $ch_annees_to_treate $(date '+%Y-%m-%d %H:%M:%S')"
