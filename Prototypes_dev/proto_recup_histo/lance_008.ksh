rm -f ../PD/*.traite*
rm -f ../PD/*.voie
rm -f ../PD/*.tmp
rm -f fi[1-7]
rm -f *.log
rm -f fivoie
>nohup.out
fichier="fichier_nabil.csv"
>$fichier
station_to_treate="pd"

fic_log=$station_to_treate"_".log

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF
truncate table sh_$station_to_treate.tmp_load_file_jour_releves;
truncate table sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana;
EOF

echo "Date debut lancement PD $(date '+%Y-%m-%d %H:%M:%S')"
for fic in `ls ../PD/*12.S[0-5][0-9] ../PD/*07.S[0-5][0-9] ../PD/*17.S[0-5][0-9]`
do
    dos2unix $fic
    echo "Date debut lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
    ./work_releves_files_avec_tor_correc.ksh PD $fic
    cat fi1 fi2 fi3 fi4 fi5 fi6 fi7 >> $fichier
    echo "Date fin lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
done
cat $station_to_treate"_"*.log > $fic_log
echo "Date fin lancement PD $(date '+%Y-%m-%d %H:%M:%S')"
