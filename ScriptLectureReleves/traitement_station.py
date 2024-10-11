import os
import re
from datetime import date
import pandas as pd
from utils import read_file_releve, extract_voies_from_header, decouper_data_en_data_jours, traiter_les_lignes_jour_sten, traiter_les_lignes_jour_automate, creer_fichier_station, detecter_type_equipement_data_quotidienne, definir_station_depuis_nom_station, time_logger

@time_logger
def traitement_station(station, directory):
    dossier_station = directory + '\\' + definir_station_depuis_nom_station(station)
    pattern = re.compile(r'.*\S\d{2}$')
    df_columns = ['horodate', 'voie', 'mesure', 'mesure_brute', 'evenement']
    dictionaries = []
    data_encodee = {}
    dates = []
    creer_fichier_station(
        f'Anomalies_{station}.csv', 'anomalie, lgine, fichier,station\n')
    creer_fichier_station(f'resultat_{station}.csv', ','.join(df_columns))
    creer_fichier_station(
        f'resultat_{station}_ana.csv', ','.join(df_columns + ['TUTL']))
    creer_fichier_station(
        f'resultat_{station}_tor.csv', ','.join(df_columns + ['TUTL']))
    arr = os.listdir( dossier_station)
    files = [dossier_station + '\\' + x for x in arr]
    for file in files:
        if pattern.match(file.split('.')[-1]):
            f = open(file, "r")
            header, data = read_file_releve(f)
            dictionnaire_voies = extract_voies_from_header(header, file, station)
            list_data_quotidienne = decouper_data_en_data_jours(data, file)
            try:
                buffer = list_data_quotidienne[0][0].split(' ')[0]
            except:
                print(file)
            buffer = list_data_quotidienne[0][0].split(' ')[0]
            jour = buffer[1:]
            datetime_object = date(int(jour[6:]), int(jour[3:5]), int(jour[0:2]))
            dictionaries.append((datetime_object, file, dictionnaire_voies, station))
            data_encodee[datetime_object] = (station, file,
                dictionnaire_voies, list_data_quotidienne)
            dates.append(datetime_object)

    dates.sort()

    for key in dates:
        for data_quotidienne in data_encodee[key][3]:
            try:
                jour = data_quotidienne[0][1:11]
            except:
                print('jour non defini', key)
            # TODO : Definir equipement a partir de key + index
            equipement, tUTL = detecter_type_equipement_data_quotidienne(
                data_quotidienne, station)
            
            success = 0
            match equipement:
                case 'STEN':
                    success, list_data = traiter_les_lignes_jour_sten(
                        data_encodee[key][2], data_quotidienne, data_encodee[key][1], station)
                case 'Automate':
                    success, list_data = traiter_les_lignes_jour_automate(
                        data_encodee[key][2], data_quotidienne, data_encodee[key][1], station)
            if success == 1 and len(list_data) > 0:
                list_data_sta = list(map(lambda x: x[0:5], filter(
                    lambda x: x[5] == 'station', list_data)))
                list_data_ana = list(map(lambda x: (*x[0:5], tUTL), filter(
                        lambda x: x[5] == 'ana', list_data)))
                list_data_tor = list(map(lambda x: (*x[0:5], tUTL), filter(
                    lambda x: x[5] == 'tor', list_data)))
                                
                if len(list_data_sta) > 0:
                    df_sta = pd.DataFrame(data=list_data_sta, columns=df_columns)                
                    df_sta.to_csv(f'resultat_{station}.csv',
                                mode='a', header=False, index=False)                

                # TODO: ajout colonne csv equipement
                if len(list_data_ana) > 0:
                    df_columns_ana = df_columns + ['TUTL']
                    df_ana = pd.DataFrame(
                        data=list_data_ana, columns=df_columns_ana)
                    df_ana.to_csv(f'resultat_{station}_ana.csv',
                                mode='a', header=False, index=False)                

                # TODO: ajout colonne csv equipement
                if len(list_data_tor) > 0:
                    df_columns_tor = df_columns + ['TUTL']
                    df_tor = pd.DataFrame(data=list_data_tor, columns=df_columns_tor)
                    df_tor.to_csv(f'resultat_{station}_tor.csv',
                                mode='a', header=False, index=False)



