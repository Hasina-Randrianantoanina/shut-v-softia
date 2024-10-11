-- 425453 mesure pour le fichier PD_fichier_nabyl_body07_12.csv 429374 en base 423393=OK + 5981=en erreur.
DROP FUNCTION public.fill_archives_TOR;
CREATE OR REPLACE FUNCTION public.fill_archives_TOR(
	station in character varying (10),
	type_station character varying (10)
	)
    RETURNS text
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_execute_1 			text;
v_execute_2 			text;
v_execute_K_100			text;
v_execute_K_0			text;

table_releves			character varying (100);
table_entetes			character varying (100);
table_tmp_releves 		character varying (100);
table_tmp_entetes 		character varying (100);
table_tmp_position		character varying (100);
table_archivage			character varying (100);
--table_log_archivage		character varying (100);
table_position			character varying (100);
table_log				character varying (100);
message_retour 			text:='OK';

v_first_time 		character varying (100);
v_time 		 		character varying (100);
v_time_date			date;
v_ordre 			integer;
v_first_ordre 		integer;
v_evenement 		character varying (10);
v_first_evenement	character varying (10);
v_fichier 			character varying (100);
v_first_fichier 	character varying (100);
--
v_ch1 character varying (100);
v_ch2 character varying (100);
v_ch3 character varying (100);
v_ch4 character varying (100);
v_ch5 character varying (100);
v_ch6 character varying (100);
v_ch7 character varying (100);
v_ch8 character varying (100);
v_ch9 character varying (100);
v_ch10 character varying (100);
v_ch11 character varying (100);
v_ch12 character varying (100);
v_ch13 character varying (100);
v_ch14 character varying (100);
v_ch15 character varying (100);
v_ch16 character varying (100);
v_ch17 character varying (100);
v_ch18 character varying (100);
v_ch19 character varying (100);
v_ch20 character varying (100);
--
v_ch21 character varying (100);
v_ch22 character varying (100);
v_ch23 character varying (100);
v_ch24 character varying (100);
v_ch25 character varying (100);
v_ch26 character varying (100);
v_ch27 character varying (100);
v_ch28 character varying (100);
v_ch29 character varying (100);
v_ch30 character varying (100);
v_ch31 character varying (100);
v_ch32 character varying (100);
v_ch33 character varying (100);
v_ch34 character varying (100);
v_ch35 character varying (100);
v_ch36 character varying (100);
v_ch37 character varying (100);
v_ch38 character varying (100);
v_ch39 character varying (100);
v_ch40 character varying (100);
--
v_ch41 character varying (100);
v_ch42 character varying (100);
v_ch43 character varying (100);
v_ch44 character varying (100);
v_ch45 character varying (100);
v_ch46 character varying (100);
v_ch47 character varying (100);
v_ch48 character varying (100);
v_ch49 character varying (100);
v_ch50 character varying (100);
v_ch51 character varying (100);
v_ch52 character varying (100);
v_ch53 character varying (100);
v_ch54 character varying (100);
v_ch55 character varying (100);
v_ch56 character varying (100);
v_ch57 character varying (100);
v_ch58 character varying (100);
v_ch59 character varying (100);
v_ch60 character varying (100);

------------------

v_first_ch1 character varying (100);
v_first_ch2 character varying (100);
v_first_ch3 character varying (100);
v_first_ch4 character varying (100);
v_first_ch5 character varying (100);
v_first_ch6 character varying (100);
v_first_ch7 character varying (100);
v_first_ch8 character varying (100);
v_first_ch9 character varying (100);
v_first_ch10 character varying (100);
v_first_ch11 character varying (100);
v_first_ch12 character varying (100);
v_first_ch13 character varying (100);
v_first_ch14 character varying (100);
v_first_ch15 character varying (100);
v_first_ch16 character varying (100);
v_first_ch17 character varying (100);
v_first_ch18 character varying (100);
v_first_ch19 character varying (100);
v_first_ch20 character varying (100);
--
v_first_ch21 character varying (100);
v_first_ch22 character varying (100);
v_first_ch23 character varying (100);
v_first_ch24 character varying (100);
v_first_ch25 character varying (100);
v_first_ch26 character varying (100);
v_first_ch27 character varying (100);
v_first_ch28 character varying (100);
v_first_ch29 character varying (100);
v_first_ch30 character varying (100);
v_first_ch31 character varying (100);
v_first_ch32 character varying (100);
v_first_ch33 character varying (100);
v_first_ch34 character varying (100);
v_first_ch35 character varying (100);
v_first_ch36 character varying (100);
v_first_ch37 character varying (100);
v_first_ch38 character varying (100);
v_first_ch39 character varying (100);
v_first_ch40 character varying (100);
--
v_first_ch41 character varying (100);
v_first_ch42 character varying (100);
v_first_ch43 character varying (100);
v_first_ch44 character varying (100);
v_first_ch45 character varying (100);
v_first_ch46 character varying (100);
v_first_ch47 character varying (100);
v_first_ch48 character varying (100);
v_first_ch49 character varying (100);
v_first_ch50 character varying (100);
v_first_ch51 character varying (100);
v_first_ch52 character varying (100);
v_first_ch53 character varying (100);
v_first_ch54 character varying (100);
v_first_ch55 character varying (100);
v_first_ch56 character varying (100);
v_first_ch57 character varying (100);
v_first_ch58 character varying (100);
v_first_ch59 character varying (100);
v_first_ch60 character varying (100);

v_hexa_ch1 character varying (100);
v_hexa_ch2 character varying (100);
v_hexa_ch3 character varying (100);
v_hexa_ch4 character varying (100);
v_hexa_ch5 character varying (100);
v_hexa_ch6 character varying (100);
v_hexa_ch7 character varying (100);
v_hexa_ch8 character varying (100);
v_hexa_ch9 character varying (100);
v_hexa_ch10 character varying (100);
v_hexa_ch11 character varying (100);
v_hexa_ch12 character varying (100);
v_hexa_ch13 character varying (100);
v_hexa_ch14 character varying (100);
v_hexa_ch15 character varying (100);
v_hexa_ch16 character varying (100);
v_hexa_ch17 character varying (100);
v_hexa_ch18 character varying (100);
v_hexa_ch19 character varying (100);
v_hexa_ch20 character varying (100);
--
v_hexa_ch21 character varying (100);
v_hexa_ch22 character varying (100);
v_hexa_ch23 character varying (100);
v_hexa_ch24 character varying (100);
v_hexa_ch25 character varying (100);
v_hexa_ch26 character varying (100);
v_hexa_ch27 character varying (100);
v_hexa_ch28 character varying (100);
v_hexa_ch29 character varying (100);
v_hexa_ch30 character varying (100);
v_hexa_ch31 character varying (100);
v_hexa_ch32 character varying (100);
v_hexa_ch33 character varying (100);
v_hexa_ch34 character varying (100);
v_hexa_ch35 character varying (100);
v_hexa_ch36 character varying (100);
v_hexa_ch37 character varying (100);
v_hexa_ch38 character varying (100);
v_hexa_ch39 character varying (100);
v_hexa_ch40 character varying (100);
--
v_hexa_ch41 character varying (100);
v_hexa_ch42 character varying (100);
v_hexa_ch43 character varying (100);
v_hexa_ch44 character varying (100);
v_hexa_ch45 character varying (100);
v_hexa_ch46 character varying (100);
v_hexa_ch47 character varying (100);
v_hexa_ch48 character varying (100);
v_hexa_ch49 character varying (100);
v_hexa_ch50 character varying (100);
v_hexa_ch51 character varying (100);
v_hexa_ch52 character varying (100);
v_hexa_ch53 character varying (100);
v_hexa_ch54 character varying (100);
v_hexa_ch55 character varying (100);
v_hexa_ch56 character varying (100);
v_hexa_ch57 character varying (100);
v_hexa_ch58 character varying (100);
v_hexa_ch59 character varying (100);
v_hexa_ch60 character varying (100);

v_first_hexa_ch1 character varying (100);
v_first_hexa_ch2 character varying (100);
v_first_hexa_ch3 character varying (100);
v_first_hexa_ch4 character varying (100);
v_first_hexa_ch5 character varying (100);
v_first_hexa_ch6 character varying (100);
v_first_hexa_ch7 character varying (100);
v_first_hexa_ch8 character varying (100);
v_first_hexa_ch9 character varying (100);
v_first_hexa_ch10 character varying (100);
v_first_hexa_ch11 character varying (100);
v_first_hexa_ch12 character varying (100);
v_first_hexa_ch13 character varying (100);
v_first_hexa_ch14 character varying (100);
v_first_hexa_ch15 character varying (100);
v_first_hexa_ch16 character varying (100);
v_first_hexa_ch17 character varying (100);
v_first_hexa_ch18 character varying (100);
v_first_hexa_ch19 character varying (100);
v_first_hexa_ch20 character varying (100);
--
v_first_hexa_ch21 character varying (100);
v_first_hexa_ch22 character varying (100);
v_first_hexa_ch23 character varying (100);
v_first_hexa_ch24 character varying (100);
v_first_hexa_ch25 character varying (100);
v_first_hexa_ch26 character varying (100);
v_first_hexa_ch27 character varying (100);
v_first_hexa_ch28 character varying (100);
v_first_hexa_ch29 character varying (100);
v_first_hexa_ch30 character varying (100);
v_first_hexa_ch31 character varying (100);
v_first_hexa_ch32 character varying (100);
v_first_hexa_ch33 character varying (100);
v_first_hexa_ch34 character varying (100);
v_first_hexa_ch35 character varying (100);
v_first_hexa_ch36 character varying (100);
v_first_hexa_ch37 character varying (100);
v_first_hexa_ch38 character varying (100);
v_first_hexa_ch39 character varying (100);
v_first_hexa_ch40 character varying (100);
--
v_first_hexa_ch41 character varying (100);
v_first_hexa_ch42 character varying (100);
v_first_hexa_ch43 character varying (100);
v_first_hexa_ch44 character varying (100);
v_first_hexa_ch45 character varying (100);
v_first_hexa_ch46 character varying (100);
v_first_hexa_ch47 character varying (100);
v_first_hexa_ch48 character varying (100);
v_first_hexa_ch49 character varying (100);
v_first_hexa_ch50 character varying (100);
v_first_hexa_ch51 character varying (100);
v_first_hexa_ch52 character varying (100);
v_first_hexa_ch53 character varying (100);
v_first_hexa_ch54 character varying (100);
v_first_hexa_ch55 character varying (100);
v_first_hexa_ch56 character varying (100);
v_first_hexa_ch57 character varying (100);
v_first_hexa_ch58 character varying (100);
v_first_hexa_ch59 character varying (100);
v_first_hexa_ch60 character varying (100);

v_nom_voie 		 character varying (100);
v_first_nom_voie character varying (100);
v_lettre_voie	 character varying (2);
v_first_lettre_voie	character varying (2);
v_column_name_1	 character varying (100);
v_column_name_2	 character varying (100);

v_chaine_column_name    			text;
v_chaine_valeur_calc 				text;
v_first_hexa_chaine_valeur_calc     text;
v_hexa_chaine_valeur_calc			text;

v_first_chaine_column_name    		text;
v_first_chaine_valeur_calc 			text;
v_first_chaine_valeur_calc_k_100 	text;
v_first_chaine_valeur_calc_k_0 		text;

v_libelle_evenement character varying (100);
v_first_libelle_evenement character varying (100);
v_libelle_evenement_K100 character varying (100):='Modif 0 %';
v_libelle_evenement_K0 character varying (100):='Modif 100 %';
v_libelle_evenement_erreur character varying (100):='Défaut format';
v_libelle_evenement_table character varying (100):='';

v_lettre			character varying (2);
v_etat				character varying (100);
v_first_etat		character varying (100);

v_valeur	character varying (100);
v_nombre    integer;
v_seul    	integer;

c_compte_nb_ch_chaine_champ integer;
c_compte_nb_ch_chaine_valeur integer;
c_compte_nb_ch_chaine_valeur_hexa integer;

v_val_calc character varying (100);
v_now 	   character varying (100);

table_log_archivage character varying (100):='sh_pd.archives_releves_log';

i integer:=1;
j integer:=1;

ligne_to_insert integer:=0;
 
compteur_ligne_insert integer:=1;
nombre_ligne integer:=0;
v_nombre_ligne integer:=0;
v_count	integer:=0;

v_erreur boolean;

table_hexa_bin_etor			character varying (100);
table_hexa_bin_stor			character varying (100);
table_es_etor				character varying (100);
table_es_stor				character varying (100);

v_decode_bin				character varying (100);
								
CurRelEvETORO CURSOR
FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time, COALESCE(r.ordre,-99999) as ordre, COALESCE(r.evenement,'') as evenement,
								
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie, 					
								
								COALESCE(r.ch2,'') as ch2,COALESCE(r.ch3,'') as ch3,COALESCE(r.ch4,'') as ch4,COALESCE(r.ch5,'') as ch5,COALESCE(r.ch6,'') as ch6,
								COALESCE(r.ch7,'') as ch7,COALESCE(r.ch8,'') as ch8,COALESCE(r.ch9,'') as ch9,COALESCE(r.ch10,'') as ch10,COALESCE(r.ch11,'') as ch11,
								COALESCE(r.ch12,'') as ch12,COALESCE(r.ch13,'') as ch13,COALESCE(r.ch14,'') as ch14,COALESCE(r.ch15,'') as ch15,COALESCE(r.ch16,'') as ch16,
								COALESCE(r.ch17,'') as ch17,COALESCE(r.ch18,'') as ch18,COALESCE(r.ch19,'') as ch19,COALESCE(r.ch20,'') as ch20,COALESCE(r.ch21,'') as ch21,
								COALESCE(r.ch22,'') as ch22,COALESCE(r.ch23,'') as ch23,COALESCE(r.ch24,'') as ch24,COALESCE(r.ch25,'') as ch25,COALESCE(r.ch26,'') as ch26,
								COALESCE(r.ch27,'') as ch27,COALESCE(r.ch28,'') as ch28,COALESCE(r.ch29,'') as ch29,COALESCE(r.ch30,'') as ch30,COALESCE(r.ch31,'') as ch31,
								COALESCE(r.ch32,'') as ch32,COALESCE(r.ch33,'') as ch33,COALESCE(r.ch34,'') as ch34,COALESCE(r.ch35,'') as ch35,COALESCE(r.ch36,'') as ch36,
								COALESCE(r.ch37,'') as ch37,COALESCE(r.ch38,'') as ch38,COALESCE(r.ch39,'') as ch39,COALESCE(r.ch40,'') as ch40,COALESCE(r.ch41,'') as ch41,
								COALESCE(r.ch42,'') as ch42,COALESCE(r.ch43,'') as ch43,COALESCE(r.ch44,'') as ch44,COALESCE(r.ch45,'') as ch45,COALESCE(r.ch46,'') as ch46,
								COALESCE(r.ch47,'') as ch47,COALESCE(r.ch48,'') as ch48,COALESCE(r.ch49,'') as ch49,COALESCE(r.ch50,'') as ch50,COALESCE(r.ch51,'') as ch51,
								COALESCE(r.ch52,'') as ch52,COALESCE(r.ch53,'') as ch53,COALESCE(r.ch54,'') as ch54,COALESCE(r.ch55,'') as ch55,COALESCE(r.ch56,'') as ch56,
								COALESCE(r.ch57,'') as ch57,COALESCE(r.ch58,'') as ch58,COALESCE(r.ch59,'') as ch59,COALESCE(r.ch60,'') as ch60
								
					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('O') ORDER BY time asc, ordre asc, lettre_voie asc, nom_voie asc;
					   
CurRelEvETORR CURSOR
FOR SELECT distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time, COALESCE(r.ordre,-99999) as ordre, COALESCE(r.evenement,'') as evenement,
								
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie, 					
								
								COALESCE(r.ch2,'') as ch2,COALESCE(r.ch3,'') as ch3,COALESCE(r.ch4,'') as ch4,COALESCE(r.ch5,'') as ch5,COALESCE(r.ch6,'') as ch6,
								COALESCE(r.ch7,'') as ch7,COALESCE(r.ch8,'') as ch8,COALESCE(r.ch9,'') as ch9,COALESCE(r.ch10,'') as ch10,COALESCE(r.ch11,'') as ch11,
								COALESCE(r.ch12,'') as ch12,COALESCE(r.ch13,'') as ch13,COALESCE(r.ch14,'') as ch14,COALESCE(r.ch15,'') as ch15,COALESCE(r.ch16,'') as ch16,
								COALESCE(r.ch17,'') as ch17,COALESCE(r.ch18,'') as ch18,COALESCE(r.ch19,'') as ch19,COALESCE(r.ch20,'') as ch20,COALESCE(r.ch21,'') as ch21,
								COALESCE(r.ch22,'') as ch22,COALESCE(r.ch23,'') as ch23,COALESCE(r.ch24,'') as ch24,COALESCE(r.ch25,'') as ch25,COALESCE(r.ch26,'') as ch26,
								COALESCE(r.ch27,'') as ch27,COALESCE(r.ch28,'') as ch28,COALESCE(r.ch29,'') as ch29,COALESCE(r.ch30,'') as ch30,COALESCE(r.ch31,'') as ch31,
								COALESCE(r.ch32,'') as ch32,COALESCE(r.ch33,'') as ch33,COALESCE(r.ch34,'') as ch34,COALESCE(r.ch35,'') as ch35,COALESCE(r.ch36,'') as ch36,
								COALESCE(r.ch37,'') as ch37,COALESCE(r.ch38,'') as ch38,COALESCE(r.ch39,'') as ch39,COALESCE(r.ch40,'') as ch40,COALESCE(r.ch41,'') as ch41,
								COALESCE(r.ch42,'') as ch42,COALESCE(r.ch43,'') as ch43,COALESCE(r.ch44,'') as ch44,COALESCE(r.ch45,'') as ch45,COALESCE(r.ch46,'') as ch46,
								COALESCE(r.ch47,'') as ch47,COALESCE(r.ch48,'') as ch48,COALESCE(r.ch49,'') as ch49,COALESCE(r.ch50,'') as ch50,COALESCE(r.ch51,'') as ch51,
								COALESCE(r.ch52,'') as ch52,COALESCE(r.ch53,'') as ch53,COALESCE(r.ch54,'') as ch54,COALESCE(r.ch55,'') as ch55,COALESCE(r.ch56,'') as ch56,
								COALESCE(r.ch57,'') as ch57,COALESCE(r.ch58,'') as ch58,COALESCE(r.ch59,'') as ch59,COALESCE(r.ch60,'') as ch60
								
								
							
					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('R') ORDER BY time asc, ordre asc, lettre_voie asc, nom_voie asc;
					   
CurRelEvETORRS CURSOR
FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time,
								
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie, COALESCE(r.evenement,'') as evenement,
								
								COALESCE(r.ordre,-99999) as ordre,
								
								public.transforme_valeur_hexa_bin_chaine(';'||COALESCE(r.ch2,'')||';'||COALESCE(r.ch3,'')||';'||COALESCE(r.ch4,'')||';'||COALESCE(r.ch5,'')||';'||COALESCE(r.ch6,'')||';'||
								COALESCE(r.ch7,'')||';'||COALESCE(r.ch8,'')||';'||COALESCE(r.ch9,'')||';'||COALESCE(r.ch10,'')||';'||COALESCE(r.ch11,'')||';'||
								COALESCE(r.ch12,'')||';'||COALESCE(r.ch13,'')||';'||COALESCE(r.ch14,'')||';'||COALESCE(r.ch15,'')||';'||COALESCE(r.ch16,'')||';'||
								COALESCE(r.ch17,'')||';'||COALESCE(r.ch18,'')||';'||COALESCE(r.ch19,'')||';'||COALESCE(r.ch20,'')||';'||COALESCE(r.ch21,'')||';'||
								COALESCE(r.ch22,'')||';'||COALESCE(r.ch23,'')||';'||COALESCE(r.ch24,'')||';'||COALESCE(r.ch25,'')||';'||COALESCE(r.ch26,'')||';'||
								COALESCE(r.ch27,'')||';'||COALESCE(r.ch28,'')||';'||COALESCE(r.ch29,'')||';'||COALESCE(r.ch30,'')||';'||COALESCE(r.ch31,'')||';'||
								COALESCE(r.ch32,'')||';'||COALESCE(r.ch33,'')||';'||COALESCE(r.ch34,'')||';'||COALESCE(r.ch35,'')||';'||COALESCE(r.ch36,'')||';'||
								COALESCE(r.ch37,'')||';'||COALESCE(r.ch38,'')||';'||COALESCE(r.ch39,'')||';'||COALESCE(r.ch40,'')||';'||COALESCE(r.ch41,'')||';'||
								COALESCE(r.ch42,'')||';'||COALESCE(r.ch43,'')||';'||COALESCE(r.ch44,'')||';'||COALESCE(r.ch45,'')||';'||COALESCE(r.ch46,'')||';'||
								COALESCE(r.ch47,'')||';'||COALESCE(r.ch48,'')||';'||COALESCE(r.ch49,'')||';'||COALESCE(r.ch50,'')||';'||COALESCE(r.ch51,'')||';'||
								COALESCE(r.ch52,'')||';'||COALESCE(r.ch53,'')||';'||COALESCE(r.ch54,'')||';'||COALESCE(r.ch55,'')||';'||COALESCE(r.ch56,'')||';'||
								COALESCE(r.ch57,'')||';'||COALESCE(r.ch58,'')||';'||COALESCE(r.ch59,'')||';'||COALESCE(r.ch60,'')||';',COALESCE(t.lettre_voie,'')) as decode_bin
								
								
							
					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('R','S') ORDER BY time asc, lettre_voie asc, nom_voie asc;

CurRelEvETOROET CURSOR
FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time,
								
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie, COALESCE(r.evenement,'') as evenement,
								
								COALESCE(r.ordre,-99999) as ordre,
								
								public.transforme_valeur_hexa_bin_chaine(';'||COALESCE(r.ch2,'')||';'||COALESCE(r.ch3,'')||';'||COALESCE(r.ch4,'')||';'||COALESCE(r.ch5,'')||';'||COALESCE(r.ch6,'')||';'||
								COALESCE(r.ch7,'')||';'||COALESCE(r.ch8,'')||';'||COALESCE(r.ch9,'')||';'||COALESCE(r.ch10,'')||';'||COALESCE(r.ch11,'')||';'||
								COALESCE(r.ch12,'')||';'||COALESCE(r.ch13,'')||';'||COALESCE(r.ch14,'')||';'||COALESCE(r.ch15,'')||';'||COALESCE(r.ch16,'')||';'||
								COALESCE(r.ch17,'')||';'||COALESCE(r.ch18,'')||';'||COALESCE(r.ch19,'')||';'||COALESCE(r.ch20,'')||';'||COALESCE(r.ch21,'')||';'||
								COALESCE(r.ch22,'')||';'||COALESCE(r.ch23,'')||';'||COALESCE(r.ch24,'')||';'||COALESCE(r.ch25,'')||';'||COALESCE(r.ch26,'')||';'||
								COALESCE(r.ch27,'')||';'||COALESCE(r.ch28,'')||';'||COALESCE(r.ch29,'')||';'||COALESCE(r.ch30,'')||';'||COALESCE(r.ch31,'')||';'||
								COALESCE(r.ch32,'')||';'||COALESCE(r.ch33,'')||';'||COALESCE(r.ch34,'')||';'||COALESCE(r.ch35,'')||';'||COALESCE(r.ch36,'')||';'||
								COALESCE(r.ch37,'')||';'||COALESCE(r.ch38,'')||';'||COALESCE(r.ch39,'')||';'||COALESCE(r.ch40,'')||';'||COALESCE(r.ch41,'')||';'||
								COALESCE(r.ch42,'')||';'||COALESCE(r.ch43,'')||';'||COALESCE(r.ch44,'')||';'||COALESCE(r.ch45,'')||';'||COALESCE(r.ch46,'')||';'||
								COALESCE(r.ch47,'')||';'||COALESCE(r.ch48,'')||';'||COALESCE(r.ch49,'')||';'||COALESCE(r.ch50,'')||';'||COALESCE(r.ch51,'')||';'||
								COALESCE(r.ch52,'')||';'||COALESCE(r.ch53,'')||';'||COALESCE(r.ch54,'')||';'||COALESCE(r.ch55,'')||';'||COALESCE(r.ch56,'')||';'||
								COALESCE(r.ch57,'')||';'||COALESCE(r.ch58,'')||';'||COALESCE(r.ch59,'')||';'||COALESCE(r.ch60,'')||';',COALESCE(t.lettre_voie,'')) as decode_bin
								
								
							
					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('O','&') ORDER BY time asc,lettre_voie asc, nom_voie asc;


CurRelEvETORS1 CURSOR
FOR SELECT 	distinct COALESCE(r.fichier,'') as ffichier, COALESCE(r.time,'') as ttime,COALESCE(r.ordre,-99999) as oordre
					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('S') group by ffichier,ttime,oordre having count(*)=1;

CurRelEvETORSP CURSOR
FOR SELECT 	distinct COALESCE(r.fichier,'') as ffichier, COALESCE(r.time,'') as ttime,COALESCE(r.ordre,-99999) as oordre

					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('S') group by ffichier,ttime,oordre having count(*)>1;
					 					  
CurRelEvETORSL CURSOR(v_fichier character varying (100),v_time character varying (100),ordre integer)
FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time,
								
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie, COALESCE(r.ordre,-99999) as ordre,
								
																
								COALESCE(r.ch2,'') as ch2,COALESCE(r.ch3,'') as ch3,COALESCE(r.ch4,'') as ch4,COALESCE(r.ch5,'') as ch5,COALESCE(r.ch6,'') as ch6,
								COALESCE(r.ch7,'') as ch7,COALESCE(r.ch8,'') as ch8,COALESCE(r.ch9,'') as ch9,COALESCE(r.ch10,'') as ch10,COALESCE(r.ch11,'') as ch11,
								COALESCE(r.ch12,'') as ch12,COALESCE(r.ch13,'') as ch13,COALESCE(r.ch14,'') as ch14,COALESCE(r.ch15,'') as ch15,COALESCE(r.ch16,'') as ch16,
								COALESCE(r.ch17,'') as ch17,COALESCE(r.ch18,'') as ch18,COALESCE(r.ch19,'') as ch19,COALESCE(r.ch20,'') as ch20,COALESCE(r.ch21,'') as ch21,
								COALESCE(r.ch22,'') as ch22,COALESCE(r.ch23,'') as ch23,COALESCE(r.ch24,'') as ch24,COALESCE(r.ch25,'') as ch25,COALESCE(r.ch26,'') as ch26,
								COALESCE(r.ch27,'') as ch27,COALESCE(r.ch28,'') as ch28,COALESCE(r.ch29,'') as ch29,COALESCE(r.ch30,'') as ch30,COALESCE(r.ch31,'') as ch31,
								COALESCE(r.ch32,'') as ch32,COALESCE(r.ch33,'') as ch33,COALESCE(r.ch34,'') as ch34,COALESCE(r.ch35,'') as ch35,COALESCE(r.ch36,'') as ch36,
								COALESCE(r.ch37,'') as ch37,COALESCE(r.ch38,'') as ch38,COALESCE(r.ch39,'') as ch39,COALESCE(r.ch40,'') as ch40,COALESCE(r.ch41,'') as ch41,
								COALESCE(r.ch42,'') as ch42,COALESCE(r.ch43,'') as ch43,COALESCE(r.ch44,'') as ch44,COALESCE(r.ch45,'') as ch45,COALESCE(r.ch46,'') as ch46,
								COALESCE(r.ch47,'') as ch47,COALESCE(r.ch48,'') as ch48,COALESCE(r.ch49,'') as ch49,COALESCE(r.ch50,'') as ch50,COALESCE(r.ch51,'') as ch51,
								COALESCE(r.ch52,'') as ch52,COALESCE(r.ch53,'') as ch53,COALESCE(r.ch54,'') as ch54,COALESCE(r.ch55,'') as ch55,COALESCE(r.ch56,'') as ch56,
								COALESCE(r.ch57,'') as ch57,COALESCE(r.ch58,'') as ch58,COALESCE(r.ch59,'') as ch59,COALESCE(r.ch60,'') as ch60

					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
					   and      a.precision=t.precision
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,2)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,2)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,2)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,2)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,2)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,2)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,2)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,2)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,2)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,2)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,2)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,2)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,2)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,2)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,2)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,2)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,2)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,2)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,2)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,2)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,2)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,2)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,2)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,2)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,2)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,2)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,2)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,2)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,2)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,2)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,2)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,2)=a.lettre_voie)
                                    or
							   (r.ch34 is not null and substr(r.ch34,1,2)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,2)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,2)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,2)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,2)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,2)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,2)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,2)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,2)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,2)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,2)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,2)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,2)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,2)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,2)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,2)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,2)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,2)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,2)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,2)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,2)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,2)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,2)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,2)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,2)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,2)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,2)=a.lettre_voie)
	                         )
					   and   r.evenement in ('S') and r.fichier=v_fichier and r.time=v_time and r.ordre=v_ordre ORDER BY time asc,ordre asc,lettre_voie asc, nom_voie asc;

					   
BEGIN

	table_es_etor:='table_es_etor';
	
	BEGIN
	v_execute_1:='DROP TABLE table_es_etor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE table_es_etor as select  row_number() OVER () AS counter,num,ordre,libel from public.es_tor where type=1 and upper(ini)='||''''||upper(station)||''''||' order by num asc';
    
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_es_etor_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE UNIQUE INDEX sh_'||lower(station)||'_ind_table_es_etor_1
    ON table_es_etor USING btree
    (counter)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	table_es_etor:='table_es_stor';
	
	BEGIN
	v_execute_1:='DROP TABLE table_es_stor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE table_es_stor as select  row_number() OVER () AS counter,num,ordre,libel from public.es_tor where type=2 and upper(ini)='||''''||upper(station)||''''||' order by num asc';
    
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_es_stor_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE UNIQUE INDEX sh_'||lower(station)||'_ind_table_es_stor_1
    ON table_es_etor USING btree
    (counter)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	

    table_hexa_bin_etor:='table_hexa_bin_etor';

    BEGIN
	v_execute_1:='DROP TABLE table_hexa_bin_etor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE table_hexa_bin_etor
    (
			fichier character varying(100) COLLATE pg_catalog."default",
			time date,
			nom_voie character varying(100) COLLATE pg_catalog."default",
			lettre_voie character varying(10) COLLATE pg_catalog."default",
			evenement character varying(10) COLLATE pg_catalog."default",
			ordre integer,
			decode_bin character varying(100) COLLATE pg_catalog."default"
			) TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_hexa_bin_etor_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_hexa_bin_etor_1
    ON table_hexa_bin_etor USING btree
    (time, fichier COLLATE pg_catalog."default" ASC NULLS LAST, lettre_voie COLLATE pg_catalog."default" ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST, ordre)
	TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	/*BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_hexa_bin_etor_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_hexa_bin_etor_2
    ON table_hexa_bin_etor USING btree
    (time COLLATE pg_catalog."default" ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST, lettre_voie COLLATE pg_catalog."default" ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;*/
	

    table_hexa_bin_stor:='table_hexa_bin_stor';

    BEGIN
	v_execute_1:='DROP TABLE table_hexa_bin_stor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE table_hexa_bin_stor
    (
			fichier character varying(100) COLLATE pg_catalog."default",
			time date,
			nom_voie character varying(100) COLLATE pg_catalog."default",
			lettre_voie character varying(10) COLLATE pg_catalog."default",
			evenement character varying(10) COLLATE pg_catalog."default",
			ordre integer,
			decode_bin character varying(100) COLLATE pg_catalog."default"
			) TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_hexa_bin_stor_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_hexa_bin_stor_1
    ON table_hexa_bin_stor USING btree
    (time, fichier COLLATE pg_catalog."default" ASC NULLS LAST, lettre_voie COLLATE pg_catalog."default" ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST, ordre)
	TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	/*BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_hexa_bin_stor_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_hexa_bin_stor_2
    ON table_hexa_bin_stor USING btree
    (time COLLATE pg_catalog."default" ASC NULLS LAST, ordre, fichier COLLATE pg_catalog."default" ASC NULLS LAST, lettre_voie COLLATE pg_catalog."default" ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;*/
	
v_first_libelle_evenement:='Enrg TS chgt jour';
v_libelle_evenement:='Enrg TS chgt jour';


FOR CUR IN CurRelEvETORRS LOOP

v_fichier:=CUR.fichier;
v_time:=CUR.time;
v_nom_voie:=CUR.nom_voie;
v_lettre_voie:=CUR.lettre_voie;
v_decode_bin:=CUR.decode_bin;
v_evenement:=CUR.evenement;
v_ordre:=CUR.ordre;

v_time_date:=substr(v_time,1,10)::date;

v_execute_1:='insert into table_hexa_bin_etor (fichier,time,nom_voie,lettre_voie,decode_bin,evenement,ordre) 
			  values('||''''||v_fichier||''''||','||''''||v_time_date||''''||','||''''||v_nom_voie||''''||','||''''||v_lettre_voie||''''||','||''''||v_decode_bin||''''||','||''''||v_evenement||''''||','||v_ordre||')';

					
			--RAISE NOTICE 'v_execute_1 table_hexa_bin_etor=%',v_execute_1;
			
			IF NULLIF(v_fichier,'') IS NOT NULL and NULLIF(v_time,'') IS NOT NULL 
			and NULLIF(v_nom_voie,'') IS NOT NULL and NULLIF(v_lettre_voie,'') IS NOT NULL and NULLIF(v_lettre_voie,'') IS NOT NULL
			and NULLIF(v_decode_bin,'') IS NOT NULL
			THEN
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR72a dans %',v_execute_1;message_retour:='NOK72a';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK73a'; RAISE NOTICE 'ERREUR73a fichier % dans % - v_execute_1 %',v_fichier,v_execute_2,v_execute_1;message_retour:='NOK73a';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;
			
END LOOP;

FOR CUR IN CurRelEvETOROET LOOP

v_fichier:=CUR.fichier;
v_time:=CUR.time;
v_nom_voie:=CUR.nom_voie;
v_lettre_voie:=CUR.lettre_voie;
v_evenement:=CUR.evenement;
v_decode_bin:=CUR.decode_bin;
v_ordre:=CUR.ordre;

v_time_date:=substr(v_time,1,10)::date;

v_execute_1:='insert into table_hexa_bin_stor (fichier,time,nom_voie,lettre_voie,decode_bin,evenement,ordre) 
			  values('||''''||v_fichier||''''||','||''''||v_time_date||''''||','||''''||v_nom_voie||''''||','||''''||v_lettre_voie||''''||','||''''||v_decode_bin||''''||','||''''||v_evenement||''''||','||v_ordre||')';

					
			--RAISE NOTICE 'v_execute_1 table_hexa_bin_etor=%',v_execute_1;
			
			IF NULLIF(v_fichier,'') IS NOT NULL and NULLIF(v_time,'') IS NOT NULL 
			and NULLIF(v_nom_voie,'') IS NOT NULL and NULLIF(v_lettre_voie,'') IS NOT NULL and NULLIF(v_lettre_voie,'') IS NOT NULL
			and NULLIF(v_decode_bin,'') IS NOT NULL
			THEN
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR72b dans %',v_execute_1;message_retour:='NOK72b';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK73b'; RAISE NOTICE 'ERREUR73b fichier % dans % - v_execute_1 %',v_fichier,v_execute_2,v_execute_1;message_retour:='NOK73b';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;
			
END LOOP;

	BEGIN
	v_execute_1:='ANALYSE table_hexa_bin_stor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR140 dans %',v_execute_1;message_retour:='NOK140 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='ANALYSE table_hexa_bin_etor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR141 dans %',v_execute_1;message_retour:='NOK141 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='ANALYSE table_es_etor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR142 dans %',v_execute_1;message_retour:='NOK142 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='ANALYSE table_es_stor';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR143 dans %',v_execute_1;message_retour:='NOK143 : '||v_execute_1;
	END;

RAISE NOTICE 'AVANT LOOP S';

FOR CURS1 IN CurRelEvETORS1 LOOP

		v_fichier:=CURS1.ffichier;
		v_time:=CURS1.ttime;
		v_ordre:=CURS1.oordre;
		
		FOR CUR IN CurRelEvETORSL(v_fichier,v_time,v_ordre)
		LOOP
		
			v_time_date:=substr(v_time,1,10)::date;
			v_nom_voie:=CUR.nom_voie;
			v_lettre_voie:=CUR.lettre_voie;
		
			v_erreur:=false;

			v_ch2:=CUR.ch2;
			v_ch3:=CUR.ch3;
			v_ch4:=CUR.ch4;
			v_ch5:=CUR.ch5;
			v_ch6:=CUR.ch6;
			v_ch7:=CUR.ch7;
			v_ch8:=CUR.ch8;
			v_ch9:=CUR.ch9;
			v_ch10:=CUR.ch10;
			v_ch11:=CUR.ch11;
			v_ch12:=CUR.ch12;
			v_ch13:=CUR.ch13;
			v_ch14:=CUR.ch14;
			v_ch15:=CUR.ch15;
			v_ch16:=CUR.ch16;
			v_ch17:=CUR.ch17;
			v_ch18:=CUR.ch18;
			v_ch19:=CUR.ch19;
			v_ch20:=CUR.ch20;
			v_ch21:=CUR.ch21;
			v_ch22:=CUR.ch22;
			v_ch23:=CUR.ch23;
			v_ch24:=CUR.ch24;
			v_ch25:=CUR.ch25;
			v_ch26:=CUR.ch26;
			v_ch27:=CUR.ch27;
			v_ch28:=CUR.ch28;
			v_ch29:=CUR.ch29;
			v_ch30:=CUR.ch30;
			v_ch31:=CUR.ch31;
			v_ch32:=CUR.ch32;
			v_ch33:=CUR.ch33;
			v_ch34:=CUR.ch34;
			v_ch35:=CUR.ch35;
			v_ch36:=CUR.ch36;
			v_ch37:=CUR.ch37;
			v_ch38:=CUR.ch38;
			v_ch39:=CUR.ch39;
			v_ch40:=CUR.ch40;
			v_ch41:=CUR.ch41;
			v_ch42:=CUR.ch42;
			v_ch43:=CUR.ch43;
			v_ch44:=CUR.ch44;
			v_ch45:=CUR.ch45;
			v_ch46:=CUR.ch46;
			v_ch47:=CUR.ch47;
			v_ch48:=CUR.ch48;
			v_ch49:=CUR.ch49;
			v_ch50:=CUR.ch50;
			v_ch51:=CUR.ch51;
			v_ch52:=CUR.ch52;
			v_ch53:=CUR.ch53;
			v_ch54:=CUR.ch54;
			v_ch55:=CUR.ch55;
			v_ch56:=CUR.ch56;
			v_ch57:=CUR.ch57;
			v_ch58:=CUR.ch58;
			v_ch59:=CUR.ch59;
			v_ch60:=CUR.ch60;

			v_chaine_valeur_calc:=';'||v_ch2||';'||v_ch3||';'||v_ch4||';'||v_ch5||';'||v_ch6||';'||v_ch7||';'||v_ch8||';'||v_ch9||';'||v_ch10||';'||v_ch11||';'||
								  v_ch12||';'||v_ch13||';'||v_ch14||';'||v_ch15||';'||v_ch16||';'||v_ch17||';'||v_ch18||';'||v_ch19||';'||v_ch20||';'||v_ch21||';'||
								  v_ch22||';'||v_ch23||';'||v_ch24||';'||v_ch25||';'||v_ch26||';'||v_ch27||';'||v_ch28||';'||v_ch29||';'||v_ch30||';'||v_ch31||';'||
								  v_ch32||';'||v_ch33||';'||v_ch34||';'||v_ch35||';'||v_ch36||';'||v_ch37||';'||v_ch38||';'||v_ch39||';'||v_ch40||';'||v_ch41||';'||
								  v_ch42||';'||v_ch43||';'||v_ch44||';'||v_ch45||';'||v_ch46||';'||v_ch47||';'||v_ch48||';'||v_ch49||';'||v_ch50||';'||v_ch51||';'||
								  v_ch52||';'||v_ch53||';'||v_ch54||';'||v_ch55||';'||v_ch56||';'||v_ch57||';'||v_ch58||';'||v_ch59||';'||v_ch60||';';

			v_valeur:=substr(v_chaine_valeur_calc,POSITION(';'||v_lettre_voie IN v_chaine_valeur_calc)+1,7);
			--RAISE NOTICE 'v_valeur=%-ascii=%',v_valeur,ascii(substr(v_valeur,7,1));
			if ascii(substr(v_valeur,7,1)) between 97 and 122
			then
			v_erreur:=true;
			else
			v_valeur:=substr(v_valeur,1,6);
			end if;

			if not v_erreur
			then

				select public.calcul_etor(v_valeur,v_fichier,v_lettre_voie,v_nom_voie,
										  v_ordre,v_time_date)
				into v_libelle_evenement;
				
				v_etat:=substr(v_libelle_evenement,POSITION('#' IN v_libelle_evenement)+1,1);
				
				if POSITION('#' IN v_libelle_evenement) != 0
				then
				
					v_libelle_evenement:=substr(v_libelle_evenement,1,POSITION('#' IN v_libelle_evenement)-1);
					
					if v_libelle_evenement = '' then v_libelle_evenement:='#'; end if;
				
				end if;
									
				if v_etat not in ('0','1') and NULLIF(v_etat,'') IS NOT NULL
				then
						v_etat:=substr(v_valeur,2,5);
				elsif NULLIF(v_etat,'') IS NULL
				then
						v_etat:=substr(v_valeur,2,5);
				end if;
									
				if POSITION('ERREUR=' IN v_libelle_evenement)!=0 then v_libelle_evenement:=substr(v_libelle_evenement,POSITION('ERREUR=' IN v_libelle_evenement)+7,4); end if;
								

			else
				
				v_etat:=substr(v_valeur,2,5);
				v_libelle_evenement:='Chgt Etor ???';
				--RAISE NOTICE 'v_libelle_evenement=%',v_libelle_evenement;
				
			end if;

			--IF v_libelle_evenement='' THEN RAISE NOTICE 'v_libelle_evenement chaine vide v_valeur=%-v_fichier=%-v_lettre_voie=%-v_nom_voie=%-v_ordre=%-v_time_date=%',v_valeur,v_fichier,v_lettre_voie,v_nom_voie,v_ordre,v_time_date; END IF;
	
			--IF v_libelle_evenement IS NULL THEN RAISE NOTICE 'v_libelle_evenement chaine NULL  v_valeur=%-v_fichier=%-v_lettre_voie=%-v_nom_voie=%-v_ordre=%-v_time_date=%',v_valeur,v_fichier,v_lettre_voie,v_nom_voie,v_ordre,v_time_date; END IF;
			
			IF trim(v_libelle_evenement) = '#' THEN --RAISE NOTICE 'pas de changement etat';
			EXIT; END IF;
			
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,tor_affiche_ana,ordre,'||v_nom_voie||')'||
						 ' values('||''''||v_time||''''||','||''''||'S'||''''||','||''''||v_libelle_evenement||''''||',true,'||v_ordre||','||''''||v_etat||''''||')';
							
			IF NULLIF(v_nom_voie,'') IS NOT NULL and NULLIF(v_time,'') IS NOT NULL and NULLIF(v_etat,'') IS NOT NULL 
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR222 dans %',v_execute_1;message_retour:='NOK222';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;					 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_fichier||''''||','||''''||'cas 1 seul evenement TOR '||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK233'; RAISE NOTICE 'ERREUR233 fichier % dans % - v_execute_1 %',v_fichier,v_execute_2,v_execute_1;message_retour:='NOK233';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;
					
			if j > 10000 then j:=1; select CURRENT_TIMESTAMP::character varying into v_now; RAISE NOTICE '10000 lignes inserees % % % % % % % %',v_now,v_libelle_evenement,v_valeur,v_fichier,v_lettre_voie,v_nom_voie,v_ordre,v_time_date; j:=1; end if;

			j:=j+1;
		
		END LOOP;					

--if j > 10 then exit; end if;
		
END LOOP;

RAISE NOTICE 'APRES LOOP S';

RAISE NOTICE 'AVANT LOOP P';

v_nombre:=0;

FOR CURSP IN CurRelEvETORSP LOOP

		v_fichier:=CURSP.ffichier;
		v_time:=CURSP.ttime;
		v_ordre:=CURSP.oordre;
		
		v_nombre_ligne:=0;
		
		v_chaine_column_name:='';
		
		c_compte_nb_ch_chaine_valeur:=0;
		c_compte_nb_ch_chaine_champ:=0;
		
		v_nombre:=v_nombre+1;
		
		--if v_nombre > 10 then exit; end if;
		
		FOR CUR IN CurRelEvETORSL(v_fichier,v_time,v_ordre)
		LOOP
		
			v_time_date:=substr(v_time,1,10)::date;
			v_nom_voie:=CUR.nom_voie;
			
			c_compte_nb_ch_chaine_champ:=c_compte_nb_ch_chaine_champ+1;
			
			if v_chaine_column_name = '' then v_chaine_column_name:=v_nom_voie; else v_chaine_column_name:=v_chaine_column_name||','||v_nom_voie; end if;
			
			v_lettre_voie:=CUR.lettre_voie;
		
			v_erreur:=false;

			v_ch2:=CUR.ch2;
			v_ch3:=CUR.ch3;
			v_ch4:=CUR.ch4;
			v_ch5:=CUR.ch5;
			v_ch6:=CUR.ch6;
			v_ch7:=CUR.ch7;
			v_ch8:=CUR.ch8;
			v_ch9:=CUR.ch9;
			v_ch10:=CUR.ch10;
			v_ch11:=CUR.ch11;
			v_ch12:=CUR.ch12;
			v_ch13:=CUR.ch13;
			v_ch14:=CUR.ch14;
			v_ch15:=CUR.ch15;
			v_ch16:=CUR.ch16;
			v_ch17:=CUR.ch17;
			v_ch18:=CUR.ch18;
			v_ch19:=CUR.ch19;
			v_ch20:=CUR.ch20;
			v_ch21:=CUR.ch21;
			v_ch22:=CUR.ch22;
			v_ch23:=CUR.ch23;
			v_ch24:=CUR.ch24;
			v_ch25:=CUR.ch25;
			v_ch26:=CUR.ch26;
			v_ch27:=CUR.ch27;
			v_ch28:=CUR.ch28;
			v_ch29:=CUR.ch29;
			v_ch30:=CUR.ch30;
			v_ch31:=CUR.ch31;
			v_ch32:=CUR.ch32;
			v_ch33:=CUR.ch33;
			v_ch34:=CUR.ch34;
			v_ch35:=CUR.ch35;
			v_ch36:=CUR.ch36;
			v_ch37:=CUR.ch37;
			v_ch38:=CUR.ch38;
			v_ch39:=CUR.ch39;
			v_ch40:=CUR.ch40;
			v_ch41:=CUR.ch41;
			v_ch42:=CUR.ch42;
			v_ch43:=CUR.ch43;
			v_ch44:=CUR.ch44;
			v_ch45:=CUR.ch45;
			v_ch46:=CUR.ch46;
			v_ch47:=CUR.ch47;
			v_ch48:=CUR.ch48;
			v_ch49:=CUR.ch49;
			v_ch50:=CUR.ch50;
			v_ch51:=CUR.ch51;
			v_ch52:=CUR.ch52;
			v_ch53:=CUR.ch53;
			v_ch54:=CUR.ch54;
			v_ch55:=CUR.ch55;
			v_ch56:=CUR.ch56;
			v_ch57:=CUR.ch57;
			v_ch58:=CUR.ch58;
			v_ch59:=CUR.ch59;
			v_ch60:=CUR.ch60;

			v_chaine_valeur_calc:=';'||v_ch2||';'||v_ch3||';'||v_ch4||';'||v_ch5||';'||v_ch6||';'||v_ch7||';'||v_ch8||';'||v_ch9||';'||v_ch10||';'||v_ch11||';'||
								  v_ch12||';'||v_ch13||';'||v_ch14||';'||v_ch15||';'||v_ch16||';'||v_ch17||';'||v_ch18||';'||v_ch19||';'||v_ch20||';'||v_ch21||';'||
								  v_ch22||';'||v_ch23||';'||v_ch24||';'||v_ch25||';'||v_ch26||';'||v_ch27||';'||v_ch28||';'||v_ch29||';'||v_ch30||';'||v_ch31||';'||
								  v_ch32||';'||v_ch33||';'||v_ch34||';'||v_ch35||';'||v_ch36||';'||v_ch37||';'||v_ch38||';'||v_ch39||';'||v_ch40||';'||v_ch41||';'||
								  v_ch42||';'||v_ch43||';'||v_ch44||';'||v_ch45||';'||v_ch46||';'||v_ch47||';'||v_ch48||';'||v_ch49||';'||v_ch50||';'||v_ch51||';'||
								  v_ch52||';'||v_ch53||';'||v_ch54||';'||v_ch55||';'||v_ch56||';'||v_ch57||';'||v_ch58||';'||v_ch59||';'||v_ch60||';';

			v_valeur:=substr(v_chaine_valeur_calc,POSITION(';'||v_lettre_voie IN v_chaine_valeur_calc)+1,7);
			--RAISE NOTICE 'v_valeur=%-ascii=%',v_valeur,ascii(substr(v_valeur,7,1));
			if ascii(substr(v_valeur,7,1)) between 97 and 122
			then
			v_erreur:=true;
			--RAISE NOTICE 'v_valeur erreur=%-v_chaine_valeur_calc=%',v_valeur,v_chaine_valeur_calc;
			else
			v_valeur:=substr(v_valeur,1,6);
			--RAISE NOTICE 'v_valeur pas erreur=%-v_chaine_valeur_calc=%',v_valeur,v_chaine_valeur_calc;
			end if;

			if not v_erreur
			then

				select public.calcul_etor(v_valeur,v_fichier,v_lettre_voie,v_nom_voie,
										  v_ordre,v_time_date)
				into v_libelle_evenement;
				
				v_etat:=substr(v_libelle_evenement,POSITION('#' IN v_libelle_evenement)+1,1);
				
				if POSITION('#' IN v_libelle_evenement) != 0
				then
				
					v_libelle_evenement:=substr(v_libelle_evenement,1,POSITION('#' IN v_libelle_evenement)-1);
					
					if v_libelle_evenement = '' then v_libelle_evenement:='#'; end if;
				
				end if;
									
				if v_etat not in ('0','1') and NULLIF(v_etat,'') IS NOT NULL
				then
						v_etat:=substr(v_valeur,2,5);
				elsif NULLIF(v_etat,'') IS NULL
				then
						v_etat:=substr(v_valeur,2,5);
				end if;
									
				if POSITION('ERREUR=' IN v_libelle_evenement)!=0 then v_libelle_evenement:=substr(v_libelle_evenement,POSITION('ERREUR=' IN v_libelle_evenement)+7,4); end if;
								
				--RAISE NOTICE 'cas pas erreur v_libelle_evenement=% v_etat=%',v_libelle_evenement,v_etat;
			else
				
				v_etat:=substr(v_valeur,2,5);
				v_libelle_evenement:='Chgt Etor ???';
				--RAISE NOTICE 'cas erreur v_libelle_evenement=% v_etat=%',v_libelle_evenement,v_etat;
				
			end if;

			--IF v_libelle_evenement='' THEN RAISE NOTICE 'v_libelle_evenement chaine vide v_valeur=%-v_fichier=%-v_lettre_voie=%-v_nom_voie=%-v_ordre=%-v_time_date=%',v_valeur,v_fichier,v_lettre_voie,v_nom_voie,v_ordre,v_time_date; END IF;
	
			--IF v_libelle_evenement IS NULL THEN RAISE NOTICE 'v_libelle_evenement chaine NULL  v_valeur=%-v_fichier=%-v_lettre_voie=%-v_nom_voie=%-v_ordre=%-v_time_date=%',v_valeur,v_fichier,v_lettre_voie,v_nom_voie,v_ordre,v_time_date; END IF;
			
			if v_nombre_ligne = 0
			then 
				v_first_etat:=v_etat;
				v_nombre_ligne:=1;
				--RAISE NOTICE '1 v_nombre_ligne = 0 v_first_etat=%',v_first_etat;
			else 
				--RAISE NOTICE '1 v_nombre_ligne != 0 % v_first_etat=%',v_nombre_ligne,v_first_etat;
			end if;
			
			IF trim(v_libelle_evenement) != '#'
			THEN --RAISE NOTICE 'pas de changement etat';
			
			
			
					v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,tor_affiche_ana,ordre,'||v_nom_voie||')'||
								 ' values('||''''||v_time||''''||','||''''||'S'||''''||','||''''||v_libelle_evenement||''''||',false,'||v_ordre||','||''''||v_etat||''''||')';
								 	
					IF NULLIF(v_nom_voie,'') IS NOT NULL and NULLIF(v_time,'') IS NOT NULL and NULLIF(v_etat,'') IS NOT NULL 
					THEN
						i:=i+1;
						j:=j+1;
						BEGIN
						--RAISE NOTICE 'insert pour TOR commande = %',v_execute_1;		
						EXECUTE v_execute_1;
						EXCEPTION
						WHEN OTHERS THEN RAISE NOTICE 'ERREUR222a dans %',v_execute_1;message_retour:='NOK222a';
						--WHEN OTHERS THEN CONTINUE;
						END;
					ELSE
						i:=i+1;
						--RAISE NOTICE 'PB insert pour TOR commande = %',v_execute_1;					 
						v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_fichier||''''||','||''''||'cas plusieurs evenement TOR insert pour TOR '||replace(v_execute_1,'''','')||''''||')';
						BEGIN
						--RAISE NOTICE 'commande = %',v_execute_1;		
						EXECUTE v_execute_2;
						EXCEPTION
						WHEN OTHERS THEN message_retour:='NOK233a'; RAISE NOTICE 'ERREUR233 fichier % dans % - v_execute_1 %',v_fichier,v_execute_2,v_execute_1;message_retour:='NOK233a';
						--WHEN OTHERS THEN CONTINUE;
						END;
					END IF;
			
            END IF;
			
			if j > 10000 then j:=1; select CURRENT_TIMESTAMP::character varying into v_now; RAISE NOTICE '10000 lignes inserees % % % % % % % %',v_now,v_libelle_evenement,v_valeur,v_fichier,v_lettre_voie,v_nom_voie,v_ordre,v_time_date; j:=1; end if;

			j:=j+1;
		
		END LOOP;	
		
		v_first_ordre:=0;
		if NULLIF(v_ch2,'') is not null
		then
			 v_ch2:=substr(v_ch2,2,length(v_ch2)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=2; end if;
		else
			 v_ch2:='';
		end if;
		if NULLIF(v_ch3,'') is not null
		then
			 v_ch3:=substr(v_ch3,2,length(v_ch3)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=3; end if;
		else
			 v_ch3:='';
		end if;
		if NULLIF(v_ch4,'') is not null
		then
			 v_ch4:=substr(v_ch4,2,length(v_ch4)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=4; end if;
		else
			 v_ch4:='';
		end if;
		if NULLIF(v_ch5,'') is not null
		then
			 v_ch5:=substr(v_ch5,2,length(v_ch5)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=5; end if;
		else
			 v_ch5:='';
		end if;
		if NULLIF(v_ch6,'') is not null
		then
			 v_ch6:=substr(v_ch6,2,length(v_ch6)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=6; end if;
		else
			 v_ch6:='';
		end if;
		if NULLIF(v_ch7,'') is not null
		then
			 v_ch7:=substr(v_ch7,2,length(v_ch7)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=7; end if;
		else
			 v_ch7:='';
		end if;
		if NULLIF(v_ch8,'') is not null
		then
			 v_ch8:=substr(v_ch8,2,length(v_ch8)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=8; end if;
		else
			 v_ch8:='';
		end if;
		if NULLIF(v_ch9,'') is not null
		then
			 v_ch9:=substr(v_ch9,2,length(v_ch9)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=9; end if;
		else
			 v_ch9:='';
		end if;
		if NULLIF(v_ch10,'') is not null
		then
			 v_ch10:=substr(v_ch10,2,length(v_ch10)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=10; end if;
		else
			 v_ch10:='';
		end if;
		if NULLIF(v_ch11,'') is not null
		then
			 v_ch11:=substr(v_ch11,2,length(v_ch11)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=11; end if;
		else
			 v_ch11:='';
		end if;
		if NULLIF(v_ch12,'') is not null
		then
			 v_ch12:=substr(v_ch12,2,length(v_ch12)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=12; end if;
		else
			 v_ch12:='';
		end if;
		if NULLIF(v_ch13,'') is not null
		then
			 v_ch13:=substr(v_ch13,2,length(v_ch13)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=13; end if;
		else
			 v_ch13:='';
		end if;
		if NULLIF(v_ch14,'') is not null
		then
			 v_ch14:=substr(v_ch14,2,length(v_ch14)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=14; end if;
		else
			 v_ch14:='';
		end if;
		if NULLIF(v_ch15,'') is not null
		then
			 v_ch15:=substr(v_ch15,2,length(v_ch15)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=15; end if;
		else
			 v_ch15:='';
		end if;
		if NULLIF(v_ch16,'') is not null
		then
			 v_ch16:=substr(v_ch16,2,length(v_ch16)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=16; end if;
		else
			 v_ch16:='';
		end if;
		if NULLIF(v_ch17,'') is not null
		then
			 v_ch17:=substr(v_ch17,2,length(v_ch17)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=17; end if;
		else
			 v_ch17:='';
		end if;
		if NULLIF(v_ch18,'') is not null
		then
			 v_ch18:=substr(v_ch18,2,length(v_ch18)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=18; end if;
		else
			 v_ch18:='';
		end if;
		if NULLIF(v_ch19,'') is not null
		then
			 v_ch19:=substr(v_ch19,2,length(v_ch19)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=19; end if;
		else
			 v_ch19:='';
		end if;
		if NULLIF(v_ch20,'') is not null
		then
			 v_ch20:=substr(v_ch20,2,length(v_ch20)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=20; end if;
		else
			 v_ch20:='';
		end if;
		if NULLIF(v_ch21,'') is not null
		then
			 v_ch21:=substr(v_ch21,2,length(v_ch21)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=21; end if;
		else
			 v_ch21:='';
		end if;
		if NULLIF(v_ch22,'') is not null
		then
			 v_ch22:=substr(v_ch22,2,length(v_ch22)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=22; end if;
		else
			 v_ch22:='';
		end if;
		if NULLIF(v_ch23,'') is not null
		then
			 v_ch23:=substr(v_ch23,2,length(v_ch23)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=23; end if;
		else
			 v_ch23:='';
		end if;
		if NULLIF(v_ch24,'') is not null
		then
			 v_ch24:=substr(v_ch24,2,length(v_ch24)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=24; end if;
		else
			 v_ch24:='';
		end if;
		if NULLIF(v_ch25,'') is not null
		then
			 v_ch25:=substr(v_ch25,2,length(v_ch25)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=25; end if;
		else
			 v_ch25:='';
		end if;
		if NULLIF(v_ch26,'') is not null
		then
			 v_ch26:=substr(v_ch26,2,length(v_ch26)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=26; end if;
		else
			 v_ch26:='';
		end if;
		if NULLIF(v_ch27,'') is not null
		then
			 v_ch27:=substr(v_ch27,2,length(v_ch27)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=27; end if;
		else
			 v_ch27:='';
		end if;
		if NULLIF(v_ch28,'') is not null
		then
			 v_ch28:=substr(v_ch28,2,length(v_ch28)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=28; end if;
		else
			 v_ch28:='';
		end if;
		if NULLIF(v_ch29,'') is not null
		then
			 v_ch29:=substr(v_ch29,2,length(v_ch29)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=29; end if;
		else
			 v_ch29:='';
		end if;
		if NULLIF(v_ch30,'') is not null
		then
			 v_ch30:=substr(v_ch30,2,length(v_ch30)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=30; end if;
		else
			 v_ch30:='';
		end if;
		if NULLIF(v_ch31,'') is not null
		then
			 v_ch31:=substr(v_ch31,2,length(v_ch31)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=31; end if;
		else
			 v_ch31:='';
		end if;
		if NULLIF(v_ch32,'') is not null
		then
			 v_ch32:=substr(v_ch32,2,length(v_ch32)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=32; end if;
		else
			 v_ch32:='';
		end if;
		if NULLIF(v_ch33,'') is not null
		then
			 v_ch33:=substr(v_ch33,2,length(v_ch33)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=33; end if;
		else
			 v_ch33:='';
		end if;
		if NULLIF(v_ch34,'') is not null
		then
			 v_ch34:=substr(v_ch34,2,length(v_ch34)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=34; end if;
		else
			 v_ch34:='';
		end if;
		if NULLIF(v_ch35,'') is not null
		then
			 v_ch35:=substr(v_ch35,2,length(v_ch35)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=35; end if;
		else
			 v_ch35:='';
		end if;
		if NULLIF(v_ch36,'') is not null
		then
			 v_ch36:=substr(v_ch36,2,length(v_ch36)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=36; end if;
		else
			 v_ch36:='';
		end if;
		if NULLIF(v_ch37,'') is not null
		then
			 v_ch37:=substr(v_ch37,2,length(v_ch37)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=37; end if;
		else
			 v_ch37:='';
		end if;
		if NULLIF(v_ch38,'') is not null
		then
			 v_ch38:=substr(v_ch38,2,length(v_ch38)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=38; end if;
		else
			 v_ch38:='';
		end if;
		if NULLIF(v_ch39,'') is not null
		then
			 v_ch39:=substr(v_ch39,2,length(v_ch39)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=39; end if;
		else
			 v_ch39:='';
		end if;
		if NULLIF(v_ch40,'') is not null
		then
			 v_ch40:=substr(v_ch40,2,length(v_ch40)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=40; end if;
		else
			 v_ch40:='';
		end if;
		if NULLIF(v_ch41,'') is not null
		then
			 v_ch41:=substr(v_ch41,2,length(v_ch41)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=41; end if;
		else
			 v_ch41:='';
		end if;
		if NULLIF(v_ch42,'') is not null
		then
			 v_ch42:=substr(v_ch42,2,length(v_ch42)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=42; end if;
		else
			 v_ch42:='';
		end if;
		if NULLIF(v_ch43,'') is not null
		then
			 v_ch43:=substr(v_ch43,2,length(v_ch43)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=43; end if;
		else
			 v_ch43:='';
		end if;
		if NULLIF(v_ch44,'') is not null
		then
			 v_ch44:=substr(v_ch44,2,length(v_ch44)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=44; end if;
		else
			 v_ch44:='';
		end if;
		if NULLIF(v_ch45,'') is not null
		then
			 v_ch45:=substr(v_ch45,2,length(v_ch45)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=45; end if;
		else
			 v_ch45:='';
		end if;
		if NULLIF(v_ch46,'') is not null
		then
			 v_ch46:=substr(v_ch46,2,length(v_ch46)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=46; end if;
		else
			 v_ch46:='';
		end if;
		if NULLIF(v_ch47,'') is not null
		then
			 v_ch47:=substr(v_ch47,2,length(v_ch47)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=47; end if;
		else
			 v_ch47:='';
		end if;
		if NULLIF(v_ch48,'') is not null
		then
			 v_ch48:=substr(v_ch48,2,length(v_ch48)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=48; end if;
		else
			 v_ch48:='';
		end if;
		if NULLIF(v_ch49,'') is not null
		then
			 v_ch49:=substr(v_ch49,2,length(v_ch49)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=49; end if;
		else
			 v_ch49:='';
		end if;
		if NULLIF(v_ch50,'') is not null
		then
			 v_ch50:=substr(v_ch50,2,length(v_ch50)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=50; end if;
		else
			 v_ch50:='';
		end if;
		if NULLIF(v_ch51,'') is not null
		then
			 v_ch51:=substr(v_ch51,2,length(v_ch51)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=51; end if;
		else
			 v_ch51:='';
		end if;
		if NULLIF(v_ch52,'') is not null
		then
			 v_ch52:=substr(v_ch52,2,length(v_ch52)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=52; end if;
		else
			 v_ch52:='';
		end if;
		if NULLIF(v_ch53,'') is not null
		then
			 v_ch53:=substr(v_ch53,2,length(v_ch53)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=53; end if;
		else
			 v_ch53:='';
		end if;
		if NULLIF(v_ch54,'') is not null
		then
			 v_ch54:=substr(v_ch54,2,length(v_ch54)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=54; end if;
		else
			 v_ch54:='';
		end if;
		if NULLIF(v_ch55,'') is not null
		then
			 v_ch55:=substr(v_ch55,2,length(v_ch55)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=55; end if;
		else
			 v_ch55:='';
		end if;
		if NULLIF(v_ch56,'') is not null
		then
			 v_ch56:=substr(v_ch56,2,length(v_ch56)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=56; end if;
		else
			 v_ch56:='';
		end if;
		if NULLIF(v_ch57,'') is not null
		then
			 v_ch57:=substr(v_ch57,2,length(v_ch57)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=57; end if;
		else
			 v_ch57:='';
		end if;
		if NULLIF(v_ch58,'') is not null
		then
			 v_ch58:=substr(v_ch58,2,length(v_ch58)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=58; end if;
		else
			 v_ch58:='';
		end if;
		if NULLIF(v_ch59,'') is not null
		then
			 v_ch59:=substr(v_ch59,2,length(v_ch59)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=59; end if;
		else
			 v_ch59:='';
		end if;
		if NULLIF(v_ch60,'') is not null
		then
			 v_ch60:=substr(v_ch60,2,length(v_ch60)-1)||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			 if v_first_ordre= 0 then v_first_ordre:=60; end if;
		else
			 v_ch60:='';
		end if;

		
		v_chaine_valeur_calc:=v_ch2||v_ch3||v_ch4||v_ch5||v_ch6||v_ch7||v_ch8||v_ch9||v_ch10||v_ch11||v_ch12||v_ch13||
							  v_ch14||v_ch15||v_ch16||v_ch17||v_ch18||v_ch19||v_ch20||v_ch21||v_ch22||v_ch23||v_ch24||
							  v_ch25||v_ch26||v_ch27||v_ch28||v_ch29||v_ch30||v_ch31||v_ch32||v_ch33||v_ch34||v_ch35||
							  v_ch36||v_ch37||v_ch38||v_ch39||v_ch40||v_ch41||v_ch42||v_ch43||v_ch44||v_ch45||v_ch46||
							  v_ch47||v_ch48||v_ch49||v_ch50||v_ch51||v_ch52||v_ch53||v_ch54||v_ch55||v_ch56||v_ch57||
							  v_ch58||v_ch59||v_ch60;
							  
		--RAISE NOTICE '2 v_chaine_valeur_calc=%-v_first_ordre=%-v_first_etat=%',v_chaine_valeur_calc,v_first_ordre,v_first_etat;
		
		v_first_etat:=v_first_etat||',';
		
		if  v_first_ordre =  2  then
			--RAISE NOTICE '2 v_first_ordre =  2 v_ch2=% v_chaine_valeur_calc avant replace=%-v_first_etat=%',v_ch2,v_chaine_valeur_calc,v_first_etat;
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch2,v_first_etat);
			--RAISE NOTICE '2 v_first_ordre =  2 v_ch2=% v_chaine_valeur_calc apres replace=%-v_first_etat=%',v_ch2,v_chaine_valeur_calc,v_first_etat;
		elsif  v_first_ordre =  3  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch3,v_first_etat);
		elsif  v_first_ordre =  4  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch4,v_first_etat);
		elsif  v_first_ordre =  5  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch5,v_first_etat);
		elsif  v_first_ordre =  6  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch6,v_first_etat);
		elsif  v_first_ordre =  7  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch7,v_first_etat);
		elsif  v_first_ordre =  8  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch8,v_first_etat);
		elsif  v_first_ordre =  9  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch9,v_first_etat);
		elsif  v_first_ordre =  10  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch10,v_first_etat);
		elsif  v_first_ordre =  11  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch11,v_first_etat);
		elsif  v_first_ordre =  12  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch12,v_first_etat);
		elsif  v_first_ordre =  13  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch13,v_first_etat);
		elsif  v_first_ordre =  14  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch14,v_first_etat);
		elsif  v_first_ordre =  15  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch15,v_first_etat);
		elsif  v_first_ordre =  16  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch16,v_first_etat);
		elsif  v_first_ordre =  17  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch17,v_first_etat);
		elsif  v_first_ordre =  18  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch18,v_first_etat);
		elsif  v_first_ordre =  19  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch19,v_first_etat);
		elsif  v_first_ordre =  20  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch20,v_first_etat);
		elsif  v_first_ordre =  21  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch21,v_first_etat);
		elsif  v_first_ordre =  22  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch22,v_first_etat);
		elsif  v_first_ordre =  23  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch23,v_first_etat);
		elsif  v_first_ordre =  24  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch24,v_first_etat);
		elsif  v_first_ordre =  25  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch25,v_first_etat);
		elsif  v_first_ordre =  26  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch26,v_first_etat);
		elsif  v_first_ordre =  27  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch27,v_first_etat);
		elsif  v_first_ordre =  28  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch28,v_first_etat);
		elsif  v_first_ordre =  29  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch29,v_first_etat);
		elsif  v_first_ordre =  30  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch30,v_first_etat);
		elsif  v_first_ordre =  31  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch31,v_first_etat);
		elsif  v_first_ordre =  32  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch32,v_first_etat);
		elsif  v_first_ordre =  33  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch33,v_first_etat);
		elsif  v_first_ordre =  34  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch34,v_first_etat);
		elsif  v_first_ordre =  35  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch35,v_first_etat);
		elsif  v_first_ordre =  36  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch36,v_first_etat);
		elsif  v_first_ordre =  37  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch37,v_first_etat);
		elsif  v_first_ordre =  38  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch38,v_first_etat);
		elsif  v_first_ordre =  39  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch39,v_first_etat);
		elsif  v_first_ordre =  40  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch40,v_first_etat);
		elsif  v_first_ordre =  41  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch41,v_first_etat);
		elsif  v_first_ordre =  42  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch42,v_first_etat);
		elsif  v_first_ordre =  43  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch43,v_first_etat);
		elsif  v_first_ordre =  44  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch44,v_first_etat);
		elsif  v_first_ordre =  45  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch45,v_first_etat);
		elsif  v_first_ordre =  46  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch46,v_first_etat);
		elsif  v_first_ordre =  47  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch47,v_first_etat);
		elsif  v_first_ordre =  48  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch48,v_first_etat);
		elsif  v_first_ordre =  49  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch49,v_first_etat);
		elsif  v_first_ordre =  50  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch50,v_first_etat);
		elsif  v_first_ordre =  51  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch51,v_first_etat);
		elsif  v_first_ordre =  52  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch52,v_first_etat);
		elsif  v_first_ordre =  53  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch53,v_first_etat);
		elsif  v_first_ordre =  54  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch54,v_first_etat);
		elsif  v_first_ordre =  55  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch55,v_first_etat);
		elsif  v_first_ordre =  56  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch56,v_first_etat);
		elsif  v_first_ordre =  57  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch57,v_first_etat);
		elsif  v_first_ordre =  58  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch58,v_first_etat);
		elsif  v_first_ordre =  59  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch59,v_first_etat);
		elsif  v_first_ordre =  60  then
			v_chaine_valeur_calc:=replace(v_chaine_valeur_calc,v_ch60,v_first_etat);
		end if;

		--RAISE NOTICE '3 v_chaine_valeur_calc replace % v_first_etat=%',v_chaine_valeur_calc,v_first_etat;
		
		v_chaine_valeur_calc:=replace(substr(v_chaine_valeur_calc,1,length(v_chaine_valeur_calc)-1),',',''',''');
		
		--RAISE NOTICE 'v_chaine_valeur_calc replace , par cote % c_compte_nb_ch_chaine_champ=% - c_compte_nb_ch_chaine_valeur=%',v_chaine_valeur_calc,c_compte_nb_ch_chaine_champ,c_compte_nb_ch_chaine_valeur;
		
		v_libelle_evenement:='chgt plusieurs Etor';
		
		v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,tor_affiche_ana,ordre,'||v_chaine_column_name||')'||
					 ' values('||''''||v_time||''''||','||''''||'S'||''''||','||''''||v_libelle_evenement||''''||',true,'||v_ordre||','||''''||v_chaine_valeur_calc||''''||')';
							
		--RAISE NOTICE 'v_execute_1 %',v_execute_1;					
		IF NULLIF(v_chaine_column_name,'') IS NOT NULL and NULLIF(v_time,'') IS NOT NULL and c_compte_nb_ch_chaine_champ = c_compte_nb_ch_chaine_valeur
		and  NULLIF(v_libelle_evenement,'') IS NOT NULL and NULLIF(v_chaine_valeur_calc,'') IS NOT NULL
		THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR222c dans %',v_execute_1;message_retour:='NOK222b';
				--WHEN OTHERS THEN CONTINUE;
				END;
		ELSE
				i:=i+1;
				RAISE NOTICE 'cas plusieurs evenement TOR insert pour ANA v_chaine_column_name=%-v_chaine_valeur_calc=%-c_compte_nb_ch_chaine_champ=%-c_compte_nb_ch_chaine_valeur=%',v_chaine_column_name,v_chaine_valeur_calc,c_compte_nb_ch_chaine_champ,c_compte_nb_ch_chaine_valeur;					 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_fichier||''''||','||''''||'cas plusieurs evenement TOR insert pour ANA '||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK233c'; RAISE NOTICE 'ERREUR233c fichier % dans % - v_execute_1 %',v_fichier,v_execute_2,v_execute_1;
				--WHEN OTHERS THEN CONTINUE;
				END;
		END IF;
					
		if j > 10000 then j:=1; select CURRENT_TIMESTAMP::character varying into v_now; RAISE NOTICE '10000 lignes inserees %',v_now; j:=1; end if;

		j:=j+1;
		

--if j > 10 then exit; end if;
		
END LOOP;

RAISE NOTICE 'APRES LOOP P';

RAISE NOTICE 'avant loop O';

v_first_chaine_column_name:='';
v_first_chaine_valeur_calc:='';
ligne_to_insert:=0;

c_compte_nb_ch_chaine_champ:=0;
c_compte_nb_ch_chaine_valeur:=0;
								
OPEN CurRelEvETORO;
FETCH FIRST FROM CurRelEvETORO INTO   v_first_fichier,v_first_time,v_first_ordre,v_first_evenement,v_first_nom_voie,v_first_lettre_voie,v_first_ch2,v_first_ch3,v_first_ch4,v_first_ch5,v_first_ch6,v_first_ch7,v_first_ch8,v_first_ch9,v_first_ch10,v_first_ch11,v_first_ch12,v_first_ch13,v_first_ch14,
									  v_first_ch15,v_first_ch16,v_first_ch17,v_first_ch18,v_first_ch19,v_first_ch20,v_first_ch21,v_first_ch22,v_first_ch23,v_first_ch24,v_first_ch25,v_first_ch26,v_first_ch27,v_first_ch28,v_first_ch29,v_first_ch30,
									  v_first_ch31,v_first_ch32,v_first_ch33,v_first_ch34,v_first_ch35,v_first_ch36,v_first_ch37,v_first_ch38,v_first_ch39,v_first_ch40,v_first_ch41,v_first_ch42,v_first_ch43,v_first_ch44,v_first_ch45,v_first_ch46,
									  v_first_ch47,v_first_ch48,v_first_ch49,v_first_ch50,v_first_ch51,v_first_ch52,v_first_ch53,v_first_ch54,v_first_ch55,v_first_ch56,v_first_ch57,v_first_ch58,v_first_ch59,v_first_ch60;

	v_first_chaine_column_name:=v_first_nom_voie;

	if NULLIF(v_first_ch2,'') is not null
	then
		 v_first_ch2:=v_first_ch2||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		 v_first_ch2:='';
	end if;
	if NULLIF(v_first_ch3,'') is not null
	then
		v_first_ch3:=v_first_ch3||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch3:='';
	end if;
	if NULLIF(v_first_ch4,'') is not null
	then
		v_first_ch4:=v_first_ch4||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch4:='';
	end if;
	if NULLIF(v_first_ch5,'') is not null
	then
		v_first_ch5:=v_first_ch5||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch5:='';
	end if;
	if NULLIF(v_first_ch6,'') is not null
	then
		v_first_ch6:=v_first_ch6||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch6:='';
	end if;
	if NULLIF(v_first_ch7,'') is not null
	then
		v_first_ch7:=v_first_ch7||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch7:='';
	end if;
	if NULLIF(v_first_ch8,'') is not null
	then
		v_first_ch8:=v_first_ch8||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch8:='';
	end if;
	if NULLIF(v_first_ch9,'') is not null
	then
		v_first_ch9:=v_first_ch9||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch9:='';
	end if;
	if NULLIF(v_first_ch10,'') is not null
	then
		v_first_ch10:=v_first_ch10||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch10:='';
	end if;
	if NULLIF(v_first_ch11,'') is not null
	then
		v_first_ch11:=v_first_ch11||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch11:='';
	end if;
	if NULLIF(v_first_ch12,'') is not null
	then
		v_first_ch12:=v_first_ch12||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch12:='';
	end if;
	if NULLIF(v_first_ch13,'') is not null
	then
		v_first_ch13:=v_first_ch13||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch13:='';
	end if;
	if NULLIF(v_first_ch14,'') is not null
	then
		v_first_ch14:=v_first_ch14||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch14:='';
	end if;
	if NULLIF(v_first_ch15,'') is not null
	then
		v_first_ch15:=v_first_ch15||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch15:='';
	end if;
	if NULLIF(v_first_ch16,'') is not null
	then
		v_first_ch16:=v_first_ch16||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch16:='';
	end if;
	if NULLIF(v_first_ch17,'') is not null
	then
		v_first_ch17:=v_first_ch17||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch17:='';
	end if;
	if NULLIF(v_first_ch18,'') is not null
	then
		v_first_ch18:=v_first_ch18||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch18:='';
	end if;
	if NULLIF(v_first_ch19,'') is not null
	then
		v_first_ch19:=v_first_ch19||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch19:='';
	end if;
	if NULLIF(v_first_ch20,'') is not null
	then
		v_first_ch20:=v_first_ch20||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch20:='';
	end if;
	if NULLIF(v_first_ch21,'') is not null
	then
		v_first_ch21:=v_first_ch21||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch21:='';
	end if;
	if NULLIF(v_first_ch22,'') is not null
	then
		v_first_ch22:=v_first_ch22||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch22:='';
	end if;
	if NULLIF(v_first_ch23,'') is not null
	then
		v_first_ch23:=v_first_ch23||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch23:='';
	end if;
	if NULLIF(v_first_ch24,'') is not null
	then
		v_first_ch24:=v_first_ch24||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch24:='';
	end if;
	if NULLIF(v_first_ch25,'') is not null
	then
		v_first_ch25:=v_first_ch25||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch25:='';
	end if;
	if NULLIF(v_first_ch26,'') is not null
	then
		v_first_ch26:=v_first_ch26||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch26:='';
	end if;
	if NULLIF(v_first_ch27,'') is not null
	then
		v_first_ch27:=v_first_ch27||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch27:='';
	end if;
	if NULLIF(v_first_ch28,'') is not null
	then
		v_first_ch28:=v_first_ch28||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch28:='';
	end if;
	if NULLIF(v_first_ch29,'') is not null
	then
		v_first_ch29:=v_first_ch29||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch29:='';
	end if;
	if NULLIF(v_first_ch30,'') is not null
	then
		v_first_ch30:=v_first_ch30||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch30:='';
	end if;
	if NULLIF(v_first_ch31,'') is not null
	then
		v_first_ch31:=v_first_ch31||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch31:='';
	end if;
	if NULLIF(v_first_ch32,'') is not null
	then
		v_first_ch32:=v_first_ch32||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch32:='';
	end if;
	if NULLIF(v_first_ch33,'') is not null
	then
		v_first_ch33:=v_first_ch33||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch33:='';
	end if;
	if NULLIF(v_first_ch34,'') is not null
	then
		v_first_ch34:=v_first_ch34||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch34:='';
	end if;
	if NULLIF(v_first_ch35,'') is not null
	then
		v_first_ch35:=v_first_ch35||',';
	else
		v_first_ch35:='';
	end if;
	if NULLIF(v_first_ch36,'') is not null
	then
		v_first_ch36:=v_first_ch36||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch36:='';
	end if;
	if NULLIF(v_first_ch37,'') is not null
	then
		v_first_ch37:=v_first_ch37||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch37:='';
	end if;
	if NULLIF(v_first_ch38,'') is not null
	then
		v_first_ch38:=v_first_ch38||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch38:='';
	end if;
	if NULLIF(v_first_ch39,'') is not null
	then
		v_first_ch39:=v_first_ch39||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch39:='';
	end if;
	if NULLIF(v_first_ch40,'') is not null
	then
		v_first_ch40:=v_first_ch40||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch40:='';
	end if;
	if NULLIF(v_first_ch41,'') is not null
	then
		v_first_ch41:=v_first_ch41||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch41:='';
	end if;
	if NULLIF(v_first_ch42,'') is not null
	then
		v_first_ch42:=v_first_ch42||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch42:='';
	end if;
	if NULLIF(v_first_ch43,'') is not null
	then
		v_first_ch43:=v_first_ch43||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch43:='';
	end if;
	if NULLIF(v_first_ch44,'') is not null
	then
		v_first_ch44:=v_first_ch44||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch44:='';
	end if;
	if NULLIF(v_first_ch45,'') is not null
	then
		v_first_ch45:=v_first_ch45||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch45:='';
	end if;
	if NULLIF(v_first_ch46,'') is not null
	then
		v_first_ch46:=v_first_ch46||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch46:='';
	end if;
	if NULLIF(v_first_ch47,'') is not null
	then
		v_first_ch47:=v_first_ch47||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch47:='';
	end if;
	if NULLIF(v_first_ch48,'') is not null
	then
		v_first_ch48:=v_first_ch48||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch48:='';
	end if;
	if NULLIF(v_first_ch49,'') is not null
	then
		v_first_ch49:=v_first_ch49||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch49:='';
	end if;
	if NULLIF(v_first_ch50,'') is not null
	then
		v_first_ch50:=v_first_ch50||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch50:='';
	end if;
	if NULLIF(v_first_ch51,'') is not null
	then
		v_first_ch51:=v_first_ch51||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch51:='';
	end if;
	if NULLIF(v_first_ch52,'') is not null
	then
		v_first_ch52:=v_first_ch52||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch52:='';
	end if;
	if NULLIF(v_first_ch53,'') is not null
	then
		v_first_ch53:=v_first_ch53||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch53:='';
	end if;
	if NULLIF(v_first_ch54,'') is not null
	then
		v_first_ch54:=v_first_ch54||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch54:='';
	end if;
	if NULLIF(v_first_ch55,'') is not null
	then
		v_first_ch55:=v_first_ch55||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch55:='';
	end if;
	if NULLIF(v_first_ch56,'') is not null
	then
		v_first_ch56:=v_first_ch56||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch56:='';
	end if;
	if NULLIF(v_first_ch57,'') is not null
	then
		v_first_ch57:=v_first_ch57||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch57:='';
	end if;
	if NULLIF(v_first_ch58,'') is not null
	then
		v_first_ch58:=v_first_ch58||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch58:='';
	end if;
	if NULLIF(v_first_ch59,'') is not null
	then
		v_first_ch59:=v_first_ch59||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch59:='';
	end if;
	if NULLIF(v_first_ch60,'') is not null
	then
		v_first_ch60:=v_first_ch60||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch60:='';
	end if;
	
	
	v_first_chaine_valeur_calc:=v_first_ch2||v_first_ch3||v_first_ch4||v_first_ch5||v_first_ch6||v_first_ch7||v_first_ch8||v_first_ch9||v_first_ch10||v_first_ch11||v_first_ch12||v_first_ch13||
								v_first_ch14||v_first_ch15||v_first_ch16||v_first_ch17||v_first_ch18||v_first_ch19||v_first_ch20||v_first_ch21||v_first_ch22||v_first_ch23||v_first_ch24||
								v_first_ch25||v_first_ch26||v_first_ch27||v_first_ch28||v_first_ch29||v_first_ch30||v_first_ch31||v_first_ch32||v_first_ch33||v_first_ch34||v_first_ch35||
								v_first_ch36||v_first_ch37||v_first_ch38||v_first_ch39||v_first_ch40||v_first_ch41||v_first_ch42||v_first_ch43||v_first_ch44||v_first_ch45||v_first_ch46||
								v_first_ch47||v_first_ch48||v_first_ch49||v_first_ch50||v_first_ch51||v_first_ch52||v_first_ch53||v_first_ch54||v_first_ch55||v_first_ch56||v_first_ch57||
								v_first_ch58||v_first_ch59||v_first_ch60;
						  
	--RAISE NOTICE 'v_column_name %-%',v_first_chaine_valeur_calc,v_first_ch2;
	
v_chaine_column_name:=v_first_chaine_column_name;
v_chaine_valeur_calc:='';
ligne_to_insert:=0;
c_compte_nb_ch_chaine_champ:=1;

v_first_libelle_evenement:='Enrg TC chgt jour';


LOOP
EXIT WHEN NOT FOUND;
FETCH NEXT FROM CurRelEvETORO INTO    v_fichier,v_time,v_ordre,v_evenement,v_nom_voie,v_lettre_voie,v_ch2,v_ch3,v_ch4,v_ch5,v_ch6,v_ch7,v_ch8,v_ch9,v_ch10,v_ch11,v_ch12,v_ch13,v_ch14,
									  v_ch15,v_ch16,v_ch17,v_ch18,v_ch19,v_ch20,v_ch21,v_ch22,v_ch23,v_ch24,v_ch25,v_ch26,v_ch27,v_ch28,v_ch29,v_ch30,
									  v_ch31,v_ch32,v_ch33,v_ch34,v_ch35,v_ch36,v_ch37,v_ch38,v_ch39,v_ch40,v_ch41,v_ch42,v_ch43,v_ch44,v_ch45,v_ch46,
									  v_ch47,v_ch48,v_ch49,v_ch50,v_ch51,v_ch52,v_ch53,v_ch54,v_ch55,v_ch56,v_ch57,v_ch58,v_ch59,v_ch60;


if NULLIF(v_time,'') IS NULL THEN EXIT; END IF;

v_libelle_evenement:=v_first_libelle_evenement;

--RAISE NOTICE 'v_time=%-v_first_time=%-v_ordre=%-v_first_ordre=%-v_evenement=%-v_first_evenement=%-v_fichier=%-v_first_fichier=%',v_time,v_first_time,v_ordre,v_first_ordre,v_evenement,v_first_evenement,v_fichier,v_first_fichier;

--if i > 10000 then exit; end if;
if j > 100000 then j:=1; RAISE NOTICE '100000 lignes inserees %',now(); j:=1; end if;

	if v_time = v_first_time and v_ordre = v_first_ordre and v_evenement=v_first_evenement and v_fichier=v_first_fichier
	then
			if POSITION(v_nom_voie IN v_first_chaine_column_name)=0
			then
				v_first_chaine_column_name:=v_first_chaine_column_name||','||v_nom_voie;
				ligne_to_insert:=0;
				c_compte_nb_ch_chaine_champ:=c_compte_nb_ch_chaine_champ+1;
				
				if NULLIF(v_ch2,'') is not null
				then
					 v_first_ch2:=v_ch2;
				else
					 v_first_ch2:='';
				end if;
				if NULLIF(v_ch3,'') is not null
				then
					 v_first_ch3:=v_ch3;
				else
					 v_first_ch3:='';
				end if;
				if NULLIF(v_ch4,'') is not null
				then
					 v_first_ch4:=v_ch4;
				else
					 v_first_ch4:='';
				end if;
				if NULLIF(v_ch5,'') is not null
				then
					 v_first_ch5:=v_ch5;
				else
					 v_first_ch5:='';
				end if;
				if NULLIF(v_ch6,'') is not null
				then
					 v_first_ch6:=v_ch6;
				else
					 v_first_ch6:='';
				end if;
				if NULLIF(v_ch7,'') is not null
				then
					 v_first_ch7:=v_ch7;
				else
					 v_first_ch7:='';
				end if;
				if NULLIF(v_ch8,'') is not null
				then
					 v_first_ch8:=v_ch8;
				else
					 v_first_ch8:='';
				end if;
				if NULLIF(v_ch9,'') is not null
				then
					 v_first_ch9:=v_ch9;
				else
					 v_first_ch9:='';
				end if;
				if NULLIF(v_ch10,'') is not null
				then
					 v_first_ch10:=v_ch10;
				else
					 v_first_ch10:='';
				end if;
				if NULLIF(v_ch11,'') is not null
				then
					 v_first_ch11:=v_ch11;
				else
					 v_first_ch11:='';
				end if;
				if NULLIF(v_ch12,'') is not null
				then
					 v_first_ch12:=v_ch12;
				else
					 v_first_ch12:='';
				end if;
				if NULLIF(v_ch13,'') is not null
				then
					 v_first_ch13:=v_ch13;
				else
					 v_first_ch13:='';
				end if;
				if NULLIF(v_ch14,'') is not null
				then
					 v_first_ch14:=v_ch14;
				else
					 v_first_ch14:='';
				end if;
				if NULLIF(v_ch15,'') is not null
				then
					 v_first_ch15:=v_ch15;
				else
					 v_first_ch15:='';
				end if;
				if NULLIF(v_ch16,'') is not null
				then
					 v_first_ch16:=v_ch16;
				else
					 v_first_ch16:='';
				end if;
				if NULLIF(v_ch17,'') is not null
				then
					 v_first_ch17:=v_ch17;
				else
					 v_first_ch17:='';
				end if;
				if NULLIF(v_ch18,'') is not null
				then
					 v_first_ch18:=v_ch18;
				else
					 v_first_ch18:='';
				end if;
				if NULLIF(v_ch19,'') is not null
				then
					 v_first_ch19:=v_ch19;
				else
					 v_first_ch19:='';
				end if;
				if NULLIF(v_ch20,'') is not null
				then
					 v_first_ch20:=v_ch20;
				else
					 v_first_ch20:='';
				end if;
				if NULLIF(v_ch21,'') is not null
				then
					 v_first_ch21:=v_ch21;
				else
					 v_first_ch21:='';
				end if;
				if NULLIF(v_ch22,'') is not null
				then
					 v_first_ch22:=v_ch22;
				else
					 v_first_ch22:='';
				end if;
				if NULLIF(v_ch23,'') is not null
				then
					 v_first_ch23:=v_ch23;
				else
					 v_first_ch23:='';
				end if;
				if NULLIF(v_ch24,'') is not null
				then
					 v_first_ch24:=v_ch24;
				else
					 v_first_ch24:='';
				end if;
				if NULLIF(v_ch25,'') is not null
				then
					 v_first_ch25:=v_ch25;
				else
					 v_first_ch25:='';
				end if;
				if NULLIF(v_ch26,'') is not null
				then
					 v_first_ch26:=v_ch26;
				else
					 v_first_ch26:='';
				end if;
				if NULLIF(v_ch27,'') is not null
				then
					 v_first_ch27:=v_ch27;
				else
					 v_first_ch27:='';
				end if;
				if NULLIF(v_ch28,'') is not null
				then
					 v_first_ch28:=v_ch28;
				else
					 v_first_ch28:='';
				end if;
				if NULLIF(v_ch29,'') is not null
				then
					 v_first_ch29:=v_ch29;
				else
					 v_first_ch29:='';
				end if;
				if NULLIF(v_ch30,'') is not null
				then
					 v_first_ch30:=v_ch30;
				else
					 v_first_ch30:='';
				end if;
				if NULLIF(v_ch31,'') is not null
				then
					 v_first_ch31:=v_ch31;
				else
					 v_first_ch31:='';
				end if;
				if NULLIF(v_ch32,'') is not null
				then
					 v_first_ch32:=v_ch32;
				else
					 v_first_ch32:='';
				end if;
				if NULLIF(v_ch33,'') is not null
				then
					 v_first_ch33:=v_ch33;
				else
					 v_first_ch33:='';
				end if;
				if NULLIF(v_ch34,'') is not null
				then
					 v_first_ch34:=v_ch34;
				else
					 v_first_ch34:='';
				end if;
				if NULLIF(v_ch35,'') is not null
				then
					 v_first_ch35:=v_ch35;
				else
					 v_first_ch35:='';
				end if;
				if NULLIF(v_ch36,'') is not null
				then
					 v_first_ch36:=v_ch36;
				else
					 v_first_ch36:='';
				end if;
				if NULLIF(v_ch37,'') is not null
				then
					 v_first_ch37:=v_ch37;
				else
					 v_first_ch37:='';
				end if;
				if NULLIF(v_ch38,'') is not null
				then
					 v_first_ch38:=v_ch38;
				else
					 v_first_ch38:='';
				end if;
				if NULLIF(v_ch39,'') is not null
				then
					 v_first_ch39:=v_ch39;
				else
					 v_first_ch39:='';
				end if;
				if NULLIF(v_ch40,'') is not null
				then
					 v_first_ch40:=v_ch40;
				else
					 v_first_ch40:='';
				end if;
				if NULLIF(v_ch41,'') is not null
				then
					 v_first_ch41:=v_ch41;
				else
					 v_first_ch41:='';
				end if;
				if NULLIF(v_ch42,'') is not null
				then
					 v_first_ch42:=v_ch42;
				else
					 v_first_ch42:='';
				end if;
				if NULLIF(v_ch43,'') is not null
				then
					 v_first_ch43:=v_ch43;
				else
					 v_first_ch43:='';
				end if;
				if NULLIF(v_ch44,'') is not null
				then
					 v_first_ch44:=v_ch44;
				else
					 v_first_ch44:='';
				end if;
				if NULLIF(v_ch45,'') is not null
				then
					 v_first_ch45:=v_ch45;
				else
					 v_first_ch45:='';
				end if;
				if NULLIF(v_ch46,'') is not null
				then
					 v_first_ch46:=v_ch46;
				else
					 v_first_ch46:='';
				end if;
				if NULLIF(v_ch47,'') is not null
				then
					 v_first_ch47:=v_ch47;
				else
					 v_first_ch47:='';
				end if;
				if NULLIF(v_ch48,'') is not null
				then
					 v_first_ch48:=v_ch48;
				else
					 v_first_ch48:='';
				end if;
				if NULLIF(v_ch49,'') is not null
				then
					 v_first_ch49:=v_ch49;
				else
					 v_first_ch49:='';
				end if;
				if NULLIF(v_ch50,'') is not null
				then
					 v_first_ch50:=v_ch50;
				else
					 v_first_ch50:='';
				end if;
				if NULLIF(v_ch51,'') is not null
				then
					 v_first_ch51:=v_ch51;
				else
					 v_first_ch51:='';
				end if;
				if NULLIF(v_ch52,'') is not null
				then
					 v_first_ch52:=v_ch52;
				else
					 v_first_ch52:='';
				end if;
				if NULLIF(v_ch53,'') is not null
				then
					 v_first_ch53:=v_ch53;
				else
					 v_first_ch53:='';
				end if;
				if NULLIF(v_ch54,'') is not null
				then
					 v_first_ch54:=v_ch54;
				else
					 v_first_ch54:='';
				end if;
				if NULLIF(v_ch55,'') is not null
				then
					 v_first_ch55:=v_ch55;
				else
					 v_first_ch55:='';
				end if;
				if NULLIF(v_ch56,'') is not null
				then
					 v_first_ch56:=v_ch56;
				else
					 v_first_ch56:='';
				end if;
				if NULLIF(v_ch57,'') is not null
				then
					 v_first_ch57:=v_ch57;
				else
					 v_first_ch57:='';
				end if;
				if NULLIF(v_ch58,'') is not null
				then
					 v_first_ch58:=v_ch58;
				else
					 v_first_ch58:='';
				end if;
				if NULLIF(v_ch59,'') is not null
				then
					 v_first_ch59:=v_ch59;
				else
					 v_first_ch59:='';
				end if;
				if NULLIF(v_ch60,'') is not null
				then
					 v_first_ch60:=v_ch60;
				else
					 v_first_ch60:='';
				end if;

				
			else
				
				v_execute_1:='Erreur de doublon de colonne : v_time='||COALESCE(v_first_time,'NULL')||'-v_first_evenement='||COALESCE(v_first_evenement,'NULL')||'-v_nom_voie='||COALESCE(v_first_nom_voie,'NULL')
				             ||'-v_lettre_voie='||COALESCE(v_first_lettre_voie,'NULL');
				v_execute_1:=v_execute_1||'-v_ch2='||v_ch2||'-v_ch3='||COALESCE(v_ch3,'NULL')||'-v_ch4='||COALESCE(v_ch4,'NULL')||'-v_ch5='||COALESCE(v_ch5,'NULL')||
							'-v_ch6='||COALESCE(v_ch6,'NULL')||'-v_ch7='||COALESCE(v_ch7,'NULL')||'-v_ch8='||COALESCE(v_ch8,'NULL')||'-v_ch9='||COALESCE(v_ch9,'NULL')||
							'-v_ch10='||COALESCE(v_ch10,'NULL')||'-v_ch11='||COALESCE(v_ch11,'NULL')||'-v_ch12='||COALESCE(v_ch12,'NULL')||'-v_ch13='||COALESCE(v_ch13,'NULL')||
							'-v_ch14='||COALESCE(v_ch14,'NULL')||'-v_ch15='||COALESCE(v_ch15,'NULL')||'-v_ch16='||COALESCE(v_ch16,'NULL')||'-v_ch17='||COALESCE(v_ch17,'NULL')||
							'-v_ch18='||COALESCE(v_ch18,'NULL')||'-v_ch19='||COALESCE(v_ch19,'NULL')||'-v_ch20='||COALESCE(v_ch20,'NULL')||'-v_ch21='||COALESCE(v_ch21,'NULL')||
							'-v_ch22='||COALESCE(v_ch22,'NULL')||'-v_ch23='||COALESCE(v_ch23,'NULL')||'-v_ch24='||COALESCE(v_ch24,'NULL')||'-v_ch25='||COALESCE(v_ch25,'NULL')||
							'-v_ch26='||COALESCE(v_ch26,'NULL')||'-v_ch27='||COALESCE(v_ch27,'NULL')||'-v_ch28='||COALESCE(v_ch28,'NULL')||'-v_ch29='||COALESCE(v_ch29,'NULL')||
							'-v_ch30='||COALESCE(v_ch30,'NULL')||'-v_ch31='||COALESCE(v_ch31,'NULL')||'-v_ch32='||COALESCE(v_ch32,'NULL')||'-v_ch33='||COALESCE(v_ch33,'NULL')||
							'-v_ch34='||COALESCE(v_ch34,'NULL')||'-v_ch35='||COALESCE(v_ch35,'NULL')||'-v_ch36='||COALESCE(v_ch36,'NULL')||'-v_ch37='||COALESCE(v_ch37,'NULL')||
							'-v_ch38='||COALESCE(v_ch38,'NULL')||'-v_ch39='||COALESCE(v_ch39,'NULL')||'-v_ch40='||COALESCE(v_ch40,'NULL')||'-v_ch41='||COALESCE(v_ch41,'NULL')||
							'-v_ch42='||COALESCE(v_ch42,'NULL')||'-v_ch43='||COALESCE(v_ch43,'NULL')||'-v_ch44='||COALESCE(v_ch44,'NULL')||'-v_ch45='||COALESCE(v_ch45,'NULL')||
							'-v_ch46='||COALESCE(v_ch46,'NULL')||'-v_ch47='||COALESCE(v_ch47,'NULL')||'-v_ch48='||COALESCE(v_ch48,'NULL')||'-v_ch49='||COALESCE(v_ch49,'NULL')||
							'-v_ch50='||COALESCE(v_ch50,'NULL')||'-v_ch51='||COALESCE(v_ch51,'NULL')||'-v_ch52='||COALESCE(v_ch52,'NULL')||'-v_ch53='||COALESCE(v_ch53,'NULL')||
							'-v_ch54='||COALESCE(v_ch54,'NULL')||'-v_ch55='||COALESCE(v_ch55,'NULL')||'-v_ch56='||COALESCE(v_ch56,'NULL')||'-v_ch57='||COALESCE(v_ch57,'NULL')||
							'-v_ch58='||COALESCE(v_ch58,'NULL')||'-v_ch59='||COALESCE(v_ch59,'NULL')||'-v_ch60='||COALESCE(v_ch60,'NULL');


				--RAISE NOTICE 'Erreur de doublonc de colonne : v_first_chaine_column_name=%-v_nom_voie=%-v_first_chaine_valeur_calc=%-v_execute_1=%',v_first_chaine_column_name,v_nom_voie,v_first_chaine_valeur_calc,v_execute_1;
				v_first_fichier:=COALESCE(v_first_fichier,'NULL');
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK678';-- RAISE NOTICE 'ERREUR678 dans %',v_execute_2;message_retour:='NOK678';
				--WHEN OTHERS THEN CONTINUE;
				END;
				ligne_to_insert:=1;
			end if;
			
	else
		
			ligne_to_insert:=1;
			
	end if;
			
	if ligne_to_insert != 0
    then	

			v_first_chaine_valeur_calc:=case substr(v_first_chaine_valeur_calc,length(v_first_chaine_valeur_calc),1) when ',' then substr(v_first_chaine_valeur_calc,1,length(v_first_chaine_valeur_calc)-1) else  v_first_chaine_valeur_calc end;
			v_first_chaine_valeur_calc:=replace(v_first_chaine_valeur_calc,',',''',''');
			--v_first_chaine_column_name:=case substr(v_first_chaine_column_name,length(v_first_chaine_column_name),1) when ',' then substr(v_first_chaine_column_name,1,length(v_first_chaine_column_name)-1) else  v_first_chaine_column_name end;
    
			--v_seul:=POSITION(',' IN v_first_chaine_column_name);
	
			--if v_seul != 0 then v_first_chaine_column_name:=replace(v_first_chaine_column_name,',',''','''); end if;
	
			--RAISE NOTICE 'avant insert v_first_chaine_column_name % v_first_chaine_valeur_calc %',v_first_chaine_column_name,v_first_chaine_valeur_calc;
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,ordre,'||v_first_chaine_column_name||')'||
						 ' values('||''''||v_first_time||''''||','||''''||v_first_evenement||''''||','||''''||v_first_libelle_evenement||''''||','||v_ordre||','||''''||v_first_chaine_valeur_calc||''''||')';
				
			IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
			and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_first_evenement,'') IS NOT NULL and NULLIF(v_libelle_evenement,'') IS NOT NULL 
			and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR678a dans %',v_execute_1;message_retour:='NOK678a';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				v_execute_1:='ERREUR678a insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,ordre,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
						     ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_first_evenement,'NULL')||''''||','||v_ordre||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
							 COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
                v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK679'; RAISE NOTICE 'ERREUR679 fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK679';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;
	
			v_first_time:=v_time;
			v_first_chaine_column_name:=v_nom_voie;
			
			v_first_ordre:=v_ordre;
			v_first_evenement:=v_evenement;
			v_first_libelle_evenement:=v_libelle_evenement;
			
			v_first_fichier:=v_fichier;
			v_first_nom_voie:=v_nom_voie;
			v_first_lettre_voie:=v_lettre_voie;
			
			c_compte_nb_ch_chaine_valeur:=0;
			c_compte_nb_ch_chaine_champ:=1;
			
			if NULLIF(v_ch2,'') is not null
			then
				 v_ch2:=v_ch2||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				 v_ch2:='';
			end if;
			if NULLIF(v_ch3,'') is not null
			then
				v_ch3:=v_ch3||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch3:='';
			end if;
			if NULLIF(v_ch4,'') is not null
			then
				v_ch4:=v_ch4||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch4:='';
			end if;
			if NULLIF(v_ch5,'') is not null
			then
				v_ch5:=v_ch5||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch5:='';
			end if;
			if NULLIF(v_ch6,'') is not null
			then
				v_ch6:=v_ch6||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch6:='';
			end if;
			if NULLIF(v_ch7,'') is not null
			then
				v_ch7:=v_ch7||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch7:='';
			end if;
			if NULLIF(v_ch8,'') is not null
			then
				v_ch8:=v_ch8||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch8:='';
			end if;
			if NULLIF(v_ch9,'') is not null
			then
				v_ch9:=v_ch9||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch9:='';
			end if;
			if NULLIF(v_ch10,'') is not null
			then
				v_ch10:=v_ch10||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch10:='';
			end if;
			if NULLIF(v_ch11,'') is not null
			then
				v_ch11:=v_ch11||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch11:='';
			end if;
			if NULLIF(v_ch12,'') is not null
			then
				v_ch12:=v_ch12||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch12:='';
			end if;
			if NULLIF(v_ch13,'') is not null
			then
				v_ch13:=v_ch13||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch13:='';
			end if;
			if NULLIF(v_ch14,'') is not null
			then
				v_ch14:=v_ch14||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch14:='';
			end if;
			if NULLIF(v_ch15,'') is not null
			then
				v_ch15:=v_ch15||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch15:='';
			end if;
			if NULLIF(v_ch16,'') is not null
			then
				v_ch16:=v_ch16||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch16:='';
			end if;
			if NULLIF(v_ch17,'') is not null
			then
				v_ch17:=v_ch17||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch17:='';
			end if;
			if NULLIF(v_ch18,'') is not null
			then
				v_ch18:=v_ch18||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch18:='';
			end if;
			if NULLIF(v_ch19,'') is not null
			then
				v_ch19:=v_ch19||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch19:='';
			end if;
			if NULLIF(v_ch20,'') is not null
			then
				v_ch20:=v_ch20||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch20:='';
			end if;
			if NULLIF(v_ch21,'') is not null
			then
				v_ch21:=v_ch21||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch21:='';
			end if;
			if NULLIF(v_ch22,'') is not null
			then
				v_ch22:=v_ch22||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch22:='';
			end if;
			if NULLIF(v_ch23,'') is not null
			then
				v_ch23:=v_ch23||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch23:='';
			end if;
			if NULLIF(v_ch24,'') is not null
			then
				v_ch24:=v_ch24||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch24:='';
			end if;
			if NULLIF(v_ch25,'') is not null
			then
				v_ch25:=v_ch25||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch25:='';
			end if;
			if NULLIF(v_ch26,'') is not null
			then
				v_ch26:=v_ch26||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch26:='';
			end if;
			if NULLIF(v_ch27,'') is not null
			then
				v_ch27:=v_ch27||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch27:='';
			end if;
			if NULLIF(v_ch28,'') is not null
			then
				v_ch28:=v_ch28||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch28:='';
			end if;
			if NULLIF(v_ch29,'') is not null
			then
				v_ch29:=v_ch29||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch29:='';
			end if;
			if NULLIF(v_ch30,'') is not null
			then
				v_ch30:=v_ch30||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch30:='';
			end if;
			if NULLIF(v_ch31,'') is not null
			then
				v_ch31:=v_ch31||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch31:='';
			end if;
			if NULLIF(v_ch32,'') is not null
			then
				v_ch32:=v_ch32||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch32:='';
			end if;
			if NULLIF(v_ch33,'') is not null
			then
				v_ch33:=v_ch33||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch33:='';
			end if;
			if NULLIF(v_ch34,'') is not null
			then
				v_ch34:=v_ch34||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch34:='';
			end if;
			if NULLIF(v_ch35,'') is not null
			then
				v_ch35:=v_ch35||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch35:='';
			end if;
			if NULLIF(v_ch36,'') is not null
			then
				v_ch36:=v_ch36||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch36:='';
			end if;
			if NULLIF(v_ch37,'') is not null
			then
				v_ch37:=v_ch37||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch37:='';
			end if;
			if NULLIF(v_ch38,'') is not null
			then
				v_ch38:=v_ch38||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch38:='';
			end if;
			if NULLIF(v_ch39,'') is not null
			then
				v_ch39:=v_ch39||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch39:='';
			end if;
			if NULLIF(v_ch40,'') is not null
			then
				v_ch40:=v_ch40||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch40:='';
			end if;
			if NULLIF(v_ch41,'') is not null
			then
				v_ch41:=v_ch41||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch41:='';
			end if;
			if NULLIF(v_ch42,'') is not null
			then
				v_ch42:=v_ch42||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch42:='';
			end if;
			if NULLIF(v_ch43,'') is not null
			then
				v_ch43:=v_ch43||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch43:='';
			end if;
			if NULLIF(v_ch44,'') is not null
			then
				v_ch44:=v_ch44||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch44:='';
			end if;
			if NULLIF(v_ch45,'') is not null
			then
				v_ch45:=v_ch45||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch45:='';
			end if;
			if NULLIF(v_ch46,'') is not null
			then
				v_ch46:=v_ch46||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch46:='';
			end if;
			if NULLIF(v_ch47,'') is not null
			then
				v_ch47:=v_ch47||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch47:='';
			end if;
			if NULLIF(v_ch48,'') is not null
			then
				v_ch48:=v_ch48||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch48:='';
			end if;
			if NULLIF(v_ch49,'') is not null
			then
				v_ch49:=v_ch49||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch49:='';
			end if;
			if NULLIF(v_ch50,'') is not null
			then
				v_ch50:=v_ch50||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch50:='';
			end if;
			if NULLIF(v_ch51,'') is not null
			then
				v_ch51:=v_ch51||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch51:='';
			end if;
			if NULLIF(v_ch52,'') is not null
			then
				v_ch52:=v_ch52||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch52:='';
			end if;
			if NULLIF(v_ch53,'') is not null
			then
				v_ch53:=v_ch53||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch53:='';
			end if;
			if NULLIF(v_ch54,'') is not null
			then
				v_ch54:=v_ch54||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch54:='';
			end if;
			if NULLIF(v_ch55,'') is not null
			then
				v_ch55:=v_ch55||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch55:='';
			end if;
			if NULLIF(v_ch56,'') is not null
			then
				v_ch56:=v_ch56||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch56:='';
			end if;
			if NULLIF(v_ch57,'') is not null
			then
				v_ch57:=v_ch57||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch57:='';
			end if;
			if NULLIF(v_ch58,'') is not null
			then
				v_ch58:=v_ch58||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch58:='';
			end if;
			if NULLIF(v_ch59,'') is not null
			then
				v_ch59:=v_ch59||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch59:='';
			end if;
			if NULLIF(v_ch60,'') is not null
			then
				v_ch60:=v_ch60||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch60:='';
			end if;
	
										
			v_first_chaine_valeur_calc:=v_ch2||v_ch3||v_ch4||v_ch5||v_ch6||v_ch7||v_ch8||v_ch9||v_ch10||v_ch11||v_ch12||v_ch13||
										v_ch14||v_ch15||v_ch16||v_ch17||v_ch18||v_ch19||v_ch20||v_ch21||v_ch22||v_ch23||v_ch24||
										v_ch25||v_ch26||v_ch27||v_ch28||v_ch29||v_ch30||v_ch31||v_ch32||v_ch33||v_ch34||v_ch35||
										v_ch36||v_ch37||v_ch38||v_ch39||v_ch40||v_ch41||v_ch42||v_ch43||v_ch44||v_ch45||v_ch46||
										v_ch47||v_ch48||v_ch49||v_ch50||v_ch51||v_ch52||v_ch53||v_ch54||v_ch55||v_ch56||v_ch57||
										v_ch58||v_ch59||v_ch60;
								  
						
	end if;						  
	
	
END LOOP;
CLOSE CurRelEvETORO;

			-- insert dernière ligne
			
		
			--RAISE NOTICE 'v_first_time=%-v_first_ordre=%-v_first_evenement=%-v_first_libelle_evenement=%-v_ch2=%',v_first_evenement,v_first_ordre,v_first_evenement,v_first_libelle_evenement,v_ch2;
			
			--v_first_time:=v_time;
			
			--v_first_ordre:=v_ordre;
			--v_first_evenement:=v_evenement;
				
			--v_first_libelle_evenement:=v_libelle_evenement;
			
			--v_first_fichier:=v_fichier;
			--v_first_nom_voie:=v_nom_voie;
			--v_first_lettre_voie:=v_lettre_voie;
			
			c_compte_nb_ch_chaine_valeur:=0;
		
			if NULLIF(v_first_ch2,'') is not null
			then
				 v_first_ch2:=v_first_ch2||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				 v_first_ch2:='';
			end if;
			if NULLIF(v_first_ch3,'') is not null
			then
				v_first_ch3:=v_first_ch3||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch3:='';
			end if;
			if NULLIF(v_first_ch4,'') is not null
			then
				v_first_ch4:=v_first_ch4||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch4:='';
			end if;
			if NULLIF(v_first_ch5,'') is not null
			then
				v_first_ch5:=v_first_ch5||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch5:='';
			end if;
			if NULLIF(v_first_ch6,'') is not null
			then
				v_first_ch6:=v_first_ch6||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch6:='';
			end if;
			if NULLIF(v_first_ch7,'') is not null
			then
				v_first_ch7:=v_first_ch7||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch7:='';
			end if;
			if NULLIF(v_first_ch8,'') is not null
			then
				v_first_ch8:=v_first_ch8||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch8:='';
			end if;
			if NULLIF(v_first_ch9,'') is not null
			then
				v_first_ch9:=v_first_ch9||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch9:='';
			end if;
			if NULLIF(v_first_ch10,'') is not null
			then
				v_first_ch10:=v_first_ch10||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch10:='';
			end if;
			if NULLIF(v_first_ch11,'') is not null
			then
				v_first_ch11:=v_first_ch11||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch11:='';
			end if;
			if NULLIF(v_first_ch12,'') is not null
			then
				v_first_ch12:=v_first_ch12||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch12:='';
			end if;
			if NULLIF(v_first_ch13,'') is not null
			then
				v_first_ch13:=v_first_ch13||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch13:='';
			end if;
			if NULLIF(v_first_ch14,'') is not null
			then
				v_first_ch14:=v_first_ch14||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch14:='';
			end if;
			if NULLIF(v_first_ch15,'') is not null
			then
				v_first_ch15:=v_first_ch15||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch15:='';
			end if;
			if NULLIF(v_first_ch16,'') is not null
			then
				v_first_ch16:=v_first_ch16||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch16:='';
			end if;
			if NULLIF(v_first_ch17,'') is not null
			then
				v_first_ch17:=v_first_ch17||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch17:='';
			end if;
			if NULLIF(v_first_ch18,'') is not null
			then
				v_first_ch18:=v_first_ch18||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch18:='';
			end if;
			if NULLIF(v_first_ch19,'') is not null
			then
				v_first_ch19:=v_first_ch19||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch19:='';
			end if;
			if NULLIF(v_first_ch20,'') is not null
			then
				v_first_ch20:=v_first_ch20||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch20:='';
			end if;
			if NULLIF(v_first_ch21,'') is not null
			then
				v_first_ch21:=v_first_ch21||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch21:='';
			end if;
			if NULLIF(v_first_ch22,'') is not null
			then
				v_first_ch22:=v_first_ch22||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch22:='';
			end if;
			if NULLIF(v_first_ch23,'') is not null
			then
				v_first_ch23:=v_first_ch23||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch23:='';
			end if;
			if NULLIF(v_first_ch24,'') is not null
			then
				v_first_ch24:=v_first_ch24||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch24:='';
			end if;
			if NULLIF(v_first_ch25,'') is not null
			then
				v_first_ch25:=v_first_ch25||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch25:='';
			end if;
			if NULLIF(v_first_ch26,'') is not null
			then
				v_first_ch26:=v_first_ch26||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch26:='';
			end if;
			if NULLIF(v_first_ch27,'') is not null
			then
				v_first_ch27:=v_first_ch27||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch27:='';
			end if;
			if NULLIF(v_first_ch28,'') is not null
			then
				v_first_ch28:=v_first_ch28||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch28:='';
			end if;
			if NULLIF(v_first_ch29,'') is not null
			then
				v_first_ch29:=v_first_ch29||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch29:='';
			end if;
			if NULLIF(v_first_ch30,'') is not null
			then
				v_first_ch30:=v_first_ch30||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch30:='';
			end if;
			if NULLIF(v_first_ch31,'') is not null
			then
				v_first_ch31:=v_first_ch31||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch31:='';
			end if;
			if NULLIF(v_first_ch32,'') is not null
			then
				v_first_ch32:=v_first_ch32||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch32:='';
			end if;
			if NULLIF(v_first_ch33,'') is not null
			then
				v_first_ch33:=v_first_ch33||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch33:='';
			end if;
			if NULLIF(v_first_ch34,'') is not null
			then
				v_first_ch34:=v_first_ch34||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch34:='';
			end if;
			if NULLIF(v_first_ch35,'') is not null
			then
				v_first_ch35:=v_first_ch35||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch35:='';
			end if;
			if NULLIF(v_first_ch36,'') is not null
			then
				v_first_ch36:=v_first_ch36||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch36:='';
			end if;
			if NULLIF(v_first_ch37,'') is not null
			then
				v_first_ch37:=v_first_ch37||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch37:='';
			end if;
			if NULLIF(v_first_ch38,'') is not null
			then
				v_first_ch38:=v_first_ch38||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch38:='';
			end if;
			if NULLIF(v_first_ch39,'') is not null
			then
				v_first_ch39:=v_first_ch39||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch39:='';
			end if;
			if NULLIF(v_first_ch40,'') is not null
			then
				v_first_ch40:=v_first_ch40||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch40:='';
			end if;
			if NULLIF(v_first_ch41,'') is not null
			then
				v_first_ch41:=v_first_ch41||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch41:='';
			end if;
			if NULLIF(v_first_ch42,'') is not null
			then
				v_first_ch42:=v_first_ch42||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch42:='';
			end if;
			if NULLIF(v_first_ch43,'') is not null
			then
				v_first_ch43:=v_first_ch43||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch43:='';
			end if;
			if NULLIF(v_first_ch44,'') is not null
			then
				v_first_ch44:=v_first_ch44||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch44:='';
			end if;
			if NULLIF(v_first_ch45,'') is not null
			then
				v_first_ch45:=v_first_ch45||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch45:='';
			end if;
			if NULLIF(v_first_ch46,'') is not null
			then
				v_first_ch46:=v_first_ch46||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch46:='';
			end if;
			if NULLIF(v_first_ch47,'') is not null
			then
				v_first_ch47:=v_first_ch47||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch47:='';
			end if;
			if NULLIF(v_first_ch48,'') is not null
			then
				v_first_ch48:=v_first_ch48||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch48:='';
			end if;
			if NULLIF(v_first_ch49,'') is not null
			then
				v_first_ch49:=v_first_ch49||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch49:='';
			end if;
			if NULLIF(v_first_ch50,'') is not null
			then
				v_first_ch50:=v_first_ch50||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch50:='';
			end if;
			if NULLIF(v_first_ch51,'') is not null
			then
				v_first_ch51:=v_first_ch51||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch51:='';
			end if;
			if NULLIF(v_first_ch52,'') is not null
			then
				v_first_ch52:=v_first_ch52||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch52:='';
			end if;
			if NULLIF(v_first_ch53,'') is not null
			then
				v_first_ch53:=v_first_ch53||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch53:='';
			end if;
			if NULLIF(v_first_ch54,'') is not null
			then
				v_first_ch54:=v_first_ch54||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch54:='';
			end if;
			if NULLIF(v_first_ch55,'') is not null
			then
				v_first_ch55:=v_first_ch55||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch55:='';
			end if;
			if NULLIF(v_first_ch56,'') is not null
			then
				v_first_ch56:=v_first_ch56||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch56:='';
			end if;
			if NULLIF(v_first_ch57,'') is not null
			then
				v_first_ch57:=v_first_ch57||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch57:='';
			end if;
			if NULLIF(v_first_ch58,'') is not null
			then
				v_first_ch58:=v_first_ch58||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch58:='';
			end if;
			if NULLIF(v_first_ch59,'') is not null
			then
				v_first_ch59:=v_first_ch59||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch59:='';
			end if;
			if NULLIF(v_first_ch60,'') is not null
			then
				v_first_ch60:=v_first_ch60||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch60:='';
			end if;
	
										
			v_first_chaine_valeur_calc:=v_first_ch2||v_first_ch3||v_first_ch4||v_first_ch5||v_first_ch6||v_first_ch7||v_first_ch8||v_first_ch9||v_first_ch10||v_first_ch11||v_first_ch12||v_first_ch13||
										v_first_ch14||v_first_ch15||v_first_ch16||v_first_ch17||v_first_ch18||v_first_ch19||v_first_ch20||v_first_ch21||v_first_ch22||v_first_ch23||v_first_ch24||
										v_first_ch25||v_first_ch26||v_first_ch27||v_first_ch28||v_first_ch29||v_first_ch30||v_first_ch31||v_first_ch32||v_first_ch33||v_first_ch34||v_first_ch35||
										v_first_ch36||v_first_ch37||v_first_ch38||v_first_ch39||v_first_ch40||v_first_ch41||v_first_ch42||v_first_ch43||v_first_ch44||v_first_ch45||v_first_ch46||
										v_first_ch47||v_first_ch48||v_first_ch49||v_first_ch50||v_first_ch51||v_first_ch52||v_first_ch53||v_first_ch54||v_first_ch55||v_first_ch56||v_first_ch57||
										v_first_ch58||v_first_ch59||v_first_ch60;

			v_first_chaine_valeur_calc:=case substr(v_first_chaine_valeur_calc,length(v_first_chaine_valeur_calc),1) when ',' then substr(v_first_chaine_valeur_calc,1,length(v_first_chaine_valeur_calc)-1) else  v_first_chaine_valeur_calc end;
			v_first_chaine_valeur_calc:=replace(v_first_chaine_valeur_calc,',',''',''');
			--v_first_chaine_column_name:=case substr(v_first_chaine_column_name,length(v_first_chaine_column_name),1) when ',' then substr(v_first_chaine_column_name,1,length(v_first_chaine_column_name)-1) else  v_first_chaine_column_name end;
    
			--v_seul:=POSITION(',' IN v_first_chaine_column_name);
	
			--if v_seul != 0 then v_first_chaine_column_name:=replace(v_first_chaine_column_name,',',''','''); end if;
	
			--RAISE NOTICE 'avant insert v_first_chaine_column_name=%-v_first_chaine_valeur_calc=%',v_first_chaine_column_name,v_first_chaine_valeur_calc;
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,ordre,'||v_first_chaine_column_name||')'||
						 ' values('||''''||v_first_time||''''||','||''''||v_first_evenement||''''||','||''''||v_first_libelle_evenement||''''||','||v_first_ordre||','||''''||v_first_chaine_valeur_calc||''''||')';
				
			IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
			and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_first_evenement,'') IS NOT NULL and NULLIF(v_libelle_evenement,'') IS NOT NULL 
			and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR678b dans %',v_execute_1;message_retour:='NOK678a';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				v_execute_1:='ERREUR679 insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,ordre,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
						     ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||v_first_ordre||','||
							 ''''||COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
                v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK679a'; RAISE NOTICE 'ERREUR679 fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK679a';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;
			
RAISE NOTICE 'APRES LOOP O';


RAISE NOTICE 'avant loop R';

v_first_chaine_column_name:='';
v_first_chaine_valeur_calc:='';
ligne_to_insert:=0;

c_compte_nb_ch_chaine_champ:=0;
c_compte_nb_ch_chaine_valeur:=0;

OPEN CurRelEvETORR;
FETCH FIRST FROM CurRelEvETORR INTO   v_first_fichier,v_first_time,v_first_ordre,v_first_evenement,v_first_nom_voie,v_first_lettre_voie,v_first_ch2,v_first_ch3,v_first_ch4,v_first_ch5,v_first_ch6,v_first_ch7,v_first_ch8,v_first_ch9,v_first_ch10,v_first_ch11,v_first_ch12,v_first_ch13,v_first_ch14,
									  v_first_ch15,v_first_ch16,v_first_ch17,v_first_ch18,v_first_ch19,v_first_ch20,v_first_ch21,v_first_ch22,v_first_ch23,v_first_ch24,v_first_ch25,v_first_ch26,v_first_ch27,v_first_ch28,v_first_ch29,v_first_ch30,
									  v_first_ch31,v_first_ch32,v_first_ch33,v_first_ch34,v_first_ch35,v_first_ch36,v_first_ch37,v_first_ch38,v_first_ch39,v_first_ch40,v_first_ch41,v_first_ch42,v_first_ch43,v_first_ch44,v_first_ch45,v_first_ch46,
									  v_first_ch47,v_first_ch48,v_first_ch49,v_first_ch50,v_first_ch51,v_first_ch52,v_first_ch53,v_first_ch54,v_first_ch55,v_first_ch56,v_first_ch57,v_first_ch58,v_first_ch59,v_first_ch60;

	v_first_chaine_column_name:=v_first_nom_voie;

	if NULLIF(v_first_ch2,'') is not null
	then
		 v_first_ch2:=v_first_ch2||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		 v_first_ch2:='';
	end if;
	if NULLIF(v_first_ch3,'') is not null
	then
		v_first_ch3:=v_first_ch3||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch3:='';
	end if;
	if NULLIF(v_first_ch4,'') is not null
	then
		v_first_ch4:=v_first_ch4||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch4:='';
	end if;
	if NULLIF(v_first_ch5,'') is not null
	then
		v_first_ch5:=v_first_ch5||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch5:='';
	end if;
	if NULLIF(v_first_ch6,'') is not null
	then
		v_first_ch6:=v_first_ch6||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch6:='';
	end if;
	if NULLIF(v_first_ch7,'') is not null
	then
		v_first_ch7:=v_first_ch7||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch7:='';
	end if;
	if NULLIF(v_first_ch8,'') is not null
	then
		v_first_ch8:=v_first_ch8||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch8:='';
	end if;
	if NULLIF(v_first_ch9,'') is not null
	then
		v_first_ch9:=v_first_ch9||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch9:='';
	end if;
	if NULLIF(v_first_ch10,'') is not null
	then
		v_first_ch10:=v_first_ch10||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch10:='';
	end if;
	if NULLIF(v_first_ch11,'') is not null
	then
		v_first_ch11:=v_first_ch11||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch11:='';
	end if;
	if NULLIF(v_first_ch12,'') is not null
	then
		v_first_ch12:=v_first_ch12||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch12:='';
	end if;
	if NULLIF(v_first_ch13,'') is not null
	then
		v_first_ch13:=v_first_ch13||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch13:='';
	end if;
	if NULLIF(v_first_ch14,'') is not null
	then
		v_first_ch14:=v_first_ch14||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch14:='';
	end if;
	if NULLIF(v_first_ch15,'') is not null
	then
		v_first_ch15:=v_first_ch15||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch15:='';
	end if;
	if NULLIF(v_first_ch16,'') is not null
	then
		v_first_ch16:=v_first_ch16||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch16:='';
	end if;
	if NULLIF(v_first_ch17,'') is not null
	then
		v_first_ch17:=v_first_ch17||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch17:='';
	end if;
	if NULLIF(v_first_ch18,'') is not null
	then
		v_first_ch18:=v_first_ch18||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch18:='';
	end if;
	if NULLIF(v_first_ch19,'') is not null
	then
		v_first_ch19:=v_first_ch19||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch19:='';
	end if;
	if NULLIF(v_first_ch20,'') is not null
	then
		v_first_ch20:=v_first_ch20||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch20:='';
	end if;
	if NULLIF(v_first_ch21,'') is not null
	then
		v_first_ch21:=v_first_ch21||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch21:='';
	end if;
	if NULLIF(v_first_ch22,'') is not null
	then
		v_first_ch22:=v_first_ch22||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch22:='';
	end if;
	if NULLIF(v_first_ch23,'') is not null
	then
		v_first_ch23:=v_first_ch23||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch23:='';
	end if;
	if NULLIF(v_first_ch24,'') is not null
	then
		v_first_ch24:=v_first_ch24||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch24:='';
	end if;
	if NULLIF(v_first_ch25,'') is not null
	then
		v_first_ch25:=v_first_ch25||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch25:='';
	end if;
	if NULLIF(v_first_ch26,'') is not null
	then
		v_first_ch26:=v_first_ch26||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch26:='';
	end if;
	if NULLIF(v_first_ch27,'') is not null
	then
		v_first_ch27:=v_first_ch27||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch27:='';
	end if;
	if NULLIF(v_first_ch28,'') is not null
	then
		v_first_ch28:=v_first_ch28||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch28:='';
	end if;
	if NULLIF(v_first_ch29,'') is not null
	then
		v_first_ch29:=v_first_ch29||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch29:='';
	end if;
	if NULLIF(v_first_ch30,'') is not null
	then
		v_first_ch30:=v_first_ch30||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch30:='';
	end if;
	if NULLIF(v_first_ch31,'') is not null
	then
		v_first_ch31:=v_first_ch31||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch31:='';
	end if;
	if NULLIF(v_first_ch32,'') is not null
	then
		v_first_ch32:=v_first_ch32||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch32:='';
	end if;
	if NULLIF(v_first_ch33,'') is not null
	then
		v_first_ch33:=v_first_ch33||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch33:='';
	end if;
	if NULLIF(v_first_ch34,'') is not null
	then
		v_first_ch34:=v_first_ch34||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch34:='';
	end if;
	if NULLIF(v_first_ch35,'') is not null
	then
		v_first_ch35:=v_first_ch35||',';
	else
		v_first_ch35:='';
	end if;
	if NULLIF(v_first_ch36,'') is not null
	then
		v_first_ch36:=v_first_ch36||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch36:='';
	end if;
	if NULLIF(v_first_ch37,'') is not null
	then
		v_first_ch37:=v_first_ch37||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch37:='';
	end if;
	if NULLIF(v_first_ch38,'') is not null
	then
		v_first_ch38:=v_first_ch38||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch38:='';
	end if;
	if NULLIF(v_first_ch39,'') is not null
	then
		v_first_ch39:=v_first_ch39||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch39:='';
	end if;
	if NULLIF(v_first_ch40,'') is not null
	then
		v_first_ch40:=v_first_ch40||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch40:='';
	end if;
	if NULLIF(v_first_ch41,'') is not null
	then
		v_first_ch41:=v_first_ch41||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch41:='';
	end if;
	if NULLIF(v_first_ch42,'') is not null
	then
		v_first_ch42:=v_first_ch42||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch42:='';
	end if;
	if NULLIF(v_first_ch43,'') is not null
	then
		v_first_ch43:=v_first_ch43||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch43:='';
	end if;
	if NULLIF(v_first_ch44,'') is not null
	then
		v_first_ch44:=v_first_ch44||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch44:='';
	end if;
	if NULLIF(v_first_ch45,'') is not null
	then
		v_first_ch45:=v_first_ch45||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch45:='';
	end if;
	if NULLIF(v_first_ch46,'') is not null
	then
		v_first_ch46:=v_first_ch46||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch46:='';
	end if;
	if NULLIF(v_first_ch47,'') is not null
	then
		v_first_ch47:=v_first_ch47||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch47:='';
	end if;
	if NULLIF(v_first_ch48,'') is not null
	then
		v_first_ch48:=v_first_ch48||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch48:='';
	end if;
	if NULLIF(v_first_ch49,'') is not null
	then
		v_first_ch49:=v_first_ch49||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch49:='';
	end if;
	if NULLIF(v_first_ch50,'') is not null
	then
		v_first_ch50:=v_first_ch50||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch50:='';
	end if;
	if NULLIF(v_first_ch51,'') is not null
	then
		v_first_ch51:=v_first_ch51||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch51:='';
	end if;
	if NULLIF(v_first_ch52,'') is not null
	then
		v_first_ch52:=v_first_ch52||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch52:='';
	end if;
	if NULLIF(v_first_ch53,'') is not null
	then
		v_first_ch53:=v_first_ch53||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch53:='';
	end if;
	if NULLIF(v_first_ch54,'') is not null
	then
		v_first_ch54:=v_first_ch54||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch54:='';
	end if;
	if NULLIF(v_first_ch55,'') is not null
	then
		v_first_ch55:=v_first_ch55||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch55:='';
	end if;
	if NULLIF(v_first_ch56,'') is not null
	then
		v_first_ch56:=v_first_ch56||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch56:='';
	end if;
	if NULLIF(v_first_ch57,'') is not null
	then
		v_first_ch57:=v_first_ch57||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch57:='';
	end if;
	if NULLIF(v_first_ch58,'') is not null
	then
		v_first_ch58:=v_first_ch58||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch58:='';
	end if;
	if NULLIF(v_first_ch59,'') is not null
	then
		v_first_ch59:=v_first_ch59||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch59:='';
	end if;
	if NULLIF(v_first_ch60,'') is not null
	then
		v_first_ch60:=v_first_ch60||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	else
		v_first_ch60:='';
	end if;
	
	
	v_first_chaine_valeur_calc:=v_first_ch2||v_first_ch3||v_first_ch4||v_first_ch5||v_first_ch6||v_first_ch7||v_first_ch8||v_first_ch9||v_first_ch10||v_first_ch11||v_first_ch12||v_first_ch13||
								v_first_ch14||v_first_ch15||v_first_ch16||v_first_ch17||v_first_ch18||v_first_ch19||v_first_ch20||v_first_ch21||v_first_ch22||v_first_ch23||v_first_ch24||
								v_first_ch25||v_first_ch26||v_first_ch27||v_first_ch28||v_first_ch29||v_first_ch30||v_first_ch31||v_first_ch32||v_first_ch33||v_first_ch34||v_first_ch35||
								v_first_ch36||v_first_ch37||v_first_ch38||v_first_ch39||v_first_ch40||v_first_ch41||v_first_ch42||v_first_ch43||v_first_ch44||v_first_ch45||v_first_ch46||
								v_first_ch47||v_first_ch48||v_first_ch49||v_first_ch50||v_first_ch51||v_first_ch52||v_first_ch53||v_first_ch54||v_first_ch55||v_first_ch56||v_first_ch57||
								v_first_ch58||v_first_ch59||v_first_ch60;
						  
	--RAISE NOTICE 'v_column_name %-%',v_first_chaine_valeur_calc,v_first_ch2;
	
v_chaine_column_name:=v_first_chaine_column_name;
v_chaine_valeur_calc:='';
ligne_to_insert:=0;
c_compte_nb_ch_chaine_champ:=1;

v_first_libelle_evenement:='Enrg TS chgt jour';


LOOP
EXIT WHEN NOT FOUND;
FETCH NEXT FROM CurRelEvETORR INTO    v_fichier,v_time,v_ordre,v_evenement,v_nom_voie,v_lettre_voie,v_ch2,v_ch3,v_ch4,v_ch5,v_ch6,v_ch7,v_ch8,v_ch9,v_ch10,v_ch11,v_ch12,v_ch13,v_ch14,
									  v_ch15,v_ch16,v_ch17,v_ch18,v_ch19,v_ch20,v_ch21,v_ch22,v_ch23,v_ch24,v_ch25,v_ch26,v_ch27,v_ch28,v_ch29,v_ch30,
									  v_ch31,v_ch32,v_ch33,v_ch34,v_ch35,v_ch36,v_ch37,v_ch38,v_ch39,v_ch40,v_ch41,v_ch42,v_ch43,v_ch44,v_ch45,v_ch46,
									  v_ch47,v_ch48,v_ch49,v_ch50,v_ch51,v_ch52,v_ch53,v_ch54,v_ch55,v_ch56,v_ch57,v_ch58,v_ch59,v_ch60;

if NULLIF(v_time,'') IS NULL THEN EXIT; END IF;

v_libelle_evenement:=v_first_libelle_evenement;

--if i > 10000 then exit; end if;
if j > 100000 then j:=1; RAISE NOTICE '100000 lignes inserees %',now(); j:=1; end if;

	if v_time = v_first_time and v_ordre = v_first_ordre and v_evenement=v_first_evenement and v_fichier=v_first_fichier
	then
			if POSITION(v_nom_voie IN v_first_chaine_column_name)=0
			then
				v_first_chaine_column_name:=v_first_chaine_column_name||','||v_nom_voie;
				ligne_to_insert:=0;
				c_compte_nb_ch_chaine_champ:=c_compte_nb_ch_chaine_champ+1;
				
				if NULLIF(v_ch2,'') is not null
				then
					 v_first_ch2:=v_ch2;
				else
					 v_first_ch2:='';
				end if;
				if NULLIF(v_ch3,'') is not null
				then
					 v_first_ch3:=v_ch3;
				else
					 v_first_ch3:='';
				end if;
				if NULLIF(v_ch4,'') is not null
				then
					 v_first_ch4:=v_ch4;
				else
					 v_first_ch4:='';
				end if;
				if NULLIF(v_ch5,'') is not null
				then
					 v_first_ch5:=v_ch5;
				else
					 v_first_ch5:='';
				end if;
				if NULLIF(v_ch6,'') is not null
				then
					 v_first_ch6:=v_ch6;
				else
					 v_first_ch6:='';
				end if;
				if NULLIF(v_ch7,'') is not null
				then
					 v_first_ch7:=v_ch7;
				else
					 v_first_ch7:='';
				end if;
				if NULLIF(v_ch8,'') is not null
				then
					 v_first_ch8:=v_ch8;
				else
					 v_first_ch8:='';
				end if;
				if NULLIF(v_ch9,'') is not null
				then
					 v_first_ch9:=v_ch9;
				else
					 v_first_ch9:='';
				end if;
				if NULLIF(v_ch10,'') is not null
				then
					 v_first_ch10:=v_ch10;
				else
					 v_first_ch10:='';
				end if;
				if NULLIF(v_ch11,'') is not null
				then
					 v_first_ch11:=v_ch11;
				else
					 v_first_ch11:='';
				end if;
				if NULLIF(v_ch12,'') is not null
				then
					 v_first_ch12:=v_ch12;
				else
					 v_first_ch12:='';
				end if;
				if NULLIF(v_ch13,'') is not null
				then
					 v_first_ch13:=v_ch13;
				else
					 v_first_ch13:='';
				end if;
				if NULLIF(v_ch14,'') is not null
				then
					 v_first_ch14:=v_ch14;
				else
					 v_first_ch14:='';
				end if;
				if NULLIF(v_ch15,'') is not null
				then
					 v_first_ch15:=v_ch15;
				else
					 v_first_ch15:='';
				end if;
				if NULLIF(v_ch16,'') is not null
				then
					 v_first_ch16:=v_ch16;
				else
					 v_first_ch16:='';
				end if;
				if NULLIF(v_ch17,'') is not null
				then
					 v_first_ch17:=v_ch17;
				else
					 v_first_ch17:='';
				end if;
				if NULLIF(v_ch18,'') is not null
				then
					 v_first_ch18:=v_ch18;
				else
					 v_first_ch18:='';
				end if;
				if NULLIF(v_ch19,'') is not null
				then
					 v_first_ch19:=v_ch19;
				else
					 v_first_ch19:='';
				end if;
				if NULLIF(v_ch20,'') is not null
				then
					 v_first_ch20:=v_ch20;
				else
					 v_first_ch20:='';
				end if;
				if NULLIF(v_ch21,'') is not null
				then
					 v_first_ch21:=v_ch21;
				else
					 v_first_ch21:='';
				end if;
				if NULLIF(v_ch22,'') is not null
				then
					 v_first_ch22:=v_ch22;
				else
					 v_first_ch22:='';
				end if;
				if NULLIF(v_ch23,'') is not null
				then
					 v_first_ch23:=v_ch23;
				else
					 v_first_ch23:='';
				end if;
				if NULLIF(v_ch24,'') is not null
				then
					 v_first_ch24:=v_ch24;
				else
					 v_first_ch24:='';
				end if;
				if NULLIF(v_ch25,'') is not null
				then
					 v_first_ch25:=v_ch25;
				else
					 v_first_ch25:='';
				end if;
				if NULLIF(v_ch26,'') is not null
				then
					 v_first_ch26:=v_ch26;
				else
					 v_first_ch26:='';
				end if;
				if NULLIF(v_ch27,'') is not null
				then
					 v_first_ch27:=v_ch27;
				else
					 v_first_ch27:='';
				end if;
				if NULLIF(v_ch28,'') is not null
				then
					 v_first_ch28:=v_ch28;
				else
					 v_first_ch28:='';
				end if;
				if NULLIF(v_ch29,'') is not null
				then
					 v_first_ch29:=v_ch29;
				else
					 v_first_ch29:='';
				end if;
				if NULLIF(v_ch30,'') is not null
				then
					 v_first_ch30:=v_ch30;
				else
					 v_first_ch30:='';
				end if;
				if NULLIF(v_ch31,'') is not null
				then
					 v_first_ch31:=v_ch31;
				else
					 v_first_ch31:='';
				end if;
				if NULLIF(v_ch32,'') is not null
				then
					 v_first_ch32:=v_ch32;
				else
					 v_first_ch32:='';
				end if;
				if NULLIF(v_ch33,'') is not null
				then
					 v_first_ch33:=v_ch33;
				else
					 v_first_ch33:='';
				end if;
				if NULLIF(v_ch34,'') is not null
				then
					 v_first_ch34:=v_ch34;
				else
					 v_first_ch34:='';
				end if;
				if NULLIF(v_ch35,'') is not null
				then
					 v_first_ch35:=v_ch35;
				else
					 v_first_ch35:='';
				end if;
				if NULLIF(v_ch36,'') is not null
				then
					 v_first_ch36:=v_ch36;
				else
					 v_first_ch36:='';
				end if;
				if NULLIF(v_ch37,'') is not null
				then
					 v_first_ch37:=v_ch37;
				else
					 v_first_ch37:='';
				end if;
				if NULLIF(v_ch38,'') is not null
				then
					 v_first_ch38:=v_ch38;
				else
					 v_first_ch38:='';
				end if;
				if NULLIF(v_ch39,'') is not null
				then
					 v_first_ch39:=v_ch39;
				else
					 v_first_ch39:='';
				end if;
				if NULLIF(v_ch40,'') is not null
				then
					 v_first_ch40:=v_ch40;
				else
					 v_first_ch40:='';
				end if;
				if NULLIF(v_ch41,'') is not null
				then
					 v_first_ch41:=v_ch41;
				else
					 v_first_ch41:='';
				end if;
				if NULLIF(v_ch42,'') is not null
				then
					 v_first_ch42:=v_ch42;
				else
					 v_first_ch42:='';
				end if;
				if NULLIF(v_ch43,'') is not null
				then
					 v_first_ch43:=v_ch43;
				else
					 v_first_ch43:='';
				end if;
				if NULLIF(v_ch44,'') is not null
				then
					 v_first_ch44:=v_ch44;
				else
					 v_first_ch44:='';
				end if;
				if NULLIF(v_ch45,'') is not null
				then
					 v_first_ch45:=v_ch45;
				else
					 v_first_ch45:='';
				end if;
				if NULLIF(v_ch46,'') is not null
				then
					 v_first_ch46:=v_ch46;
				else
					 v_first_ch46:='';
				end if;
				if NULLIF(v_ch47,'') is not null
				then
					 v_first_ch47:=v_ch47;
				else
					 v_first_ch47:='';
				end if;
				if NULLIF(v_ch48,'') is not null
				then
					 v_first_ch48:=v_ch48;
				else
					 v_first_ch48:='';
				end if;
				if NULLIF(v_ch49,'') is not null
				then
					 v_first_ch49:=v_ch49;
				else
					 v_first_ch49:='';
				end if;
				if NULLIF(v_ch50,'') is not null
				then
					 v_first_ch50:=v_ch50;
				else
					 v_first_ch50:='';
				end if;
				if NULLIF(v_ch51,'') is not null
				then
					 v_first_ch51:=v_ch51;
				else
					 v_first_ch51:='';
				end if;
				if NULLIF(v_ch52,'') is not null
				then
					 v_first_ch52:=v_ch52;
				else
					 v_first_ch52:='';
				end if;
				if NULLIF(v_ch53,'') is not null
				then
					 v_first_ch53:=v_ch53;
				else
					 v_first_ch53:='';
				end if;
				if NULLIF(v_ch54,'') is not null
				then
					 v_first_ch54:=v_ch54;
				else
					 v_first_ch54:='';
				end if;
				if NULLIF(v_ch55,'') is not null
				then
					 v_first_ch55:=v_ch55;
				else
					 v_first_ch55:='';
				end if;
				if NULLIF(v_ch56,'') is not null
				then
					 v_first_ch56:=v_ch56;
				else
					 v_first_ch56:='';
				end if;
				if NULLIF(v_ch57,'') is not null
				then
					 v_first_ch57:=v_ch57;
				else
					 v_first_ch57:='';
				end if;
				if NULLIF(v_ch58,'') is not null
				then
					 v_first_ch58:=v_ch58;
				else
					 v_first_ch58:='';
				end if;
				if NULLIF(v_ch59,'') is not null
				then
					 v_first_ch59:=v_ch59;
				else
					 v_first_ch59:='';
				end if;
				if NULLIF(v_ch60,'') is not null
				then
					 v_first_ch60:=v_ch60;
				else
					 v_first_ch60:='';
				end if;
				
			else
				
				v_execute_1:='Erreur de doublon de colonne : v_time='||COALESCE(v_first_time,'NULL')||'-v_first_evenement='||COALESCE(v_first_evenement,'NULL')||'-v_nom_voie='||COALESCE(v_first_nom_voie,'NULL')
				             ||'-v_lettre_voie='||COALESCE(v_first_lettre_voie,'NULL');
				v_execute_1:=v_execute_1||'-v_ch2='||v_ch2||'-v_ch3='||COALESCE(v_ch3,'NULL')||'-v_ch4='||COALESCE(v_ch4,'NULL')||'-v_ch5='||COALESCE(v_ch5,'NULL')||
							'-v_ch6='||COALESCE(v_ch6,'NULL')||'-v_ch7='||COALESCE(v_ch7,'NULL')||'-v_ch8='||COALESCE(v_ch8,'NULL')||'-v_ch9='||COALESCE(v_ch9,'NULL')||
							'-v_ch10='||COALESCE(v_ch10,'NULL')||'-v_ch11='||COALESCE(v_ch11,'NULL')||'-v_ch12='||COALESCE(v_ch12,'NULL')||'-v_ch13='||COALESCE(v_ch13,'NULL')||
							'-v_ch14='||COALESCE(v_ch14,'NULL')||'-v_ch15='||COALESCE(v_ch15,'NULL')||'-v_ch16='||COALESCE(v_ch16,'NULL')||'-v_ch17='||COALESCE(v_ch17,'NULL')||
							'-v_ch18='||COALESCE(v_ch18,'NULL')||'-v_ch19='||COALESCE(v_ch19,'NULL')||'-v_ch20='||COALESCE(v_ch20,'NULL')||'-v_ch21='||COALESCE(v_ch21,'NULL')||
							'-v_ch22='||COALESCE(v_ch22,'NULL')||'-v_ch23='||COALESCE(v_ch23,'NULL')||'-v_ch24='||COALESCE(v_ch24,'NULL')||'-v_ch25='||COALESCE(v_ch25,'NULL')||
							'-v_ch26='||COALESCE(v_ch26,'NULL')||'-v_ch27='||COALESCE(v_ch27,'NULL')||'-v_ch28='||COALESCE(v_ch28,'NULL')||'-v_ch29='||COALESCE(v_ch29,'NULL')||
							'-v_ch30='||COALESCE(v_ch30,'NULL')||'-v_ch31='||COALESCE(v_ch31,'NULL')||'-v_ch32='||COALESCE(v_ch32,'NULL')||'-v_ch33='||COALESCE(v_ch33,'NULL')||
							'-v_ch34='||COALESCE(v_ch34,'NULL')||'-v_ch35='||COALESCE(v_ch35,'NULL')||'-v_ch36='||COALESCE(v_ch36,'NULL')||'-v_ch37='||COALESCE(v_ch37,'NULL')||
							'-v_ch38='||COALESCE(v_ch38,'NULL')||'-v_ch39='||COALESCE(v_ch39,'NULL')||'-v_ch40='||COALESCE(v_ch40,'NULL')||'-v_ch41='||COALESCE(v_ch41,'NULL')||
							'-v_ch42='||COALESCE(v_ch42,'NULL')||'-v_ch43='||COALESCE(v_ch43,'NULL')||'-v_ch44='||COALESCE(v_ch44,'NULL')||'-v_ch45='||COALESCE(v_ch45,'NULL')||
							'-v_ch46='||COALESCE(v_ch46,'NULL')||'-v_ch47='||COALESCE(v_ch47,'NULL')||'-v_ch48='||COALESCE(v_ch48,'NULL')||'-v_ch49='||COALESCE(v_ch49,'NULL')||
							'-v_ch50='||COALESCE(v_ch50,'NULL')||'-v_ch51='||COALESCE(v_ch51,'NULL')||'-v_ch52='||COALESCE(v_ch52,'NULL')||'-v_ch53='||COALESCE(v_ch53,'NULL')||
							'-v_ch54='||COALESCE(v_ch54,'NULL')||'-v_ch55='||COALESCE(v_ch55,'NULL')||'-v_ch56='||COALESCE(v_ch56,'NULL')||'-v_ch57='||COALESCE(v_ch57,'NULL')||
							'-v_ch58='||COALESCE(v_ch58,'NULL')||'-v_ch59='||COALESCE(v_ch59,'NULL')||'-v_ch60='||COALESCE(v_ch60,'NULL');


				--RAISE NOTICE 'Erreur de doublonc de colonne : v_first_chaine_column_name=%-v_nom_voie=%-v_first_chaine_valeur_calc=%-v_execute_1=%',v_first_chaine_column_name,v_nom_voie,v_first_chaine_valeur_calc,v_execute_1;
				v_first_fichier:=COALESCE(v_first_fichier,'NULL');
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK678';-- RAISE NOTICE 'ERREUR678 dans %',v_execute_2;message_retour:='NOK678';
				--WHEN OTHERS THEN CONTINUE;
				END;
				ligne_to_insert:=1;
			end if;
			
	else
		
			ligne_to_insert:=1;
			
	end if;
			
	if ligne_to_insert != 0
    then	

			v_first_chaine_valeur_calc:=case substr(v_first_chaine_valeur_calc,length(v_first_chaine_valeur_calc),1) when ',' then substr(v_first_chaine_valeur_calc,1,length(v_first_chaine_valeur_calc)-1) else  v_first_chaine_valeur_calc end;
			v_first_chaine_valeur_calc:=replace(v_first_chaine_valeur_calc,',',''',''');
			--v_first_chaine_column_name:=case substr(v_first_chaine_column_name,length(v_first_chaine_column_name),1) when ',' then substr(v_first_chaine_column_name,1,length(v_first_chaine_column_name)-1) else  v_first_chaine_column_name end;
    
			--v_seul:=POSITION(',' IN v_first_chaine_column_name);
	
			--if v_seul != 0 then v_first_chaine_column_name:=replace(v_first_chaine_column_name,',',''','''); end if;
	
			--RAISE NOTICE 'avant insert v_first_chaine_column_name % v_first_chaine_valeur_calc %',v_first_chaine_column_name,v_first_chaine_valeur_calc;
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,ordre,'||v_first_chaine_column_name||')'||
						 ' values('||''''||v_first_time||''''||','||''''||v_first_evenement||''''||','||''''||v_first_libelle_evenement||''''||','||v_ordre||','||''''||v_first_chaine_valeur_calc||''''||')';
				
			IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
			and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_first_evenement,'') IS NOT NULL and NULLIF(v_libelle_evenement,'') IS NOT NULL 
			and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR678b dans %',v_execute_1;message_retour:='NOK678b';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				v_execute_1:='NOK679b insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,ordre,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
						     ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_first_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
							 ||','||v_ordre||','||COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
                v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK679b'; RAISE NOTICE 'ERREUR679b fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK679b';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;
	
			v_first_time:=v_time;
			v_first_chaine_column_name:=v_nom_voie;
			
			v_first_ordre:=v_ordre;
			v_first_evenement:=v_evenement;
			v_first_libelle_evenement:=v_libelle_evenement;
			
			v_first_fichier:=v_fichier;
			v_first_nom_voie:=v_nom_voie;
			v_first_lettre_voie:=v_lettre_voie;
			
			c_compte_nb_ch_chaine_valeur:=0;
			c_compte_nb_ch_chaine_champ:=1;
			
			if NULLIF(v_ch2,'') is not null
			then
				 v_ch2:=v_ch2||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				 v_ch2:='';
			end if;
			if NULLIF(v_ch3,'') is not null
			then
				v_ch3:=v_ch3||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch3:='';
			end if;
			if NULLIF(v_ch4,'') is not null
			then
				v_ch4:=v_ch4||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch4:='';
			end if;
			if NULLIF(v_ch5,'') is not null
			then
				v_ch5:=v_ch5||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch5:='';
			end if;
			if NULLIF(v_ch6,'') is not null
			then
				v_ch6:=v_ch6||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch6:='';
			end if;
			if NULLIF(v_ch7,'') is not null
			then
				v_ch7:=v_ch7||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch7:='';
			end if;
			if NULLIF(v_ch8,'') is not null
			then
				v_ch8:=v_ch8||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch8:='';
			end if;
			if NULLIF(v_ch9,'') is not null
			then
				v_ch9:=v_ch9||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch9:='';
			end if;
			if NULLIF(v_ch10,'') is not null
			then
				v_ch10:=v_ch10||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch10:='';
			end if;
			if NULLIF(v_ch11,'') is not null
			then
				v_ch11:=v_ch11||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch11:='';
			end if;
			if NULLIF(v_ch12,'') is not null
			then
				v_ch12:=v_ch12||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch12:='';
			end if;
			if NULLIF(v_ch13,'') is not null
			then
				v_ch13:=v_ch13||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch13:='';
			end if;
			if NULLIF(v_ch14,'') is not null
			then
				v_ch14:=v_ch14||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch14:='';
			end if;
			if NULLIF(v_ch15,'') is not null
			then
				v_ch15:=v_ch15||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch15:='';
			end if;
			if NULLIF(v_ch16,'') is not null
			then
				v_ch16:=v_ch16||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch16:='';
			end if;
			if NULLIF(v_ch17,'') is not null
			then
				v_ch17:=v_ch17||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch17:='';
			end if;
			if NULLIF(v_ch18,'') is not null
			then
				v_ch18:=v_ch18||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch18:='';
			end if;
			if NULLIF(v_ch19,'') is not null
			then
				v_ch19:=v_ch19||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch19:='';
			end if;
			if NULLIF(v_ch20,'') is not null
			then
				v_ch20:=v_ch20||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch20:='';
			end if;
			if NULLIF(v_ch21,'') is not null
			then
				v_ch21:=v_ch21||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch21:='';
			end if;
			if NULLIF(v_ch22,'') is not null
			then
				v_ch22:=v_ch22||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch22:='';
			end if;
			if NULLIF(v_ch23,'') is not null
			then
				v_ch23:=v_ch23||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch23:='';
			end if;
			if NULLIF(v_ch24,'') is not null
			then
				v_ch24:=v_ch24||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch24:='';
			end if;
			if NULLIF(v_ch25,'') is not null
			then
				v_ch25:=v_ch25||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch25:='';
			end if;
			if NULLIF(v_ch26,'') is not null
			then
				v_ch26:=v_ch26||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch26:='';
			end if;
			if NULLIF(v_ch27,'') is not null
			then
				v_ch27:=v_ch27||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch27:='';
			end if;
			if NULLIF(v_ch28,'') is not null
			then
				v_ch28:=v_ch28||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch28:='';
			end if;
			if NULLIF(v_ch29,'') is not null
			then
				v_ch29:=v_ch29||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch29:='';
			end if;
			if NULLIF(v_ch30,'') is not null
			then
				v_ch30:=v_ch30||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch30:='';
			end if;
			if NULLIF(v_ch31,'') is not null
			then
				v_ch31:=v_ch31||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch31:='';
			end if;
			if NULLIF(v_ch32,'') is not null
			then
				v_ch32:=v_ch32||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch32:='';
			end if;
			if NULLIF(v_ch33,'') is not null
			then
				v_ch33:=v_ch33||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch33:='';
			end if;
			if NULLIF(v_ch34,'') is not null
			then
				v_ch34:=v_ch34||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch34:='';
			end if;
			if NULLIF(v_ch35,'') is not null
			then
				v_ch35:=v_ch35||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch35:='';
			end if;
			if NULLIF(v_ch36,'') is not null
			then
				v_ch36:=v_ch36||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch36:='';
			end if;
			if NULLIF(v_ch37,'') is not null
			then
				v_ch37:=v_ch37||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch37:='';
			end if;
			if NULLIF(v_ch38,'') is not null
			then
				v_ch38:=v_ch38||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch38:='';
			end if;
			if NULLIF(v_ch39,'') is not null
			then
				v_ch39:=v_ch39||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch39:='';
			end if;
			if NULLIF(v_ch40,'') is not null
			then
				v_ch40:=v_ch40||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch40:='';
			end if;
			if NULLIF(v_ch41,'') is not null
			then
				v_ch41:=v_ch41||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch41:='';
			end if;
			if NULLIF(v_ch42,'') is not null
			then
				v_ch42:=v_ch42||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch42:='';
			end if;
			if NULLIF(v_ch43,'') is not null
			then
				v_ch43:=v_ch43||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch43:='';
			end if;
			if NULLIF(v_ch44,'') is not null
			then
				v_ch44:=v_ch44||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch44:='';
			end if;
			if NULLIF(v_ch45,'') is not null
			then
				v_ch45:=v_ch45||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch45:='';
			end if;
			if NULLIF(v_ch46,'') is not null
			then
				v_ch46:=v_ch46||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch46:='';
			end if;
			if NULLIF(v_ch47,'') is not null
			then
				v_ch47:=v_ch47||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch47:='';
			end if;
			if NULLIF(v_ch48,'') is not null
			then
				v_ch48:=v_ch48||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch48:='';
			end if;
			if NULLIF(v_ch49,'') is not null
			then
				v_ch49:=v_ch49||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch49:='';
			end if;
			if NULLIF(v_ch50,'') is not null
			then
				v_ch50:=v_ch50||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch50:='';
			end if;
			if NULLIF(v_ch51,'') is not null
			then
				v_ch51:=v_ch51||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch51:='';
			end if;
			if NULLIF(v_ch52,'') is not null
			then
				v_ch52:=v_ch52||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch52:='';
			end if;
			if NULLIF(v_ch53,'') is not null
			then
				v_ch53:=v_ch53||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch53:='';
			end if;
			if NULLIF(v_ch54,'') is not null
			then
				v_ch54:=v_ch54||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch54:='';
			end if;
			if NULLIF(v_ch55,'') is not null
			then
				v_ch55:=v_ch55||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch55:='';
			end if;
			if NULLIF(v_ch56,'') is not null
			then
				v_ch56:=v_ch56||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch56:='';
			end if;
			if NULLIF(v_ch57,'') is not null
			then
				v_ch57:=v_ch57||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch57:='';
			end if;
			if NULLIF(v_ch58,'') is not null
			then
				v_ch58:=v_ch58||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch58:='';
			end if;
			if NULLIF(v_ch59,'') is not null
			then
				v_ch59:=v_ch59||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch59:='';
			end if;
			if NULLIF(v_ch60,'') is not null
			then
				v_ch60:=v_ch60||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_ch60:='';
			end if;
	
										
			v_first_chaine_valeur_calc:=v_ch2||v_ch3||v_ch4||v_ch5||v_ch6||v_ch7||v_ch8||v_ch9||v_ch10||v_ch11||v_ch12||v_ch13||
										v_ch14||v_ch15||v_ch16||v_ch17||v_ch18||v_ch19||v_ch20||v_ch21||v_ch22||v_ch23||v_ch24||
										v_ch25||v_ch26||v_ch27||v_ch28||v_ch29||v_ch30||v_ch31||v_ch32||v_ch33||v_ch34||v_ch35||
										v_ch36||v_ch37||v_ch38||v_ch39||v_ch40||v_ch41||v_ch42||v_ch43||v_ch44||v_ch45||v_ch46||
										v_ch47||v_ch48||v_ch49||v_ch50||v_ch51||v_ch52||v_ch53||v_ch54||v_ch55||v_ch56||v_ch57||
										v_ch58||v_ch59||v_ch60;
								  
						
	end if;						  
	
	
END LOOP;
CLOSE CurRelEvETORR;

--RAISE NOTICE 'v_first_time=%-v_first_ordre=%-v_first_evenement=%-v_first_libelle_evenement=%-v_ch2=%',v_first_evenement,v_first_ordre,v_first_evenement,v_first_libelle_evenement,v_ch2;
			
			--v_first_time:=v_time;
			
			--v_first_ordre:=v_ordre;
			--v_first_evenement:=v_evenement;
				
			--v_first_libelle_evenement:=v_libelle_evenement;
			
			--v_first_fichier:=v_fichier;
			--v_first_nom_voie:=v_nom_voie;
			--v_first_lettre_voie:=v_lettre_voie;
			
			c_compte_nb_ch_chaine_valeur:=0;
		
			if NULLIF(v_first_ch2,'') is not null
			then
				 v_first_ch2:=v_first_ch2||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				 v_first_ch2:='';
			end if;
			if NULLIF(v_first_ch3,'') is not null
			then
				v_first_ch3:=v_first_ch3||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch3:='';
			end if;
			if NULLIF(v_first_ch4,'') is not null
			then
				v_first_ch4:=v_first_ch4||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch4:='';
			end if;
			if NULLIF(v_first_ch5,'') is not null
			then
				v_first_ch5:=v_first_ch5||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch5:='';
			end if;
			if NULLIF(v_first_ch6,'') is not null
			then
				v_first_ch6:=v_first_ch6||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch6:='';
			end if;
			if NULLIF(v_first_ch7,'') is not null
			then
				v_first_ch7:=v_first_ch7||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch7:='';
			end if;
			if NULLIF(v_first_ch8,'') is not null
			then
				v_first_ch8:=v_first_ch8||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch8:='';
			end if;
			if NULLIF(v_first_ch9,'') is not null
			then
				v_first_ch9:=v_first_ch9||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch9:='';
			end if;
			if NULLIF(v_first_ch10,'') is not null
			then
				v_first_ch10:=v_first_ch10||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch10:='';
			end if;
			if NULLIF(v_first_ch11,'') is not null
			then
				v_first_ch11:=v_first_ch11||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch11:='';
			end if;
			if NULLIF(v_first_ch12,'') is not null
			then
				v_first_ch12:=v_first_ch12||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch12:='';
			end if;
			if NULLIF(v_first_ch13,'') is not null
			then
				v_first_ch13:=v_first_ch13||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch13:='';
			end if;
			if NULLIF(v_first_ch14,'') is not null
			then
				v_first_ch14:=v_first_ch14||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch14:='';
			end if;
			if NULLIF(v_first_ch15,'') is not null
			then
				v_first_ch15:=v_first_ch15||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch15:='';
			end if;
			if NULLIF(v_first_ch16,'') is not null
			then
				v_first_ch16:=v_first_ch16||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch16:='';
			end if;
			if NULLIF(v_first_ch17,'') is not null
			then
				v_first_ch17:=v_first_ch17||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch17:='';
			end if;
			if NULLIF(v_first_ch18,'') is not null
			then
				v_first_ch18:=v_first_ch18||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch18:='';
			end if;
			if NULLIF(v_first_ch19,'') is not null
			then
				v_first_ch19:=v_first_ch19||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch19:='';
			end if;
			if NULLIF(v_first_ch20,'') is not null
			then
				v_first_ch20:=v_first_ch20||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch20:='';
			end if;
			if NULLIF(v_first_ch21,'') is not null
			then
				v_first_ch21:=v_first_ch21||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch21:='';
			end if;
			if NULLIF(v_first_ch22,'') is not null
			then
				v_first_ch22:=v_first_ch22||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch22:='';
			end if;
			if NULLIF(v_first_ch23,'') is not null
			then
				v_first_ch23:=v_first_ch23||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch23:='';
			end if;
			if NULLIF(v_first_ch24,'') is not null
			then
				v_first_ch24:=v_first_ch24||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch24:='';
			end if;
			if NULLIF(v_first_ch25,'') is not null
			then
				v_first_ch25:=v_first_ch25||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch25:='';
			end if;
			if NULLIF(v_first_ch26,'') is not null
			then
				v_first_ch26:=v_first_ch26||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch26:='';
			end if;
			if NULLIF(v_first_ch27,'') is not null
			then
				v_first_ch27:=v_first_ch27||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch27:='';
			end if;
			if NULLIF(v_first_ch28,'') is not null
			then
				v_first_ch28:=v_first_ch28||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch28:='';
			end if;
			if NULLIF(v_first_ch29,'') is not null
			then
				v_first_ch29:=v_first_ch29||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch29:='';
			end if;
			if NULLIF(v_first_ch30,'') is not null
			then
				v_first_ch30:=v_first_ch30||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch30:='';
			end if;
			if NULLIF(v_first_ch31,'') is not null
			then
				v_first_ch31:=v_first_ch31||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch31:='';
			end if;
			if NULLIF(v_first_ch32,'') is not null
			then
				v_first_ch32:=v_first_ch32||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch32:='';
			end if;
			if NULLIF(v_first_ch33,'') is not null
			then
				v_first_ch33:=v_first_ch33||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch33:='';
			end if;
			if NULLIF(v_first_ch34,'') is not null
			then
				v_first_ch34:=v_first_ch34||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch34:='';
			end if;
			if NULLIF(v_first_ch35,'') is not null
			then
				v_first_ch35:=v_first_ch35||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch35:='';
			end if;
			if NULLIF(v_first_ch36,'') is not null
			then
				v_first_ch36:=v_first_ch36||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch36:='';
			end if;
			if NULLIF(v_first_ch37,'') is not null
			then
				v_first_ch37:=v_first_ch37||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch37:='';
			end if;
			if NULLIF(v_first_ch38,'') is not null
			then
				v_first_ch38:=v_first_ch38||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch38:='';
			end if;
			if NULLIF(v_first_ch39,'') is not null
			then
				v_first_ch39:=v_first_ch39||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch39:='';
			end if;
			if NULLIF(v_first_ch40,'') is not null
			then
				v_first_ch40:=v_first_ch40||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch40:='';
			end if;
			if NULLIF(v_first_ch41,'') is not null
			then
				v_first_ch41:=v_first_ch41||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch41:='';
			end if;
			if NULLIF(v_first_ch42,'') is not null
			then
				v_first_ch42:=v_first_ch42||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch42:='';
			end if;
			if NULLIF(v_first_ch43,'') is not null
			then
				v_first_ch43:=v_first_ch43||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch43:='';
			end if;
			if NULLIF(v_first_ch44,'') is not null
			then
				v_first_ch44:=v_first_ch44||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch44:='';
			end if;
			if NULLIF(v_first_ch45,'') is not null
			then
				v_first_ch45:=v_first_ch45||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch45:='';
			end if;
			if NULLIF(v_first_ch46,'') is not null
			then
				v_first_ch46:=v_first_ch46||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch46:='';
			end if;
			if NULLIF(v_first_ch47,'') is not null
			then
				v_first_ch47:=v_first_ch47||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch47:='';
			end if;
			if NULLIF(v_first_ch48,'') is not null
			then
				v_first_ch48:=v_first_ch48||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch48:='';
			end if;
			if NULLIF(v_first_ch49,'') is not null
			then
				v_first_ch49:=v_first_ch49||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch49:='';
			end if;
			if NULLIF(v_first_ch50,'') is not null
			then
				v_first_ch50:=v_first_ch50||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch50:='';
			end if;
			if NULLIF(v_first_ch51,'') is not null
			then
				v_first_ch51:=v_first_ch51||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch51:='';
			end if;
			if NULLIF(v_first_ch52,'') is not null
			then
				v_first_ch52:=v_first_ch52||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch52:='';
			end if;
			if NULLIF(v_first_ch53,'') is not null
			then
				v_first_ch53:=v_first_ch53||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch53:='';
			end if;
			if NULLIF(v_first_ch54,'') is not null
			then
				v_first_ch54:=v_first_ch54||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch54:='';
			end if;
			if NULLIF(v_first_ch55,'') is not null
			then
				v_first_ch55:=v_first_ch55||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch55:='';
			end if;
			if NULLIF(v_first_ch56,'') is not null
			then
				v_first_ch56:=v_first_ch56||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch56:='';
			end if;
			if NULLIF(v_first_ch57,'') is not null
			then
				v_first_ch57:=v_first_ch57||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch57:='';
			end if;
			if NULLIF(v_first_ch58,'') is not null
			then
				v_first_ch58:=v_first_ch58||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch58:='';
			end if;
			if NULLIF(v_first_ch59,'') is not null
			then
				v_first_ch59:=v_first_ch59||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch59:='';
			end if;
			if NULLIF(v_first_ch60,'') is not null
			then
				v_first_ch60:=v_first_ch60||',';c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			else
				v_first_ch60:='';
			end if;
	
										
			v_first_chaine_valeur_calc:=v_first_ch2||v_first_ch3||v_first_ch4||v_first_ch5||v_first_ch6||v_first_ch7||v_first_ch8||v_first_ch9||v_first_ch10||v_first_ch11||v_first_ch12||v_first_ch13||
										v_first_ch14||v_first_ch15||v_first_ch16||v_first_ch17||v_first_ch18||v_first_ch19||v_first_ch20||v_first_ch21||v_first_ch22||v_first_ch23||v_first_ch24||
										v_first_ch25||v_first_ch26||v_first_ch27||v_first_ch28||v_first_ch29||v_first_ch30||v_first_ch31||v_first_ch32||v_first_ch33||v_first_ch34||v_first_ch35||
										v_first_ch36||v_first_ch37||v_first_ch38||v_first_ch39||v_first_ch40||v_first_ch41||v_first_ch42||v_first_ch43||v_first_ch44||v_first_ch45||v_first_ch46||
										v_first_ch47||v_first_ch48||v_first_ch49||v_first_ch50||v_first_ch51||v_first_ch52||v_first_ch53||v_first_ch54||v_first_ch55||v_first_ch56||v_first_ch57||
										v_first_ch58||v_first_ch59||v_first_ch60;

			v_first_chaine_valeur_calc:=case substr(v_first_chaine_valeur_calc,length(v_first_chaine_valeur_calc),1) when ',' then substr(v_first_chaine_valeur_calc,1,length(v_first_chaine_valeur_calc)-1) else  v_first_chaine_valeur_calc end;
			v_first_chaine_valeur_calc:=replace(v_first_chaine_valeur_calc,',',''',''');
			--v_first_chaine_column_name:=case substr(v_first_chaine_column_name,length(v_first_chaine_column_name),1) when ',' then substr(v_first_chaine_column_name,1,length(v_first_chaine_column_name)-1) else  v_first_chaine_column_name end;
    
			--v_seul:=POSITION(',' IN v_first_chaine_column_name);
	
			--if v_seul != 0 then v_first_chaine_column_name:=replace(v_first_chaine_column_name,',',''','''); end if;
	
			--RAISE NOTICE 'avant insert v_first_chaine_column_name=%-v_first_chaine_valeur_calc=%',v_first_chaine_column_name,v_first_chaine_valeur_calc;
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,ordre,'||v_first_chaine_column_name||')'||
						 ' values('||''''||v_first_time||''''||','||''''||v_first_evenement||''''||','||''''||v_first_libelle_evenement||''''||','||v_first_ordre||','||''''||v_first_chaine_valeur_calc||''''||')';
				
			IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
			and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_first_evenement,'') IS NOT NULL and NULLIF(v_libelle_evenement,'') IS NOT NULL 
			and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR678c dans %',v_execute_1;message_retour:='NOK678c';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				v_execute_1:='ERREUR679c insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,ordre,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
						     ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
							 ','||v_first_ordre||','||COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
                v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK679c'; RAISE NOTICE 'ERREUR679c fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK679c';
				--WHEN OTHERS THEN CONTINUE;
				END;
			END IF;

RAISE NOTICE 'APRES LOOP R';

return message_retour;

--EXCEPTION

--WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;