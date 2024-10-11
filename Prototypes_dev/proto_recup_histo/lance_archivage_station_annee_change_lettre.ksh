if [ $# -ne 2 ]
then
    echo "Erreur : Entrer le nom de la station, et l'ensemble des années à traiter sur 2 chiffres espacés de virgule!!! ou ALL pour toutes les annees"
    exit 1
fi

echo "debut"
station_to_treate=$1
annees_to_treate=$2

ch_annees_to_treate=""
tmp=tmp$$
if [ `echo $annees_to_treate |tr A-Z a-z` == "all" ]
then

	ls ../$station_to_treate/*.S[0-5][0-9] | awk -F".S" '{print substr($1,length($1)-1,2)}' | sort -u > $tmp
	for ligne in `cat $tmp`
	do
		ch_annees_to_treate=$ch_annees_to_treate"_"$ligne
	done
      
	ch_annees_to_treate=`echo $ch_annees_to_treate | awk '{print substr($0,2,length($0)-1)}'`
	file_ch_annees_to_treate="all"
	
else
	
	ch_annees_to_treate=`echo ${annees_to_treate} | sed -e "s/,/_/g"`
	long=`echo $ch_annees_to_treate | awk '{print length($0)}'`

	if [ $long -gt 12 ]
	then
		file_ch_annees_to_treate=`echo $ch_annees_to_treate | awk '{print substr($0,1,12)}'`
		file_ch_annees_to_treate=$file_ch_annees_to_treate"_et_autres_annees"
        else
	        file_ch_annees_to_treate=$ch_annees_to_treate
	fi
fi

echo "liste="$ch_annees_to_treate

rm -f ../$station_to_treate/*.traite*
rm -f ../$station_to_treate/*.voie
rm -f ../$station_to_treate/*.tmp
rm -f *fichier_nabyl*
rm -f fi[1-9]*
rm -f *.log
rm -f vfivoie
rm -f vfibody
>nohup.out


fichier_nabyl_body=${station_to_treate}_"fichier_nabyl_body_"$file_ch_annees_to_treate".csv"
>${fichier_nabyl_body}

fichier_nabyl_entete=${station_to_treate}_"fichier_nabyl_entete_"$file_ch_annees_to_treate".csv"
>${fichier_nabyl_entete}

fic_log=$station_to_treate"_".log

m_station_to_treate=`echo $station_to_treate |tr A-Z a-z`
fic_log=$station_to_treate"_".log

#export PGPASSWORD="shutweb"
#psql -h localhost -p 5432 -U postgres -d archives << EOF
#truncate table sh_$m_station_to_treate.tmp_load_file_jour_releves;
#truncate table sh_$m_station_to_treate.tmp_load_file_jour_releves_voie_ana;
#EOF

echo "apres truncate="$ch_annees_to_treate

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
    ./work_releves_files_avec_tor_correc_new.ksh ${station_to_treate} $fic
    cat fi* >> $fichier_nabyl_body
    cat vfivoie >> $fichier_nabyl_entete
    rm -f fi* vfivoie 
    echo "Date fin lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
done
rm -f fi* vfivoie
./change_entete.ksh $fichier_nabyl_entete
./change_body.ksh $fichier_nabyl_body
cp -p $fichier_nabyl_entete vfivoie
cp -p $fichier_nabyl_body vfibody

rm -f ../$station_to_treate/*.traite*
rm -f ../$station_to_treate/*.voie
rm -f ../$station_to_treate/*.tmp

exit 0
>$tmp
echo "\copy sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana(fichier,lettre_voie,nom_voie,precision) FROM 'fivoie' DELIMITER ';';" >> $tmp
echo "\copy sh_$m_station_to_treate.tmp_load_file_jour_releves(ordre,fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'vfibody' DELIMITER ';';" >> tmp

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF >> $fic_log
`cat $tmp`
EOF


cat *.log > $fic_log
echo "Date fin lancement ${station_to_treate} annee $ch_annees_to_treate $(date '+%Y-%m-%d %H:%M:%S')"
