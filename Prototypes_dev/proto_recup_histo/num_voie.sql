DROP FUNCTION public.nom_voie;
CREATE OR REPLACE FUNCTION public.nom_voie(
	p_station in character varying (10),
	num_voie in character varying (10),
	type_voie in character varying (2),
	premier_libelle in character varying (100),
	libelle_erreur in character varying (100)
	)
    RETURNS character varying (100)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_voie             		character varying (100);
v_nombre				integer;
v_num_voie              smallint;

BEGIN

v_num_voie:=num_voie::smallint;
v_num_voie:=v_num_voie+1;

BEGIN

IF type_voie='RU' THEN

	select premier_libelle||' '||libel into v_voie from public.voies_internes where ini=p_station and num=v_num_voie limit 1;

ELSIF type_voie='RO' THEN

	select premier_libelle||' '||libel into v_voie from public.voies_resobs where ini=p_station and num=v_num_voie limit 1;

ELSIF type_voie='ET' THEN

	select premier_libelle||' '||libel into v_voie from public.es_tor where ini=p_station and num=v_num_voie and type=1 limit 1;

ELSIF type_voie='ST' THEN

	select premier_libelle||' '||libel into v_voie from public.es_tor where ini=p_station and num=v_num_voie and type=2 limit 1;

ELSIF type_voie='ET' THEN

	select premier_libelle||' '||libel into v_voie from public.es_tor where ini=p_station and num=v_num_voie and type=3 limit 1;

ELSE
	RETURN libelle_erreur;
END IF;

RETURN v_voie;
EXCEPTION
WHEN OTHERS THEN RETURN libelle_erreur;
END;				

EXCEPTION

WHEN OTHERS THEN RETURN libelle_erreur;

END
$BODY$;