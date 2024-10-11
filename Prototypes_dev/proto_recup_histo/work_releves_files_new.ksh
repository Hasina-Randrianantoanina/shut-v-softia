if [ $# -ne 2 ]
then
    echo "Erreur : Entrer le nom de la station et du fichier à traiter !!!"
    exit 1
fi

station_to_treate=$1
#station_to_treate="PD"
fic_to_treate=../$1/$2
#fic_to_treate=$station_to_treate/"PD311814.S33"
#fic_to_treate=../$station_to_treate/"PD301517.S18"

fic_voie="$fic_to_treate".voie
fic_sortie="$fic_to_treate".traite
fic_tmp="$fic_to_treate".tmp
cp -p $fic_to_treate $fic_sortie

cat $fic_to_treate | grep "#" | sed -e 's/\\t/ /g' | sed -e 's/  / /g' | sed -e 's/#//g' | awk -F" " '{print $1";"$2";"$3}' > $fic_voie

cat $fic_sortie | sed -e 's/^ /m/g' > $fic_tmp
cp -p $fic_tmp $fic_sortie


cat $fic_sortie | grep -v '^#' | grep -v '^D' | grep -v '^F' | grep -v '^H' | sed -e 's/\\t/ /g' | \
sed -e 's/   / /g' | sed -e 's/  / /g' | sed -e 's/ /;/g' > $fic_tmp
mv $fic_tmp $fic_sortie

rm -f $fic_tmp

awk 'BEGIN{file="./"(FILENAME)""}/^E/{file="./"(FILENAME)(++i)""}{print > file}' $fic_sortie

j=9012
BIN=$(echo "obase=2; ibase=16; $j" | bc )

exit 0

#for fic in `ls $fic_sortie.[1-9]`
#do
#
#export PGPASSWORD="shutweb"
#psql -h localhost -p 5432 -U postgres -d archives << EOF
#\copy 
#EOF
#
#done
