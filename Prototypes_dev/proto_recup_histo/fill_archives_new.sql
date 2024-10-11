-- 425453 mesure pour le fichier PD_fichier_nabyl_body07_12.csv 429374 en base 423393=OK + 5981=en erreur.
DROP FUNCTION public.fill_archives;
CREATE OR REPLACE FUNCTION public.fill_archives(
	station in character varying (10),
	chaine_table_releves in character varying (1000),
	chaine_table_entetes in character varying (1000)
	)
    RETURNS text
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_execute_1 			text;
v_execute_2 			text;
table_releves			character varying (100);
table_entetes			character varying (100);
table_tmp_releves 		character varying (100);
table_tmp_entetes 		character varying (100);
table_tmp_position		character varying (100);
table_archivage			character varying (100);
table_log_archivage		character varying (100);
table_position			character varying (100);
table_log				character varying (100);
message_retour 			text:='OK';

v_libelle_evenement_K100 	character varying (100):='Modif 0 %';
v_libelle_evenement_K0 		character varying (100):='Modif 100 %';
v_libelle_evenement_erreur 	character varying (100):='Défaut format';
v_libelle_evenement_table 	character varying (100):='';

v_first_chaine_valeur_calc_k_100 	text;
v_first_chaine_valeur_calc_k_0 		text;

v_execute_K_100			text;
v_execute_K_0			text;

v_first_time 		character varying (100);
v_time 		 		character varying (100);
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

v_nom_voie 		 character varying (100);
v_first_nom_voie character varying (100);
v_lettre_voie	 character varying (2);
v_first_lettre_voie	character varying (2);
v_column_name_1	 character varying (100);
v_column_name_2	 character varying (100);

v_chaine_column_name    			text;
v_chaine_valeur_calc 				text;
v_first_chaine_column_name    		text;
v_first_chaine_valeur_calc 			text;

v_libelle_evenement character varying (100);
v_first_libelle_evenement character varying (100);	

v_lettre	character varying (2);

v_valeur	character varying (100);
v_nombre    integer;
v_seul    	integer;

c_compte_nb_ch_chaine_champ integer;
c_compte_nb_ch_chaine_valeur integer;

v_val_calc character varying (100);	

i integer:=1;
j integer:=1;

ligne_to_insert integer:=0;
 
compteur_ligne_insert integer:=1;
nombre_ligne integer:=0;
v_nombre_ligne integer:=0;
v_count	integer:=0;

CurColumnMulti CURSOR(v_station character varying (10)) FOR SELECT substr(column_name,1,position('____' in column_name)-1) as column_name_orig
														FROM   INFORMATION_SCHEMA.COLUMNS
														where  table_name='archives_releves' 
														and    table_schema='sh_'||lower(v_station)
														and    ordinal_position>3
														group by column_name_orig
														having count(*) > 1;

CurColumnMulti2 CURSOR(v_station character varying (10), v_column character varying (100))
	                                                    FOR select column_name 
	                                                    from  INFORMATION_SCHEMA.COLUMNS
	                                                    where table_name='archives_releves'
	                                                    and   table_schema='sh_'||lower(v_station)
	                                                    and   position(v_column in column_name)!=0;
															

CurRelEvE 			CURSOR FOR SELECT r.time,r.evenement,COALESCE(r.ch1,'') as ch1 ,COALESCE(r.ch2,'') as ch2 FROM sh_pd.tmp_load_file_jour_releves r where r.evenement ='E' ORDER BY TIME;
CurRelEvAnaPrec 	CURSOR FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time,
								COALESCE(r.ordre,-99999) as ordre,COALESCE(r.evenement,'') as evenement ,
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie,
								CASE WHEN COALESCE(length(NULLIF(r.ch2,'')),null,0) -1 <=2 THEN COALESCE(r.ch2,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch2,3,length(r.ch2)-1),t.precision) end as ch2,
								CASE WHEN COALESCE(length(NULLIF(r.ch3,'')),null,0) -1 <=2 THEN COALESCE(r.ch3,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch3,3,length(r.ch3)-1),t.precision) end as ch3,
								CASE WHEN COALESCE(length(NULLIF(r.ch4,'')),null,0) -1 <=2 THEN COALESCE(r.ch4,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch4,3,length(r.ch4)-1),t.precision) end as ch4,
								CASE WHEN COALESCE(length(NULLIF(r.ch5,'')),null,0) -1 <=2 THEN COALESCE(r.ch5,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch5,3,length(r.ch5)-1),t.precision) end as ch5,
								CASE WHEN COALESCE(length(NULLIF(r.ch6,'')),null,0) -1 <=2 THEN COALESCE(r.ch6,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch6,3,length(r.ch6)-1),t.precision) end as ch6,
								CASE WHEN COALESCE(length(NULLIF(r.ch7,'')),null,0) -1 <=2 THEN COALESCE(r.ch7,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch7,3,length(r.ch7)-1),t.precision) end as ch7,
								CASE WHEN COALESCE(length(NULLIF(r.ch8,'')),null,0) -1 <=2 THEN COALESCE(r.ch8,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch8,3,length(r.ch8)-1),t.precision) end as ch8,
								CASE WHEN COALESCE(length(NULLIF(r.ch9,'')),null,0) -1 <=2 THEN COALESCE(r.ch9,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch9,3,length(r.ch9)-1),t.precision) end as ch9,
								CASE WHEN COALESCE(length(NULLIF(r.ch10,'')),null,0) -1 <=2 THEN COALESCE(r.ch10,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch10,3,length(r.ch10)-1),t.precision) end as ch10,
								CASE WHEN COALESCE(length(NULLIF(r.ch11,'')),null,0) -1 <=2 THEN COALESCE(r.ch11,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch11,3,length(r.ch11)-1),t.precision) end as ch11,
								CASE WHEN COALESCE(length(NULLIF(r.ch12,'')),null,0) -1 <=2 THEN COALESCE(r.ch12,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch12,3,length(r.ch12)-1),t.precision) end as ch12,
								CASE WHEN COALESCE(length(NULLIF(r.ch13,'')),null,0) -1 <=2 THEN COALESCE(r.ch13,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch13,3,length(r.ch13)-1),t.precision) end as ch13,
								CASE WHEN COALESCE(length(NULLIF(r.ch14,'')),null,0) -1 <=2 THEN COALESCE(r.ch14,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch14,3,length(r.ch14)-1),t.precision) end as ch14,
								CASE WHEN COALESCE(length(NULLIF(r.ch15,'')),null,0) -1 <=2 THEN COALESCE(r.ch15,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch15,3,length(r.ch15)-1),t.precision) end as ch15,
								CASE WHEN COALESCE(length(NULLIF(r.ch16,'')),null,0) -1 <=2 THEN COALESCE(r.ch16,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch16,3,length(r.ch16)-1),t.precision) end as ch16,
								CASE WHEN COALESCE(length(NULLIF(r.ch17,'')),null,0) -1 <=2 THEN COALESCE(r.ch17,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch17,3,length(r.ch17)-1),t.precision) end as ch17,
								CASE WHEN COALESCE(length(NULLIF(r.ch18,'')),null,0) -1 <=2 THEN COALESCE(r.ch18,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch18,3,length(r.ch18)-1),t.precision) end as ch18,
								CASE WHEN COALESCE(length(NULLIF(r.ch19,'')),null,0) -1 <=2 THEN COALESCE(r.ch19,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch19,3,length(r.ch19)-1),t.precision) end as ch19,
								CASE WHEN COALESCE(length(NULLIF(r.ch20,'')),null,0) -1 <=2 THEN COALESCE(r.ch20,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch20,3,length(r.ch20)-1),t.precision) end as ch20,
								CASE WHEN COALESCE(length(NULLIF(r.ch21,'')),null,0) -1 <=2 THEN COALESCE(r.ch21,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch21,3,length(r.ch21)-1),t.precision) end as ch21,
								CASE WHEN COALESCE(length(NULLIF(r.ch22,'')),null,0) -1 <=2 THEN COALESCE(r.ch22,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch22,3,length(r.ch22)-1),t.precision) end as ch22,
								CASE WHEN COALESCE(length(NULLIF(r.ch23,'')),null,0) -1 <=2 THEN COALESCE(r.ch23,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch23,3,length(r.ch23)-1),t.precision) end as ch23,
								CASE WHEN COALESCE(length(NULLIF(r.ch24,'')),null,0) -1 <=2 THEN COALESCE(r.ch24,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch24,3,length(r.ch24)-1),t.precision) end as ch24,
								CASE WHEN COALESCE(length(NULLIF(r.ch25,'')),null,0) -1 <=2 THEN COALESCE(r.ch25,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch25,3,length(r.ch25)-1),t.precision) end as ch25,
								CASE WHEN COALESCE(length(NULLIF(r.ch26,'')),null,0) -1 <=2 THEN COALESCE(r.ch26,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch26,3,length(r.ch26)-1),t.precision) end as ch26,
								CASE WHEN COALESCE(length(NULLIF(r.ch27,'')),null,0) -1 <=2 THEN COALESCE(r.ch27,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch27,3,length(r.ch27)-1),t.precision) end as ch27,
								CASE WHEN COALESCE(length(NULLIF(r.ch28,'')),null,0) -1 <=2 THEN COALESCE(r.ch28,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch28,3,length(r.ch28)-1),t.precision) end as ch28,
								CASE WHEN COALESCE(length(NULLIF(r.ch29,'')),null,0) -1 <=2 THEN COALESCE(r.ch29,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch29,3,length(r.ch29)-1),t.precision) end as ch29,
								CASE WHEN COALESCE(length(NULLIF(r.ch30,'')),null,0) -1 <=2 THEN COALESCE(r.ch30,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch30,3,length(r.ch30)-1),t.precision) end as ch30,
								CASE WHEN COALESCE(length(NULLIF(r.ch31,'')),null,0) -1 <=2 THEN COALESCE(r.ch31,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch31,3,length(r.ch31)-1),t.precision) end as ch31,
								CASE WHEN COALESCE(length(NULLIF(r.ch32,'')),null,0) -1 <=2 THEN COALESCE(r.ch32,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch32,3,length(r.ch32)-1),t.precision) end as ch32,
								CASE WHEN COALESCE(length(NULLIF(r.ch33,'')),null,0) -1 <=2 THEN COALESCE(r.ch33,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch33,3,length(r.ch33)-1),t.precision) end as ch33,
								CASE WHEN COALESCE(length(NULLIF(r.ch34,'')),null,0) -1 <=2 THEN COALESCE(r.ch34,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch34,3,length(r.ch34)-1),t.precision) end as ch34,
								CASE WHEN COALESCE(length(NULLIF(r.ch35,'')),null,0) -1 <=2 THEN COALESCE(r.ch35,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch35,3,length(r.ch35)-1),t.precision) end as ch35,
								CASE WHEN COALESCE(length(NULLIF(r.ch36,'')),null,0) -1 <=2 THEN COALESCE(r.ch36,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch36,3,length(r.ch36)-1),t.precision) end as ch36,
								CASE WHEN COALESCE(length(NULLIF(r.ch37,'')),null,0) -1 <=2 THEN COALESCE(r.ch37,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch37,3,length(r.ch37)-1),t.precision) end as ch37,
								CASE WHEN COALESCE(length(NULLIF(r.ch38,'')),null,0) -1 <=2 THEN COALESCE(r.ch38,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch38,3,length(r.ch38)-1),t.precision) end as ch38,
								CASE WHEN COALESCE(length(NULLIF(r.ch39,'')),null,0) -1 <=2 THEN COALESCE(r.ch39,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch39,3,length(r.ch39)-1),t.precision) end as ch39,
								CASE WHEN COALESCE(length(NULLIF(r.ch40,'')),null,0) -1 <=2 THEN COALESCE(r.ch40,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch40,3,length(r.ch40)-1),t.precision) end as ch40,
								CASE WHEN COALESCE(length(NULLIF(r.ch41,'')),null,0) -1 <=2 THEN COALESCE(r.ch41,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch41,3,length(r.ch41)-1),t.precision) end as ch41,
								CASE WHEN COALESCE(length(NULLIF(r.ch42,'')),null,0) -1 <=2 THEN COALESCE(r.ch42,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch42,3,length(r.ch42)-1),t.precision) end as ch42,
								CASE WHEN COALESCE(length(NULLIF(r.ch43,'')),null,0) -1 <=2 THEN COALESCE(r.ch43,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch43,3,length(r.ch43)-1),t.precision) end as ch43,
								CASE WHEN COALESCE(length(NULLIF(r.ch44,'')),null,0) -1 <=2 THEN COALESCE(r.ch44,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch44,3,length(r.ch44)-1),t.precision) end as ch44,
								CASE WHEN COALESCE(length(NULLIF(r.ch45,'')),null,0) -1 <=2 THEN COALESCE(r.ch45,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch45,3,length(r.ch45)-1),t.precision) end as ch45,
								CASE WHEN COALESCE(length(NULLIF(r.ch46,'')),null,0) -1 <=2 THEN COALESCE(r.ch46,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch46,3,length(r.ch46)-1),t.precision) end as ch46,
								CASE WHEN COALESCE(length(NULLIF(r.ch47,'')),null,0) -1 <=2 THEN COALESCE(r.ch47,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch47,3,length(r.ch47)-1),t.precision) end as ch47,
								CASE WHEN COALESCE(length(NULLIF(r.ch48,'')),null,0) -1 <=2 THEN COALESCE(r.ch48,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch48,3,length(r.ch48)-1),t.precision) end as ch48,
								CASE WHEN COALESCE(length(NULLIF(r.ch49,'')),null,0) -1 <=2 THEN COALESCE(r.ch49,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch49,3,length(r.ch49)-1),t.precision) end as ch49,
								CASE WHEN COALESCE(length(NULLIF(r.ch50,'')),null,0) -1 <=2 THEN COALESCE(r.ch50,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch50,3,length(r.ch50)-1),t.precision) end as ch50,
								CASE WHEN COALESCE(length(NULLIF(r.ch51,'')),null,0) -1 <=2 THEN COALESCE(r.ch51,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch51,3,length(r.ch51)-1),t.precision) end as ch51,
								CASE WHEN COALESCE(length(NULLIF(r.ch52,'')),null,0) -1 <=2 THEN COALESCE(r.ch52,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch52,3,length(r.ch52)-1),t.precision) end as ch52,
								CASE WHEN COALESCE(length(NULLIF(r.ch53,'')),null,0) -1 <=2 THEN COALESCE(r.ch53,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch53,3,length(r.ch53)-1),t.precision) end as ch53,
								CASE WHEN COALESCE(length(NULLIF(r.ch54,'')),null,0) -1 <=2 THEN COALESCE(r.ch54,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch54,3,length(r.ch54)-1),t.precision) end as ch54,
								CASE WHEN COALESCE(length(NULLIF(r.ch55,'')),null,0) -1 <=2 THEN COALESCE(r.ch55,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch55,3,length(r.ch55)-1),t.precision) end as ch55,
								CASE WHEN COALESCE(length(NULLIF(r.ch56,'')),null,0) -1 <=2 THEN COALESCE(r.ch56,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch56,3,length(r.ch56)-1),t.precision) end as ch56,
								CASE WHEN COALESCE(length(NULLIF(r.ch57,'')),null,0) -1 <=2 THEN COALESCE(r.ch57,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch57,3,length(r.ch57)-1),t.precision) end as ch57,
								CASE WHEN COALESCE(length(NULLIF(r.ch58,'')),null,0) -1 <=2 THEN COALESCE(r.ch58,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch58,3,length(r.ch58)-1),t.precision) end as ch58,
								CASE WHEN COALESCE(length(NULLIF(r.ch59,'')),null,0) -1 <=2 THEN COALESCE(r.ch59,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch59,3,length(r.ch59)-1),t.precision) end as ch59,
								CASE WHEN COALESCE(length(NULLIF(r.ch60,'')),null,0) -1 <=2 THEN COALESCE(r.ch60,'')
								ELSE public.transforme_valeur_mesure(substr(r.ch60,3,length(r.ch60)-1),t.precision) end as ch60
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
					   and   r.evenement in (' ','mm','G','P','T','I','J','U','V','L') ORDER BY evenement asc,time asc,ordre asc,lettre_voie asc, nom_voie asc;
				       --and   r.evenement in (' ','m','G','P','T','I','J','U','V','L') ORDER BY r.TIME;
					   
CurRelEvAnaCQ 	CURSOR FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time,
									COALESCE(r.ordre,-99999) as ordre,COALESCE(r.evenement,'') as evenement ,
									COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie,
									CASE WHEN COALESCE(length(NULLIF(r.ch2,'')),null,0) -1 <=2 THEN COALESCE(r.ch2,'')
									ELSE 
										 CASE
												WHEN substr(r.ch2,1,2) = ('za') then '('||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) = ('zb') then ')'||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) = ('zc') then '['||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) = ('zd') then ']'||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) = ('ze') then '{'||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) = ('zf') then '}'||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch2,3,length(r.ch2)-1),'')
												WHEN substr(r.ch2,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch2,2,length(r.ch2)-1),'')
										 END
									END as ch2,
									CASE WHEN COALESCE(length(NULLIF(r.ch3,'')),null,0) -1 <=2 THEN COALESCE(r.ch3,'')
									ELSE 
										 CASE
												WHEN substr(r.ch3,1,2) = ('za') then '('||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) = ('zb') then ')'||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) = ('zc') then '['||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) = ('zd') then ']'||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) = ('ze') then '{'||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) = ('zf') then '}'||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch3,3,length(r.ch3)-1),'')
												WHEN substr(r.ch3,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch3,2,length(r.ch3)-1),'')
										 END
									END as ch3,
									CASE WHEN COALESCE(length(NULLIF(r.ch4,'')),null,0) -1 <=2 THEN COALESCE(r.ch4,'')
									ELSE 
										 CASE
												WHEN substr(r.ch4,1,2) = ('za') then '('||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) = ('zb') then ')'||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) = ('zc') then '['||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) = ('zd') then ']'||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) = ('ze') then '{'||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) = ('zf') then '}'||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch4,3,length(r.ch4)-1),'')
												WHEN substr(r.ch4,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch4,2,length(r.ch4)-1),'')
										 END
									END as ch4,
									CASE WHEN COALESCE(length(NULLIF(r.ch5,'')),null,0) -1 <=2 THEN COALESCE(r.ch5,'')
									ELSE 
										 CASE
												WHEN substr(r.ch5,1,2) = ('za') then '('||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) = ('zb') then ')'||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) = ('zc') then '['||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) = ('zd') then ']'||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) = ('ze') then '{'||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) = ('zf') then '}'||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch5,3,length(r.ch5)-1),'')
												WHEN substr(r.ch5,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch5,2,length(r.ch5)-1),'')
										 END
									END as ch5,
									CASE WHEN COALESCE(length(NULLIF(r.ch6,'')),null,0) -1 <=2 THEN COALESCE(r.ch6,'')
									ELSE 
										 CASE
												WHEN substr(r.ch6,1,2) = ('za') then '('||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) = ('zb') then ')'||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) = ('zc') then '['||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) = ('zd') then ']'||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) = ('ze') then '{'||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) = ('zf') then '}'||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch6,3,length(r.ch6)-1),'')
												WHEN substr(r.ch6,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch6,2,length(r.ch6)-1),'')
										 END
									END as ch6,
									CASE WHEN COALESCE(length(NULLIF(r.ch7,'')),null,0) -1 <=2 THEN COALESCE(r.ch7,'')
									ELSE 
										 CASE
												WHEN substr(r.ch7,1,2) = ('za') then '('||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) = ('zb') then ')'||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) = ('zc') then '['||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) = ('zd') then ']'||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) = ('ze') then '{'||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) = ('zf') then '}'||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch7,3,length(r.ch7)-1),'')
												WHEN substr(r.ch7,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch7,2,length(r.ch7)-1),'')
										 END
									END as ch7,
									CASE WHEN COALESCE(length(NULLIF(r.ch8,'')),null,0) -1 <=2 THEN COALESCE(r.ch8,'')
									ELSE 
										 CASE
												WHEN substr(r.ch8,1,2) = ('za') then '('||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) = ('zb') then ')'||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) = ('zc') then '['||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) = ('zd') then ']'||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) = ('ze') then '{'||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) = ('zf') then '}'||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch8,3,length(r.ch8)-1),'')
												WHEN substr(r.ch8,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch8,2,length(r.ch8)-1),'')
										 END
									END as ch8,
									CASE WHEN COALESCE(length(NULLIF(r.ch9,'')),null,0) -1 <=2 THEN COALESCE(r.ch9,'')
									ELSE 
										 CASE
												WHEN substr(r.ch9,1,2) = ('za') then '('||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) = ('zb') then ')'||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) = ('zc') then '['||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) = ('zd') then ']'||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) = ('ze') then '{'||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) = ('zf') then '}'||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch9,3,length(r.ch9)-1),'')
												WHEN substr(r.ch9,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch9,2,length(r.ch9)-1),'')
										 END
									END as ch9,
									CASE WHEN COALESCE(length(NULLIF(r.ch10,'')),null,0) -1 <=2 THEN COALESCE(r.ch10,'')
									ELSE 
										 CASE
												WHEN substr(r.ch10,1,2) = ('za') then '('||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) = ('zb') then ')'||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) = ('zc') then '['||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) = ('zd') then ']'||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) = ('ze') then '{'||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) = ('zf') then '}'||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch10,3,length(r.ch10)-1),'')
												WHEN substr(r.ch10,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch10,2,length(r.ch10)-1),'')
										 END
									END as ch10,
									CASE WHEN COALESCE(length(NULLIF(r.ch11,'')),null,0) -1 <=2 THEN COALESCE(r.ch11,'')
									ELSE 
										 CASE
												WHEN substr(r.ch11,1,2) = ('za') then '('||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) = ('zb') then ')'||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) = ('zc') then '['||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) = ('zd') then ']'||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) = ('ze') then '{'||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) = ('zf') then '}'||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch11,3,length(r.ch11)-1),'')
												WHEN substr(r.ch11,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch11,2,length(r.ch11)-1),'')
										 END
									END as ch11,
									CASE WHEN COALESCE(length(NULLIF(r.ch12,'')),null,0) -1 <=2 THEN COALESCE(r.ch12,'')
									ELSE 
										 CASE
												WHEN substr(r.ch12,1,2) = ('za') then '('||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) = ('zb') then ')'||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) = ('zc') then '['||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) = ('zd') then ']'||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) = ('ze') then '{'||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) = ('zf') then '}'||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch12,3,length(r.ch12)-1),'')
												WHEN substr(r.ch12,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch12,2,length(r.ch12)-1),'')
										 END
									END as ch12,
									CASE WHEN COALESCE(length(NULLIF(r.ch13,'')),null,0) -1 <=2 THEN COALESCE(r.ch13,'')
									ELSE 
										 CASE
												WHEN substr(r.ch13,1,2) = ('za') then '('||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) = ('zb') then ')'||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) = ('zc') then '['||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) = ('zd') then ']'||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) = ('ze') then '{'||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) = ('zf') then '}'||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch13,3,length(r.ch13)-1),'')
												WHEN substr(r.ch13,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch13,2,length(r.ch13)-1),'')
										 END
									END as ch13,
									CASE WHEN COALESCE(length(NULLIF(r.ch14,'')),null,0) -1 <=2 THEN COALESCE(r.ch14,'')
									ELSE 
										 CASE
												WHEN substr(r.ch14,1,2) = ('za') then '('||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) = ('zb') then ')'||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) = ('zc') then '['||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) = ('zd') then ']'||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) = ('ze') then '{'||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) = ('zf') then '}'||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch14,3,length(r.ch14)-1),'')
												WHEN substr(r.ch14,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch14,2,length(r.ch14)-1),'')
										 END
									END as ch14,
									CASE WHEN COALESCE(length(NULLIF(r.ch15,'')),null,0) -1 <=2 THEN COALESCE(r.ch15,'')
									ELSE 
										 CASE
												WHEN substr(r.ch15,1,2) = ('za') then '('||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) = ('zb') then ')'||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) = ('zc') then '['||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) = ('zd') then ']'||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) = ('ze') then '{'||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) = ('zf') then '}'||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch15,3,length(r.ch15)-1),'')
												WHEN substr(r.ch15,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch15,2,length(r.ch15)-1),'')
										 END
									END as ch15,
									CASE WHEN COALESCE(length(NULLIF(r.ch16,'')),null,0) -1 <=2 THEN COALESCE(r.ch16,'')
									ELSE 
										 CASE
												WHEN substr(r.ch16,1,2) = ('za') then '('||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) = ('zb') then ')'||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) = ('zc') then '['||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) = ('zd') then ']'||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) = ('ze') then '{'||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) = ('zf') then '}'||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch16,3,length(r.ch16)-1),'')
												WHEN substr(r.ch16,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch16,2,length(r.ch16)-1),'')
										 END
									END as ch16,
									CASE WHEN COALESCE(length(NULLIF(r.ch17,'')),null,0) -1 <=2 THEN COALESCE(r.ch17,'')
									ELSE 
										 CASE
												WHEN substr(r.ch17,1,2) = ('za') then '('||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) = ('zb') then ')'||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) = ('zc') then '['||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) = ('zd') then ']'||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) = ('ze') then '{'||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) = ('zf') then '}'||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch17,3,length(r.ch17)-1),'')
												WHEN substr(r.ch17,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch17,2,length(r.ch17)-1),'')
										 END
									END as ch17,
									CASE WHEN COALESCE(length(NULLIF(r.ch18,'')),null,0) -1 <=2 THEN COALESCE(r.ch18,'')
									ELSE 
										 CASE
												WHEN substr(r.ch18,1,2) = ('za') then '('||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) = ('zb') then ')'||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) = ('zc') then '['||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) = ('zd') then ']'||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) = ('ze') then '{'||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) = ('zf') then '}'||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch18,3,length(r.ch18)-1),'')
												WHEN substr(r.ch18,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch18,2,length(r.ch18)-1),'')
										 END
									END as ch18,
									CASE WHEN COALESCE(length(NULLIF(r.ch19,'')),null,0) -1 <=2 THEN COALESCE(r.ch19,'')
									ELSE 
										 CASE
												WHEN substr(r.ch19,1,2) = ('za') then '('||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) = ('zb') then ')'||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) = ('zc') then '['||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) = ('zd') then ']'||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) = ('ze') then '{'||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) = ('zf') then '}'||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch19,3,length(r.ch19)-1),'')
												WHEN substr(r.ch19,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch19,2,length(r.ch19)-1),'')
										 END
									END as ch19,
									CASE WHEN COALESCE(length(NULLIF(r.ch20,'')),null,0) -1 <=2 THEN COALESCE(r.ch20,'')
									ELSE 
										 CASE
												WHEN substr(r.ch20,1,2) = ('za') then '('||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) = ('zb') then ')'||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) = ('zc') then '['||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) = ('zd') then ']'||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) = ('ze') then '{'||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) = ('zf') then '}'||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch20,3,length(r.ch20)-1),'')
												WHEN substr(r.ch20,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch20,2,length(r.ch20)-1),'')
										 END
									END as ch20,
									CASE WHEN COALESCE(length(NULLIF(r.ch21,'')),null,0) -1 <=2 THEN COALESCE(r.ch21,'')
									ELSE 
										 CASE
												WHEN substr(r.ch21,1,2) = ('za') then '('||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) = ('zb') then ')'||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) = ('zc') then '['||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) = ('zd') then ']'||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) = ('ze') then '{'||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) = ('zf') then '}'||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch21,3,length(r.ch21)-1),'')
												WHEN substr(r.ch21,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch21,2,length(r.ch21)-1),'')
										 END
									END as ch21,
									CASE WHEN COALESCE(length(NULLIF(r.ch22,'')),null,0) -1 <=2 THEN COALESCE(r.ch22,'')
									ELSE 
										 CASE
												WHEN substr(r.ch22,1,2) = ('za') then '('||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) = ('zb') then ')'||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) = ('zc') then '['||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) = ('zd') then ']'||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) = ('ze') then '{'||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) = ('zf') then '}'||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch22,3,length(r.ch22)-1),'')
												WHEN substr(r.ch22,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch22,2,length(r.ch22)-1),'')
										 END
									END as ch22,
									CASE WHEN COALESCE(length(NULLIF(r.ch23,'')),null,0) -1 <=2 THEN COALESCE(r.ch23,'')
									ELSE 
										 CASE
												WHEN substr(r.ch23,1,2) = ('za') then '('||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) = ('zb') then ')'||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) = ('zc') then '['||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) = ('zd') then ']'||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) = ('ze') then '{'||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) = ('zf') then '}'||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch23,3,length(r.ch23)-1),'')
												WHEN substr(r.ch23,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch23,2,length(r.ch23)-1),'')
										 END
									END as ch23,
									CASE WHEN COALESCE(length(NULLIF(r.ch24,'')),null,0) -1 <=2 THEN COALESCE(r.ch24,'')
									ELSE 
										 CASE
												WHEN substr(r.ch24,1,2) = ('za') then '('||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) = ('zb') then ')'||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) = ('zc') then '['||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) = ('zd') then ']'||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) = ('ze') then '{'||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) = ('zf') then '}'||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch24,3,length(r.ch24)-1),'')
												WHEN substr(r.ch24,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch24,2,length(r.ch24)-1),'')
										 END
									END as ch24,
									CASE WHEN COALESCE(length(NULLIF(r.ch25,'')),null,0) -1 <=2 THEN COALESCE(r.ch25,'')
									ELSE 
										 CASE
												WHEN substr(r.ch25,1,2) = ('za') then '('||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) = ('zb') then ')'||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) = ('zc') then '['||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) = ('zd') then ']'||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) = ('ze') then '{'||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) = ('zf') then '}'||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch25,3,length(r.ch25)-1),'')
												WHEN substr(r.ch25,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch25,2,length(r.ch25)-1),'')
										 END
									END as ch25,
									CASE WHEN COALESCE(length(NULLIF(r.ch26,'')),null,0) -1 <=2 THEN COALESCE(r.ch26,'')
									ELSE 
										 CASE
												WHEN substr(r.ch26,1,2) = ('za') then '('||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) = ('zb') then ')'||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) = ('zc') then '['||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) = ('zd') then ']'||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) = ('ze') then '{'||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) = ('zf') then '}'||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch26,3,length(r.ch26)-1),'')
												WHEN substr(r.ch26,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch26,2,length(r.ch26)-1),'')
										 END
									END as ch26,
									CASE WHEN COALESCE(length(NULLIF(r.ch27,'')),null,0) -1 <=2 THEN COALESCE(r.ch27,'')
									ELSE 
										 CASE
												WHEN substr(r.ch27,1,2) = ('za') then '('||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) = ('zb') then ')'||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) = ('zc') then '['||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) = ('zd') then ']'||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) = ('ze') then '{'||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) = ('zf') then '}'||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch27,3,length(r.ch27)-1),'')
												WHEN substr(r.ch27,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch27,2,length(r.ch27)-1),'')
										 END
									END as ch27,
									CASE WHEN COALESCE(length(NULLIF(r.ch28,'')),null,0) -1 <=2 THEN COALESCE(r.ch28,'')
									ELSE 
										 CASE
												WHEN substr(r.ch28,1,2) = ('za') then '('||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) = ('zb') then ')'||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) = ('zc') then '['||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) = ('zd') then ']'||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) = ('ze') then '{'||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) = ('zf') then '}'||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch28,3,length(r.ch28)-1),'')
												WHEN substr(r.ch28,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch28,2,length(r.ch28)-1),'')
										 END
									END as ch28,
									CASE WHEN COALESCE(length(NULLIF(r.ch29,'')),null,0) -1 <=2 THEN COALESCE(r.ch29,'')
									ELSE 
										 CASE
												WHEN substr(r.ch29,1,2) = ('za') then '('||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) = ('zb') then ')'||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) = ('zc') then '['||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) = ('zd') then ']'||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) = ('ze') then '{'||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) = ('zf') then '}'||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch29,3,length(r.ch29)-1),'')
												WHEN substr(r.ch29,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch29,2,length(r.ch29)-1),'')
										 END
									END as ch29,
									CASE WHEN COALESCE(length(NULLIF(r.ch30,'')),null,0) -1 <=2 THEN COALESCE(r.ch30,'')
									ELSE 
										 CASE
												WHEN substr(r.ch30,1,2) = ('za') then '('||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) = ('zb') then ')'||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) = ('zc') then '['||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) = ('zd') then ']'||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) = ('ze') then '{'||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) = ('zf') then '}'||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch30,3,length(r.ch30)-1),'')
												WHEN substr(r.ch30,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch30,2,length(r.ch30)-1),'')
										 END
									END as ch30,
									CASE WHEN COALESCE(length(NULLIF(r.ch31,'')),null,0) -1 <=2 THEN COALESCE(r.ch31,'')
									ELSE 
										 CASE
												WHEN substr(r.ch31,1,2) = ('za') then '('||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) = ('zb') then ')'||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) = ('zc') then '['||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) = ('zd') then ']'||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) = ('ze') then '{'||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) = ('zf') then '}'||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch31,3,length(r.ch31)-1),'')
												WHEN substr(r.ch31,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch31,2,length(r.ch31)-1),'')
										 END
									END as ch31,
									CASE WHEN COALESCE(length(NULLIF(r.ch32,'')),null,0) -1 <=2 THEN COALESCE(r.ch32,'')
									ELSE 
										 CASE
												WHEN substr(r.ch32,1,2) = ('za') then '('||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) = ('zb') then ')'||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) = ('zc') then '['||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) = ('zd') then ']'||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) = ('ze') then '{'||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) = ('zf') then '}'||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch32,3,length(r.ch32)-1),'')
												WHEN substr(r.ch32,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch32,2,length(r.ch32)-1),'')
										 END
									END as ch32,
									CASE WHEN COALESCE(length(NULLIF(r.ch33,'')),null,0) -1 <=2 THEN COALESCE(r.ch33,'')
									ELSE 
										 CASE
												WHEN substr(r.ch33,1,2) = ('za') then '('||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) = ('zb') then ')'||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) = ('zc') then '['||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) = ('zd') then ']'||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) = ('ze') then '{'||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) = ('zf') then '}'||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch33,3,length(r.ch33)-1),'')
												WHEN substr(r.ch33,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch33,2,length(r.ch33)-1),'')
										 END
									END as ch33,
									CASE WHEN COALESCE(length(NULLIF(r.ch34,'')),null,0) -1 <=2 THEN COALESCE(r.ch34,'')
									ELSE 
										 CASE
												WHEN substr(r.ch34,1,2) = ('za') then '('||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) = ('zb') then ')'||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) = ('zc') then '['||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) = ('zd') then ']'||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) = ('ze') then '{'||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) = ('zf') then '}'||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch34,3,length(r.ch34)-1),'')
												WHEN substr(r.ch34,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch34,2,length(r.ch34)-1),'')
										 END
									END as ch34,
									CASE WHEN COALESCE(length(NULLIF(r.ch35,'')),null,0) -1 <=2 THEN COALESCE(r.ch35,'')
									ELSE 
										 CASE
												WHEN substr(r.ch35,1,2) = ('za') then '('||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) = ('zb') then ')'||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) = ('zc') then '['||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) = ('zd') then ']'||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) = ('ze') then '{'||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) = ('zf') then '}'||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch35,3,length(r.ch35)-1),'')
												WHEN substr(r.ch35,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch35,2,length(r.ch35)-1),'')
										 END
									END as ch35,
									CASE WHEN COALESCE(length(NULLIF(r.ch36,'')),null,0) -1 <=2 THEN COALESCE(r.ch36,'')
									ELSE 
										 CASE
												WHEN substr(r.ch36,1,2) = ('za') then '('||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) = ('zb') then ')'||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) = ('zc') then '['||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) = ('zd') then ']'||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) = ('ze') then '{'||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) = ('zf') then '}'||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch36,3,length(r.ch36)-1),'')
												WHEN substr(r.ch36,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch36,2,length(r.ch36)-1),'')
										 END
									END as ch36,
									CASE WHEN COALESCE(length(NULLIF(r.ch37,'')),null,0) -1 <=2 THEN COALESCE(r.ch37,'')
									ELSE 
										 CASE
												WHEN substr(r.ch37,1,2) = ('za') then '('||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) = ('zb') then ')'||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) = ('zc') then '['||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) = ('zd') then ']'||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) = ('ze') then '{'||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) = ('zf') then '}'||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch37,3,length(r.ch37)-1),'')
												WHEN substr(r.ch37,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch37,2,length(r.ch37)-1),'')
										 END
									END as ch37,
									CASE WHEN COALESCE(length(NULLIF(r.ch38,'')),null,0) -1 <=2 THEN COALESCE(r.ch38,'')
									ELSE 
										 CASE
												WHEN substr(r.ch38,1,2) = ('za') then '('||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) = ('zb') then ')'||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) = ('zc') then '['||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) = ('zd') then ']'||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) = ('ze') then '{'||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) = ('zf') then '}'||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch38,3,length(r.ch38)-1),'')
												WHEN substr(r.ch38,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch38,2,length(r.ch38)-1),'')
										 END
									END as ch38,
									CASE WHEN COALESCE(length(NULLIF(r.ch39,'')),null,0) -1 <=2 THEN COALESCE(r.ch39,'')
									ELSE 
										 CASE
												WHEN substr(r.ch39,1,2) = ('za') then '('||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) = ('zb') then ')'||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) = ('zc') then '['||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) = ('zd') then ']'||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) = ('ze') then '{'||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) = ('zf') then '}'||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch39,3,length(r.ch39)-1),'')
												WHEN substr(r.ch39,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch39,2,length(r.ch39)-1),'')
										 END
									END as ch39,
									CASE WHEN COALESCE(length(NULLIF(r.ch40,'')),null,0) -1 <=2 THEN COALESCE(r.ch40,'')
									ELSE 
										 CASE
												WHEN substr(r.ch40,1,2) = ('za') then '('||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) = ('zb') then ')'||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) = ('zc') then '['||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) = ('zd') then ']'||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) = ('ze') then '{'||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) = ('zf') then '}'||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch40,3,length(r.ch40)-1),'')
												WHEN substr(r.ch40,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch40,2,length(r.ch40)-1),'')
										 END
									END as ch40,
									CASE WHEN COALESCE(length(NULLIF(r.ch41,'')),null,0) -1 <=2 THEN COALESCE(r.ch41,'')
									ELSE 
										 CASE
												WHEN substr(r.ch41,1,2) = ('za') then '('||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) = ('zb') then ')'||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) = ('zc') then '['||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) = ('zd') then ']'||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) = ('ze') then '{'||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) = ('zf') then '}'||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch41,3,length(r.ch41)-1),'')
												WHEN substr(r.ch41,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch41,2,length(r.ch41)-1),'')
										 END
									END as ch41,
									CASE WHEN COALESCE(length(NULLIF(r.ch42,'')),null,0) -1 <=2 THEN COALESCE(r.ch42,'')
									ELSE 
										 CASE
												WHEN substr(r.ch42,1,2) = ('za') then '('||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) = ('zb') then ')'||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) = ('zc') then '['||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) = ('zd') then ']'||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) = ('ze') then '{'||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) = ('zf') then '}'||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch42,3,length(r.ch42)-1),'')
												WHEN substr(r.ch42,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch42,2,length(r.ch42)-1),'')
										 END
									END as ch42,
									CASE WHEN COALESCE(length(NULLIF(r.ch43,'')),null,0) -1 <=2 THEN COALESCE(r.ch43,'')
									ELSE 
										 CASE
												WHEN substr(r.ch43,1,2) = ('za') then '('||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) = ('zb') then ')'||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) = ('zc') then '['||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) = ('zd') then ']'||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) = ('ze') then '{'||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) = ('zf') then '}'||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch43,3,length(r.ch43)-1),'')
												WHEN substr(r.ch43,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch43,2,length(r.ch43)-1),'')
										 END
									END as ch43,
									CASE WHEN COALESCE(length(NULLIF(r.ch44,'')),null,0) -1 <=2 THEN COALESCE(r.ch44,'')
									ELSE 
										 CASE
												WHEN substr(r.ch44,1,2) = ('za') then '('||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) = ('zb') then ')'||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) = ('zc') then '['||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) = ('zd') then ']'||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) = ('ze') then '{'||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) = ('zf') then '}'||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch44,3,length(r.ch44)-1),'')
												WHEN substr(r.ch44,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch44,2,length(r.ch44)-1),'')
										 END
									END as ch44,
									CASE WHEN COALESCE(length(NULLIF(r.ch45,'')),null,0) -1 <=2 THEN COALESCE(r.ch45,'')
									ELSE 
										 CASE
												WHEN substr(r.ch45,1,2) = ('za') then '('||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) = ('zb') then ')'||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) = ('zc') then '['||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) = ('zd') then ']'||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) = ('ze') then '{'||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) = ('zf') then '}'||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch45,3,length(r.ch45)-1),'')
												WHEN substr(r.ch45,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch45,2,length(r.ch45)-1),'')
										 END
									END as ch45,
									CASE WHEN COALESCE(length(NULLIF(r.ch46,'')),null,0) -1 <=2 THEN COALESCE(r.ch46,'')
									ELSE 
										 CASE
												WHEN substr(r.ch46,1,2) = ('za') then '('||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) = ('zb') then ')'||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) = ('zc') then '['||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) = ('zd') then ']'||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) = ('ze') then '{'||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) = ('zf') then '}'||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch46,3,length(r.ch46)-1),'')
												WHEN substr(r.ch46,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch46,2,length(r.ch46)-1),'')
										 END
									END as ch46,
									CASE WHEN COALESCE(length(NULLIF(r.ch47,'')),null,0) -1 <=2 THEN COALESCE(r.ch47,'')
									ELSE 
										 CASE
												WHEN substr(r.ch47,1,2) = ('za') then '('||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) = ('zb') then ')'||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) = ('zc') then '['||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) = ('zd') then ']'||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) = ('ze') then '{'||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) = ('zf') then '}'||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch47,3,length(r.ch47)-1),'')
												WHEN substr(r.ch47,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch47,2,length(r.ch47)-1),'')
										 END
									END as ch47,
									CASE WHEN COALESCE(length(NULLIF(r.ch48,'')),null,0) -1 <=2 THEN COALESCE(r.ch48,'')
									ELSE 
										 CASE
												WHEN substr(r.ch48,1,2) = ('za') then '('||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) = ('zb') then ')'||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) = ('zc') then '['||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) = ('zd') then ']'||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) = ('ze') then '{'||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) = ('zf') then '}'||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch48,3,length(r.ch48)-1),'')
												WHEN substr(r.ch48,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch48,2,length(r.ch48)-1),'')
										 END
									END as ch48,
									CASE WHEN COALESCE(length(NULLIF(r.ch49,'')),null,0) -1 <=2 THEN COALESCE(r.ch49,'')
									ELSE 
										 CASE
												WHEN substr(r.ch49,1,2) = ('za') then '('||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) = ('zb') then ')'||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) = ('zc') then '['||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) = ('zd') then ']'||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) = ('ze') then '{'||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) = ('zf') then '}'||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch49,3,length(r.ch49)-1),'')
												WHEN substr(r.ch49,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch49,2,length(r.ch49)-1),'')
										 END
									END as ch49,
									CASE WHEN COALESCE(length(NULLIF(r.ch50,'')),null,0) -1 <=2 THEN COALESCE(r.ch50,'')
									ELSE 
										 CASE
												WHEN substr(r.ch50,1,2) = ('za') then '('||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) = ('zb') then ')'||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) = ('zc') then '['||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) = ('zd') then ']'||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) = ('ze') then '{'||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) = ('zf') then '}'||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch50,3,length(r.ch50)-1),'')
												WHEN substr(r.ch50,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch50,2,length(r.ch50)-1),'')
										 END
									END as ch50,
									CASE WHEN COALESCE(length(NULLIF(r.ch51,'')),null,0) -1 <=2 THEN COALESCE(r.ch51,'')
									ELSE 
										 CASE
												WHEN substr(r.ch51,1,2) = ('za') then '('||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) = ('zb') then ')'||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) = ('zc') then '['||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) = ('zd') then ']'||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) = ('ze') then '{'||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) = ('zf') then '}'||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch51,3,length(r.ch51)-1),'')
												WHEN substr(r.ch51,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch51,2,length(r.ch51)-1),'')
										 END
									END as ch51,
									CASE WHEN COALESCE(length(NULLIF(r.ch52,'')),null,0) -1 <=2 THEN COALESCE(r.ch52,'')
									ELSE 
										 CASE
												WHEN substr(r.ch52,1,2) = ('za') then '('||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) = ('zb') then ')'||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) = ('zc') then '['||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) = ('zd') then ']'||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) = ('ze') then '{'||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) = ('zf') then '}'||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch52,3,length(r.ch52)-1),'')
												WHEN substr(r.ch52,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch52,2,length(r.ch52)-1),'')
										 END
									END as ch52,
									CASE WHEN COALESCE(length(NULLIF(r.ch53,'')),null,0) -1 <=2 THEN COALESCE(r.ch53,'')
									ELSE 
										 CASE
												WHEN substr(r.ch53,1,2) = ('za') then '('||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) = ('zb') then ')'||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) = ('zc') then '['||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) = ('zd') then ']'||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) = ('ze') then '{'||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) = ('zf') then '}'||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch53,3,length(r.ch53)-1),'')
												WHEN substr(r.ch53,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch53,2,length(r.ch53)-1),'')
										 END
									END as ch53,
									CASE WHEN COALESCE(length(NULLIF(r.ch54,'')),null,0) -1 <=2 THEN COALESCE(r.ch54,'')
									ELSE 
										 CASE
												WHEN substr(r.ch54,1,2) = ('za') then '('||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) = ('zb') then ')'||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) = ('zc') then '['||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) = ('zd') then ']'||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) = ('ze') then '{'||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) = ('zf') then '}'||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch54,3,length(r.ch54)-1),'')
												WHEN substr(r.ch54,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch54,2,length(r.ch54)-1),'')
										 END
									END as ch54,
									CASE WHEN COALESCE(length(NULLIF(r.ch55,'')),null,0) -1 <=2 THEN COALESCE(r.ch55,'')
									ELSE 
										 CASE
												WHEN substr(r.ch55,1,2) = ('za') then '('||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) = ('zb') then ')'||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) = ('zc') then '['||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) = ('zd') then ']'||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) = ('ze') then '{'||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) = ('zf') then '}'||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch55,3,length(r.ch55)-1),'')
												WHEN substr(r.ch55,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch55,2,length(r.ch55)-1),'')
										 END
									END as ch55,
									CASE WHEN COALESCE(length(NULLIF(r.ch56,'')),null,0) -1 <=2 THEN COALESCE(r.ch56,'')
									ELSE 
										 CASE
												WHEN substr(r.ch56,1,2) = ('za') then '('||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) = ('zb') then ')'||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) = ('zc') then '['||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) = ('zd') then ']'||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) = ('ze') then '{'||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) = ('zf') then '}'||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch56,3,length(r.ch56)-1),'')
												WHEN substr(r.ch56,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch56,2,length(r.ch56)-1),'')
										 END
									END as ch56,
									CASE WHEN COALESCE(length(NULLIF(r.ch57,'')),null,0) -1 <=2 THEN COALESCE(r.ch57,'')
									ELSE 
										 CASE
												WHEN substr(r.ch57,1,2) = ('za') then '('||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) = ('zb') then ')'||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) = ('zc') then '['||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) = ('zd') then ']'||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) = ('ze') then '{'||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) = ('zf') then '}'||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch57,3,length(r.ch57)-1),'')
												WHEN substr(r.ch57,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch57,2,length(r.ch57)-1),'')
										 END
									END as ch57,
									CASE WHEN COALESCE(length(NULLIF(r.ch58,'')),null,0) -1 <=2 THEN COALESCE(r.ch58,'')
									ELSE 
										 CASE
												WHEN substr(r.ch58,1,2) = ('za') then '('||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) = ('zb') then ')'||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) = ('zc') then '['||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) = ('zd') then ']'||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) = ('ze') then '{'||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) = ('zf') then '}'||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch58,3,length(r.ch58)-1),'')
												WHEN substr(r.ch58,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch58,2,length(r.ch58)-1),'')
										 END
									END as ch58,
									CASE WHEN COALESCE(length(NULLIF(r.ch59,'')),null,0) -1 <=2 THEN COALESCE(r.ch59,'')
									ELSE 
										 CASE
												WHEN substr(r.ch59,1,2) = ('za') then '('||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) = ('zb') then ')'||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) = ('zc') then '['||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) = ('zd') then ']'||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) = ('ze') then '{'||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) = ('zf') then '}'||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch59,3,length(r.ch59)-1),'')
												WHEN substr(r.ch59,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch59,2,length(r.ch59)-1),'')
										 END
									END as ch59,
									CASE WHEN COALESCE(length(NULLIF(r.ch60,'')),null,0) -1 <=2 THEN COALESCE(r.ch60,'')
									ELSE 
										 CASE
												WHEN substr(r.ch60,1,2) = ('za') then '('||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) = ('zb') then ')'||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) = ('zc') then '['||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) = ('zd') then ']'||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) = ('ze') then '{'||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) = ('zf') then '}'||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) = ('z1') then 'z'||COALESCE(substr(r.ch60,3,length(r.ch60)-1),'')
												WHEN substr(r.ch60,1,2) not in ('za','zb','zc','zd','ze','zf') then COALESCE(substr(r.ch60,2,length(r.ch60)-1),'')
										 END
									END as ch60
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
					   and   r.evenement in ('C','Q','X','N','B','M','Y') ORDER BY evenement asc,time asc,ordre asc,lettre_voie asc, nom_voie asc;
					   
CurRelEvDollar CURSOR (v_station character varying (10)) FOR SELECT COALESCE(r.time,'') as time,
										 CASE 
										 WHEN r.ch2='aa9999' THEN 
											 CASE
												 WHEN COALESCE(substr(r.ch3,3,2),'')='21' THEN
													  (select public.nom_voie(v_station,COALESCE(substr(r.ch3,5,2),''),public.type_resobs(v_station),'Début de défaut capteur','Début de défaut capteur de nom ???'))
												 
												 WHEN COALESCE(substr(r.ch3,3,2),'')='20' THEN 
												      (select public.nom_voie(v_station,COALESCE(substr(r.ch3,5,2),''),public.type_resobs(v_station),'Fin de défaut capteur','Fin de défaut capteur de nom ???'))
										
												 WHEN COALESCE(substr(r.ch3,3,2),'')='10' THEN 
													  (select public.nom_voie(v_station,COALESCE(substr(r.ch3,5,2),''),'ET','Fin déf état','Fin déf voie état de nom ???'))
																 
												 WHEN COALESCE(substr(r.ch3,3,2),'')='11' THEN 
													  (select public.nom_voie(v_station,COALESCE(substr(r.ch3,5,2),''),'ET','Début déf état','Début déf voie état de nom ???'))																 																									 
											 END
										 WHEN r.ch2='aa8888' THEN
											 CASE
												 WHEN COALESCE(substr(r.ch3,1,6),'')='bb0001' THEN 'Début de session (AutoShut)'
												 WHEN COALESCE(substr(r.ch3,1,6),'')='bb0002' THEN 'Fin de session (AutoShut)'
												 WHEN COALESCE(substr(r.ch3,1,6),'') not in ('bb0001','bb0002') THEN
                                                      CASE
															WHEN COALESCE(substr(r.ch3,6,1),'')='1' THEN 'Début de session'
															WHEN COALESCE(substr(r.ch3,6,1),'')='2' THEN 'Fin de session'
															WHEN COALESCE(substr(r.ch3,6,1),'') not in ('1','2') THEN 'DEFAUT DANS CETTE LIGNE'
													  END
											 END
										 WHEN r.ch2 not in ('aa8888','aa9999') THEN 
											 CASE
												   WHEN COALESCE(substr(r.ch3,6,1),'')='1' THEN 'Début de session'
												   WHEN COALESCE(substr(r.ch3,6,1),'')='2' THEN 'Fin de session'
												   WHEN COALESCE(substr(r.ch3,6,1),'') not in ('bb0001','bb0002') THEN 'DEFAUT DANS CETTE LIGNE'
	                                         END
										 END as libelle_evenement 
					   FROM 	sh_pd.tmp_load_file_jour_releves r
					   where    r.evenement in ('$') ORDER BY time asc;
					   
					   
CurRelEvAnaK CURSOR
FOR SELECT 	distinct COALESCE(r.fichier,'') as fichier, COALESCE(r.time,'') as time,
								COALESCE(r.ordre,-99999) as ordre,COALESCE(r.evenement,'') as evenement ,
                                COALESCE(t.nom_voie,'') as nom_voie ,COALESCE(t.lettre_voie,'') as lettre_voie,
								public.transforme_valeur_mesure_K(r.ch2,t.precision) as ch2,public.transforme_valeur_mesure_K(r.ch3,t.precision) as ch3,
								public.transforme_valeur_mesure_K(r.ch4,t.precision) as ch4,public.transforme_valeur_mesure_K(r.ch5,t.precision) as ch5,
								public.transforme_valeur_mesure_K(r.ch6,t.precision) as ch6,public.transforme_valeur_mesure_K(r.ch7,t.precision) as ch7,
								public.transforme_valeur_mesure_K(r.ch8,t.precision) as ch8,public.transforme_valeur_mesure_K(r.ch9,t.precision) as ch9,
								public.transforme_valeur_mesure_K(r.ch10,t.precision) as ch10,public.transforme_valeur_mesure_K(r.ch11,t.precision) as ch11,
								public.transforme_valeur_mesure_K(r.ch12,t.precision) as ch12,public.transforme_valeur_mesure_K(r.ch13,t.precision) as ch13,
								public.transforme_valeur_mesure_K(r.ch14,t.precision) as ch14,public.transforme_valeur_mesure_K(r.ch15,t.precision) as ch15,
								public.transforme_valeur_mesure_K(r.ch16,t.precision) as ch16,public.transforme_valeur_mesure_K(r.ch17,t.precision) as ch17,
								public.transforme_valeur_mesure_K(r.ch18,t.precision) as ch18,public.transforme_valeur_mesure_K(r.ch19,t.precision) as ch19,
								public.transforme_valeur_mesure_K(r.ch20,t.precision) as ch20,public.transforme_valeur_mesure_K(r.ch21,t.precision) as ch21,
								public.transforme_valeur_mesure_K(r.ch22,t.precision) as ch22,public.transforme_valeur_mesure_K(r.ch23,t.precision) as ch23,
								public.transforme_valeur_mesure_K(r.ch24,t.precision) as ch24,public.transforme_valeur_mesure_K(r.ch25,t.precision) as ch25,
								public.transforme_valeur_mesure_K(r.ch26,t.precision) as ch26,public.transforme_valeur_mesure_K(r.ch27,t.precision) as ch27,
								public.transforme_valeur_mesure_K(r.ch28,t.precision) as ch28,public.transforme_valeur_mesure_K(r.ch29,t.precision) as ch29,
								public.transforme_valeur_mesure_K(r.ch30,t.precision) as ch30,public.transforme_valeur_mesure_K(r.ch31,t.precision) as ch31,
								public.transforme_valeur_mesure_K(r.ch32,t.precision) as ch32,public.transforme_valeur_mesure_K(r.ch33,t.precision) as ch33,
								public.transforme_valeur_mesure_K(r.ch34,t.precision) as ch34,public.transforme_valeur_mesure_K(r.ch35,t.precision) as ch35,
								public.transforme_valeur_mesure_K(r.ch36,t.precision) as ch36,public.transforme_valeur_mesure_K(r.ch37,t.precision) as ch37,
								public.transforme_valeur_mesure_K(r.ch38,t.precision) as ch38,public.transforme_valeur_mesure_K(r.ch39,t.precision) as ch39,
								public.transforme_valeur_mesure_K(r.ch40,t.precision) as ch40,public.transforme_valeur_mesure_K(r.ch41,t.precision) as ch41,
								public.transforme_valeur_mesure_K(r.ch42,t.precision) as ch42,public.transforme_valeur_mesure_K(r.ch43,t.precision) as ch43,
								public.transforme_valeur_mesure_K(r.ch44,t.precision) as ch44,public.transforme_valeur_mesure_K(r.ch45,t.precision) as ch45,
								public.transforme_valeur_mesure_K(r.ch46,t.precision) as ch46,public.transforme_valeur_mesure_K(r.ch47,t.precision) as ch47,
								public.transforme_valeur_mesure_K(r.ch48,t.precision) as ch48,public.transforme_valeur_mesure_K(r.ch49,t.precision) as ch49,
								public.transforme_valeur_mesure_K(r.ch50,t.precision) as ch50,public.transforme_valeur_mesure_K(r.ch51,t.precision) as ch51,
								public.transforme_valeur_mesure_K(r.ch52,t.precision) as ch52,public.transforme_valeur_mesure_K(r.ch53,t.precision) as ch53,
								public.transforme_valeur_mesure_K(r.ch54,t.precision) as ch54,public.transforme_valeur_mesure_K(r.ch55,t.precision) as ch55,
								public.transforme_valeur_mesure_K(r.ch56,t.precision) as ch56,public.transforme_valeur_mesure_K(r.ch57,t.precision) as ch57,
								public.transforme_valeur_mesure_K(r.ch58,t.precision) as ch58,public.transforme_valeur_mesure_K(r.ch59,t.precision) as ch59,
								public.transforme_valeur_mesure_K(r.ch60,t.precision) as ch60
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
					   and   r.evenement in ('K') ORDER BY time asc,ordre asc,lettre_voie asc, nom_voie asc;					 
			
error_msg text;

BEGIN

table_tmp_releves:='table_temporaire_releves';
table_tmp_entetes:='table_temporaire_entetes';
table_tmp_position:='table_temporaire_position';
table_log='sh_'||lower(station)||'.load_log';

table_releves:='sh_'||lower(station)||'.'||lower(chaine_table_releves);
table_entetes:='sh_'||lower(station)||'.'||lower(chaine_table_entetes);
table_archivage:='sh_'||lower(station)||'.archives_releves';
table_log_archivage:='sh_'||lower(station)||'.archives_releves_log';
table_position:='sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive';

BEGIN
v_execute_1:='DROP TABLE '||table_tmp_position;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;
END;

BEGIN
v_execute_1:='CREATE TEMPORARY TABLE '||table_tmp_position||' AS SELECT * FROM '||table_position;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR1 dans %',v_execute_1;message_retour:='NOK1 : '||v_execute_1;
END;

BEGIN
v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_position_1';
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
END;

BEGIN
v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_position_1
ON table_temporaire_position USING btree
(position)
TABLESPACE pg_default';
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR2 dans %',v_execute_1;message_retour:='NOK2 : '||v_execute_1;
END;

BEGIN
v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_position_2';
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
END;

BEGIN
v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_position_2
ON table_temporaire_position USING btree
(lettre_voie  COLLATE pg_catalog."default" ASC NULLS LAST)
TABLESPACE pg_default';
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR3 dans %',v_execute_1;message_retour:='NOK3 : '||v_execute_1;
END;
	

BEGIN
v_execute_1:='DROP TABLE '||table_tmp_releves;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;
END;

BEGIN
v_execute_1:='CREATE TEMPORARY TABLE '||table_tmp_releves||' AS SELECT * FROM '||table_releves;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR4 dans %',v_execute_1;message_retour:='NOK4 : '||v_execute_1;
END;

BEGIN
v_execute_1:='DROP TABLE '||table_tmp_entetes;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;
END;

BEGIN
v_execute_1:='CREATE TEMPORARY TABLE '||table_tmp_entetes||' AS SELECT * FROM '||table_entetes;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR5 dans %',v_execute_1;message_retour:='NOK5 : '||v_execute_1;
END;

	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_releves_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
    BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_releves_1
    ON table_temporaire_releves USING btree
    (time, fichier COLLATE pg_catalog."default" ASC NULLS LAST, evenement COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR6 dans %',v_execute_1;message_retour:='NOK6 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_releves_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_releves_2
    ON table_temporaire_releves USING btree
	(time, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR7 dans %',v_execute_1;message_retour:='NOK7 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_releves_3';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_releves_3
    ON table_temporaire_releves USING btree
    (time)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR8 dans %',v_execute_1;message_retour:='NOK8 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_releves_4';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_releves_4
    ON table_temporaire_releves USING btree
    (fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR9 dans %',v_execute_1;message_retour:='NOK9 : '||v_execute_1;
	END;
	--
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_entetes_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_entetes_1
    ON table_temporaire_entetes USING btree
    (lettre_voie COLLATE pg_catalog."default" ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR10 dans %',v_execute_1;message_retour:='NOK10 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_entetes_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_entetes_2
    ON table_temporaire_entetes USING btree
    (lettre_voie COLLATE pg_catalog."default" ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR11 dans %',v_execute_1;message_retour:='NOK11 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_entetes_3';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_entetes_3
    ON table_temporaire_entetes USING btree
    (lettre_voie ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR12 dans %',v_execute_1;message_retour:='NOK12 : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_table_temporaire_entetes_4';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_table_temporaire_entetes_4
    ON table_temporaire_entetes USING btree
    (fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR13 dans %',v_execute_1;message_retour:='NOK13 : '||v_execute_1;
	END;

BEGIN
v_execute_1:='ANALYSE '||table_releves;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR14 dans %',v_execute_1;message_retour:='NOK14 : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_entetes;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR15 dans %',v_execute_1;message_retour:='NOK15 : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_tmp_releves;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR16 dans %',v_execute_1;message_retour:='NOK16 : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_tmp_entetes;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR17 dans %',v_execute_1;message_retour:='NOK17 : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_position;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR18 dans %',v_execute_1;message_retour:='NOK18 : '||v_execute_1;
END;

BEGIN
v_execute_1:='TRUNCATE TABLE '||table_archivage;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR19 dans %',v_execute_1;message_retour:='NOK19 : '||v_execute_1;
END;

BEGIN
v_execute_1:='TRUNCATE TABLE '||table_log_archivage;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR20 dans %',v_execute_1;message_retour:='NOK20 : '||v_execute_1;
END;

RAISE NOTICE 'AVANT LOOP E';

v_libelle_evenement:='Changement de jour';

select lower(nom_voie) into v_column_name_1 from table_temporaire_position where position=4;
select lower(nom_voie) into v_column_name_2 from table_temporaire_position where position=5;

FOR CUR IN CurRelEvE LOOP

	--RAISE NOTICE 'DANS LOOP E';
	
	v_time:=CUR.time;
	v_evenement:=CUR.evenement;

	v_ch1:=CUR.ch1;
	v_ch2:=CUR.ch2;
	
	

	BEGIN
	v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,'||v_column_name_1||','||v_column_name_2||','||'libelle_evenement) 
	values('||''''||v_time||''''||','||''''||v_evenement||''''||','||''''||v_ch1||''''||','||''''||v_ch2||''''||','||''''||v_libelle_evenement||''''||')';
	--RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR21 dans %',v_execute_1;message_retour:='NOK21 : '||v_execute_1;
	END;

END LOOP;

RAISE NOTICE 'AVANT LOOP evenement analogique';


v_first_chaine_column_name:='';
v_first_chaine_valeur_calc:='';
ligne_to_insert:=0;

c_compte_nb_ch_chaine_champ:=0;
c_compte_nb_ch_chaine_valeur:=0;

OPEN CurRelEvAnaPrec;
FETCH FIRST FROM CurRelEvAnaPrec INTO   v_first_fichier,v_first_time,v_first_ordre,v_first_evenement,v_first_nom_voie,v_first_lettre_voie,v_first_ch2,v_first_ch3,v_first_ch4,v_first_ch5,v_first_ch6,v_first_ch7,v_first_ch8,v_first_ch9,v_first_ch10,v_first_ch11,v_first_ch12,v_first_ch13,v_first_ch14,
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
						  
	RAISE NOTICE 'v_column_name %-%',v_first_chaine_valeur_calc,v_first_ch2;
	
v_chaine_column_name:=v_first_chaine_column_name;
v_chaine_valeur_calc:='';
ligne_to_insert:=0;
c_compte_nb_ch_chaine_champ:=1;

if v_first_evenement in (' ','mm','G','P','T')
then
	v_first_libelle_evenement:='mesure';
elsif v_first_evenement = 'I'
then
	v_first_libelle_evenement:='événement initial';
elsif v_first_evenement = 'J'
then
	v_first_libelle_evenement:='événement journalier';
elsif v_first_evenement = 'U'
then
	v_first_libelle_evenement:='modification seuil';
elsif v_first_evenement = 'V'
then
	v_first_libelle_evenement:='modification delta';
elsif v_first_evenement = 'L'
then
	v_first_libelle_evenement:='événement sur vidage';
end if;
 
LOOP
EXIT WHEN NOT FOUND;
FETCH NEXT FROM CurRelEvAnaPrec INTO    v_fichier,v_time,v_ordre,v_evenement,v_nom_voie,v_lettre_voie,v_ch2,v_ch3,v_ch4,v_ch5,v_ch6,v_ch7,v_ch8,v_ch9,v_ch10,v_ch11,v_ch12,v_ch13,v_ch14,
										v_ch15,v_ch16,v_ch17,v_ch18,v_ch19,v_ch20,v_ch21,v_ch22,v_ch23,v_ch24,v_ch25,v_ch26,v_ch27,v_ch28,v_ch29,v_ch30,
										v_ch31,v_ch32,v_ch33,v_ch34,v_ch35,v_ch36,v_ch37,v_ch38,v_ch39,v_ch40,v_ch41,v_ch42,v_ch43,v_ch44,v_ch45,v_ch46,
										v_ch47,v_ch48,v_ch49,v_ch50,v_ch51,v_ch52,v_ch53,v_ch54,v_ch55,v_ch56,v_ch57,v_ch58,v_ch59,v_ch60;

--RAISE NOTICE 'v_fichier=%',v_fichier;

if v_evenement in (' ','mm','G','P','T')
then
	v_libelle_evenement:='mesure';
elsif v_evenement = 'I'
then
	v_libelle_evenement:='événement initial';
elsif v_evenement = 'J'
then
	v_libelle_evenement:='événement journalier';
elsif v_evenement = 'U'
then
	v_libelle_evenement:='modification seuil';
elsif v_evenement = 'V'
then
	v_libelle_evenement:='modification delta';
elsif v_evenement = 'L'
then
	v_libelle_evenement:='événement sur vidage';
end if;

--if i > 10000 then exit; end if;
if j > 100000 then j:=1; RAISE NOTICE '100000 lignes inserees %',now(); j:=1; end if;

	if v_time = v_first_time and v_ordre = v_first_ordre and v_evenement=v_first_evenement and v_fichier=v_first_fichier
	then
			if POSITION(v_nom_voie IN v_first_chaine_column_name)=0
			then
				v_first_chaine_column_name:=v_first_chaine_column_name||','||v_nom_voie;
				ligne_to_insert:=0;
				c_compte_nb_ch_chaine_champ:=c_compte_nb_ch_chaine_champ+1;
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
				WHEN OTHERS THEN message_retour:='NOK233';-- RAISE NOTICE 'ERREUR233 dans %',v_execute_2;message_retour:='NOK233';
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
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,'||v_first_chaine_column_name||')'||
						 ' values('||''''||v_first_time||''''||','||''''||v_first_evenement||''''||','||''''||v_first_libelle_evenement||''''||','||''''||v_first_chaine_valeur_calc||''''||')';
				
			IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
			and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_evenement,'') IS NOT NULL and NULLIF(v_libelle_evenement,'') IS NOT NULL 
			and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR22 dans %',v_execute_1;message_retour:='NOK22';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				v_execute_1:='insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
						     ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
							 COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
                v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK23'; RAISE NOTICE 'ERREUR23 fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK23';
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
				v_ch35:=v_ch35||',';
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
CLOSE CurRelEvAnaPrec;

-- penser à ajouter la dernière ligne

RAISE NOTICE 'APRES LOOP evenement';

RAISE NOTICE 'AVANT LOOP C et Q';

v_first_chaine_column_name:='';
v_first_chaine_valeur_calc:='';
ligne_to_insert:=0;

c_compte_nb_ch_chaine_champ:=0;
c_compte_nb_ch_chaine_valeur:=0;

OPEN CurRelEvAnaCQ;
FETCH FIRST FROM CurRelEvAnaCQ INTO   v_first_fichier,v_first_time,v_first_ordre,v_first_evenement,v_first_nom_voie,v_first_lettre_voie,v_first_ch2,v_first_ch3,v_first_ch4,v_first_ch5,v_first_ch6,v_first_ch7,v_first_ch8,v_first_ch9,v_first_ch10,v_first_ch11,v_first_ch12,v_first_ch13,v_first_ch14,
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
						  
	RAISE NOTICE 'v_column_name %-%',v_first_chaine_valeur_calc,v_first_ch2;
	
v_chaine_column_name:=v_first_chaine_column_name;
v_chaine_valeur_calc:='';
ligne_to_insert:=0;
c_compte_nb_ch_chaine_champ:=1;

if v_first_evenement = 'C'
then
	v_first_libelle_evenement:='Modif cadence';
elsif v_first_evenement = 'Q'
then
	v_first_libelle_evenement:='Modif config';
elsif v_first_evenement = 'X'
then
	v_first_libelle_evenement:='reset';
elsif v_first_evenement = 'N'
then
	v_first_libelle_evenement:='effacement mémoire';
elsif v_first_evenement = 'B'
then
	v_first_libelle_evenement:='bouclage jour';
elsif v_first_evenement = 'M'
then
	v_first_libelle_evenement:='bouclage mémoire';
elsif v_first_evenement = 'Y'
then
	v_first_libelle_evenement:='coupure secteur';
end if;

LOOP
EXIT WHEN NOT FOUND;
FETCH NEXT FROM CurRelEvAnaCQ INTO    v_fichier,v_time,v_ordre,v_evenement,v_nom_voie,v_lettre_voie,v_ch2,v_ch3,v_ch4,v_ch5,v_ch6,v_ch7,v_ch8,v_ch9,v_ch10,v_ch11,v_ch12,v_ch13,v_ch14,
									  v_ch15,v_ch16,v_ch17,v_ch18,v_ch19,v_ch20,v_ch21,v_ch22,v_ch23,v_ch24,v_ch25,v_ch26,v_ch27,v_ch28,v_ch29,v_ch30,
									  v_ch31,v_ch32,v_ch33,v_ch34,v_ch35,v_ch36,v_ch37,v_ch38,v_ch39,v_ch40,v_ch41,v_ch42,v_ch43,v_ch44,v_ch45,v_ch46,
									  v_ch47,v_ch48,v_ch49,v_ch50,v_ch51,v_ch52,v_ch53,v_ch54,v_ch55,v_ch56,v_ch57,v_ch58,v_ch59,v_ch60;

--RAISE NOTICE 'v_fichier=%',v_fichier;

if v_evenement = 'C'
then
	v_libelle_evenement:='Modif cadence';
elsif v_evenement = 'Q'
then
	v_libelle_evenement:='Modif config';
elsif v_evenement = 'X'
then
	v_libelle_evenement:='reset';
elsif v_evenement = 'N'
then
	v_libelle_evenement:='effacement mémoire';
elsif v_evenement = 'B'
then
	v_libelle_evenement:='bouclage jour';
elsif v_evenement = 'M'
then
	v_libelle_evenement:='bouclage mémoire';
elsif v_evenement = 'Y'
then
	v_libelle_evenement:='coupure secteur';
end if;

--if i > 10000 then exit; end if;
if j > 100000 then j:=1; RAISE NOTICE '100000 lignes inserees %',now(); j:=1; end if;

	if v_time = v_first_time and v_ordre = v_first_ordre and v_evenement=v_first_evenement and v_fichier=v_first_fichier
	then
			if POSITION(v_nom_voie IN v_first_chaine_column_name)=0
			then
				v_first_chaine_column_name:=v_first_chaine_column_name||','||v_nom_voie;
				ligne_to_insert:=0;
				c_compte_nb_ch_chaine_champ:=c_compte_nb_ch_chaine_champ+1;
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
				WHEN OTHERS THEN message_retour:='NOK233';-- RAISE NOTICE 'ERREUR233 dans %',v_execute_2;message_retour:='NOK233';
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
			
			v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,'||v_first_chaine_column_name||')'||
						 ' values('||''''||v_first_time||''''||','||''''||v_first_evenement||''''||','||''''||v_first_libelle_evenement||''''||','||''''||v_first_chaine_valeur_calc||''''||')';
				
			IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
			and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_evenement,'') IS NOT NULL and NULLIF(v_libelle_evenement,'') IS NOT NULL 
			and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur
			THEN
				i:=i+1;
				j:=j+1;
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR22 dans %',v_execute_1;message_retour:='NOK22';
				--WHEN OTHERS THEN CONTINUE;
				END;
			ELSE
				i:=i+1;
				--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
				v_execute_1:='insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
						     ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
							 COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
                v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
				v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
				BEGIN
				--RAISE NOTICE 'commande = %',v_execute_1;		
				EXECUTE v_execute_2;
				EXCEPTION
				WHEN OTHERS THEN message_retour:='NOK23'; RAISE NOTICE 'ERREUR23 fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK23';
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
				v_ch35:=v_ch35||',';
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
CLOSE CurRelEvAnaCQ;

RAISE NOTICE 'APRES LOOP Q et C';

RAISE NOTICE 'AVANT LOOP DOLLARS';

FOR CUR IN CurRelEvDollar(station)
LOOP
				
	v_first_time:=CUR.time;
	v_libelle_evenement:=CUR.libelle_evenement;
				
    v_execute_1:='insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement)'||
				 ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||'$'||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||')';
							 
    --RAISE NOTICE 'v_execute_1=%',v_execute_1;							 
			
	BEGIN
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN message_retour:='NOK24'; RAISE NOTICE 'ERREUR24 dans v_execute_1=%',v_execute_1;message_retour:='NOK24';
				--WHEN OTHERS THEN CONTINUE;
	END;
				
END LOOP;

RAISE NOTICE 'FIN LOOP DOLLARS';

RAISE NOTICE 'DEBUT LOOP K';

v_first_chaine_column_name:='';
v_first_chaine_valeur_calc:='';
ligne_to_insert:=0;

c_compte_nb_ch_chaine_champ:=0;
c_compte_nb_ch_chaine_valeur:=0;

OPEN CurRelEvAnaK;
FETCH FIRST FROM CurRelEvAnaK INTO    v_first_fichier,v_first_time,v_first_ordre,v_first_evenement,v_first_nom_voie,v_first_lettre_voie,v_first_ch2,v_first_ch3,v_first_ch4,v_first_ch5,v_first_ch6,v_first_ch7,v_first_ch8,v_first_ch9,v_first_ch10,v_first_ch11,v_first_ch12,v_first_ch13,v_first_ch14,
									  v_first_ch15,v_first_ch16,v_first_ch17,v_first_ch18,v_first_ch19,v_first_ch20,v_first_ch21,v_first_ch22,v_first_ch23,v_first_ch24,v_first_ch25,v_first_ch26,v_first_ch27,v_first_ch28,v_first_ch29,v_first_ch30,
									  v_first_ch31,v_first_ch32,v_first_ch33,v_first_ch34,v_first_ch35,v_first_ch36,v_first_ch37,v_first_ch38,v_first_ch39,v_first_ch40,v_first_ch41,v_first_ch42,v_first_ch43,v_first_ch44,v_first_ch45,v_first_ch46,
									  v_first_ch47,v_first_ch48,v_first_ch49,v_first_ch50,v_first_ch51,v_first_ch52,v_first_ch53,v_first_ch54,v_first_ch55,v_first_ch56,v_first_ch57,v_first_ch58,v_first_ch59,v_first_ch60;

	v_first_chaine_column_name:=v_first_nom_voie;

	if NULLIF(v_first_ch2,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch3,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch4,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch5,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch6,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch7,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch8,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch9,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch10,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch11,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch12,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch13,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch14,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch15,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch16,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch17,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch18,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch19,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch20,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch21,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch22,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch23,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch24,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch25,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch26,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch27,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch28,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch29,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch30,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch31,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch32,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch33,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch34,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch35,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch36,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch37,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch38,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch39,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch40,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch41,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch42,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch43,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch44,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch45,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch46,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch47,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch48,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch49,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch50,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch51,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch52,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch53,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch54,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch55,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch56,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch57,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch58,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch59,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	if NULLIF(v_first_ch60,'') is not null
	then
		c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
	end if;
	
	v_first_chaine_valeur_calc:=v_first_ch2||v_first_ch3||v_first_ch4||v_first_ch5||v_first_ch6||v_first_ch7||v_first_ch8||v_first_ch9||v_first_ch10||v_first_ch11||v_first_ch12||v_first_ch13||
								v_first_ch14||v_first_ch15||v_first_ch16||v_first_ch17||v_first_ch18||v_first_ch19||v_first_ch20||v_first_ch21||v_first_ch22||v_first_ch23||v_first_ch24||
								v_first_ch25||v_first_ch26||v_first_ch27||v_first_ch28||v_first_ch29||v_first_ch30||v_first_ch31||v_first_ch32||v_first_ch33||v_first_ch34||v_first_ch35||
								v_first_ch36||v_first_ch37||v_first_ch38||v_first_ch39||v_first_ch40||v_first_ch41||v_first_ch42||v_first_ch43||v_first_ch44||v_first_ch45||v_first_ch46||
								v_first_ch47||v_first_ch48||v_first_ch49||v_first_ch50||v_first_ch51||v_first_ch52||v_first_ch53||v_first_ch54||v_first_ch55||v_first_ch56||v_first_ch57||
								v_first_ch58||v_first_ch59||v_first_ch60;
						  
	RAISE NOTICE 'v_column_name %-%',v_first_chaine_valeur_calc,v_first_ch2;
	
v_chaine_column_name:=v_first_chaine_column_name;
v_chaine_valeur_calc:='';
ligne_to_insert:=0;
c_compte_nb_ch_chaine_champ:=1;
 
LOOP
EXIT WHEN NOT FOUND;
FETCH NEXT FROM CurRelEvAnaK INTO   v_fichier,v_time,v_ordre,v_evenement,v_nom_voie,v_lettre_voie,v_ch2,v_ch3,v_ch4,v_ch5,v_ch6,v_ch7,v_ch8,v_ch9,v_ch10,v_ch11,v_ch12,v_ch13,v_ch14,
									v_ch15,v_ch16,v_ch17,v_ch18,v_ch19,v_ch20,v_ch21,v_ch22,v_ch23,v_ch24,v_ch25,v_ch26,v_ch27,v_ch28,v_ch29,v_ch30,
									v_ch31,v_ch32,v_ch33,v_ch34,v_ch35,v_ch36,v_ch37,v_ch38,v_ch39,v_ch40,v_ch41,v_ch42,v_ch43,v_ch44,v_ch45,v_ch46,
									v_ch47,v_ch48,v_ch49,v_ch50,v_ch51,v_ch52,v_ch53,v_ch54,v_ch55,v_ch56,v_ch57,v_ch58,v_ch59,v_ch60;

--if i > 10000 then exit; end if;
if j > 100000 then j:=1; RAISE NOTICE '100000 lignes inserees %',now(); j:=1; end if;

	if v_time = v_first_time and v_ordre = v_first_ordre and v_evenement=v_first_evenement and v_fichier=v_first_fichier
	then
			if POSITION(v_nom_voie IN v_first_chaine_column_name)=0
			then
				v_first_chaine_column_name:=v_first_chaine_column_name||','||v_nom_voie;
				ligne_to_insert:=0;
				c_compte_nb_ch_chaine_champ:=c_compte_nb_ch_chaine_champ+1;
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
				WHEN OTHERS THEN message_retour:='NOK233';-- RAISE NOTICE 'ERREUR233 dans %',v_execute_2;message_retour:='NOK233';
				--WHEN OTHERS THEN CONTINUE;
				END;
				ligne_to_insert:=1;
			end if;
			
	else
		
			ligne_to_insert:=1;
			
	end if;
			
	if ligne_to_insert != 0
    then	
			v_libelle_evenement:='Modif 100% et/ou 0%';
			v_execute_K_100:='';
			v_execute_K_0:='';
			
			IF POSITION('ERREUR#' in v_first_chaine_valeur_calc) = 0
			THEN
			
					v_first_chaine_valeur_calc_k_100:=public.hash_evenement_K(v_first_chaine_valeur_calc);
					
					--RAISE NOTICE 'v_first_chaine_valeur_calc=%-v_first_chaine_valeur_calc_k_100=%',v_first_chaine_valeur_calc,v_first_chaine_valeur_calc_k_100;
					
					if POSITION(';K;' in v_first_chaine_valeur_calc_k_100) != 0
					then
							if POSITION(';k;' in v_first_chaine_valeur_calc_k_100) != 0
							then
							
								v_first_chaine_valeur_calc_k_100:=substr(v_first_chaine_valeur_calc_k_100,4,POSITION(';k;' in v_first_chaine_valeur_calc_k_100)-4);
							
							else
								
								v_first_chaine_valeur_calc_k_100:=substr(v_first_chaine_valeur_calc_k_100,4,length(v_first_chaine_valeur_calc_k_100)-4+1);
								
							end if;
							
					else
							v_first_chaine_valeur_calc_k_100:='';
					end if;
					
					v_first_chaine_valeur_calc_k_0:=public.hash_evenement_K(v_first_chaine_valeur_calc);
					
					--RAISE NOTICE 'v_first_chaine_valeur_calc=%-v_first_chaine_valeur_calc_k_0=%',v_first_chaine_valeur_calc,v_first_chaine_valeur_calc_k_0;
					
					if POSITION(';k;' in v_first_chaine_valeur_calc_k_0) != 0
					then
							if POSITION(';K;' in v_first_chaine_valeur_calc_k_0) != 0
							then
							
								v_first_chaine_valeur_calc_k_0:=substr(v_first_chaine_valeur_calc_k_0,POSITION(';k;' in v_first_chaine_valeur_calc_k_0)+3,length(v_first_chaine_valeur_calc_k_0)-POSITION(';k;' in v_first_chaine_valeur_calc_k_0)-2);
							   
							else
								
								v_first_chaine_valeur_calc_k_0:=substr(v_first_chaine_valeur_calc_k_0,4,length(v_first_chaine_valeur_calc_k_0)-4+1);
								
							end if;
							
					else
							v_first_chaine_valeur_calc_k_0:='';
					end if;

				
					v_libelle_evenement_table:='';
					
					if v_first_chaine_valeur_calc_k_100 != ''
					then
					
						select case WHEN v_first_chaine_valeur_calc_k_100 ~* '[A-F]' THEN v_libelle_evenement_erreur||' '||v_libelle_evenement_K100 else v_libelle_evenement_K100 end into v_libelle_evenement_table;
							
						v_execute_K_100:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,'||v_first_chaine_column_name||')'||
										 ' values('||''''||v_first_time||''''||','||''''||'K'||''''||','||''''||v_libelle_evenement_table||''''||','||v_first_chaine_valeur_calc_k_100||')';
										
						--RAISE NOTICE 'v_execute_K_100=%',v_execute_K_100;
						
					end if;
					
					if v_first_chaine_valeur_calc_k_0 != ''
					then
					
						select case WHEN v_first_chaine_valeur_calc_k_0 ~* '[A-F]' THEN v_libelle_evenement_erreur||' '||v_libelle_evenement_K0 else v_libelle_evenement_K0 end into v_libelle_evenement_table;
					
						v_execute_K_0:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,'||v_first_chaine_column_name||')'||
									   ' values('||''''||v_first_time||''''||','||''''||'k'||''''||','||''''||v_libelle_evenement_table||''''||','||v_first_chaine_valeur_calc_k_0||')';
					
						--RAISE NOTICE 'v_execute_K_0=%',v_execute_K_0;
						
					end if;
					
					--RAISE NOTICE 'c_compte_nb_ch_chaine_champ=%-c_compte_nb_ch_chaine_valeur/2=%',c_compte_nb_ch_chaine_champ,c_compte_nb_ch_chaine_valeur/2;
					
					IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
					and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_evenement,'') IS NOT NULL and v_first_chaine_valeur_calc_k_0 != ''
					and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur/2
					THEN
						i:=i+1;
						j:=j+1;
						BEGIN
						--RAISE NOTICE 'commande = %',v_execute_K_0;		
						EXECUTE v_execute_K_0;
						EXCEPTION
						WHEN OTHERS THEN RAISE NOTICE 'ERREUR22b dans %',v_execute_K_0;message_retour:='NOK22';
						--WHEN OTHERS THEN CONTINUE;
						END;
					ELSE
						i:=i+1;
						--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
						v_execute_1:='insert v_first_chaine_valeur_calc_k_0 into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
									 ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
									 COALESCE(v_first_chaine_valeur_calc_k_0,'NULL')||''''||')';
						v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
						v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
						BEGIN
						--RAISE NOTICE 'commande = %',v_execute_1;		
						EXECUTE v_execute_2;
						EXCEPTION
						WHEN OTHERS THEN message_retour:='NOK23a'; RAISE NOTICE 'ERREUR23b fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK23';
						--WHEN OTHERS THEN CONTINUE;
						END;
					END IF;
					
					IF NULLIF(v_first_chaine_column_name,'') IS NOT NULL and NULLIF(v_first_chaine_valeur_calc,'') IS NOT NULL 
					and NULLIF(v_first_time,'') IS NOT NULL and NULLIF(v_evenement,'') IS NOT NULL and v_first_chaine_valeur_calc_k_100 != ''
					and c_compte_nb_ch_chaine_champ=c_compte_nb_ch_chaine_valeur/2
					THEN
						i:=i+1;
						j:=j+1;
						BEGIN
						--RAISE NOTICE 'commande = %',v_execute_K_100;		
						EXECUTE v_execute_K_100;
						EXCEPTION
						WHEN OTHERS THEN RAISE NOTICE 'ERREUR22a dans %',v_execute_K_100;message_retour:='NOK22';
						--WHEN OTHERS THEN CONTINUE;
						END;
					ELSE
						i:=i+1;
						--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
						v_execute_1:='Erreur v_first_chaine_valeur_calc_k_100 insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
									 ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
									 COALESCE(v_first_chaine_valeur_calc_k_100,'NULL')||''''||')';
						v_first_fichier:=COALESCE(v_first_fichier,'NULL');							 
						v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
						BEGIN
						--RAISE NOTICE 'commande = %',v_execute_1;		
						EXECUTE v_execute_2;
						EXCEPTION
						WHEN OTHERS THEN message_retour:='NOK23'; RAISE NOTICE 'ERREUR23a fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK23';
						--WHEN OTHERS THEN CONTINUE;
						END;
					END IF;
	
			ELSE
					i:=i+1;
						--RAISE NOTICE 'PB chaine cmd=%',v_execute_1;
					v_execute_1:='ERREUR# insert into sh_'||COALESCE(lower(station),'NULL')||'.archives_releves (time,evenement,libelle_evenement,'||COALESCE(v_first_chaine_column_name,'NULL')||')'||
								 ' values('||''''||COALESCE(v_first_time,'NULL')||''''||','||''''||COALESCE(v_evenement,'NULL')||''''||','||''''||COALESCE(v_libelle_evenement,'NULL')||''''||','||''''||
								  COALESCE(v_first_chaine_valeur_calc,'NULL')||''''||')';
					v_first_fichier:=COALESCE(v_first_fichier,'NULL');
                    --RAISE NOTICE 'Cas FFFF : %-%',v_execute_1,v_first_chaine_valeur_calc;					
					v_execute_2:='insert into ' ||table_log_archivage||' values('||''''||v_first_fichier||''''||','||''''||replace(v_execute_1,'''','')||''''||')';
					BEGIN
						--RAISE NOTICE 'commande = %',v_execute_1;		
					EXECUTE v_execute_2;
					EXCEPTION
					WHEN OTHERS THEN message_retour:='NOK23b'; RAISE NOTICE 'ERREUR23 fichier % dans % - v_execute_1 %',v_first_fichier,v_execute_2,v_execute_1;message_retour:='NOK23';
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
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch3,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch4,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch5,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch6,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch7,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch8,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch9,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch10,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch11,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch12,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch13,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch14,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch15,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch16,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch17,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch18,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch19,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch20,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch21,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch22,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch23,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch24,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch25,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch26,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch27,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch28,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch29,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch30,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch31,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch32,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch33,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch34,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch35,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch36,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch37,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch38,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch39,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch40,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch41,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch42,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch43,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch44,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch45,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch46,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch47,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch48,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch49,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch50,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch51,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch52,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch53,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch54,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch55,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch56,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch57,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch58,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch59,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
			if NULLIF(v_ch60,'') is not null
			then
				c_compte_nb_ch_chaine_valeur:=c_compte_nb_ch_chaine_valeur+1;
			end if;
	
										
			v_first_chaine_valeur_calc:=v_ch2||v_ch3||v_ch4||v_ch5||v_ch6||v_ch7||v_ch8||v_ch9||v_ch10||v_ch11||v_ch12||v_ch13||
										v_ch14||v_ch15||v_ch16||v_ch17||v_ch18||v_ch19||v_ch20||v_ch21||v_ch22||v_ch23||v_ch24||
										v_ch25||v_ch26||v_ch27||v_ch28||v_ch29||v_ch30||v_ch31||v_ch32||v_ch33||v_ch34||v_ch35||
										v_ch36||v_ch37||v_ch38||v_ch39||v_ch40||v_ch41||v_ch42||v_ch43||v_ch44||v_ch45||v_ch46||
										v_ch47||v_ch48||v_ch49||v_ch50||v_ch51||v_ch52||v_ch53||v_ch54||v_ch55||v_ch56||v_ch57||
										v_ch58||v_ch59||v_ch60;
								  
						
	end if;						  
	
	
END LOOP;
CLOSE CurRelEvAnaK;

RAISE NOTICE 'FIN LOOP K';

message_retour:='';

FOR CUR1 IN CurColumnMulti(station)
LOOP

	--RAISE NOTICE 'DANS LOOP E';
	v_chaine_column_name:='( ';
	
	v_column_name_1:=CUR1.column_name_orig;
	
	FOR CUR2 in CurColumnMulti2(station,v_column_name_1)
	LOOP
	
			v_chaine_column_name:=v_chaine_column_name||CUR2.column_name||' is not null and ';
			 
	END LOOP;
	
	v_chaine_column_name:=v_chaine_column_name||');';
	v_chaine_column_name:=replace(v_chaine_column_name,'and )',')');

	v_execute_1:='select count(*) from sh_'||lower(station)||'.archives_releves where '||v_chaine_column_name;


	BEGIN
	EXECUTE v_execute_1 INTO v_count;
	if v_count > 0 then message_retour:=message_retour||'-Incohence dans les champs '||v_chaine_column_name||' : nombre de lignes non 0 : '||v_count; end if;
    RAISE NOTICE 'v_chaine_column_name=%-nombre=%',v_chaine_column_name,v_count;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR211 dans %',v_execute_1;
	END;

END LOOP;

if NULLIF(message_retour,'') IS NULL then message_retour:='OK'; end if;

RAISE NOTICE 'APRES VERIF COLUMN';

return message_retour;

--EXCEPTION

--WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;