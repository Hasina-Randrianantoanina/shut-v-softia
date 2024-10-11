DROP FUNCTION public.transforme_valeur_mesure_K;
CREATE OR REPLACE FUNCTION public.transforme_valeur_mesure_K(
	valeur_brut in character varying,
	v_precision in character varying
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_valeur_brut character varying(10);

BEGIN

if COALESCE(valeur_brut,'') = '' then return ''; end if;

if      ascii(substr(valeur_brut,1,1)) between 48 and 57 or ascii(substr(valeur_brut,1,1)) between 65 and 90
then
		BEGIN
		v_valeur_brut:=valeur_brut;
		return 'k'||(v_valeur_brut::integer/power(10,abs(v_precision::smallint)))::character varying;
		EXCEPTION
		WHEN OTHERS THEN return 'k'||v_valeur_brut;
		END;
else
		BEGIN
		v_valeur_brut:=substr(valeur_brut,3,length(valeur_brut)-1);
		return 'K'||(v_valeur_brut::integer/power(10,abs(v_precision::smallint)))::character varying;
		EXCEPTION
		WHEN OTHERS THEN return 'K'||v_valeur_brut;
		END;
end if;
		      			   
EXCEPTION

WHEN OTHERS THEN return 'ERREUR='||valeur_brut;

END
$BODY$;
