DROP FUNCTION public.transforme_valeur_hexa;
CREATE OR REPLACE FUNCTION public.transforme_valeur_hexa(
	valeur_hexa in character varying
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_valeur_hexa character varying(10);
v_valeur_bin character varying(100);

BEGIN

if COALESCE(valeur_hexa,'') = '' then return ''; end if;


BEGIN
v_valeur_hexa:=substr(valeur_hexa,3,length(valeur_hexa)-1);
RAISE NOTICE 'v_valeur_hexa=%',v_valeur_hexa;
select public.hex_to_bin(v_valeur_hexa) into v_valeur_bin;
RAISE NOTICE 'v_valeur_bin=%',v_valeur_bin;
if length(v_valeur_bin) != 16 then return 'ERREUR='||v_valeur_bin; end if;
EXCEPTION
WHEN OTHERS THEN return 'ERREUR='||v_valeur_hexa;
END;

return v_valeur_bin;
		      			   
EXCEPTION

WHEN OTHERS THEN return 'ERREUR='||valeur_hexa;

END
$BODY$;
