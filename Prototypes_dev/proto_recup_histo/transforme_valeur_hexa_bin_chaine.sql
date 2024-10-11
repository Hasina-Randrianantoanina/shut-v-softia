DROP FUNCTION public.transforme_valeur_hexa_bin_chaine;
CREATE OR REPLACE FUNCTION public.transforme_valeur_hexa_bin_chaine(
	valeur_brut in text,
	valeur_lettre in character varying(10)
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_valeur_brut text;
v_valeur_bin character varying(100);
v_lettre character varying(10);

BEGIN

if COALESCE(valeur_brut,'') = '' then return 'ERREUR=sans_valeur'; end if;
if COALESCE(valeur_lettre,'') = '' then return 'ERREUR=lettre_absente'; end if;

--RAISE NOTICE '%-%-%',POSITION(';'||valeur_lettre IN valeur_brut)+3,length(valeur_brut),POSITION(';'||valeur_lettre IN valeur_brut)-2;

v_valeur_brut:=substr(valeur_brut,POSITION(';'||valeur_lettre IN valeur_brut)+3,length(valeur_brut)-POSITION(';'||valeur_lettre IN valeur_brut)-2);
--RAISE NOTICE 'val=%#%',v_valeur_brut,POSITION(';' IN v_valeur_brut)-1;
v_valeur_brut:=substr(v_valeur_brut,1,POSITION(';' IN v_valeur_brut)-1);
--RAISE NOTICE 'v_valeur_brut=%',v_valeur_brut;

if length(v_valeur_brut)!=4 then return 'ERREUR='||valeur_brut||'-plus_de_4_caracteres'; end if;

for i in 1..4
loop
          v_valeur_bin:=substr(v_valeur_brut,i,1);
		  --RAISE NOTICE 'v_valeur_bin=%',v_valeur_bin;
		  if ascii(v_valeur_bin) not between 48 and 57 and ascii(v_valeur_bin) not between 65 and 70
		  then
				return 'ERREUR='||valeur_brut||'-caractere non hexa 16 : '||v_valeur_bin;
		  end if;
end loop;

select public.hex_to_bin(v_valeur_brut) into strict v_valeur_bin;

return v_valeur_bin;
		      			   
EXCEPTION

WHEN OTHERS THEN return 'ERREUR='||valeur_brut;

END
$BODY$;