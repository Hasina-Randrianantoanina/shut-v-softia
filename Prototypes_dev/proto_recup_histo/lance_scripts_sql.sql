delete from sh_pd.archives_releves_log;
delete from sh_pd.archives_releves where evenement in ('R','O','S','&');
select public.fill_archives_TOR(
	'PD','X'
	);
select * from sh_pd.archives_releves_log; 


bug sur num 
R	   a2090 0010000010010000
S08h00 a2098 0010000010011000 13         12 "INT_POS4_PD"
S08h00 a0018 0000000000011000 plusieurs
S08h00 a0818 0000100000011000 5			 4  "INT_POS4_PD"

"2009-09-08 08:00:00"	"S"	"chgt Etor de num 4 et de nom PPL_PERS_PD"	"1"
"2009-09-08 08:00:00"	"S"	"chgt plusieurs Etor"	"aa0018"
"2009-09-08 08:00:00"	"S"	"chgt Etor de num 12 et de nom INT_POS4_PD"	"1"

	a2090

R	   a2090 0010000010010000
S08h00 a2098 0010000010011000 13          12 "INT_POS4_PD"
S08h00 a0018 0000000000011000 plusieurs
S08h00 a0818 0000100000011000 5		4	"PPL_PERS_PD"

OK sur num.

select *
from sh_pd.archives_releves where evenement in ('R','O','S','&') --and time='2017-01-05 11:33:00'
and length(COALESCE(yav_v1_pd____aa________100,'')||
COALESCE(yav_v1_pd3____aa________100,'')||
COALESCE(qav_v1_pd____bb________100,'')||
COALESCE(qav_v1_pd3____bb________100,'')||
COALESCE(vav_v1_pd____cc________100,'')||
COALESCE(vav_v1_pd3____cc________100,'')||
COALESCE(yco_vmer_pd____cc________100,'')||
COALESCE(vav_v1_pd____cc________1000,'')||
COALESCE(qco_vmer_pd____dd________100,'')||
COALESCE(yco_vmer_pd____dd________100,'')||
COALESCE(yco_vmer_pd3____dd________100,'')||
COALESCE(qco_vmer_pd____ee________100,'')||
COALESCE(qco_vmer_pd3____ee________100,'')||
COALESCE(vco_2vmer_pd____ee________100,'')||
COALESCE(vco_1vmer_pd____ff________100,'')||
COALESCE(vco_1vmer_pd3____ff________100,'')||
COALESCE(yam_v4_pd____ff________100,'')||
COALESCE(vco_1vmer_pd____ff________1000,'')||
COALESCE(vco_2vmer_pd____gg________100,'')||
COALESCE(vco_2vmer_pd3____gg________100,'')||
COALESCE(yam_v1_pd____gg________100,'')||
COALESCE(vco_2vmer_pd____gg________1000,'')||
COALESCE(vco_3vmer_pd____hh________100,'')||
COALESCE(vco_3vmer_pd3____hh________100,'')||
COALESCE(ybp_bp_pd____hh________100,'')||
COALESCE(vco_3vmer_pd____hh________1000,'')||
COALESCE(yam_v4_pd____ii________100,'')||
COALESCE(yam_v4_pd3____ii________100,'')||
COALESCE(yam_v1_pd____jj________100,'')||
COALESCE(yam_v1_pd3____jj________100,'')||
COALESCE(ybp_bp_pd____kk________100,'')||
COALESCE(ybp_bp_pd3____kk________100,'')||
COALESCE(yep_moree_pd____ll________100,'')||
COALESCE(yep_moree_pd3____ll________100,'')||
COALESCE(qep_moree_pd____mm________100,'')||
COALESCE(qep_moree_pd3____mm________100,'')||
COALESCE(vep_1moree_pd____nn________100,'')||
COALESCE(vep_1moree_pd3____nn________100,'')||
COALESCE(vep_1moree_pd____nn________1000,'')||
COALESCE(vep_2moree_pd____oo________100,'')||
COALESCE(vep_2moree_pd3____oo________100,'')||
COALESCE(vep_2moree_pd____oo________1000,'')||
COALESCE(vep_3moree_pd____pp________100,'')||
COALESCE(vep_3moree_pd3____pp________100,'')||
COALESCE(vep_3moree_pd____pp________1000,'')||
COALESCE(nb_appel_pd____qq________10,'')||
COALESCE(nb_appel_pd3____qq________10,'')||
COALESCE(qav_uf_v1_pd____rr________100,'')||
COALESCE(qav2_v1_pd____rr________100,'')||
COALESCE(qav2_v1_pd3____rr________100,'')||
COALESCE(yep_croult_cr____ss________100,'')||
COALESCE(qep_croult_cr____tt________100,'')||
COALESCE(qep_uf_moree_pd____tt________100,'')||
COALESCE(qep_croult_uf_cr____uu________100,'')||
COALESCE(vep_1dcroult_cr____vv________1000,'')||
COALESCE(vep_2dcroult_cr____ww________1000,'')||
COALESCE(vep_3dcroult_cr____xx________1000,'')||
COALESCE(vep_1gcroult_cr____yy________1000,'')||
COALESCE(vep_2gcroult_cr____z1________1000,'')||
COALESCE(vep_3gcroult_cr____za________1000,'')||
COALESCE(qco_uf_vmer_pd____zb________100,'')) > 6 


select *
from sh_pd.archives_releves where evenement in ('R','O','S','&')
and NULLIF(libelle_evenement,'') IS NULL



select * from sh_pd.archives_releves_log where evenement in ('R','O','S','&')
	

S04h20 c0C24
S04h20 c0824

select 'COALESCE('||column_name||','||''''')'||'||'
	                                                    from  INFORMATION_SCHEMA.COLUMNS
	                                                    where table_name='archives_releves'
	                                                    and   table_schema='sh_pd'
	                                                    and   ordinal_position > 3
	                                                    order by ordinal_position asc

S04h02 c0C24
S04h02 c0824

select public.hex_to_bin('0824') 
select public.hex_to_bin('0C24') -- 0000110000100100

R 0824 0000100000100100
S 0C24 0000110000100100	
S 0824 0000100000100100		
select 	
select * from sh_pd.archives_releves where evenement in ('R','O','S','&')
and yav_v1_pd____aa________100||
yav_v1_pd3____aa________100||
qav_v1_pd____bb________100||
qav_v1_pd3____bb________100||
vav_v1_pd____cc________100||
vav_v1_pd3____cc________100||
yco_vmer_pd____cc________100||
vav_v1_pd____cc________1000||
qco_vmer_pd____dd________100||
yco_vmer_pd____dd________100||
yco_vmer_pd3____dd________100||
qco_vmer_pd____ee________100||
qco_vmer_pd3____ee________100||
vco_2vmer_pd____ee________100||
vco_1vmer_pd____ff________100||
vco_1vmer_pd3____ff________100||
yam_v4_pd____ff________100||
vco_1vmer_pd____ff________1000||
vco_2vmer_pd____gg________100||
vco_2vmer_pd3____gg________100||
yam_v1_pd____gg________100||
vco_2vmer_pd____gg________1000||
vco_3vmer_pd____hh________100||
vco_3vmer_pd3____hh________100||
ybp_bp_pd____hh________100||
vco_3vmer_pd____hh________1000||
yam_v4_pd____ii________100||
yam_v4_pd3____ii________100||
yam_v1_pd____jj________100||
yam_v1_pd3____jj________100||
ybp_bp_pd____kk________100||
ybp_bp_pd3____kk________100||
yep_moree_pd____ll________100||
yep_moree_pd3____ll________100||
qep_moree_pd____mm________100||
qep_moree_pd3____mm________100||
vep_1moree_pd____nn________100||
vep_1moree_pd3____nn________100||
vep_1moree_pd____nn________1000||
vep_2moree_pd____oo________100||
vep_2moree_pd3____oo________100||
vep_2moree_pd____oo________1000||
vep_3moree_pd____pp________100||
vep_3moree_pd3____pp________100||
vep_3moree_pd____pp________1000||
nb_appel_pd____qq________10||
nb_appel_pd3____qq________10||
qav_uf_v1_pd____rr________100||
qav2_v1_pd____rr________100||
qav2_v1_pd3____rr________100||
yep_croult_cr____ss________100||
qep_croult_cr____tt________100||
qep_uf_moree_pd____tt________100||
qep_croult_uf_cr____uu________100||
vep_1dcroult_cr____vv________1000||
vep_2dcroult_cr____ww________1000||
vep_3dcroult_cr____xx________1000||
vep_1gcroult_cr____yy________1000||
vep_2gcroult_cr____z1________1000||
vep_3gcroult_cr____za________1000||
qco_uf_vmer_pd____zb________100 not in ('0','1')
select * from sh_pd.archives_releves_log where evenement in ('R','O','S','&')

delete from sh_pd.archives_releves_log;
delete from sh_pd.archives_releves where evenement in ('R','O','S','&');

select substr('chgt Etor de num 43 et de nom DPP_P1_PD#1',1,POSITION('#' IN 'chgt Etor de num 43 et de nom DPP_P1_PD#1')-1);

	"insert into sh_pd.archives_releves (time,evenement,libelle_evenement,tor_affiche_ana,VAV_V1_PD____cc________100) values(2007-01-01 16:52:00,S,chgt plusieurs Etor,true,)"

insert into sh_pd.archives_releves (time,evenement,libelle_evenement,tor_affiche_ana,VAV_V1_PD____cc________100) 
values(2007-01-01 00:16:00,S,chgt Etor de num 43 et de nom DPP_P1_PD#1,true,)"
		
select public.calcul_etor(
	'cc0C24',
	'PD301107.S01',
	'cc',
	'VAV_V1_PD____cc________100',
	18,
	'2007-01-01 16:52:00')

	"0000100000100100"
"0000110000100100"

	select public.diff_bit_etor('0000110000100100','0000100000100100','cc')


	"0000100000100100"

	"0000100000100100"

	select * from table_hexa_bin_etor where fichier='PD301107.S01' and lettre_voie='cc' and nom_voie='VAV_V1_PD____cc________100'
	and time='2007-01-01'
	order by ordre "0000100000100100" "0000100000100100"

select *  from table_hexa_bin_etor t1 where t1.fichier='PD301107.S01' order by ordre asc,time asc
where  t1.time='2007-01-01'
and    t1.fichier='PD301107.S01'
and    t1.lettre_voie='cc'
and    nom_voie='VAV_V1_PD____cc________100'
and    t1.ordre<18 order by t1.ordre desc limit 1;

0000100000100100
	
delete from sh_pd.archives_releves_log

==============================
	
	
insert into sh_pd.archives_releves (time,evenement,libelle_evenement,tor_affiche_ana,YAV_V1_PD____aa________100,VAV_V1_PD____cc________100) values('2009-07-06 10:22:00','S','chgt Etor de num 13 et de nom OK_INT_PD#1',true,'aa1210','cc4B91')

select
 public.creation_environnement_load(
	'PD',
	'12');

select 
public.creation_environnement_archivage(
	'PD',
	'tmp_load_file_jour_releves_voie_ana'
	);
	
select public.fill_archives(
	'PD',
	'tmp_load_file_jour_releves',
	'tmp_load_file_jour_releves_voie_ana'
	);
	
select public.fill_reconditionne_archives('PD');

select public.fill_archives_TOR(
	'PD','X'
	);
	
delete from sh_pd.archives_releves_log;
delete from sh_pd.archives_releves where evenement in ('R','O','S','&');
	
delete from sh_pd.archives_releves_log where message_log like '%Modif%';
delete from sh_pd.archives_releves where evenement in ('K','k');
select public.fill_archives_K('PD');
	
PD301O12.S40;e;QCO_VMER_PD;-2

select * from table_temporaire_entetes where lower(nom_voie) like 'qav_v1%'

select * from table_temporaire_entetes_insert where lower(nom_voie) like 'qav_v1%'

SELECT 	*
							
					   FROM sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
							sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where a.fichier=r.fichier 
					   and a.lettre_voie=t.lettre_voie and a.nom_voie=t.nom_voie
	                   --and a.nom_voie='YAV_V1_PD'
				       and   (
	                           (r.ch2 is not null and substr(r.ch2,1,1)=a.lettre_voie)
	                               or
	                           (r.ch3 is not null and substr(r.ch3,1,1)=a.lettre_voie)	
									or
							   (r.ch4 is not null and substr(r.ch4,1,1)=a.lettre_voie)
                                    or
	                           (r.ch5 is not null and substr(r.ch5,1,1)=a.lettre_voie)
							        or
	                           (r.ch6 is not null and substr(r.ch6,1,1)=a.lettre_voie)
									or
	                           (r.ch7 is not null and substr(r.ch7,1,1)=a.lettre_voie)
                                    or
	                           (r.ch8 is not null and substr(r.ch8,1,1)=a.lettre_voie)
                                    or
	                           (r.ch9 is not null and substr(r.ch9,1,1)=a.lettre_voie)     
									or
	                           (r.ch10 is not null and substr(r.ch10,1,1)=a.lettre_voie)
                                    or
	                           (r.ch11 is not null and substr(r.ch11,1,1)=a.lettre_voie)
                                    or
	                           (r.ch12 is not null and substr(r.ch12,1,1)=a.lettre_voie)
                                    or
	                           (r.ch13 is not null and substr(r.ch13,1,1)=a.lettre_voie)
                                    or
	                           (r.ch14 is not null and substr(r.ch14,1,1)=a.lettre_voie)
                                    or
	                           (r.ch15 is not null and substr(r.ch15,1,1)=a.lettre_voie)
                                    or
	                           (r.ch16 is not null and substr(r.ch16,1,1)=a.lettre_voie)
                                    or
	                           (r.ch17 is not null and substr(r.ch17,1,1)=a.lettre_voie)
                                    or
	                           (r.ch18 is not null and substr(r.ch18,1,1)=a.lettre_voie)
                                    or
	                           (r.ch19 is not null and substr(r.ch19,1,1)=a.lettre_voie)
                                    or
	                           (r.ch20 is not null and substr(r.ch20,1,1)=a.lettre_voie)
                                    or
	                           (r.ch21 is not null and substr(r.ch21,1,1)=a.lettre_voie)
                                    or
	                           (r.ch22 is not null and substr(r.ch22,1,1)=a.lettre_voie)
                                    or
	                           (r.ch23 is not null and substr(r.ch23,1,1)=a.lettre_voie)
                                    or
	                           (r.ch24 is not null and substr(r.ch24,1,1)=a.lettre_voie)
                                    or
	                           (r.ch25 is not null and substr(r.ch25,1,1)=a.lettre_voie)
                                    or
	                           (r.ch26 is not null and substr(r.ch26,1,1)=a.lettre_voie)
                                    or
	                           (r.ch27 is not null and substr(r.ch27,1,1)=a.lettre_voie)
                                    or
	                           (r.ch28 is not null and substr(r.ch28,1,1)=a.lettre_voie)
                                    or
	                           (r.ch29 is not null and substr(r.ch29,1,1)=a.lettre_voie)
                                    or
	                           (r.ch30 is not null and substr(r.ch30,1,1)=a.lettre_voie)
                                    or
	                           (r.ch31 is not null and substr(r.ch31,1,1)=a.lettre_voie)
                                    or
	                           (r.ch32 is not null and substr(r.ch32,1,1)=a.lettre_voie)
                                    or
	                           (r.ch33 is not null and substr(r.ch33,1,1)=a.lettre_voie)
                                    or
	                           (r.ch35 is not null and substr(r.ch35,1,1)=a.lettre_voie)
                                    or
	                           (r.ch36 is not null and substr(r.ch36,1,1)=a.lettre_voie)
                                    or
	                           (r.ch37 is not null and substr(r.ch37,1,1)=a.lettre_voie)
                                    or
	                           (r.ch38 is not null and substr(r.ch38,1,1)=a.lettre_voie)
                                    or
	                           (r.ch39 is not null and substr(r.ch39,1,1)=a.lettre_voie)
                                    or
	                           (r.ch40 is not null and substr(r.ch40,1,1)=a.lettre_voie)
                                    or
	                           (r.ch41 is not null and substr(r.ch41,1,1)=a.lettre_voie)
                                    or
	                           (r.ch42 is not null and substr(r.ch42,1,1)=a.lettre_voie)
                                    or
	                           (r.ch43 is not null and substr(r.ch43,1,1)=a.lettre_voie)
                                    or
	                           (r.ch44 is not null and substr(r.ch44,1,1)=a.lettre_voie)
                                    or
	                           (r.ch45 is not null and substr(r.ch45,1,1)=a.lettre_voie)
                                    or
	                           (r.ch46 is not null and substr(r.ch46,1,1)=a.lettre_voie)
                                    or
	                           (r.ch47 is not null and substr(r.ch47,1,1)=a.lettre_voie)
                                    or
	                           (r.ch48 is not null and substr(r.ch48,1,1)=a.lettre_voie)
                                    or
	                           (r.ch49 is not null and substr(r.ch49,1,1)=a.lettre_voie)
                                    or
	                           (r.ch50 is not null and substr(r.ch50,1,1)=a.lettre_voie)
                                    or
	                           (r.ch51 is not null and substr(r.ch51,1,1)=a.lettre_voie)
                                    or
	                           (r.ch52 is not null and substr(r.ch52,1,1)=a.lettre_voie)
                                    or
	                           (r.ch53 is not null and substr(r.ch53,1,1)=a.lettre_voie)
                                    or
	                           (r.ch54 is not null and substr(r.ch54,1,1)=a.lettre_voie)
                                    or
	                           (r.ch55 is not null and substr(r.ch55,1,1)=a.lettre_voie)
                                    or
	                           (r.ch56 is not null and substr(r.ch56,1,1)=a.lettre_voie)
                                    or
	                           (r.ch57 is not null and substr(r.ch57,1,1)=a.lettre_voie)
                                    or
	                           (r.ch58 is not null and substr(r.ch58,1,1)=a.lettre_voie)
                                    or
	                           (r.ch59 is not null and substr(r.ch59,1,1)=a.lettre_voie)
                                    or
	                           (r.ch60 is not null and substr(r.ch60,1,1)=a.lettre_voie)
	                         )
					   and   r.evenement in (' ','m','G','P','T')
					   and   r.fichier='PD301107.S01'
	                   order by time asc,a.fichier asc
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 
																												 ==========================
																												 
																												 
																												 
																												 
																												 
																												 
																												 select
 public.creation_environnement_load(
	'PD',
	'12');

select 
public.creation_environnement_archivage(
	'PD',
	'tmp_load_file_jour_releves_voie_ana'
	);

drop test;
create table test(i integer);
insert into test values(-1);
insert into test values(-2);
insert into test values(00);

select * from test order by i desc

select * from table_temporaire_entetes where lower(nom_voie)  like 'qav_v1%'
select * from table_temporaire_entetes_insert where lower(nom_voie)  like 'qav_v1%'
select * from sh_pd.archives_releves

SELECT 	* FROM INFORMATION_SCHEMA.COLUMNS
			where 	table_name='archives_releves' 
			and 	table_schema='sh_pd'
            and     column_name like 'vep_3moree%'
	
CREATE TABLE sh_pd.archives_releves(time 		timestamp without time zone,
									evenement 	character varying(10),
									libelle_evenement   character varying(100));

select * from sh_pd.archives_releves
	ADD COLUMN QCO_VMER____inconnue________100 character varying(100)
	
select * from sh_pd.archives_releves
	
select * from sh_pd.tmp_load_file_jour_releves_voie_ana

INSERT INTO table_temporaire_entetes select distinct case a.lettre_voie
								when 'a' then ascii(a.lettre_voie)
								when 'b' then ascii(a.lettre_voie)
								when 'c' then ascii(a.lettre_voie)
								when 'd' then ascii(a.lettre_voie)
								when 'e' then ascii(a.lettre_voie)
								when 'f' then ascii(a.lettre_voie)
								when 'g' then ascii(a.lettre_voie)
								when 'h' then ascii(a.lettre_voie)
								when 'i' then ascii(a.lettre_voie)
								when 'j' then ascii(a.lettre_voie)
								when 'k' then ascii(a.lettre_voie)
								when 'l' then ascii(a.lettre_voie)
								when 'm' then ascii(a.lettre_voie)
								when 'n' then ascii(a.lettre_voie)
								when 'o' then ascii(a.lettre_voie)
								when 'p' then ascii(a.lettre_voie)
								when 'q' then ascii(a.lettre_voie)
								when 'r' then ascii(a.lettre_voie)
								when 's' then ascii(a.lettre_voie)
								when 't' then ascii(a.lettre_voie)
								when 'u' then ascii(a.lettre_voie)
								when 'v' then ascii(a.lettre_voie)
								when 'w' then ascii(a.lettre_voie)
								when 'x' then ascii(a.lettre_voie)
								when 'y' then ascii(a.lettre_voie)
								when 'z' then ascii(a.lettre_voie)
								when '(' then 123
								when ')' then 124
								when '[' then 125
								when ']' then 126
								when '{' then 127
								when '}' then 128
						   end as ordre,
	   a.lettre_voie,a.nom_voie,a.precision
	   from sh_pd.tmp_load_file_jour_releves_voie_ana a order by ordre asc

create table table_temporaire_entetes(ordre integer,
							   lettre_voie character varying (2),
							   nom_voie character varying (100),
							   precision smallint
							   );

create table table_temporaire_entetes_insert(ordre integer,
							   lettre_voie character varying (2),
							   nom_voie character varying (100),
							   precision smallint
							   );

delete from table_temporaire_entetes;
delete from table_temporaire_entetes_insert;


select * from table_temporaire_entetes where lower(nom_voie) like 'qav_v1%'

select * from table_temporaire_entetes_insert where lower(nom_voie) like 'qav_v1%'

select distinct lettre_voie,nom_voie,precision
from sh_pd.tmp_load_file_jour_releves_voie_ana where lower(nom_voie) like 'qav_v1%'

select ascii('r')

select 	lower(lettre_voie),replace(lower(nom_voie),'_'||lower('PD'),''),lower(lettre_voie),precision
				from 	table_temporaire_entetes
				where 	lower(nom_voie)=lower('QAV_V1_PD') --and ordre=CUR.ordre and precision=CUR.precision
				and  	(replace(lower(nom_voie),'_'||lower('PD'),''),lower(lettre_voie),precision) not in 	(select 
																												 lower(
																												 substr(
																														substr(nom_voie,1,
																																		case POSITION('____' IN nom_voie)-1
																																		when -1 then length(nom_voie) else 
																																		POSITION('____' IN nom_voie)-1
																																		end
																															  ),
																														1,
																														case POSITION('________' IN nom_voie)-1
																														when -1 then length(nom_voie) else 
																														POSITION('________' IN nom_voie)-1
																														end
																														)
																													   ) as nom_voie,
																												 lower(lettre_voie),precision
																												 from table_temporaire_entetes_insert); 