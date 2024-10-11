rm -f ../AS/*.traite*
rm -f ../AS/*.voie
rm -f ../AS/*.tmp
rm -f fi[1-7]
rm -f *.log
rm -f fivoie
>nohup.out
station_to_treate="as"

fic_log=$station_to_treate"_".log

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF
truncate table sh_$station_to_treate.tmp_load_file_jour_releves;
truncate table sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana;
EOF

echo "Date debut lancement AS $(date '+%Y-%m-%d %H:%M:%S')"
for fic in `ls ../AS/AS*[0-5][0-9]`
do
    dos2unix $fic
    echo "Date debut lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic" 
    ./work_releves_files_avec_tor_test_log.ksh AS $fic 
    echo "Date fin lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
done
station_to_treate=`echo $station_to_treate | tr 'a-z' 'A-Z'`
cat $station_to_treate"_"*.log > $fic_log
echo "Date fin lancement AS $(date '+%Y-%m-%d %H:%M:%S')"
