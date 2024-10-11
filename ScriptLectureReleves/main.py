import os
# import re
import const
# from datetime import date
from traitement_station import traitement_station
from utils import creer_repertoire_resultat, read_station_from_stdin, transfer_to_remote_machine

#detecter_equiepemnts_stations()



# station STEN traitees
# 'AS', 'BT', 'CA', 'CB', 'CI', 'CN', 'CO', 'CS', 'CT', 'CU', 'CX', 'DB', 'DJ', 'EB', 'EE',
# 'EN', 'ES', 'FA', 'FO', 'GE', 'GG', 'GI', 'GO', 'HB', 'JJ', 'JP', 'KR', 'LE', 'LI', 'LJ', 'LP', 'LY', 'MA', 'MD', 'ML', 'MN', 'MP', 'MR', 'MU', 'MY', 'NG', 'PA', 'PH', 'PL', 'PP', 'PR', 'PT', 'PV', 'PY'
# 'QS', 'RA', 'RB', 'RE', 'RO', 'SA', 'SF', 'SO', 'SX', 'TD','VE', 'VG', 'VT', 'VV', 'ZD'

# stations avec  bugs : EE, GP, GM, HE, JE, PY,'TU',


station = read_station_from_stdin()
traitement_station(station, const.path_archive)
creer_repertoire_resultat(station)
transfer_to_remote_machine(station)







