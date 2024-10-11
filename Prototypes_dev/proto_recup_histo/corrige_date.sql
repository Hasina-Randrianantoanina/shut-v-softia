DROP FUNCTION public.corrige_date;
CREATE OR REPLACE FUNCTION public.corrige_date(
	station in character varying (10)
	)
    RETURNS text
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_execute_1 			text;
v_execute_2 			text;
table_tmp				text;
message_retour 			text:='OK';

v_date_to_corrige	 	character varying (100);
v_jour	 				character varying (100);
v_fichier	 			character varying (100);
v_ordre                 character varying (10);
v_evenement             character varying (10);

v_chaine_column_name text;
v_chaine_column_name_affichage text;

CurDistinctJour CURSOR
                       for select distinct fichier, ch1 as jour from table_tmp
					   where time not like '%-%' and evenement='E' order by fichier,ch1; 
																				
			
error_msg text;

BEGIN

table_tmp:='table_tmp';

BEGIN
v_execute_1:='DROP TABLE '||table_tmp;
RAISE NOTICE 'DROP=%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR dans %',v_execute_1;
END;

BEGIN
v_execute_1:='CREATE TEMPORARY TABLE '||table_tmp||' AS SELECT * FROM sh_'||lower(station)||'.tmp_load_file_jour_releves
			  where time not like '||''''||'%-%'||'''';
			  
RAISE NOTICE 'CREATE=%',v_execute_1;		
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREUR1 dans %',v_execute_1;message_retour:='NOK1 : '||v_execute_1;
END;

RAISE NOTICE 'avant loop';

FOR CUR1 IN CurDistinctJour
LOOP

	v_jour:=CUR1.jour;
	v_fichier:=CUR1.fichier;
	
	RAISE NOTICE 'v_jour=%-v_fichier=%',v_jour,v_fichier;
	
	v_execute_1:='update sh_'||lower(station)||'.tmp_load_file_jour_releves set time='||''''||substr(v_jour,7,4)||'-'||substr(v_jour,4,2)||'-'||substr(v_jour,1,2)||''''||
	'||'||'time where fichier='||''''||v_fichier||''''||' and time not like '||''''||'%-%'||'''';
			
	RAISE NOTICE 'UPDATE=%',v_execute_1;
			
	BEGIN
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR2 dans %',v_execute_1;message_retour:='NOK';
	END;


END LOOP;

RAISE NOTICE 'apres loop';

return message_retour;

EXCEPTION

WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;