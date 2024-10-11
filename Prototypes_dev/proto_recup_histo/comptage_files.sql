DROP TABLE public.files_comptage;
CREATE TABLE public.files_comptage(fichier character varying (100));
delete from files_comptage;
insert into files_comptage values('PD301107.S01');
insert into files_comptage values('PD301O07.S40');
insert into files_comptage values('PD301O12.S40');
insert into files_comptage values('PD302112.S01');
insert into files_comptage values('PD302407.S14');
insert into files_comptage values('PD302412.S14');
insert into files_comptage values('PD302707.S27');
insert into files_comptage values('PD302712.S27');
insert into files_comptage values('PD303907.S36');
insert into files_comptage values('PD303912.S36');
insert into files_comptage values('PD303D07.S49');
insert into files_comptage values('PD303D12.S49');
insert into files_comptage values('PD304607.S23');
insert into files_comptage values('PD304612.S23');
insert into files_comptage values('PD305207.S06');
insert into files_comptage values('PD305307.S10');
insert into files_comptage values('PD305312.S10');
insert into files_comptage values('PD305N07.S45');
insert into files_comptage values('PD305N12.S45');
insert into files_comptage values('PD306212.S06');
insert into files_comptage values('PD306807.S32');
insert into files_comptage values('PD306812.S32');
insert into files_comptage values('PD307507.S19');
insert into files_comptage values('PD307512.S19');
insert into files_comptage values('PD308107.S02');
insert into files_comptage values('PD308O07.S41');
insert into files_comptage values('PD308O12.S41');
insert into files_comptage values('PD309112.S02');
insert into files_comptage values('PD309407.S15');
insert into files_comptage values('PD309412.S15');
insert into files_comptage values('PD309707.S28');
insert into files_comptage values('PD309712.S28');
insert into files_comptage values('PD310907.S37');
insert into files_comptage values('PD310912.S37');
insert into files_comptage values('PD310D07.S50');
insert into files_comptage values('PD310D12.S50');
insert into files_comptage values('PD311612.S24');
insert into files_comptage values('PD312207.S07');
insert into files_comptage values('PD312307.S11');
insert into files_comptage values('PD312312.S11');
insert into files_comptage values('PD312N07.S46');
insert into files_comptage values('PD312N12.S46');
insert into files_comptage values('PD313212.S07');
insert into files_comptage values('PD313807.S33');
insert into files_comptage values('PD313812.S33');
insert into files_comptage values('PD314507.S20');
insert into files_comptage values('PD314512.S20');
insert into files_comptage values('PD315107.S03');
insert into files_comptage values('PD315O07.S42');
insert into files_comptage values('PD315O12.S42');
insert into files_comptage values('PD316112.S03');
insert into files_comptage values('PD316407.S16');
insert into files_comptage values('PD316412.S16');
insert into files_comptage values('PD316707.S29');
insert into files_comptage values('PD316712.S29');
insert into files_comptage values('PD317907.S38');
insert into files_comptage values('PD317912.S38');
insert into files_comptage values('PD317D07.S51');
insert into files_comptage values('PD317D12.S51');
insert into files_comptage values('PD318612.S25');
insert into files_comptage values('PD319207.S08');
insert into files_comptage values('PD319307.S12');
insert into files_comptage values('PD319312.S12');
insert into files_comptage values('PD319N07.S47');
insert into files_comptage values('PD319N12.S47');
insert into files_comptage values('PD320212.S08');
insert into files_comptage values('PD320807.S34');
insert into files_comptage values('PD320812.S34');
insert into files_comptage values('PD321507.S21');
insert into files_comptage values('PD321512.S21');
insert into files_comptage values('PD322107.S04');
insert into files_comptage values('PD322O07.S43');
insert into files_comptage values('PD322O12.S43');
insert into files_comptage values('PD323112.S04');
insert into files_comptage values('PD323407.S17');
insert into files_comptage values('PD323412.S17');
insert into files_comptage values('PD323707.S30');
insert into files_comptage values('PD323712.S30');
insert into files_comptage values('PD324907.S39');
insert into files_comptage values('PD324912.S39');
insert into files_comptage values('PD324D07.S52');
insert into files_comptage values('PD324D12.S52');
insert into files_comptage values('PD325612.S26');
insert into files_comptage values('PD326207.S09');
insert into files_comptage values('PD326307.S13');
insert into files_comptage values('PD326312.S13');
insert into files_comptage values('PD326N07.S48');
insert into files_comptage values('PD326N12.S48');
insert into files_comptage values('PD327212.S09');
insert into files_comptage values('PD327807.S35');
insert into files_comptage values('PD327812.S35');
insert into files_comptage values('PD328507.S22');
insert into files_comptage values('PD328512.S22');
insert into files_comptage values('PD329107.S05');
insert into files_comptage values('PD329O07.S44');
insert into files_comptage values('PD329O12.S44');
insert into files_comptage values('PD330112.S05');
insert into files_comptage values('PD330407.S18');
insert into files_comptage values('PD330412.S18');
insert into files_comptage values('PD330707.S31');
insert into files_comptage values('PD330712.S31');
insert into files_comptage values('PD331D07.S53');
insert into files_comptage values('PD331D12.S53');
commit;
DROP TABLE public.comptage_files;
CREATE TABLE public.comptage_files(fichier character varying (100),nbr integer);
DROP FUNCTION public.comptage_requete;
CREATE OR REPLACE FUNCTION public.comptage_requete()
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
v_compte  integer;
CurRelEnt CURSOR FOR SELECT fichier FROM files_comptage;
BEGIN
FOR CUR in CurRelEnt
LOOP
SELECT count(*) into v_compte
							
					   FROM sh_pd.tmp_load_file_jour_releves_voie_ana a, sh_pd.tmp_load_file_jour_releves r,
							sh_pd.tmp_load_file_jour_releves_voie_ana_archive t
					   where a.fichier=r.fichier and a.lettre_voie=t.lettre_voie
	                   and a.nom_voie=substr(t.nom_voie,1,position('___' in t.nom_voie)-1)
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
					   and   r.fichier=CUR.fichier;
insert into comptage_files values(CUR.fichier,v_compte);
RAISE NOTICE '%;%',CUR.fichier,v_compte;
END LOOP;
END
$BODY$

select public.comptage_requete()