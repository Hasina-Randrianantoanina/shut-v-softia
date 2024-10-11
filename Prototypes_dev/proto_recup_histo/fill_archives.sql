-- 425453 mesure pour le fichier PD_fichier_nabyl_body07_12.csv
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

v_execute_1 			character varying (10000);
table_releves			character varying (100);
table_entetes			character varying (100);
table_tmp_releves 		character varying (100);
table_tmp_entetes 		character varying (100);
table_tmp_position		character varying (100);
table_archivage			character varying (100);
table_position			character varying (100);
table_log				character varying (100);
message_retour 			character varying (100):='OK';

v_time timestamp without time zone;
v_rupture_date timestamp without time zone;
v_evenement character varying (10);
v_fichier character varying (100);
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

v_nom_voie character varying (100);
v_column_name_1	 character varying (100);
v_column_name_2	 character varying (100);

v_old_chaine_column_name    		text;
v_old_chaine_valeur_calc 			text;

v_libelle_evenement character varying (100);	

v_lettre	character varying (2);

v_valeur	character varying (100);
v_nombre    integer;
v_seul    	integer;

v_val_calc character varying (100);	

i integer:=1;
j integer:=1;

ligne_to_insert integer:=0;
 
compteur_ligne_insert integer:=1;
nombre_ligne integer:=0;
v_nombre_ligne integer:=0;

CurRelEvE 	CURSOR FOR SELECT * FROM table_temporaire_releves where evenement ='E' and 1=2 ORDER BY TIME;
CurRelEvM 	CURSOR FOR SELECT 	r.time,r.evenement,t.nom_voie,
								CASE WHEN COALESCE(length(NULLIF(r.ch2,'')),null,0) -1 <=2 THEN r.ch2
								ELSE public.transforme_valeur_mesure(substr(r.ch2,2,length(r.ch2)-1),t.precision) end as ch2,
								CASE WHEN COALESCE(length(NULLIF(r.ch3,'')),null,0) -1 <=2 THEN r.ch3
								ELSE public.transforme_valeur_mesure(substr(r.ch3,2,length(r.ch3)-1),t.precision) end as ch3,
								CASE WHEN COALESCE(length(NULLIF(r.ch4,'')),null,0) -1 <=2 THEN r.ch4
								ELSE public.transforme_valeur_mesure(substr(r.ch4,2,length(r.ch4)-1),t.precision) end as ch4,
								CASE WHEN COALESCE(length(NULLIF(r.ch5,'')),null,0) -1 <=2 THEN r.ch5
								ELSE public.transforme_valeur_mesure(substr(r.ch5,2,length(r.ch5)-1),t.precision) end as ch5,
								CASE WHEN COALESCE(length(NULLIF(r.ch6,'')),null,0) -1 <=2 THEN r.ch6
								ELSE public.transforme_valeur_mesure(substr(r.ch6,2,length(r.ch6)-1),t.precision) end as ch6,
								CASE WHEN COALESCE(length(NULLIF(r.ch7,'')),null,0) -1 <=2 THEN r.ch7
								ELSE public.transforme_valeur_mesure(substr(r.ch7,2,length(r.ch7)-1),t.precision) end as ch7,
								CASE WHEN COALESCE(length(NULLIF(r.ch8,'')),null,0) -1 <=2 THEN r.ch8
								ELSE public.transforme_valeur_mesure(substr(r.ch8,2,length(r.ch8)-1),t.precision) end as ch8,
								CASE WHEN COALESCE(length(NULLIF(r.ch9,'')),null,0) -1 <=2 THEN r.ch9
								ELSE public.transforme_valeur_mesure(substr(r.ch9,2,length(r.ch9)-1),t.precision) end as ch9,
								CASE WHEN COALESCE(length(NULLIF(r.ch10,'')),null,0) -1 <=2 THEN r.ch10
								ELSE public.transforme_valeur_mesure(substr(r.ch10,2,length(r.ch10)-1),t.precision) end as ch10,
								CASE WHEN COALESCE(length(NULLIF(r.ch11,'')),null,0) -1 <=2 THEN r.ch11
								ELSE public.transforme_valeur_mesure(substr(r.ch11,2,length(r.ch11)-1),t.precision) end as ch11,
								CASE WHEN COALESCE(length(NULLIF(r.ch12,'')),null,0) -1 <=2 THEN r.ch12
								ELSE public.transforme_valeur_mesure(substr(r.ch12,2,length(r.ch12)-1),t.precision) end as ch12,
								CASE WHEN COALESCE(length(NULLIF(r.ch13,'')),null,0) -1 <=2 THEN r.ch13
								ELSE public.transforme_valeur_mesure(substr(r.ch13,2,length(r.ch13)-1),t.precision) end as ch13,
								CASE WHEN COALESCE(length(NULLIF(r.ch14,'')),null,0) -1 <=2 THEN r.ch14
								ELSE public.transforme_valeur_mesure(substr(r.ch14,2,length(r.ch14)-1),t.precision) end as ch14,
								CASE WHEN COALESCE(length(NULLIF(r.ch15,'')),null,0) -1 <=2 THEN r.ch15
								ELSE public.transforme_valeur_mesure(substr(r.ch15,2,length(r.ch15)-1),t.precision) end as ch15,
								CASE WHEN COALESCE(length(NULLIF(r.ch16,'')),null,0) -1 <=2 THEN r.ch16
								ELSE public.transforme_valeur_mesure(substr(r.ch16,2,length(r.ch16)-1),t.precision) end as ch16,
								CASE WHEN COALESCE(length(NULLIF(r.ch17,'')),null,0) -1 <=2 THEN r.ch17
								ELSE public.transforme_valeur_mesure(substr(r.ch17,2,length(r.ch17)-1),t.precision) end as ch17,
								CASE WHEN COALESCE(length(NULLIF(r.ch18,'')),null,0) -1 <=2 THEN r.ch18
								ELSE public.transforme_valeur_mesure(substr(r.ch18,2,length(r.ch18)-1),t.precision) end as ch18,
								CASE WHEN COALESCE(length(NULLIF(r.ch19,'')),null,0) -1 <=2 THEN r.ch19
								ELSE public.transforme_valeur_mesure(substr(r.ch19,2,length(r.ch19)-1),t.precision) end as ch19,
								CASE WHEN COALESCE(length(NULLIF(r.ch20,'')),null,0) -1 <=2 THEN r.ch20
								ELSE public.transforme_valeur_mesure(substr(r.ch20,2,length(r.ch20)-1),t.precision) end as ch20,
								CASE WHEN COALESCE(length(NULLIF(r.ch21,'')),null,0) -1 <=2 THEN r.ch21
								ELSE public.transforme_valeur_mesure(substr(r.ch21,2,length(r.ch21)-1),t.precision) end as ch21,
								CASE WHEN COALESCE(length(NULLIF(r.ch22,'')),null,0) -1 <=2 THEN r.ch22
								ELSE public.transforme_valeur_mesure(substr(r.ch22,2,length(r.ch22)-1),t.precision) end as ch22,
								CASE WHEN COALESCE(length(NULLIF(r.ch23,'')),null,0) -1 <=2 THEN r.ch23
								ELSE public.transforme_valeur_mesure(substr(r.ch23,2,length(r.ch23)-1),t.precision) end as ch23,
								CASE WHEN COALESCE(length(NULLIF(r.ch24,'')),null,0) -1 <=2 THEN r.ch24
								ELSE public.transforme_valeur_mesure(substr(r.ch24,2,length(r.ch24)-1),t.precision) end as ch24,
								CASE WHEN COALESCE(length(NULLIF(r.ch25,'')),null,0) -1 <=2 THEN r.ch25
								ELSE public.transforme_valeur_mesure(substr(r.ch25,2,length(r.ch25)-1),t.precision) end as ch25,
								CASE WHEN COALESCE(length(NULLIF(r.ch26,'')),null,0) -1 <=2 THEN r.ch26
								ELSE public.transforme_valeur_mesure(substr(r.ch26,2,length(r.ch26)-1),t.precision) end as ch26,
								CASE WHEN COALESCE(length(NULLIF(r.ch27,'')),null,0) -1 <=2 THEN r.ch27
								ELSE public.transforme_valeur_mesure(substr(r.ch27,2,length(r.ch27)-1),t.precision) end as ch27,
								CASE WHEN COALESCE(length(NULLIF(r.ch28,'')),null,0) -1 <=2 THEN r.ch28
								ELSE public.transforme_valeur_mesure(substr(r.ch28,2,length(r.ch28)-1),t.precision) end as ch28,
								CASE WHEN COALESCE(length(NULLIF(r.ch29,'')),null,0) -1 <=2 THEN r.ch29
								ELSE public.transforme_valeur_mesure(substr(r.ch29,2,length(r.ch29)-1),t.precision) end as ch29,
								CASE WHEN COALESCE(length(NULLIF(r.ch30,'')),null,0) -1 <=2 THEN r.ch30
								ELSE public.transforme_valeur_mesure(substr(r.ch30,2,length(r.ch30)-1),t.precision) end as ch30,
								CASE WHEN COALESCE(length(NULLIF(r.ch31,'')),null,0) -1 <=2 THEN r.ch31
								ELSE public.transforme_valeur_mesure(substr(r.ch31,2,length(r.ch31)-1),t.precision) end as ch31,
								CASE WHEN COALESCE(length(NULLIF(r.ch32,'')),null,0) -1 <=2 THEN r.ch32
								ELSE public.transforme_valeur_mesure(substr(r.ch32,2,length(r.ch32)-1),t.precision) end as ch32,
								CASE WHEN COALESCE(length(NULLIF(r.ch33,'')),null,0) -1 <=2 THEN r.ch33
								ELSE public.transforme_valeur_mesure(substr(r.ch33,2,length(r.ch33)-1),t.precision) end as ch33,
								CASE WHEN COALESCE(length(NULLIF(r.ch34,'')),null,0) -1 <=2 THEN r.ch34
								ELSE public.transforme_valeur_mesure(substr(r.ch34,2,length(r.ch34)-1),t.precision) end as ch34,
								CASE WHEN COALESCE(length(NULLIF(r.ch35,'')),null,0) -1 <=2 THEN r.ch35
								ELSE public.transforme_valeur_mesure(substr(r.ch35,2,length(r.ch35)-1),t.precision) end as ch35,
								CASE WHEN COALESCE(length(NULLIF(r.ch36,'')),null,0) -1 <=2 THEN r.ch36
								ELSE public.transforme_valeur_mesure(substr(r.ch36,2,length(r.ch36)-1),t.precision) end as ch36,
								CASE WHEN COALESCE(length(NULLIF(r.ch37,'')),null,0) -1 <=2 THEN r.ch37
								ELSE public.transforme_valeur_mesure(substr(r.ch37,2,length(r.ch37)-1),t.precision) end as ch37,
								CASE WHEN COALESCE(length(NULLIF(r.ch38,'')),null,0) -1 <=2 THEN r.ch38
								ELSE public.transforme_valeur_mesure(substr(r.ch38,2,length(r.ch38)-1),t.precision) end as ch38,
								CASE WHEN COALESCE(length(NULLIF(r.ch39,'')),null,0) -1 <=2 THEN r.ch39
								ELSE public.transforme_valeur_mesure(substr(r.ch39,2,length(r.ch39)-1),t.precision) end as ch39,
								CASE WHEN COALESCE(length(NULLIF(r.ch40,'')),null,0) -1 <=2 THEN r.ch40
								ELSE public.transforme_valeur_mesure(substr(r.ch40,2,length(r.ch40)-1),t.precision) end as ch40,
								CASE WHEN COALESCE(length(NULLIF(r.ch41,'')),null,0) -1 <=2 THEN r.ch41
								ELSE public.transforme_valeur_mesure(substr(r.ch41,2,length(r.ch41)-1),t.precision) end as ch41,
								CASE WHEN COALESCE(length(NULLIF(r.ch42,'')),null,0) -1 <=2 THEN r.ch42
								ELSE public.transforme_valeur_mesure(substr(r.ch42,2,length(r.ch42)-1),t.precision) end as ch42,
								CASE WHEN COALESCE(length(NULLIF(r.ch43,'')),null,0) -1 <=2 THEN r.ch43
								ELSE public.transforme_valeur_mesure(substr(r.ch43,2,length(r.ch43)-1),t.precision) end as ch43,
								CASE WHEN COALESCE(length(NULLIF(r.ch44,'')),null,0) -1 <=2 THEN r.ch44
								ELSE public.transforme_valeur_mesure(substr(r.ch44,2,length(r.ch44)-1),t.precision) end as ch44,
								CASE WHEN COALESCE(length(NULLIF(r.ch45,'')),null,0) -1 <=2 THEN r.ch45
								ELSE public.transforme_valeur_mesure(substr(r.ch45,2,length(r.ch45)-1),t.precision) end as ch45,
								CASE WHEN COALESCE(length(NULLIF(r.ch46,'')),null,0) -1 <=2 THEN r.ch46
								ELSE public.transforme_valeur_mesure(substr(r.ch46,2,length(r.ch46)-1),t.precision) end as ch46,
								CASE WHEN COALESCE(length(NULLIF(r.ch47,'')),null,0) -1 <=2 THEN r.ch47
								ELSE public.transforme_valeur_mesure(substr(r.ch47,2,length(r.ch47)-1),t.precision) end as ch47,
								CASE WHEN COALESCE(length(NULLIF(r.ch48,'')),null,0) -1 <=2 THEN r.ch48
								ELSE public.transforme_valeur_mesure(substr(r.ch48,2,length(r.ch48)-1),t.precision) end as ch48,
								CASE WHEN COALESCE(length(NULLIF(r.ch49,'')),null,0) -1 <=2 THEN r.ch49
								ELSE public.transforme_valeur_mesure(substr(r.ch49,2,length(r.ch49)-1),t.precision) end as ch49,
								CASE WHEN COALESCE(length(NULLIF(r.ch50,'')),null,0) -1 <=2 THEN r.ch50
								ELSE public.transforme_valeur_mesure(substr(r.ch50,2,length(r.ch50)-1),t.precision) end as ch50,
								CASE WHEN COALESCE(length(NULLIF(r.ch51,'')),null,0) -1 <=2 THEN r.ch51
								ELSE public.transforme_valeur_mesure(substr(r.ch51,2,length(r.ch51)-1),t.precision) end as ch51,
								CASE WHEN COALESCE(length(NULLIF(r.ch52,'')),null,0) -1 <=2 THEN r.ch52
								ELSE public.transforme_valeur_mesure(substr(r.ch52,2,length(r.ch52)-1),t.precision) end as ch52,
								CASE WHEN COALESCE(length(NULLIF(r.ch53,'')),null,0) -1 <=2 THEN r.ch53
								ELSE public.transforme_valeur_mesure(substr(r.ch53,2,length(r.ch53)-1),t.precision) end as ch53,
								CASE WHEN COALESCE(length(NULLIF(r.ch54,'')),null,0) -1 <=2 THEN r.ch54
								ELSE public.transforme_valeur_mesure(substr(r.ch54,2,length(r.ch54)-1),t.precision) end as ch54,
								CASE WHEN COALESCE(length(NULLIF(r.ch55,'')),null,0) -1 <=2 THEN r.ch55
								ELSE public.transforme_valeur_mesure(substr(r.ch55,2,length(r.ch55)-1),t.precision) end as ch55,
								CASE WHEN COALESCE(length(NULLIF(r.ch56,'')),null,0) -1 <=2 THEN r.ch56
								ELSE public.transforme_valeur_mesure(substr(r.ch56,2,length(r.ch56)-1),t.precision) end as ch56,
								CASE WHEN COALESCE(length(NULLIF(r.ch57,'')),null,0) -1 <=2 THEN r.ch57
								ELSE public.transforme_valeur_mesure(substr(r.ch57,2,length(r.ch57)-1),t.precision) end as ch57,
								CASE WHEN COALESCE(length(NULLIF(r.ch58,'')),null,0) -1 <=2 THEN r.ch58
								ELSE public.transforme_valeur_mesure(substr(r.ch58,2,length(r.ch58)-1),t.precision) end as ch58,
								CASE WHEN COALESCE(length(NULLIF(r.ch59,'')),null,0) -1 <=2 THEN r.ch59
								ELSE public.transforme_valeur_mesure(substr(r.ch59,2,length(r.ch59)-1),t.precision) end as ch59,
								CASE WHEN COALESCE(length(NULLIF(r.ch60,'')),null,0) -1 <=2 THEN r.ch60
								ELSE public.transforme_valeur_mesure(substr(r.ch60,2,length(r.ch60)-1),t.precision) end as ch60
					   FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and 		a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,1)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,1)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,1)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,1)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,1)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,1)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,1)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,1)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,1)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,1)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,1)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,1)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,1)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,1)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,1)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,1)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,1)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,1)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,1)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,1)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,1)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,1)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,1)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,1)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,1)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,1)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,1)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,1)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,1)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,1)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,1)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,1)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,1)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,1)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,1)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,1)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,1)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,1)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,1)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,1)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,1)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,1)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,1)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,1)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,1)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,1)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,1)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,1)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,1)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,1)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,1)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,1)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,1)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,1)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,1)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,1)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,1)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,1)=a.lettre_voie)
	                         )
					   and   r.evenement in (' ','m','G','P','T') ORDER BY r.TIME asc,t.lettre_voie,t.nom_voie asc;
				       --and   r.evenement in (' ','m','G','P','T','I','J','U','V','L') ORDER BY r.TIME;
					   
CurRelEvMMin  CURSOR FOR SELECT count(*) as nombre_ligne, MIN(r.time)::timestamp without time zone - interval '1 day' as time				   
						 FROM 	sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
								sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
						 where 	a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                     and 	a.nom_voie=substr(t.nom_voie,1,position('____' in t.nom_voie)-1)
				         and   (
	                           (r.ch2 is not null and substr(r.ch2,1,1)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,1)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,1)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,1)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,1)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,1)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,1)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,1)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,1)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,1)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,1)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,1)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,1)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,1)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,1)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,1)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,1)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,1)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,1)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,1)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,1)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,1)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,1)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,1)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,1)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,1)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,1)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,1)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,1)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,1)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,1)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,1)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,1)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,1)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,1)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,1)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,1)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,1)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,1)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,1)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,1)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,1)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,1)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,1)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,1)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,1)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,1)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,1)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,1)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,1)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,1)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,1)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,1)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,1)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,1)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,1)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,1)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,1)=a.lettre_voie)
	                         )
					   and   r.evenement in (' ','m','G','P','T');
/*
CurColumnP 	CURSOR(v_position integer) 
			FOR select nom_voie,precision FROM table_temporaire_position where position=v_position;
CurColumnL 	CURSOR(v_lettre,v_position integer) 
			FOR select nom_voie,precision FROM table_temporaire_position where lettre_voie=v_lettre;
*/
			
error_msg text;

BEGIN

table_tmp_releves:='table_temporaire_releves';
table_tmp_entetes:='table_temporaire_entetes';
table_tmp_position:='table_temporaire_position';

table_releves:='sh_'||lower(station)||'.'||lower(chaine_table_releves);
table_entetes:='sh_'||lower(station)||'.'||lower(chaine_table_entetes);
table_archivage:='sh_'||lower(station)||'.archives_releves';
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
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
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
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;

BEGIN
v_execute_1:='ANALYSE '||table_releves;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_entetes;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_tmp_releves;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_tmp_entetes;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

BEGIN
v_execute_1:='ANALYSE '||table_position;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

BEGIN
v_execute_1:='TRUNCATE TABLE '||table_archivage;
RAISE NOTICE '%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

RAISE NOTICE 'AVANT LOOP E';

v_libelle_evenement:='Changement de jour';
FOR CUR IN CurRelEvE LOOP

	RAISE NOTICE 'DANS LOOP E';
	
	v_time:=CUR.time;
	v_evenement:=CUR.evenement;

	v_ch1:=CUR.ch1;
	v_ch2:=CUR.ch2;
	
	select lower(nom_voie) into v_column_name_1 from table_temporaire_position where position=4;
	select lower(nom_voie) into v_column_name_2 from table_temporaire_position where position=5;

	BEGIN
	v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,'||v_column_name_1||','||v_column_name_2||','||'libelle_evenement) 
	values('||''''||v_time||''''||','||''''||v_evenement||''''||','||''''||v_ch1||''''||','||''''||v_ch2||''''||','||''''||v_libelle_evenement||''''||')';
	--RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;

END LOOP;

RAISE NOTICE 'AVANT LOOP evenement analogique';

FOR CUR IN CurRelEvMMin LOOP
	
		v_rupture_date:=CUR.time;
		v_nombre_ligne:=CUR.nombre_ligne;
	
END LOOP;

v_old_chaine_column_name:='';
v_old_chaine_valeur_calc:='';
ligne_to_insert:=0;

v_libelle_evenement:='mesure';

OPEN CurRelEvM;
FETCH FIRST FROM CurRelEvM INTO v_time,v_evenement,v_nom_voie,v_ch2,v_ch3,v_ch4,v_ch5,v_ch6,v_ch7,v_ch8,v_ch9,v_ch10,v_ch11,v_ch12,v_ch13,v_ch14,
						        v_ch15,v_ch16,v_ch17,v_ch18,v_ch19,v_ch20,v_ch21,v_ch22,v_ch23,v_ch24,v_ch25,v_ch26,v_ch27,v_ch28,v_ch29,v_ch30,
								v_ch31,v_ch32,v_ch33,v_ch24,v_ch35,v_ch36,v_ch37,v_ch38,v_ch39,v_ch40,v_ch41,v_ch42,v_ch43,v_ch44,v_ch45,v_ch46,
								v_ch47,v_ch48,v_ch49,v_ch50,v_ch51,v_ch52,v_ch53,v_ch54,v_ch55,v_ch56,v_ch57,v_ch58,v_ch59,v_ch60;

LOOP
EXIT WHEN NOT FOUND;
if i > 10000 then exit; end if;
if j > 100000 then j:=1; RAISE NOTICE '10000 lignes inserees %',now(); j:=1; end if;
	
END LOOP;
CLOSE CurRelEvM;

FOR CUR IN CurRelEvM LOOP
	
	
	
	v_time:=CUR.time;
	v_evenement:=CUR.evenement;
	v_nom_voie:=CUR.nom_voie;
	
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
    
	if v_time = v_rupture_date
	then
			if v_old_chaine_column_name=''
			then
				v_old_chaine_column_name:=v_nom_voie;
			else
				v_old_chaine_column_name:=v_old_chaine_column_name||','||v_nom_voie;
			end if;
			ligne_to_insert:=0;
			
	else
			v_rupture_date:=v_time;
			ligne_to_insert:=1;
			v_chaine_column_name:=v_nom_voie;
	end if;
	
	if NULLIF(v_ch2,'') is not null
	then
		 v_ch2:=v_ch2||',';
	else
		 v_ch2:='';
	end if;
	if NULLIF(v_ch10,'') is not null
	then
		v_ch10:=v_ch10||',';
	else
		v_ch10:='';
	end if;
	if NULLIF(v_ch11,'') is not null
	then
		v_ch11:=v_ch11||',';
	else
		v_ch11:='';
	end if;
	if NULLIF(v_ch12,'') is not null
	then
		v_ch12:=v_ch12||',';
	else
		v_ch12:='';
	end if;
	if NULLIF(v_ch13,'') is not null
	then
		v_ch13:=v_ch13||',';
	else
		v_ch13:='';
	end if;
	if NULLIF(v_ch14,'') is not null
	then
		v_ch14:=v_ch14||',';
	else
		v_ch14:='';
	end if;
	if NULLIF(v_ch15,'') is not null
	then
		v_ch15:=v_ch15||',';
	else
		v_ch15:='';
	end if;
	if NULLIF(v_ch16,'') is not null
	then
		v_ch16:=v_ch16||',';
	else
		v_ch16:='';
	end if;
	if NULLIF(v_ch17,'') is not null
	then
		v_ch17:=v_ch17||',';
	else
		v_ch17:='';
	end if;
	if NULLIF(v_ch18,'') is not null
	then
		v_ch18:=v_ch18||',';
	else
		v_ch18:='';
	end if;
	if NULLIF(v_ch19,'') is not null
	then
		v_ch19:=v_ch19||',';
	else
		v_ch19:='';
	end if;
	if NULLIF(v_ch20,'') is not null
	then
		v_ch20:=v_ch20||',';
	else
		v_ch20:='';
	end if;
	if NULLIF(v_ch21,'') is not null
	then
		v_ch21:=v_ch21||',';
	else
		v_ch21:='';
	end if;
	if NULLIF(v_ch22,'') is not null
	then
		v_ch22:=v_ch22||',';
	else
		v_ch22:='';
	end if;
	if NULLIF(v_ch23,'') is not null
	then
		v_ch23:=v_ch23||',';
	else
		v_ch23:='';
	end if;
	if NULLIF(v_ch24,'') is not null
	then
		v_ch24:=v_ch24||',';
	else
		v_ch24:='';
	end if;
	if NULLIF(v_ch25,'') is not null
	then
		v_ch25:=v_ch25||',';
	else
		v_ch25:='';
	end if;
	if NULLIF(v_ch26,'') is not null
	then
		v_ch26:=v_ch26||',';
	else
		v_ch26:='';
	end if;
	if NULLIF(v_ch27,'') is not null
	then
		v_ch27:=v_ch27||',';
	else
		v_ch27:='';
	end if;
	if NULLIF(v_ch28,'') is not null
	then
		v_ch28:=v_ch28||',';
	else
		v_ch28:='';
	end if;
	if NULLIF(v_ch29,'') is not null
	then
		v_ch29:=v_ch29||',';
	else
		v_ch29:='';
	end if;
	if NULLIF(v_ch3,'') is not null
	then
		v_ch3:=v_ch3||',';
	else
		v_ch3:='';
	end if;
	if NULLIF(v_ch30,'') is not null
	then
		v_ch30:=v_ch30||',';
	else
		v_ch30:='';
	end if;
	if NULLIF(v_ch31,'') is not null
	then
		v_ch31:=v_ch31||',';
	else
		v_ch31:='';
	end if;
	if NULLIF(v_ch32,'') is not null
	then
		v_ch32:=v_ch32||',';
	else
		v_ch32:='';
	end if;
	if NULLIF(v_ch33,'') is not null
	then
		v_ch33:=v_ch33||',';
	else
		v_ch33:='';
	end if;
	if NULLIF(v_ch35,'') is not null
	then
		v_ch35:=v_ch35||',';
	else
		v_ch35:='';
	end if;
	if NULLIF(v_ch36,'') is not null
	then
		v_ch36:=v_ch36||',';
	else
		v_ch36:='';
	end if;
	if NULLIF(v_ch37,'') is not null
	then
		v_ch37:=v_ch37||',';
	else
		v_ch37:='';
	end if;
	if NULLIF(v_ch38,'') is not null
	then
		v_ch38:=v_ch38||',';
	else
		v_ch38:='';
	end if;
	if NULLIF(v_ch39,'') is not null
	then
		v_ch39:=v_ch39||',';
	else
		v_ch39:='';
	end if;
	if NULLIF(v_ch4,'') is not null
	then
		v_ch4:=v_ch4||',';
	else
		v_ch4:='';
	end if;
	if NULLIF(v_ch40,'') is not null
	then
		v_ch40:=v_ch40||',';
	else
		v_ch40:='';
	end if;
	if NULLIF(v_ch41,'') is not null
	then
		v_ch41:=v_ch41||',';
	else
		v_ch41:='';
	end if;
	if NULLIF(v_ch42,'') is not null
	then
		v_ch42:=v_ch42||',';
	else
		v_ch42:='';
	end if;
	if NULLIF(v_ch43,'') is not null
	then
		v_ch43:=v_ch43||',';
	else
		v_ch43:='';
	end if;
	if NULLIF(v_ch44,'') is not null
	then
		v_ch44:=v_ch44||',';
	else
		v_ch44:='';
	end if;
	if NULLIF(v_ch45,'') is not null
	then
		v_ch45:=v_ch45||',';
	else
		v_ch45:='';
	end if;
	if NULLIF(v_ch46,'') is not null
	then
		v_ch46:=v_ch46||',';
	else
		v_ch46:='';
	end if;
	if NULLIF(v_ch47,'') is not null
	then
		v_ch47:=v_ch47||',';
	else
		v_ch47:='';
	end if;
	if NULLIF(v_ch48,'') is not null
	then
		v_ch48:=v_ch48||',';
	else
		v_ch48:='';
	end if;
	if NULLIF(v_ch49,'') is not null
	then
		v_ch49:=v_ch49||',';
	else
		v_ch49:='';
	end if;
	if NULLIF(v_ch5,'') is not null
	then
		v_ch5:=v_ch5||',';
	else
		v_ch5:='';
	end if;
	if NULLIF(v_ch50,'') is not null
	then
		v_ch50:=v_ch50||',';
	else
		v_ch50:='';
	end if;
	if NULLIF(v_ch51,'') is not null
	then
		v_ch51:=v_ch51||',';
	else
		v_ch51:='';
	end if;
	if NULLIF(v_ch52,'') is not null
	then
		v_ch52:=v_ch52||',';
	else
		v_ch52:='';
	end if;
	if NULLIF(v_ch53,'') is not null
	then
		v_ch53:=v_ch53||',';
	else
		v_ch53:='';
	end if;
	if NULLIF(v_ch54,'') is not null
	then
		v_ch54:=v_ch54||',';
	else
		v_ch54:='';
	end if;
	if NULLIF(v_ch55,'') is not null
	then
		v_ch55:=v_ch55||',';
	else
		v_ch55:='';
	end if;
	if NULLIF(v_ch56,'') is not null
	then
		v_ch56:=v_ch56||',';
	else
		v_ch56:='';
	end if;
	if NULLIF(v_ch57,'') is not null
	then
		v_ch57:=v_ch57||',';
	else
		v_ch57:='';
	end if;
	if NULLIF(v_ch58,'') is not null
	then
		v_ch58:=v_ch58||',';
	else
		v_ch58:='';
	end if;
	if NULLIF(v_ch59,'') is not null
	then
		v_ch59:=v_ch59||',';
	else
		v_ch59:='';
	end if;
	if NULLIF(v_ch6,'') is not null
	then
		v_ch6:=v_ch6||',';
	else
		v_ch6:='';
	end if;
	if NULLIF(v_ch60,'') is not null
	then
		v_ch60:=v_ch60||',';
	else
		v_ch60:='';
	end if;
	if NULLIF(v_ch7,'') is not null
	then
		v_ch7:=v_ch7||',';
	else
		v_ch7:='';
	end if;
	if NULLIF(v_ch8,'') is not null
	then
		v_ch8:=v_ch8||',';
	else
		v_ch8:='';
	end if;
	if NULLIF(v_ch9,'') is not null
	then
		v_ch9:=v_ch9||',';
	else
		v_ch9:='';
	end if;

	
	v_old_chaine_valeur_calc:=v_ch2||v_ch3||v_ch4||v_ch5||v_ch6||v_ch7||v_ch8||v_ch9||v_ch10||v_ch11||v_ch12||v_ch13||
							  v_ch14||v_ch15||v_ch16||v_ch17||v_ch18||v_ch19||v_ch20||v_ch21||v_ch22||v_ch23||v_ch24||
							  v_ch25||v_ch26||v_ch27||v_ch28||v_ch29||v_ch30||v_ch31||v_ch32||v_ch33||v_ch24||v_ch35||
							  v_ch36||v_ch37||v_ch38||v_ch39||v_ch40||v_ch41||v_ch42||v_ch43||v_ch44||v_ch45||v_ch46||
							  v_ch47||v_ch48||v_ch49||v_ch50||v_ch51||v_ch52||v_ch53||v_ch54||v_ch55||v_ch56||v_ch57||
							  v_ch58||v_ch59||v_ch60;
						  
	RAISE NOTICE 'v_column_name %-%',v_old_chaine_valeur_calc,v_ch2;
	
	v_old_chaine_valeur_calc:=case substr(v_old_chaine_valeur_calc,length(v_old_chaine_valeur_calc),1) when ',' then substr(v_old_chaine_valeur_calc,1,length(v_old_chaine_valeur_calc)-1) else  v_old_chaine_valeur_calc end;
    v_old_chaine_column_name:=case substr(v_old_chaine_column_name,length(v_old_chaine_column_name),1) when ',' then substr(v_old_chaine_column_name,1,length(v_old_chaine_column_name)-1) else  v_old_chaine_column_name end;
    
	v_seul:=POSITION(',' IN v_old_chaine_valeur_calc);
	
	if v_seul != 0 then v_old_chaine_valeur_calc:=replace(v_old_chaine_valeur_calc,',',''',''');
	
	RAISE NOTICE 'v_column_name % et/ou v_old_chaine_column_name % et/ou v_old_chaine_valeur_calc avant test : %',v_nom_voie,v_old_chaine_column_name,v_old_chaine_valeur_calc;
	
	IF NULLIF(v_nom_voie,'') IS NOT NULL and NULLIF(v_old_chaine_column_name,'') IS NOT NULL and NULLIF(v_old_chaine_valeur_calc,'') IS NOT NULL and ligne_to_insert = 1
	THEN
		i:=i+1;
		j:=j+1;
		BEGIN
		v_execute_1:='insert into sh_'||lower(station)||'.archives_releves (time,evenement,libelle_evenement,'||v_old_chaine_column_name||')'||
		' values('||''''||v_time||''''||','||''''||v_evenement||''''||','||''''||v_libelle_evenement||''''||','||''''||v_old_chaine_valeur_calc||''''||')';
		RAISE NOTICE 'commande = %',v_execute_1;		
		EXECUTE v_execute_1;
		EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
		END;
	ELSE
		i:=i+1;
		RAISE NOTICE '2 v_column_name % et/ou v_old_chaine_column_name % et/ou v_old_chaine_valeur_calc vide : %',v_nom_voie,v_old_chaine_column_name,v_old_chaine_valeur_calc;
	END IF;


END LOOP;

RAISE NOTICE 'APRES LOOP evenement';

return message_retour;

--EXCEPTION

--WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;