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
table_releves_affichage		character varying (100);
message_retour 				text:='OK';

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
									 lettre_voie,ordre,precision
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
													 libelle_evenement   character varying(100),
													 tor_affiche_ana     boolean,
													 ordre               integer,
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
    (time, ordre, evenement COLLATE pg_catalog."default" ASC NULLS LAST)
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
    (time, ordre)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_archivage_3';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_archivage_3
    ON '||table_archivage||' USING btree
    (time)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	---
	
	BEGIN
	table_releves_affichage:='sh_'||lower(station)||'.archives_releves_affichage';
	v_execute_1:='DROP TABLE '||table_releves_affichage;
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR DROP TABLE %',table_releves_affichage;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE '||table_releves_affichage||'(time 				 timestamp without time zone,
															 evenement 			 character varying(10),
													         libelle_evenement   character varying(100),
															 tor_affiche_ana     boolean,
															 ordre               integer,
													         )';
	RAISE NOTICE '%',v_execute_1;
	EXECUTE v_execute_1;
	EXCEPTION
		WHEN OTHERS THEN RAISE NOTICE 'ERREUR CREATE TABLE %',table_releves_affichage;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_archivage_affichage_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_archivage_affichage_1
    ON '||table_archivage||' USING btree
    (time, ordre, evenement COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_archivage_affichage_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_archivage_affichage_2
    ON '||table_archivage||' USING btree
    (time, ordre)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_table_archivage_affichage_3';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_table_archivage_affichage_3
    ON '||table_archivage||' USING btree
    (time)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	---
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
								when '||'''aa'''||' then 97
								when '||'''bb'''||' then 98
								when '||'''cc'''||' then 99
								when '||'''dd'''||' then 100
								when '||'''ee'''||' then 101
								when '||'''ff'''||' then 102
								when '||'''gg'''||' then 103
								when '||'''hh'''||' then 104
								when '||'''ii'''||' then 105
								when '||'''jj'''||' then 106
								when '||'''kk'''||' then 107
								when '||'''ll'''||' then 108
								when '||'''mm'''||' then 109
								when '||'''nn'''||' then 110
								when '||'''oo'''||' then 111
								when '||'''pp'''||' then 112
								when '||'''qq'''||' then 113
								when '||'''rr'''||' then 114
								when '||'''ss'''||' then 115
								when '||'''tt'''||' then 116
								when '||'''uu'''||' then 117
								when '||'''vv'''||' then 118
								when '||'''ww'''||' then 119
								when '||'''xx'''||' then 120
								when '||'''yy'''||' then 121
								when '||'''z1'''||' then 122
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