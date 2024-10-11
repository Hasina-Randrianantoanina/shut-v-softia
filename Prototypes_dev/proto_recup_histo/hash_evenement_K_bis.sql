DROP FUNCTION public.hash_evenement_K;
CREATE OR REPLACE FUNCTION public.hash_evenement_K(
	v_chaine in character varying
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_arret boolean:=true;

v_chaine_tmp 			character varying(100);
v_chaine_tmp_2 			character varying(100);

v_chaine_retour_k_100 	character varying(1000):='';
v_chaine_retour_k_0 	character varying(1000):='';

v_position_k_100 		integer;
v_position_k_0 			integer;
v_position_k_100_plus_1	integer:=1;
v_position_k_0_local 	integer;

v_occurence_k_100 		integer:=1;
v_occurence_k_0   		integer:=1;


BEGIN

v_chaine_tmp:=v_chaine;

if public.instr(v_chaine,'K',1,v_occurence_k_100) = 0 then return substr(replace(v_chaine,'k',','),2,length(replace(v_chaine,'k',','))-1); end if;
if public.instr(v_chaine,'k',1,v_occurence_k_0) = 0 then return substr(replace(v_chaine,'K',','),2,length(replace(v_chaine,'K',','))-1); end if;

v_chaine_tmp:=COALESCE(v_chaine,'');

while v_arret
loop
				
		v_position_k_100:=public.instr(v_chaine,'K',1,v_occurence_k_100);
		v_position_k_100_plus_1:=public.instr(v_chaine,'K',1,v_occurence_k_100+1);
		v_position_k_0:=public.instr(v_chaine,'k',1,v_occurence_k_0);
		
		if v_position_k_100=0 and v_position_k_0=0 then exit; end if;
		
		if v_position_k_100>0 and v_position_k_100_plus_1>0
		then
		
			v_chaine_tmp:=substr(v_chaine,v_position_k_100,v_position_k_100_plus_1-v_position_k_100);
			RAISE NOTICE '%--if v_position_k_100>0 and v_position_k_100_plus_1>0 v_chaine_tmp=%',v_occurence_k_0,v_chaine_tmp;
			v_position_k_0_local:=public.instr(v_chaine_tmp,'k',1,1);
			
			if v_position_k_0_local=0
			then
					v_chaine_retour_k_0:=v_chaine_retour_k_0||',';
					v_chaine_tmp_2:=substr(v_chaine_tmp,1+1,length(v_chaine_tmp)-1);
					RAISE NOTICE '%--if v_position_k_100>0 and v_position_k_100_plus_1>0 + if v_position_k_0_local=0 v_chaine_tmp_2=%',v_occurence_k_0,v_chaine_tmp_2;
					v_chaine_retour_k_100:=v_chaine_retour_k_100||','||v_chaine_tmp_2;
			else
									
					v_chaine_tmp_2:=substr(v_chaine_tmp,1+1,v_position_k_0_local-1-1);
					RAISE NOTICE '%--if v_position_k_100>0 and v_position_k_100_plus_1>0 + !! if v_position_k_0_local=0 v_chaine_tmp_2 100=%',v_occurence_k_0,v_chaine_tmp_2;
					v_chaine_retour_k_100:=v_chaine_retour_k_100||','||v_chaine_tmp_2;
					
					v_chaine_tmp_2:=substr(v_chaine_tmp,v_position_k_0_local+1,v_position_k_100_plus_1-v_position_k_0_local-1);
					RAISE NOTICE '%--if v_position_k_100>0 and v_position_k_100_plus_1>0 + !! if v_position_k_0_local=0 v_chaine_tmp_2 0 =%',v_occurence_k_0,v_chaine_tmp_2;
					v_chaine_retour_k_0:=v_chaine_retour_k_0||','||v_chaine_tmp_2;
			
			end if;
			
		else
			
			v_chaine_tmp:=substr(v_chaine,v_position_k_100,length(v_chaine)-v_position_k_100+1);
			RAISE NOTICE '%--!! if v_position_k_100>0 and v_position_k_100_plus_1>0 v_chaine_tmp=%',v_occurence_k_0,v_chaine_tmp;
			v_position_k_0_local:=public.instr(v_chaine_tmp,'k',1,1);
			
			if v_position_k_0_local=0
			then
					v_chaine_retour_k_0:=v_chaine_retour_k_0||',';
					v_chaine_tmp_2:=substr(v_chaine_tmp,1+1,length(v_chaine_tmp)-1);
					RAISE NOTICE '%--!! if v_position_k_100>0 and v_position_k_100_plus_1>0 + if v_position_k_0_local=0 v_chaine_tmp_2 100=%',v_occurence_k_0,v_chaine_tmp_2;
					v_chaine_retour_k_100:=v_chaine_retour_k_100||','||v_chaine_tmp_2;
			else
					v_chaine_tmp_2:=substr(v_chaine_tmp,1+1,v_position_k_0_local-1-1);
					RAISE NOTICE '%--!! if v_position_k_100>0 and v_position_k_100_plus_1>0 + !! if v_position_k_0_local=0 v_chaine_tmp_2 100=%',v_occurence_k_0,v_chaine_tmp_2;
					v_chaine_retour_k_100:=v_chaine_retour_k_100||','||v_chaine_tmp_2;
					
					v_position_k_0:=public.instr(v_chaine,'k',1,v_occurence_k_0);
					v_chaine_tmp_2:=substr(v_chaine_tmp,v_position_k_0_local+1,length(v_chaine_tmp)-v_position_k_0_local);
					RAISE NOTICE '%--!! if v_position_k_100>0 and v_position_k_100_plus_1>0 + !! if v_position_k_0_local=0 v_chaine_tmp_2 0=%',v_occurence_k_0,v_chaine_tmp_2;
					v_chaine_retour_k_0:=v_chaine_retour_k_0||','||v_chaine_tmp_2;
					
			end if;

        end if;		
		
		v_occurence_k_100:=v_occurence_k_100+1;
		v_occurence_k_0:=v_occurence_k_0+1;
		
end loop;

return ';K;'||v_chaine_retour_k_100||';k;'||v_chaine_retour_k_0;
		      			   
EXCEPTION

WHEN OTHERS THEN return 'ERREUR';

END
$BODY$;