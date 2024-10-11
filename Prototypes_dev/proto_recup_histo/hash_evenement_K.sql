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
v_chaine_tmp 			character varying(1000);
v_chaine_retour_k_100 	character varying(1000):='';
v_chaine_retour_k_0 	character varying(1000):='';
v_position_k_100 		integer;
v_position_k_0 			integer;

v_occurence_k_100 		integer:=1;
v_occurence_k_0   		integer:=1;
v_position_k_100_plus_1	integer:=1;

BEGIN

v_chaine_tmp:=v_chaine;

if public.instr(v_chaine,'K',1,v_occurence_k_100) = 0 then return replace(v_chaine,'k',','); end if;
if public.instr(v_chaine,'k',1,v_occurence_k_0) = 0 then return replace(v_chaine,'K',','); end if;


while v_arret
loop
		v_chaine_tmp:=COALESCE(v_chaine_tmp,'');
		
		v_position_k_100:=public.instr(v_chaine_tmp,'K',1,v_occurence_k_100);
		if v_position_k_100=0 then exit; end if;
		
		v_position_k_0:=public.instr(v_chaine_tmp,'k',1,v_occurence_k_0);
		v_position_k_100_plus_1:=public.instr(v_chaine_tmp,'K',1,v_occurence_k_100+1);
		
		if v_position_k_0=0 or (v_position_k_0 > v_position_k_100_plus_1 and v_position_k_100_plus_1 > 0)
		then 
			
			v_chaine_retour_k_0:=v_chaine_retour_k_0||',,';
			
			if v_position_k_100_plus_1-v_position_k_100-1 > 0
			then
				v_chaine_retour_k_100:=v_chaine_retour_k_100||','||substr(v_chaine_tmp,v_position_k_100+1,v_position_k_100_plus_1-v_position_k_100-1);
				RAISE NOTICE 'if v_position_k_100_plus_1-v_position_k_100-1 > 0 v_chaine_retour_k_100=%',v_chaine_retour_k_100;
			else
				if v_position_k_0=0
				then
				
					v_chaine_retour_k_100:=v_chaine_retour_k_100||','||substr(v_chaine_tmp,v_position_k_100+1,length(v_chaine_tmp)-v_position_k_100);
					RAISE NOTICE '!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 !!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 if v_position_k_0=0 v_chaine_retour_k_100=%',v_chaine_retour_k_100;
				
				else
				
					v_chaine_retour_k_100:=v_chaine_retour_k_100||','||substr(v_chaine_tmp,v_position_k_100+1,length(v_chaine_tmp)-v_position_k_100);
					RAISE NOTICE '!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 !!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 if v_position_k_0=0 v_chaine_retour_k_100=%',v_chaine_retour_k_100;
				
				
				end if;
			
			end if;
		
		else
			
			if v_position_k_100_plus_1 > 0
			then
			
				v_chaine_retour_k_0:=v_chaine_retour_k_0||','||substr(v_chaine_tmp,v_position_k_0+1,v_position_k_100_plus_1-v_position_k_0-1);
				RAISE NOTICE '!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 et if v_position_k_100_plus_1 > 0 v_chaine_retour_k_0=%',v_chaine_retour_k_0;
			
			else
				
				v_chaine_retour_k_0:=v_chaine_retour_k_0||','||substr(v_chaine_tmp,v_position_k_0+1,length(v_chaine_tmp)-v_position_k_0);
				RAISE NOTICE '!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 et !!! if v_position_k_100_plus_1 > 0 v_chaine_retour_k_0=%',v_chaine_retour_k_0;
				
			end if;
			
			v_chaine_retour_k_100:=v_chaine_retour_k_100||','||substr(v_chaine_tmp,v_position_k_100+1,v_position_k_0-v_position_k_100-1);
			RAISE NOTICE '!! if v_position_k_100_plus_1-v_position_k_100-1 > 0 v_chaine_retour_k_100=%-v_position_k_0=%-v_position_k_100=%',v_chaine_retour_k_100,v_position_k_0,v_position_k_100;
		
		end if;
		
		v_occurence_k_100:=v_occurence_k_100+1;
		v_occurence_k_0:=v_occurence_k_0+1;
		
end loop;

return ';K;'||v_chaine_retour_k_100||';k;'||v_chaine_retour_k_0;
		      			   
EXCEPTION

WHEN OTHERS THEN return 'ERREUR';

END
$BODY$;