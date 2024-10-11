DROP FUNCTION public.calcul_etor;
CREATE OR REPLACE FUNCTION public.calcul_etor(
	p_valeur_brut in character varying,
	p_fichier in character varying,
	p_lettre_voie in character varying,
	p_nom_voie in character varying,
	p_ordre    in integer,
	p_time     in date
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_valeur_brut 			character varying(10);
v_valeur_bin_courant 	character varying(100);
v_valeur_bin_avant 		character varying(100);
v_num					character varying(10);
v_nom					character varying(100);
v_etat                  character varying(1);
v_nombre_chgt           smallint;
v_libelle               character varying(1000);

BEGIN

if NULLIF(p_valeur_brut,'') IS NULL then return ''; end if;

v_nombre_chgt:=0;
v_valeur_brut:=substr(p_valeur_brut,3,length(p_valeur_brut)-1);

select public.hex_to_bin(v_valeur_brut) into v_valeur_bin_courant;
--RAISE NOTICE 'v_valeur_bin_courant=%-v_valeur_brut=%',v_valeur_bin_courant,v_valeur_brut;

BEGIN
/*select decode_bin into strict v_valeur_bin_avant from table_hexa_bin_etor t1
where  t1.time=p_time
and    t1.fichier=p_fichier
and    t1.lettre_voie=p_lettre_voie 
and    nom_voie=p_nom_voie
and    t1.ordre=(select max(t2.ordre) from table_hexa_bin_etor t2 where t2.time=t1.time
                 and   t2.fichier=t1.fichier and t2.lettre_voie=t1.lettre_voie 
			     and   t2.nom_voie=t1.nom_voie and t2.ordre<p_ordre);*/
				 
select decode_bin into strict v_valeur_bin_avant from table_hexa_bin_etor t1
where  t1.time=p_time
and    t1.fichier=p_fichier
and    t1.lettre_voie=p_lettre_voie 
and    nom_voie=p_nom_voie
and    t1.ordre<p_ordre order by t1.ordre desc limit 1;

EXCEPTION
WHEN OTHERS THEN v_libelle:='Chgt Etor ???#'; return v_libelle;
END;

--RAISE NOTICE 'v_valeur_bin_avant=%',v_valeur_bin_avant;

select public.diff_bit_etor(v_valeur_bin_courant,v_valeur_bin_avant,p_lettre_voie) into v_libelle;
--v_libelle:=v_valeur_bin_avant;
--RAISE NOTICE 'v_libelle=%',v_libelle;

return v_libelle;

EXCEPTION

WHEN OTHERS THEN return 'ERREUR='||p_valeur_brut||'#';

END
$BODY$;