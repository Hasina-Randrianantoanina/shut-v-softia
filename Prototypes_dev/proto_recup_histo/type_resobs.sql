DROP FUNCTION public.type_resobs;
CREATE OR REPLACE FUNCTION public.type_resobs(
	p_station in character varying (10)
	)
    RETURNS character varying (2)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_station             	character varying (10);
v_nombre				integer;

BEGIN

BEGIN
select 1 into v_station from public.voies_internes where station=p_station;
RETURN 'RU';
EXCEPTION
WHEN OTHERS THEN
				BEGIN
				select 1 into v_station from public.voies_resobs where station=p_station;
				RETURN 'RO';
				EXCEPTION
				WHEN OTHERS THEN
								 BEGIN
								 v_nombre:=p_station::integer;
								 RETURN 'RO';
								 EXCEPTION
								 WHEN OTHERS THEN
									RETURN 'RU';
								 END;
								
				END;
END;

EXCEPTION

WHEN OTHERS THEN RETURN 'Réseau Inconnu';

END
$BODY$;