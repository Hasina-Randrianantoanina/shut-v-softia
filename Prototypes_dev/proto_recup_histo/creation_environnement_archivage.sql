DROP FUNCTION public.creation_environnement_archivage;
CREATE OR REPLACE FUNCTION public.creation_environnement_archivage(
	station in character varying (10),
	chaine_table_entetes in character varying (1000)
	)
    RETURNS text
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_execute_1 				text;
table_entetes				character varying (100);
table_tmp_entetes 			character varying (100);
table_tmp_entetes_insert	character varying (100);
table_archivage         	character varying (100);
table_log_archivage			character varying (100);
message_retour 				character varying (100):='OK';

v_nom_voie_sans_sta			character varying (100);
v_nom_voie_avec_sta			character varying (100);
		
v_lettre_voie				character varying (10);
v_lettre_voie_rec			character varying (10);
v_lettre_voie_rec_2			character varying (10);
v_ordre						smallint:=3;
v_precision					character varying (100);
n_precision					integer;
nb_col_multi_lettre			smallint;
nb_col_multi_precision		smallint;

v_chaine_col_multi_lettre  	  character varying (10):='____';
v_chaine_col_multi_precision  character varying (10):='________';

creation_column				boolean;
reussite_alter				boolean;

CurRelEnt CURSOR FOR SELECT distinct replace(nom_voie,'_'||upper(station),'') as nom_voie_sans_sta,nom_voie as nom_voie_avec_sta,
									 replace(replace(replace(replace(replace(replace(lettre_voie,')','za'),
									 '(','zb'),
									 '[','zc'),
									 ']','zd'),
									 '{','ze'),
									 '}','zf') as lettre_voie
									 ,ordre,precision
                     FROM table_temporaire_entetes order by ordre asc,precision desc;
											

BEGIN

	BEGIN
	v_execute_1:='TRUNCATE TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive';
	RAISE NOTICE '%',v_execute_1;
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	table_tmp_entetes:='table_temporaire_entetes';

	table_entetes:='sh_'||lower(station)||'.'||lower(chaine_table_entetes);

	table_tmp_entetes_insert:='table_temporaire_entetes_insert';


	BEGIN
	v_execute_1:='DROP TABLE '||table_tmp_entetes_insert;
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR DROP TABLE %',table_tmp_entetes_insert;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE '||table_tmp_entetes_insert||'(ordre integer,
																		lettre_voie character varying (2),
																		nom_voie character varying (100),
																		precision smallint
																	  )';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR CREATE TEMPORARY TABLE %',table_tmp_entetes_insert;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP TABLE '||table_tmp_entetes;
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR DROP TABLE %',table_entetes;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE '||table_tmp_entetes||'(ordre integer,
																 lettre_voie character varying (2),
																 nom_voie character varying (100),
																 precision smallint
														         )';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR CREATE TEMPORARY TABLE %',table_tmp_entetes;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	table_archivage:='sh_'||lower(station)||'.archives_releves';
	v_execute_1:='DROP TABLE '||table_archivage;
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR DROP TABLE %',table_archivage;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE '||table_archivage||'(time 				timestamp without time zone,
													 evenement 			character varying(10),
													 libelle_evenement   character varying(100)
													)';
	RAISE NOTICE '%',v_execute_1;
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR CREATE TABLE %',table_archivage;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_archivage_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_archivage_1
    ON '||table_archivage||' USING btree
    (time, evenement COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_archivage_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_archivage_2
    ON '||table_archivage||' USING btree
    (time)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	table_log_archivage:='sh_'||lower(station)||'.archives_releves_log';
	v_execute_1:='DROP TABLE '||table_log_archivage;
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR DROP TABLE %',table_log_archivage;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE '||table_log_archivage||' (fichier character varying(30), message_log text)';
	RAISE NOTICE '%',v_execute_1;
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR CREATE TABLE %',table_log_archivage;message_retour:='NOK : '||v_execute_1;
	END;

	BEGIN
	v_execute_1:='INSERT INTO '||table_tmp_entetes||' select distinct case a.lettre_voie
								when '||'''a'''||' then ascii(a.lettre_voie)
								when '||'''b'''||' then ascii(a.lettre_voie)
								when '||'''c'''||' then ascii(a.lettre_voie)
								when '||'''d'''||' then ascii(a.lettre_voie)
								when '||'''e'''||' then ascii(a.lettre_voie)
								when '||'''f'''||' then ascii(a.lettre_voie)
								when '||'''g'''||' then ascii(a.lettre_voie)
								when '||'''h'''||' then ascii(a.lettre_voie)
								when '||'''i'''||' then ascii(a.lettre_voie)
								when '||'''j'''||' then ascii(a.lettre_voie)
								when '||'''k'''||' then ascii(a.lettre_voie)
								when '||'''l'''||' then ascii(a.lettre_voie)
								when '||'''m'''||' then ascii(a.lettre_voie)
								when '||'''n'''||' then ascii(a.lettre_voie)
								when '||'''o'''||' then ascii(a.lettre_voie)
								when '||'''p'''||' then ascii(a.lettre_voie)
								when '||'''q'''||' then ascii(a.lettre_voie)
								when '||'''r'''||' then ascii(a.lettre_voie)
								when '||'''s'''||' then ascii(a.lettre_voie)
								when '||'''t'''||' then ascii(a.lettre_voie)
								when '||'''u'''||' then ascii(a.lettre_voie)
								when '||'''v'''||' then ascii(a.lettre_voie)
								when '||'''w'''||' then ascii(a.lettre_voie)
								when '||'''x'''||' then ascii(a.lettre_voie)
								when '||'''y'''||' then ascii(a.lettre_voie)
								when '||'''z'''||' then ascii(a.lettre_voie)
								when '||'''za'''||' then 123
								when '||'''zb'''||' then 124
								when '||'''zc'''||' then 125
								when '||'''zd'''||' then 126
								when '||'''ze'''||' then 127
								when '||'''zf'''||' then 128
						   end as ordre,
	   a.lettre_voie,a.nom_voie,a.precision::smallint
	   from '||table_entetes||' a order by ordre asc';
	   RAISE NOTICE '%',v_execute_1;	
	   EXECUTE v_execute_1;
	   EXCEPTION
	   WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	   END;

	   RAISE NOTICE 'AVANT LOOP';
	   
	   FOR CUR IN CurRelEnt LOOP
	   
			RAISE NOTICE 'DANS LOOP';
			
			creation_column:=true;
			
			/*select 	case CUR.ORDRE
						 when 97 then 4
						 when 98 then 5
						 when 99 then 6
						 when 100 then 7
						 when 101 then 8
						 when 102 then 9
						 when 103 then 10
						 when 104 then 11
						 when 105 then 12
						 when 106 then 13
						 when 107 then 14
						 when 108 then 15
						 when 109 then 16
						 when 110 then 17
						 when 111 then 18
						 when 112 then 19
						 when 113 then 20
						 when 114 then 21
						 when 115 then 22
						 when 116 then 23
						 when 117 then 24
						 when 118 then 25
						 when 119 then 26
						 when 120 then 27
						 when 121 then 28
						 when 122 then 29
						 when 123 then 30
						 when 124 then 31
						 when 125 then 32
						 when 126 then 33
						 when 127 then 34
						 when 128 then 35
						 end into v_ordre; */
						
			SELECT 	count(*) INTO nb_col_multi_lettre FROM INFORMATION_SCHEMA.COLUMNS
			where 	table_name='archives_releves' 
			and 	table_schema='sh_'||lower(station)
			and     (
						(POSITION(v_chaine_col_multi_lettre IN column_name)!=0 
						 and
	                     substr(column_name,1,length(column_name) - POSITION(v_chaine_col_multi_lettre IN column_name) - 1)=lower(CUR.nom_voie_sans_sta)
						 )
	                  or
						(column_name=lower(CUR.nom_voie_sans_sta))
					);
					
			SELECT 	count(*) INTO nb_col_multi_precision FROM INFORMATION_SCHEMA.COLUMNS
			where 	table_name='archives_releves' 
			and 	table_schema='sh_'||lower(station)
			and     (
						(POSITION(v_chaine_col_multi_precision IN column_name)!=0 
						 and
	                     substr(column_name,1,length(column_name) - POSITION(v_chaine_col_multi_precision IN column_name) - 1)=lower(CUR.nom_voie_sans_sta)
						 )
	                  or
						(column_name=lower(CUR.nom_voie_sans_sta))
					);
					
			nb_col_multi_lettre:=0;
			nb_col_multi_precision:=0;
			
			if nb_col_multi_lettre > 0 and nb_col_multi_precision = 0 then
					
				BEGIN
				select 	lower(lettre_voie) into strict v_lettre_voie
				from 	table_temporaire_entetes
				where 	lower(nom_voie)=lower(CUR.nom_voie_sta) and ordre=CUR.ordre and precision=CUR.precision
				and  	(replace(lower(nom_voie),'_'||lower(station),''),lower(lettre_voie),precision) not in 	(select 
																												 lower(
																												 substr(
																														substr(nom_voie,1,
																																		case POSITION(v_chaine_col_multi_lettre IN nom_voie)-1
																																		when -1 then length(nom_voie) else 
																																		POSITION(v_chaine_col_multi_lettre IN nom_voie)-1
																																		end
																															  ),
																														1,
																														case POSITION(v_chaine_col_multi_precision IN nom_voie)-1
																														when -1 then length(nom_voie) else 
																														POSITION(v_chaine_col_multi_precision IN nom_voie)-1
																														end
																														)
																													   ) as nom_voie,
																												 lower(lettre_voie),precision
																												 from table_temporaire_entetes_insert); 
				EXCEPTION	
					WHEN OTHERS THEN v_lettre_voie:='inconnue';
				END;
				
				select 
				case
				v_lettre_voie
                when '(' then 'aa'
				when ')' then 'bb'
				when '[' then 'cc'
				when ']' then 'dd'
				when '{' then 'ee'
				when '}' then 'ff'
				else v_lettre_voie
				end into v_lettre_voie_rec;
				
				v_nom_voie_sans_sta:=CUR.nom_voie_sans_sta||v_chaine_col_multi_lettre||v_lettre_voie_rec;
				
				RAISE NOTICE 'On trouve plusieurs voies de même nom avec meme precision % %',CUR.nom_voie_sans_sta,v_nom_voie_sans_sta;
			
			elsif nb_col_multi_lettre = 0 and nb_col_multi_precision > 0
			then
				
				select power(10,abs(CUR.precision))::character varying into v_lettre_voie_rec;
				
				v_nom_voie_sans_sta:=CUR.nom_voie_sans_sta||v_chaine_col_multi_precision||v_lettre_voie_rec;
				
				RAISE NOTICE 'On trouve une voie de même nom mais pas avec meme precision % %',CUR.nom_voie_sans_sta,v_nom_voie_sans_sta;
				
			elsif nb_col_multi_lettre > 0 and nb_col_multi_precision > 0
			then
				
				v_lettre_voie:='inconnue';
				BEGIN
				select 	lettre_voie into v_lettre_voie
				from 	table_temporaire_entetes
				where 	lower(nom_voie)=lower(CUR.nom_voie_sta) and ordre=CUR.ordre and precision=CUR.precision
				and  	(replace(lower(nom_voie),'_'||lower(station),''),lower(lettre_voie),precision) not in 
				                                                                                (select                                                                                     
																								 lower(
																								 substr(
																									substr(nom_voie,1,
																									                  case POSITION(v_chaine_col_multi_lettre IN nom_voie)
																													  when - 1 then length(nom_voie)
																													           else POSITION(v_chaine_col_multi_lettre IN nom_voie)-1
																													  end),
																										1,
																										substr(nom_voie,1,
																									                  case POSITION(v_chaine_col_multi_lettre IN nom_voie)
																													  when - 1 then length(nom_voie)
																													           else POSITION(v_chaine_col_multi_lettre IN nom_voie)-1
																													  end)
																									    )) as nom_voie_2,
																								 lower(lettre_voie),precision
																								 from table_temporaire_entetes_insert);
				EXCEPTION	
					WHEN OTHERS THEN v_lettre_voie:='inconnue';
				END;
				
				select 
				case
				v_lettre_voie
                when '(' then 'aa'
				when ')' then 'bb'
				when '[' then 'cc'
				when ']' then 'dd'
				when '{' then 'ee'
				when '}' then 'ff'
				else v_lettre_voie
				end into v_lettre_voie_rec;
				
				select power(10,abs(CUR.precision))::character varying into v_lettre_voie_rec_2;
				
				--v_nom_voie_sans_sta:=CUR.nom_voie_sans_sta||v_chaine_col_multi_lettre||v_lettre_voie_rec||v_chaine_col_multi_precision||v_lettre_voie_rec_2;
				v_nom_voie_avec_sta:=CUR.nom_voie_avec_sta||v_chaine_col_multi_lettre||v_lettre_voie_rec||v_chaine_col_multi_precision||v_lettre_voie_rec_2;
				
				--RAISE NOTICE 'On trouve plusieurs voie de même nom et avec precision differente % %',CUR.nom_voie_sans_sta,v_nom_voie_sans_sta;
				RAISE NOTICE 'On trouve plusieurs voie de même nom et avec precision differente % %',CUR.nom_voie_avec_sta,v_nom_voie_avec_sta;
				
			elsif nb_col_multi_lettre = 0 and nb_col_multi_precision = 0
			then
						
				v_ordre:=v_ordre+1;
				--v_nom_voie_sans_sta:=CUR.nom_voie_sans_sta;
				BEGIN
				n_precision:=CUR.precision::smallint;
				v_precision:=power(10,abs(n_precision))::character varying;
				EXCEPTION
				WHEN OTHERS THEN
				v_precision:=CUR.precision;
				END;
				v_nom_voie_avec_sta:=CUR.nom_voie_avec_sta||v_chaine_col_multi_lettre||CUR.lettre_voie||v_chaine_col_multi_precision||v_precision;
				--RAISE NOTICE 'On ne trouve pas plusieurs voie de même nom ou avec precision differente % % ',CUR.nom_voie_sans_sta,v_nom_voie_sans_sta;
				RAISE NOTICE 'On ne trouve pas plusieurs voie de même nom ou avec precision differente % % ',CUR.nom_voie_avec_sta,v_nom_voie_avec_sta;
			
			else
			
				 v_execute_1:='PB dans le if elsif';
				 message_retour:='NOK : '||v_execute_1;
				 creation_column:=false;
				 RAISE NOTICE 'Probleme % ',v_execute_1;
			
			end if;
			
			if creation_column then
			
				reussite_alter:=true;
				--v_execute_1:='ALTER TABLE '||table_archivage||' ADD COLUMN '||v_nom_voie_sans_sta||' character varying(100)';
				v_execute_1:='ALTER TABLE '||table_archivage||' ADD COLUMN '||v_nom_voie_avec_sta||' character varying(100)';
				RAISE NOTICE '%',v_execute_1;		
				BEGIN
				EXECUTE v_execute_1;
				EXCEPTION
				WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1; reussite_alter:=false;
				WHEN OTHERS THEN CONTINUE;
				END;
				
				if reussite_alter
				then
					
					RAISE NOTICE 'Reussite ALTER %',v_execute_1;
					--v_execute_1:='INSERT INTO '||table_tmp_entetes_insert||' values ('||CUR.ordre||','||''''||CUR.lettre_voie||''''||','||''''||v_nom_voie_sans_sta||''''||','||CUR.precision||')';
					v_execute_1:='INSERT INTO '||table_tmp_entetes_insert||' values ('||CUR.ordre||','||''''||CUR.lettre_voie||''''||','||''''||v_nom_voie_avec_sta||''''||','||CUR.precision||')';
					RAISE NOTICE '%',v_execute_1;
					BEGIN
					EXECUTE v_execute_1;
					EXCEPTION
					WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
					END;
					
					--v_execute_1:='INSERT INTO sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive(position,lettre_voie,nom_voie,precision) '||' values ('||v_ordre||','||''''||CUR.lettre_voie||''''||','||''''||v_nom_voie_sans_sta||''''||','||CUR.precision||')';
					v_execute_1:='INSERT INTO sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive(position,lettre_voie,nom_voie,precision) '||' values ('||v_ordre||','||''''||CUR.lettre_voie||''''||','||''''||v_nom_voie_avec_sta||''''||','||CUR.precision||')';
					RAISE NOTICE '%',v_execute_1;
					BEGIN
					EXECUTE v_execute_1;
					EXCEPTION
					WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
					END;
				
				else
				
					RAISE NOTICE 'Echec ALTER %',v_execute_1;
				
				end if;
				
			end if;
			
		END LOOP;
		
		return message_retour;

EXCEPTION

WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;