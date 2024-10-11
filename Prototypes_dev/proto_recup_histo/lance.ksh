rm -f ../PD/*.traite*
rm -f ../PD/*.voie
station_to_treate="pd"

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF
truncate table sh_$station_to_treate.tmp_load_file_jour_releves;
truncate table sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana;
EOF

echo "Date debut lancement $(date '+%Y-%m-%d %H:%M:%S')"
for fic in `ls ../PD/*.S[0-5][0-9]`
do
    dos2unix $fic 
    echo "Date debut lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic" 
    ./work_releves_files_tor.ksh PD $fic 
    echo "Date fin lancement $(date '+%Y-%m-%d %H:%M:%S') fichier $fic"
done
echo "Date fin lancement $(date '+%Y-%m-%d %H:%M:%S')"
