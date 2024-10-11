DROP FUNCTION public.creation_environnement_load;
CREATE OR REPLACE FUNCTION public.creation_environnement_load(
	station in character varying(10),
	annee in character varying(4)
	)
    RETURNS text
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

	v_execute_1 text;
    message_retour text:='OK';
	rd record;
			 
BEGIN
 
	
	BEGIN
	v_execute_1:='DROP SCHEMA sh_'||lower(station)||' cascade';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	
	BEGIN
	v_execute_1:='CREATE SCHEMA sh_'||lower(station);
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
		
	
	BEGIN
	v_execute_1:='DROP TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana(
    lettre_voie character varying(100),
	nom_voie character varying(100),
	precision character varying(100),
	fichier character varying(100))';
	RAISE NOTICE '%',v_execute_1;
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	
	BEGIN
	v_execute_1:='ALTER TABLE IF EXISTS sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana
    OWNER to postgres';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_1
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana USING btree
    (lettre_voie ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_2
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana USING btree
    (lettre_voie ASC NULLS LAST, nom_voie COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_3';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_3
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana USING btree
    (lettre_voie ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_4';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_voie_ana_4
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana USING btree
    (fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	
	BEGIN
	v_execute_1:='DROP TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves
    (
			time character varying(100) COLLATE pg_catalog."default",
			evenement character varying(10) COLLATE pg_catalog."default",
			fichier character varying(100) COLLATE pg_catalog."default",
			ordre integer,
			ch1 character varying(100) COLLATE pg_catalog."default",
			ch2 character varying(100) COLLATE pg_catalog."default",
			ch3 character varying(100) COLLATE pg_catalog."default",
			ch4 character varying(100) COLLATE pg_catalog."default",
			ch5 character varying(100) COLLATE pg_catalog."default",
			ch6 character varying(100) COLLATE pg_catalog."default",
			ch7 character varying(100) COLLATE pg_catalog."default",
			ch8 character varying(100) COLLATE pg_catalog."default",
			ch9 character varying(100) COLLATE pg_catalog."default",
			ch10 character varying(100) COLLATE pg_catalog."default",
			ch11 character varying(100) COLLATE pg_catalog."default",
			ch12 character varying(100) COLLATE pg_catalog."default",
			ch13 character varying(100) COLLATE pg_catalog."default",
			ch14 character varying(100) COLLATE pg_catalog."default",
			ch15 character varying(100) COLLATE pg_catalog."default",
			ch16 character varying(100) COLLATE pg_catalog."default",
			ch17 character varying(100) COLLATE pg_catalog."default",
			ch18 character varying(100) COLLATE pg_catalog."default",
			ch19 character varying(100) COLLATE pg_catalog."default",
			ch20 character varying(100) COLLATE pg_catalog."default",
			ch21 character varying(100) COLLATE pg_catalog."default",
			ch22 character varying(100) COLLATE pg_catalog."default",
			ch23 character varying(100) COLLATE pg_catalog."default",
			ch24 character varying(100) COLLATE pg_catalog."default",
			ch25 character varying(100) COLLATE pg_catalog."default",
			ch26 character varying(100) COLLATE pg_catalog."default",
			ch27 character varying(100) COLLATE pg_catalog."default",
			ch28 character varying(100) COLLATE pg_catalog."default",
			ch29 character varying(100) COLLATE pg_catalog."default",
			ch30 character varying(100) COLLATE pg_catalog."default",
			ch31 character varying(100) COLLATE pg_catalog."default",
			ch32 character varying(100) COLLATE pg_catalog."default",
			ch33 character varying(100) COLLATE pg_catalog."default",
			ch34 character varying(100) COLLATE pg_catalog."default",
			ch35 character varying(100) COLLATE pg_catalog."default",
			ch36 character varying(100) COLLATE pg_catalog."default",
			ch37 character varying(100) COLLATE pg_catalog."default",
			ch38 character varying(100) COLLATE pg_catalog."default",
			ch39 character varying(100) COLLATE pg_catalog."default",
			ch40 character varying(100) COLLATE pg_catalog."default",
			ch41 character varying(100) COLLATE pg_catalog."default",
			ch42 character varying(100) COLLATE pg_catalog."default",
			ch43 character varying(100) COLLATE pg_catalog."default",
			ch44 character varying(100) COLLATE pg_catalog."default",
			ch45 character varying(100) COLLATE pg_catalog."default",
			ch46 character varying(100) COLLATE pg_catalog."default",
			ch47 character varying(100) COLLATE pg_catalog."default",
			ch48 character varying(100) COLLATE pg_catalog."default",
			ch49 character varying(100) COLLATE pg_catalog."default",
			ch50 character varying(100) COLLATE pg_catalog."default",
			ch51 character varying(100) COLLATE pg_catalog."default",
			ch52 character varying(100) COLLATE pg_catalog."default",
			ch53 character varying(100) COLLATE pg_catalog."default",
			ch54 character varying(100) COLLATE pg_catalog."default",
			ch55 character varying(100) COLLATE pg_catalog."default",
			ch56 character varying(100) COLLATE pg_catalog."default",
			ch57 character varying(100) COLLATE pg_catalog."default",
			ch58 character varying(100) COLLATE pg_catalog."default",
			ch59 character varying(100) COLLATE pg_catalog."default",
			ch60 character varying(100) COLLATE pg_catalog."default"
			) TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_1';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_1
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves USING btree
    (time, evenement COLLATE pg_catalog."default" ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST,ordre)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_2';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_2
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves USING btree
    (time, evenement COLLATE pg_catalog."default" ASC NULLS LAST, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_3';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_3
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves USING btree
    (time, fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_4';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_4
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves USING btree
   (time)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_5';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE INDEX sh_'||lower(station)||'_ind_tmp_load_file_jour_releves_5
    ON sh_'||lower(station)||'.tmp_load_file_jour_releves USING btree
   (fichier COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP TABLE sh_'||lower(station)||'.load_log';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE sh_'||lower(station)||'.load_log(time character varying(100),evenement character varying(100),error_msg text)';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='DROP TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TABLE sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive(
    lettre_voie character varying(10),
	nom_voie character varying(100),
	precision character varying(100),
	position smallint)';
	RAISE NOTICE '%',v_execute_1;
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	
	BEGIN
	v_execute_1:='ALTER TABLE IF EXISTS sh_'||lower(station)||'.tmp_load_file_jour_releves_voie_ana_archive
    OWNER to postgres';
	RAISE NOTICE '%',v_execute_1;		
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'Erreur dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
	END;
	
	return message_retour;

EXCEPTION

WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;