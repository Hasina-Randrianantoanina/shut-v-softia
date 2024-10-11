DROP FUNCTION public.fill_reconditionne_archives;
CREATE OR REPLACE FUNCTION public.fill_reconditionne_archives(
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
message_retour 			text:='OK';

v_column_name	 character varying (100);
v_column_name_1	 character varying (100);

v_chaine_column_name text;
v_chaine_column_name_affichage text;

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
v_position integer:=0;

reussite_alter boolean:=true;
reussite_insert	boolean:=true;

CurArchivageAffichage CURSOR(v_station character varying (10)) 
                        FOR select column_name as nom_colonne 
						from  INFORMATION_SCHEMA.COLUMNS
						where table_name='archives_releves_affichage'
						and   table_schema='sh_'||lower(v_station)
						and   ordinal_position>5
						order by ordinal_position;
						
CurArchivage CURSOR(v_station character varying (10), v_column character varying (100)) 
                        FOR select column_name as nom_colonne 
						from  INFORMATION_SCHEMA.COLUMNS
						where table_name='archives_releves'
						and   table_schema='sh_'||lower(v_station)
						and   ordinal_position>5
						and   substr(column_name,1,position('____' in column_name)-1)=v_column;

CurColumnTable CURSOR FOR select nom_colonne,ordinal_position from TMP_TABLE order by ordinal_position;

CurColumn CURSOR(v_station character varying (10))
	             FOR select distinct substr(column_name,1,position('____' in column_name)-1) as nom_colonne 
	             from  INFORMATION_SCHEMA.COLUMNS
	             where table_name='archives_releves'
	             and   table_schema='sh_'||lower(v_station)
	 			 and   ordinal_position>5;
														

CurColumnMulti CURSOR(v_station character varying (10)) FOR SELECT substr(column_name,1,position('____' in column_name)-1) as column_name_orig
														FROM   INFORMATION_SCHEMA.COLUMNS
														where  table_name='archives_releves' 
														and    table_schema='sh_'||lower(v_station)
														and    ordinal_position>5
														group by column_name_orig
														having count(*) > 1;

CurColumnMulti2 CURSOR(v_station character varying (10), v_column character varying (100))
	                                                    FOR select column_name 
	                                                    from  INFORMATION_SCHEMA.COLUMNS
	                                                    where table_name='archives_releves'
	                                                    and   table_schema='sh_'||lower(v_station)
	                                                    and   position(v_column in column_name)!=0;
															
			
error_msg text;

BEGIN

message_retour:='';
v_nombre_ligne:=0;

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
	if v_count > 0 then message_retour:=message_retour||'-Incohence dans les champs '||v_chaine_column_name||' : nombre de lignes non 0 : '||v_count; v_nombre_ligne:=1; end if;
    RAISE NOTICE 'v_chaine_column_name=%-nombre=%',v_chaine_column_name,v_count;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR1 dans %',v_execute_1;
	END;

END LOOP;

if NULLIF(message_retour,'') IS NULL then message_retour:='OK'; end if;

RAISE NOTICE 'APRES VERIF COLUMN';

if v_nombre_ligne = 0
then

v_chaine_column_name:='(';

FOR CUR1 IN CurColumn(station)
LOOP
		 v_column_name:=CUR1.nom_colonne;
		 
		 v_chaine_column_name:=v_chaine_column_name||''''||v_column_name||''''||',';
		 
END LOOP;

v_chaine_column_name:=substr(v_chaine_column_name,1,length(v_chaine_column_name)-1)||')';
RAISE NOTICE 'v_chaine_column_name=%',v_chaine_column_name;
v_execute_2:='';
v_execute_2:='select substr(column_name,1,position('||''''||'____'||''''||' in column_name)-1) as nom_colonne,ordinal_position
			  from  INFORMATION_SCHEMA.COLUMNS
	          where table_name='||''''||'archives_releves'||''''||' and table_schema='||''''||'sh_'||lower(station)||''''||
	          ' and   ordinal_position>3 and substr(column_name,1,position('||''''||'____'||''''||' in column_name)-1) in '||v_chaine_column_name||'
				order by ordinal_position';

RAISE NOTICE 'requete=%',v_execute_2;

	BEGIN
	v_execute_1:='DROP TABLE TMP_TABLE';
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR2a dans %',v_execute_1;
	END;
	
	BEGIN
	v_execute_1:='CREATE TEMPORARY TABLE TMP_TABLE(nom_colonne character varying (100),ordinal_position integer)';
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR2b dans %',v_execute_1;
	END;

	BEGIN
	v_execute_1:='INSERT INTO TMP_TABLE '||v_execute_2;
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR3 dans %',v_execute_2;
	END;
	
	
	BEGIN
	v_execute_1:='truncate table sh_'||lower(station)||'.archives_releves_affichage';
	EXECUTE v_execute_1;
	EXCEPTION
	WHEN OTHERS THEN RAISE NOTICE 'ERREUR3b dans %',v_execute_2;
	END;
	
RAISE NOTICE 'AVANT AJOUT COLUMN table archivage affichage';

FOR CUR2 IN CurColumnTable
LOOP
		 v_column_name:=CUR2.nom_colonne;
		 v_execute_1:='ALTER TABLE sh_'||lower(station)||'.archives_releves_affichage ADD COLUMN '||v_column_name||' character varying(100)';
		 RAISE NOTICE '%',v_execute_1;		
		 BEGIN
		 EXECUTE v_execute_1;
		 EXCEPTION
		 WHEN OTHERS THEN RAISE NOTICE 'ERREUR4 dans %',v_execute_1;message_retour:='NOK : '||v_execute_1; reussite_alter:=false;
		 WHEN OTHERS THEN CONTINUE;
		 END;
END LOOP;

reussite_alter:=true;
message_retour:='OK';

RAISE NOTICE 'APRES AJOUT COLUMN table archivage affichage';

if not reussite_alter then message_retour:='NOK;Echec lors de l''ajout de colonne dans la table sh_'||lower(station)||'.archives_releves_affichage';
end if;

reussite_insert:=true;

RAISE NOTICE 'AVANT insert dans table archivage affichage';


v_chaine_column_name_affichage:='insert into sh_'||lower(station)||'.archives_releves_affichage(time,evenement,libelle_evenement,';
FOR CUR IN CurArchivageAffichage(station)
LOOP
		 v_chaine_column_name_affichage:=v_chaine_column_name_affichage||','||CUR.nom_colonne;	 	 
END LOOP;
v_chaine_column_name_affichage:=substr(v_chaine_column_name_affichage,1,length(v_chaine_column_name_affichage)-1)||')';

RAISE NOTICE 'v_chaine_column_name_affichage=%',v_chaine_column_name_affichage;

v_chaine_column_name:=' select time,evenement,libelle_evenement,COALESCE(';

FOR CUR IN CurArchivageAffichage(station)
LOOP
		 
		 FOR CUR2 IN CurArchivage(station,CUR.nom_colonne)
		 LOOP
			 
			 v_chaine_column_name:=v_chaine_column_name||CUR2.nom_colonne||',';
			 
		 END LOOP;
		 
		 v_chaine_column_name:=substr(v_chaine_column_name,1,length(v_chaine_column_name)-1)||')';
		 
		 v_chaine_column_name:=v_chaine_column_name||',COALESCE(';
		  		
		 	 
END LOOP;

v_chaine_column_name:=substr(v_chaine_column_name,1,length(v_chaine_column_name)-11)||')';
RAISE NOTICE 'v_chaine_column_name=%',v_chaine_column_name;

v_execute_1:=v_chaine_column_name_affichage||v_chaine_column_name;
RAISE NOTICE 'v_execute_1=%',v_execute_1;

BEGIN
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREURInsert dans %',v_execute_1;message_retour:='NOK : '||v_execute_1; reussite_insert:=false;
WHEN OTHERS THEN RAISE 'ERREURInsert';
END;

RAISE NOTICE 'APRES insert dans table archivage affichage';

end if;

if not reussite_insert then RAISE NOTICE 'Echec lors Insert dans sh_%.archives_releves_affichage',lower(station); end if;


v_execute_1:='ANALYSE sh_'||lower(station)||'.archives_releves_affichage';
	 
BEGIN
EXECUTE v_execute_1;
EXCEPTION
WHEN OTHERS THEN RAISE NOTICE 'ERREURANALYSE dans %',v_execute_1;message_retour:='NOK : '||v_execute_1;
END;

			 
return message_retour;

EXCEPTION

WHEN OTHERS THEN message_retour:='GLOBALNOK';return message_retour;

END
$BODY$;