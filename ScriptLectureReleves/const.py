# K, et k, m sont des evenements de mm nature : midif seuils
# K est pour les STEN elle definit la voie touchee par la modif (lettre et correspondance entete)
# k, m designe la voie touchee par la modif (lettre et correspondance entete) et la valeur du seuil (k pour le 0 , m pour le 100)
evenements_sten_ana_precision = [' ', '-', 'G', 'P', 'T', 'I', 'J', 'U', 'V', 'L']
evenements_sten_ana_ss_precision = ['C', 'Q']
evenements_sten_ss_info_rattachee_a_une_voie = ['E', 'X', 'N', 'B', 'M', 'Y']
evenement_sten_changement_echelle = ['K']
evenements_sten_TOR = ['R', 'O', 'S', '&']
evenemet_sten = evenements_sten_ana_precision + evenements_sten_ana_ss_precision + evenements_sten_TOR
evenement_changement_heure = ['H']
# R enregistrements ETOR a minuit
# O enregistrements STOR a minuit
# S enregitrement ETOR
# & enregistrement STOR

evenements_automates_ana_precision = [' ', '-', 'J']
evenement_automates_changement_echelle = ['k', 'm']
# k , m modif dechelle k=0% m=100%
#  et - correspondent a des changements de valeurs
# J enregistrement a minuit
evenements_automates_ss_info_rattachee_a_une_voie = ['E']
evenements_automates_TOR = ['r', 'o', 's', 'c']
# r enregistrements ETOR a minuit
# o enregistrements STOR a minuit
# s enregitrement ETOR
# c enregistrement STOR
evenemets_automates = evenements_automates_ana_precision + evenements_automates_TOR

evenements_automates_sten_defauts = ['$']



# Automates
# ANA : $00h00 a9999 b20/21xx disparition ou apparition de defaut capteur xx correspond a l'indice de la voie'
# TOR : $00h00 axxxx b2000/2100 disparition ou apparition de battement xxxx correspond au numero de la voie'

# STEN
# ANA : $00h00 a9999 b disparition ou apparition de defaut capteur
# TOR:
# ATTENTION $ peut correspondre a un simple commentaire aussi
# xdq a definir

automates = ['EN', 'RF', 'RC', 'RH', 'RR', 'BA', 'BBp', 'BD', 'CH',
            'GA', 'JE', 'LR', 'OUp', 'QM', 'VIp', '153', '156', '172',
            '173', '175', '176', '177', '178', '179', '184', '191', 'CV', 'HE', 'XY']

stens = ['AZ', 'CI', 'CLp', 'CO', 'CX', 'DB', 'DRp', 'ES', 'GYp', 'HB', 'JJ', 'JP', 'KK',
        'KR', 'LE', 'LI', 'LJ', 'LP', 'LVp', 'LY', 'MA', 'ML', 'MTp', 'MU', 'NCp', 'NEp',
        'NMp', 'NPp', 'PA', 'PL', 'PM', 'PNp', 'PO', 'PP', 'PR', 'QS', 'RL', 'SA', 'SC', 'SL', 
        'SO','TFp', 'VE', 'VT', 'XU', 'XX', 'AB', 'AC', 'AF', 'AN', 'AP', 'AS', 'BGp', 'BM',
        'BRp', 'BT', 'CA', 'CB', 'CM', 'CN', 'CR', 'CS', 'CT', 'CU', 'DJ', 'DU', 'EB', 'EE',
        'FA', 'FB', 'FO', 'GD', 'GE', 'GG', 'GI', 'GM', 'GO', 'GP', 'GR', 'JM', 'LGp', 'MD',
        'MN', 'MO', 'MP', 'MR', 'MY', 'NG', 'PD','PH', 'PT', 'PV', 'PY', 'RA', 'RBp', 'RE',
        'RO', 'RV', 'SF', 'SX', 'TD', 'TH', 'TU', 'VG', 'VV', 'ZD']

autres = ['001', '008', '009', '012', '020', '028', '029', '037', '050',
        '061', '075', '088', '100', '103', '105', '123', '124', '136', '137',
        '159', '161', '169', '170', '174', '180', '181', '182', '183', '185',
        '186', '190', '192', '194', '195', '196', '197', '199', '200', '201',
        '202', '203', '204', '206','207', '208', '209', '210', '212', '215',
        '219', '226', 'AM', 'AV', 'BB', 'BG', 'BP', 'BR', 'CL', 'DR', 'EC',
        'EP', 'EX', 'Gi', 'GY', 'JF', 'LG', 'LO', 'LQ', 'LV', 'MI', 'MT', 'NC',
        'NE', 'NM', 'NP', 'OU', 'P27', 'P32', 'P33', 'PG', 'PN', 'QA', 'QC',
        'RB', 'RS', 'RY', 'SD', 'Sf', 'TF', 'UK', 'VI', 'WD']

path_archive = r'C:\Users\Nabil\Desktop\Softia_Work_Repos\SHUT\archive\archive'
path_result = r'C:\Users\Nabil\Desktop\Softia_Work_Repos\SHUTRefonte\shut-refonte\ScriptLectureReleves'

