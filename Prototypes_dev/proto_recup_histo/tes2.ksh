fic_voie="fic_voie"
fic_tmp="fic_tmp"
>$fic_tmp
echo "a;AAAA" > $fic_voie
fic_sortie="fic_sortie"
echo "m23h38;QCO_VMER_PD=0186;QEP_MOREE_PD=0088" > $fic_sortie
for ligne in `cat $fic_voie`
do
    voie_lettre=`echo $ligne | awk -F";" '{print $1}'`
    voie_nom=`echo $ligne | awk -F";" '{print $2}'`
    for ligne2 in `cat $fic_sortie`
	do
		ligne3=`echo $ligne2 | awk -F"!" '{if ((substr($0,1,1)=="s") || (substr($0,1,1)=="S") || (substr($0,1,1)=="r") || (substr($0,1,1)=="&")) print $0; else print "";}'`
		if [ "X$ligne3" != "X" ]
		then
			echo $ligne2 | sed -e "s/$voie_nom/${voie_lettre}/g" >> $fic_tmp
		else
			echo $ligne2 >> $fic_tmp
		fi
    done
done
