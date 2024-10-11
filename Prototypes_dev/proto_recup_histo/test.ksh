ch2_annees_to_treate="07_12_17"
arret=1
station_to_treate="PD"
echo "ch2="$ch2_annees_to_treate
while [ $arret -eq 1 ]
do
   annee=`echo $ch2_annees_to_treate | awk -F"_" '{print $1}'`
   echo "annee=$annee"	
   if [ ${#ch2_annees_to_treate} -gt 2 ]
   then
	chaine_commande=$chaine_commande" ../$station_to_treate/*$annee.S[0-5][0-9]"
        echo "chaine_commande=$chaine_commande"
	ch2_annees_to_treate=`echo $ch2_annees_to_treate | sed -e "s/${annee}_//"`
	echo "ch2_annees_to_treate=$ch2_annees_to_treate"
   else
        if [ $arret -eq 1 ]
	then
            chaine_commande=$chaine_commande" ../$station_to_treate/*$annee.S[0-5][0-9]"
	fi	
	arret=0
   fi
					
done
echo "chaine_commande=$chaine_commande"
