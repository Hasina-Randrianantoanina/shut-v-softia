DROP FUNCTION public.diff_bit_etor;
CREATE OR REPLACE FUNCTION public.diff_bit_etor(
	p_valeur_bin_courant in character varying,
	p_valeur_bin_avant in character varying,
	p_lettre_voie in character varying
	)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE

v_lettre 				character varying(10);
v_bit_courant 			character varying(1);
v_bit_avant 			character varying(1);
v_num_carte             smallint;
v_valeur_bin_courant 	character varying(100);
v_valeur_bin_avant 		character varying(100);
v_num					smallint;
v_nom					character varying(100);
v_etat                  character varying(1);
v_nombre_chgt           smallint;
v_counter				smallint;
v_libelle               character varying(1000);

v_chaine                text;

i                       smallint;

BEGIN


if p_lettre_voie in ('za','zb','zc','zd','ze','zf')
then

	v_lettre:=substr(p_lettre_voie,2,1);
	
	v_num_carte:=ascii(v_lettre) -70;

else

	v_num_carte:=ascii(p_lettre_voie) -96;
	
end if;

--RAISE NOTICE 'v_num_carte=%',v_num_carte;

v_nombre_chgt:=0;

if p_valeur_bin_courant!=p_valeur_bin_avant
then

for i in 1..16
loop

v_bit_courant:=substr(p_valeur_bin_courant,length(p_valeur_bin_courant)-i+1,1);
v_bit_avant:=substr(p_valeur_bin_avant,length(p_valeur_bin_avant)-i+1,1);

	if v_bit_courant != v_bit_avant
	then
			
		  v_etat:=v_bit_courant;
		  v_num:=16-i+1;
		  v_nombre_chgt:=v_nombre_chgt+1;
	
	end if;

end loop;

--RAISE NOTICE 'v_nombre_chgt=%',v_nombre_chgt;

if v_nombre_chgt > 1
then
	return 'chgt plusieurs Etor#';
elsif v_nombre_chgt = 1
then
    --RAISE NOTICE '1 seul changement sur voie %',16*(v_num_carte-1)+16-v_num+1;
	BEGIN
	v_counter:=16*(v_num_carte-1)+16-v_num+1;
	select num::character varying as numero,libel into strict v_num,v_nom from table_es_etor where counter=v_counter;
	v_libelle:='chgt Etor de num '||COALESCE(v_num::character varying,'')||' et de nom '||v_nom||'#'||v_etat;
	EXCEPTION
	WHEN OTHERS THEN v_libelle:='Chgt Etor ???#';
	END;
else
	v_libelle:='#';
end if;

else
	v_libelle:='#';
end if;

return v_libelle;
		      			   
EXCEPTION

WHEN OTHERS THEN return 'ERREUR=';

END
$BODY$;