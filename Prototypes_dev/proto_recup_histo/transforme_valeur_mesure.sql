DROP FUNCTION public.transforme_valeur_mesure;
CREATE OR REPLACE FUNCTION public.transforme_valeur_mesure(
	valeur_brut in character varying,
	v_precision in character varying
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN

return (valeur_brut::integer/power(10,abs(v_precision::smallint)))::character varying;
		      			   
EXCEPTION

WHEN OTHERS THEN return valeur_brut;

END
$BODY$;