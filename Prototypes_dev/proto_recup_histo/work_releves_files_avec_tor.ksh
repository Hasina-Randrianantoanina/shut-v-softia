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
name_fic_to_treate=`basename $fic_to_treate`

fic_voie="$fic_to_treate".voie
fic_sortie="$fic_to_treate".traite
fic_tmp="$fic_to_treate".tmp
cp -p $fic_to_treate $fic_sortie
fic_log=$1"_"$name_fic_to_treate".log"

cat $fic_to_treate | grep "#" | sed -e 's/\\t/ /g' | sed -e 's/  / /g' | sed -e 's/#//g' | awk -F" " '{print $1";"$2";"$3}' > $fic_voie
mv $fic_voie fivoie
cat fivoie | awk -F";" -v FIC=$name_fic_to_treate '{print FIC";"$0}' > $fic_tmp
mv $fic_tmp fivoie

cat $fic_sortie | sed -e 's/^ /m/g' > $fic_tmp
cp -p $fic_tmp $fic_sortie


cat $fic_sortie | grep -v '^#' | grep -v '^D' | grep -v '^F' | grep -v '^H' | sed -e 's/\\t/ /g' | \
sed -e 's/   / /g' | sed -e 's/  / /g' | sed -e 's/ /;/g' > $fic_tmp
mv $fic_tmp $fic_sortie

awk 'BEGIN{file="./"(FILENAME)""}/^E/{file="./"(FILENAME)(++i)""}{print > file}' $fic_sortie

>$fic_tmp
for i in 1 2 3 4 5 6 7
do
if [ ! -f ${fic_sortie}${i} ]
then
    >${fic_sortie}${i}
else
    cat ${fic_sortie}${i} | awk '{print substr($0,1,1)";"substr($0,2,length($0)-1)}' > $fic_tmp
    cp -p $fic_tmp ${fic_sortie}${i}
fi
done

jour1=`head -1 ${fic_sortie}"1" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`
jour2=`head -1 ${fic_sortie}"2" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`
jour3=`head -1 ${fic_sortie}"3" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`
jour4=`head -1 ${fic_sortie}"4" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`
jour5=`head -1 ${fic_sortie}"5" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`
jour6=`head -1 ${fic_sortie}"6" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`
jour7=`head -1 ${fic_sortie}"7" | awk -F";" '{print substr($2,7,4)"-"substr($2,4,2)"-"substr($2,1,2)}'`

for i in 1 2 3 4 5 6 7
do
    va="jour"$i
    var=`echo ${!va}`
    cat ${fic_sortie}"$i" | awk -v JOUR=$var '{print JOUR";"$0}' > $fic_tmp
    mv $fic_tmp ${fic_sortie}"$i"
    cp ${fic_sortie}"$i" fi"$i"
done

comp1=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp2=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp3=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp4=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp5=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp6=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp7=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp8=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp9=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp10=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp11=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp12=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp13=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp14=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp15=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp16=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp17=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp18=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp19=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp20=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp21=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp22=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp23=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp24=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp25=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp26=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp27=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp28=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp29=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp30=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp31=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp32=";;;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp33=";;;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp34=";;;;;;;;;;;;;;;;;;;;;;;;;;;"
comp35=";;;;;;;;;;;;;;;;;;;;;;;;;;"
comp36=";;;;;;;;;;;;;;;;;;;;;;;;;"
comp37=";;;;;;;;;;;;;;;;;;;;;;;;"
comp38=";;;;;;;;;;;;;;;;;;;;;;;"
comp39=";;;;;;;;;;;;;;;;;;;;;;"
comp40=";;;;;;;;;;;;;;;;;;;;;"
comp41=";;;;;;;;;;;;;;;;;;;;"
comp42=";;;;;;;;;;;;;;;;;;;"
comp43=";;;;;;;;;;;;;;;;;;"
comp44=";;;;;;;;;;;;;;;;;"
comp45=";;;;;;;;;;;;;;;;"
comp46=";;;;;;;;;;;;;;;"
comp47=";;;;;;;;;;;;;;"
comp48=";;;;;;;;;;;;;"
comp49=";;;;;;;;;;;;"
comp50=";;;;;;;;;;;"
comp51=";;;;;;;;;;"
comp52=";;;;;;;;;"
comp53=";;;;;;;;"
comp54=";;;;;;;"
comp55=";;;;;;"
comp56=";;;;;"
comp57=";;;;"
comp58=";;;"
comp59=";;"
comp60=";"

for i in 1 2 3 4 5 6 7
do
   
    cat ${fic_sortie}"$i" | awk -F";"   -v CH1=$comp1 \
										-v CH2=$comp2 \
										-v CH3=$comp3 \
										-v CH4=$comp4 \
										-v CH5=$comp5 \
										-v CH6=$comp6 \
										-v CH7=$comp7 \
										-v CH8=$comp8 \
										-v CH9=$comp9 \
										-v CH10=$comp10 \
										-v CH11=$comp11 \
										-v CH12=$comp12 \
										-v CH13=$comp13 \
										-v CH14=$comp14 \
										-v CH15=$comp15 \
										-v CH16=$comp16 \
										-v CH17=$comp17 \
										-v CH18=$comp18 \
										-v CH19=$comp19 \
										-v CH20=$comp20 \
										-v CH21=$comp21 \
										-v CH22=$comp22 \
										-v CH23=$comp23 \
										-v CH24=$comp24 \
										-v CH25=$comp25 \
										-v CH26=$comp26 \
										-v CH27=$comp27 \
										-v CH28=$comp28 \
										-v CH29=$comp29 \
										-v CH30=$comp30 \
										-v CH31=$comp31 \
										-v CH32=$comp32 \
										-v CH33=$comp33 \
										-v CH34=$comp34 \
										-v CH35=$comp35 \
										-v CH36=$comp36 \
										-v CH37=$comp37 \
										-v CH38=$comp38 \
										-v CH39=$comp39 \
										-v CH40=$comp40 \
										-v CH41=$comp41 \
										-v CH42=$comp42 \
										-v CH43=$comp43 \
										-v CH44=$comp44 \
										-v CH45=$comp45 \
										-v CH46=$comp46 \
										-v CH47=$comp47 \
										-v CH48=$comp48 \
										-v CH49=$comp49 \
										-v CH50=$comp50 \
										-v CH51=$comp51 \
										-v CH52=$comp52 \
										-v CH53=$comp53 \
										-v CH54=$comp54 \
										-v CH55=$comp55 \
										-v CH56=$comp56 \
										-v CH57=$comp57 \
										-v CH58=$comp58 \
										-v CH59=$comp59 \
										-v CH60=$comp60 \
										'{if ($1=="") print $0CH1; \
										 else if ($2=="") print $0CH2; \
										 else if ($3=="") print $0CH3; \
										 else if ($4=="") print $0CH4; \
										 else if ($5=="") print $0CH5; \
										 else if ($6=="") print $0CH6; \
										 else if ($7=="") print $0CH7; \
										 else if ($8=="") print $0CH8; \
										 else if ($9=="") print $0CH9; \
										 else if ($10=="") print $0CH10; \
										 else if ($11=="") print $0CH11; \
										 else if ($12=="") print $0CH12; \
										 else if ($13=="") print $0CH13; \
										 else if ($14=="") print $0CH14; \
										 else if ($15=="") print $0CH15; \
										 else if ($16=="") print $0CH16; \
										 else if ($17=="") print $0CH17; \
										 else if ($18=="") print $0CH18; \
										 else if ($19=="") print $0CH19; \
										 else if ($20=="") print $0CH20; \
										 else if ($21=="") print $0CH21; \
										 else if ($22=="") print $0CH22; \
										 else if ($23=="") print $0CH23; \
										 else if ($24=="") print $0CH24; \
										 else if ($25=="") print $0CH25; \
										 else if ($26=="") print $0CH26; \
										 else if ($27=="") print $0CH27; \
										 else if ($28=="") print $0CH28; \
										 else if ($29=="") print $0CH29; \
										 else if ($30=="") print $0CH30; \
										 else if ($31=="") print $0CH31; \
										 else if ($32=="") print $0CH32; \
										 else if ($33=="") print $0CH33; \
										 else if ($34=="") print $0CH34; \
										 else if ($35=="") print $0CH35; \
										 else if ($36=="") print $0CH36; \
										 else if ($37=="") print $0CH37; \
										 else if ($38=="") print $0CH38; \
										 else if ($39=="") print $0CH39; \
										 else if ($40=="") print $0CH40; \
										 else if ($41=="") print $0CH41; \
										 else if ($42=="") print $0CH42; \
										 else if ($43=="") print $0CH43; \
										 else if ($44=="") print $0CH44; \
										 else if ($45=="") print $0CH45; \
										 else if ($46=="") print $0CH46; \
										 else if ($47=="") print $0CH47; \
										 else if ($48=="") print $0CH48; \
										 else if ($49=="") print $0CH49; \
										 else if ($50=="") print $0CH50; \
										 else if ($51=="") print $0CH51; \
										 else if ($52=="") print $0CH52; \
										 else if ($53=="") print $0CH53; \
										 else if ($54=="") print $0CH54; \
										 else if ($55=="") print $0CH55; \
										 else if ($56=="") print $0CH56; \
										 else if ($57=="") print $0CH57; \
										 else if ($58=="") print $0CH58; \
										 else if ($59=="") print $0CH59; \
										 else if ($60=="") print $0CH60;}' > $fic_tmp
    mv $fic_tmp ${fic_sortie}"$i"
    cp ${fic_sortie}"$i" fi"$i"
done

>$fic_tmp
for i in 1 2 3 4 5 6 7
do
    #echo "avant"
    #cat fi${i} | awk -F";" '{print NF}' | sort -u
    cat fi${i} | awk -F";" -v FILE=$name_fic_to_treate -v COMP=$comp2 '{if (NF==58) print FILE";"$0";;"; \
									else if (NF==59) print FILE";"$0";"; \
									else if (NF==60) print FILE";"$0; \
									else if (NF==61) print FILE";"substr($0,1,length($0)-1); \
								        else if (NF==62) print FILE";"substr($0,1,length($0)-2); \
									else print FILE""COMP;}' > $fic_tmp
    cp -p $fic_tmp fi${i}
    #echo "apres" 
    #cat fi${i} | awk -F";" '{print NF}' | sort -u
done

echo "avant PG"

exit 0

export PGPASSWORD="shutweb"
psql -h localhost -p 5432 -U postgres -d archives << EOF >> $fic_log
--truncate table sh_$station_to_treate.tmp_load_file_jour_releves;
--truncate table sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana;
\copy sh_$station_to_treate.tmp_load_file_jour_releves_voie_ana(fichier,lettre_voie,nom_voie,precision) FROM 'fivoie' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi1' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi2' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi3' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi4' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi5' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi6' DELIMITER ';';
\copy sh_$station_to_treate.tmp_load_file_jour_releves(fichier,time,evenement,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14, \
ch15,ch16,ch17,ch18,ch19,ch20,ch21,ch22,ch23,ch24,ch25,ch26,ch27,ch28,ch29,ch30,ch31,ch32,ch33,ch34,ch35,ch36,ch37,ch38,ch39,ch40,ch41,ch42, \
ch43,ch44,ch45,ch46,ch47,ch48,ch49,ch50,ch51,ch52,ch53,ch54,ch55,ch56, \
ch57,ch58) FROM 'fi7' DELIMITER ';';
EOF
