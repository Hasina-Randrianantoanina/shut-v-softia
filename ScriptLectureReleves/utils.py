import math
import time
import datetime
import inquirer.prompt
import pytz
import sqlite3
import os
import sys
import shutil
import re
import paramiko
from scp import SCPClient
import const
import pandas as pd
import inquirer
from inquirer.themes import GreenPassion
import subprocess



def time_logger(func):
    def wrapper(*args, **kwargs):
        start_time = time.time()
        result = func(*args, **kwargs)
        end_time = time.time()
        execution_time = end_time - start_time
        print(
            f"Function '{func.__name__}' took {execution_time:.4f} seconds to execute.")
        return result
    return wrapper

def read_file_releve(file):
    header = []
    data = []
    try:
        Lines = file.readlines()
        for line in Lines:
            if str(line)[0] == '#':
                header.append(line)
            else:
                data.append(line)
        data.pop(0)
        data.pop(-1)
    except:
        pass
    return header, data

def extract_voies_from_header(header, file, station):
    dictionnaire_voies = {}
    try:
        for line in header:
            items = line.strip('\n').split('\t')
            lettre = items[0][1]
            voie = items[1].strip('\t').strip(' ')
            list_comp_voie = voie.split('_')[:-1]
            list_comp_voie.append(station)
            voie_modified = '_'.join(list_comp_voie)
            precision = items[2].strip('\n')
            numero_voie = items[3].strip('\t').strip(' ')
            dictionnaire_voies[lettre] = (
                voie_modified, precision, numero_voie)
    except:
        dictionnaire_voies = {}
    return dictionnaire_voies

def decouper_data_en_data_jours(data, file):
    list_data_quotidienne = []
    data_quotidienne = []
    try:
        for index, line in enumerate(data):
            if ((line[0] != "E") & (index == 0)):
                return list_data_quotidienne
            if ((line[0] == "E") & (index > 0)):
                list_data_quotidienne.append(data_quotidienne)
                data_quotidienne = []
            data_quotidienne.append(line)
            if (index == len(data) - 1):
                list_data_quotidienne.append(data_quotidienne)
    except:
        list_data_quotidienne = []
    return list_data_quotidienne



def get_fichiers_corrompus(station):
    list_fichiers = []
    with open(f'./Traitement_Analyse/Fichiers_corrompus.txt', "r") as f:
        for line in f:
            items = line.strip('\n').split('\\')
            if len(items) > 2 and str(items[-2]) == station:
                list_fichiers.append(items[-1])    
    return list_fichiers

def extraire_fichiers_corrompus():
    f = open('./Traitement_Analyse/resultats_semaine.txt')
    lines = f.readlines()
    for line in lines:
        buffer = line.split(' ')
        if (line.startswith('fichier data corrompue') or line.startswith('fichier entete corrompue') or line.startswith('fichier n a pas pu etre lu')):
            filename = buffer[-1]
            f = open("./Traitement_Analyse/Fichiers_Corrompus.txt", "a")
            f.write(f"{filename}")
    f.close()

def compare_entete(entete1, entete2):
    # renvoie true si les entetes sont egales et false s'il y a une diff
    # comprer deux dictionnaires
    # comparer les clés
    diff_list_1 = [item for item in entete1.keys() if item not in entete2.keys()]
    diff_list_2 = [item for item in entete2.keys() if item not in entete1.keys()]
    if (len(diff_list_1) > 0) | (len(diff_list_2) > 0):
        return False
    for key in entete1:
        tuple1 = entete1[key]
        tuple2 = entete2[key]
        # verifierr l'egalite des tuples
        if (tuple1[0] != tuple2[0]) | (tuple1[1] != tuple2[1]):
            return False
    return True
def detecter_changement_header(dictionaries):
    for i in range(len(dictionaries)-1):
        entete_i = dictionaries[i][2]
        entete_i_1 = dictionaries[i+1][2]
        if not compare_entete(entete_i, entete_i_1):
            f = open("resultats_man.txt", "a")
            f.write("Herve reveille toi\n")
            f.write("divergence entete\n")
            f.write(str(dictionaries[i][0]) + ' ' + str(dictionaries[i]
                    [1]) + ' ' + str(dictionaries[i][3]) + '\n')
            f.write(str(dictionaries[i+1][0]) + ' ' + str(dictionaries[i+1]
                    [1]) + ' ' + str(dictionaries[i+1][3]) + '\n')
            f.close()
def agrege_entete(entete1, entete2):
    # renvoie entete <la plus englobante> si les entetes sont differents
    entete_englobante = entete1
    for key in entete1.keys():
        if entete2.get(key) is not None:
            if entete2[key] != entete1[key]:
                return {}
    for key in entete2.keys():
        if entete1.get(key) is None:
            entete_englobante[key] = entete2[key]
    return entete_englobante


@time_logger
def agreger_entetes(dictionaries):
    entete_finale = {}
    for i in range(len(dictionaries) - 1):
        entete_i = dictionaries[i][2]
        entete_i_1 = dictionaries[i+1][2]        
        if i == 0:
            entete_finale = agrege_entete(entete_i, entete_i_1)
        else:
            entete_finale = agrege_entete(entete_finale, entete_i_1)
    return entete_finale


def traiter_les_lignes_jour_sten(dictionnaires_voies, data_quotidienne, file, station):
    jour = data_quotidienne[0][1:11]
    list_data_quotidienne_mesure = []
    voies_indicees = dict(
        map(lambda x: (x[1][2], x[1][0]), dictionnaires_voies.items()))

    for line in data_quotidienne:
        cle_ligne = line[0]
        buffer = line.strip('\n')
        match cle_ligne:

            case cle_ligne if (cle_ligne in const.evenements_sten_ana_precision):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                for index, item in enumerate(items):
                    if item != '' and 1 <= index <= len(items) - 1:
                        if not (item[0] in dictionnaires_voies.keys()):
                            if index != 1:
                                with open(f'Anomalies_{station}.csv', "a") as f:
                                    f.write(
                                        f'pb taitement evenemnt {cle_ligne} voie non presente en entete, {buffer}, {file}, {station}\n')
                            continue
                        try:
                            (voie, precision, _) = dictionnaires_voies[item[0]]
                            mesure_brute = int(item[1:])
                            factor = -1 if cle_ligne == '-' else 1
                            mesure = factor * int(item[1:]) / \
                                math.pow(10, abs(int(precision)))
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, mesure, mesure_brute, cle_ligne, 'ana'))
                        except:
                            mesure_brute = item[1:]
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, 0, mesure_brute, cle_ligne, 'ana'))
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {buffer}, {file}, {station}\n')
                            continue

            case cle_ligne if (cle_ligne in const.evenements_sten_ana_ss_precision):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                for index, item in enumerate(items):
                    if item != '' and 1 <= index <= len(items) - 1:
                        if not (item[0] in dictionnaires_voies.keys()):
                            if index != 1:
                                with open(f'Anomalies_{station}.csv', "a") as f:
                                    f.write(
                                        f'pb taitement evenemnt {cle_ligne} voie non presente en entete, {line}, {file}, {station}\n')
                            continue
                        try:
                            (voie, precision, _) = dictionnaires_voies[item[0]]
                            mesure_brute = item[1:]
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, 0, mesure_brute, cle_ligne, 'ana'))
                        except:
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {buffer}, {file}, {station}\n')
                            continue

            case cle_ligne if (cle_ligne in const.evenements_sten_ss_info_rattachee_a_une_voie ):
                try:
                    horodatage = jour + " 00h00"
                    nbre_evenements = int(line.strip('\n').split(' ')[-1])
                    if (cle_ligne =='B' or cle_ligne == 'M'):
                        horodatage = jour + ' ' + line[1:6]
                    list_data_quotidienne_mesure.append(
                        (horodatage, "Pas de voie attachee", 0, nbre_evenements, cle_ligne, 'station'))
                except:
                    with open(f'Anomalies_{station}.csv', "a") as f:
                        f.write(f'pb taitement evenemnt {cle_ligne}, {buffer}, {file}, {station}\n')
                    continue

            case cle_ligne if (cle_ligne in const.evenement_sten_changement_echelle):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                for index, item in enumerate(items):
                    if item != '' and index <= len(items) - 1 and index % 2 == 1:
                        if not (item[0] in dictionnaires_voies.keys()):
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'pb taitement evenemnt {cle_ligne} voie non presente en entete, {buffer}, {file}, {station}\n')
                            continue
                        try:
                            (voie, precision, _) = dictionnaires_voies[item[0]]
                            mesure_0 = int(item[1:]) / math.pow(10, abs(int(precision)))
                            mesure_100 = int(
                                items[index+1])/math.pow(10, abs(int(precision)))
                            list_data_quotidienne_mesure.append(
                            (horodatage, voie, mesure_0, mesure_100, cle_ligne, 'ana'))
                        except:
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {buffer}, {file}, {station}\n')
                            continue

            case cle_ligne if (cle_ligne in const.evenements_sten_TOR):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                references_etor = [item for item in items[1:] if item != '']
                prefix = 'e' if cle_ligne in ['R', 'S'] else 's'
                nums_voie, valeurs_voie =calcul_evenements_etor(
                    references_etor, prefix, cle_ligne, buffer, file, station)
                if len(nums_voie) + len(valeurs_voie) == 0:
                    continue
                for num_voie, valeur_voie in zip(nums_voie, valeurs_voie):
                    list_data_quotidienne_mesure.append(
                        (horodatage, num_voie, valeur_voie, valeur_voie, cle_ligne, 'tor'))
                continue


            case cle_ligne if (cle_ligne in const.evenement_changement_heure):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                list_data_quotidienne_mesure.append(
                    (horodatage, 'Pas de voie attachee', 0, str(items[1:]), cle_ligne, 'station'))
                continue

            case "$":
                horodatage = jour + ' ' + line[1:6]
                buffer = line.strip('\n')
                items = buffer.split(' ')
                try:
                    firstWord = items[1]
                    secondWord = items[2]
                    if (firstWord == "a9999"):
                        if (secondWord.startswith("b21") or secondWord.startswith("b20")):
                            # defaut voie analogique
                            num_voie = int(secondWord[3:])
                            if not (num_voie in voies_indicees.keys()):
                                with open(f'Anomalies_{station}.csv', "a") as f:
                                    f.write(
                                        f'evenement defaut sur voie analogique {cle_ligne}, {buffer}, {file}, {station} portant sur voie non enregistree {num_voie}\n')
                                continue
                            voie_ana = voies_indicees[num_voie]
                            # TODO: chercher la voie ana correspondant au num_voie a partir des dictionnaires
                            code_defaut = 9999 if secondWord[2] == "1" else 8888
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie_ana, 0, code_defaut, cle_ligne, 'ana'))
                        elif (secondWord.startswith("b11") or secondWord.startswith("b10")):
                            num_module = int(secondWord[3:])
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'evenement defaut sur voie etat {cle_ligne}, {buffer}, {file}, {station} portant sur module {num_module}\n')
                    else:
                        with open(f'Anomalies_{station}.csv', "a") as f:
                            f.write(
                                f'evenement defaut non reconnu non interpretable {cle_ligne}, {buffer}, {file}, {station} incluant dedut fin session autoshut\n')
                except:
                    with open(f'Anomalies_{station}.csv', "a") as f:
                        f.write(
                            f'pb taitement evenemnt {cle_ligne}, {buffer}, {file}, {station}\n')
                continue
               
            case _:
                with open(f'Anomalies_{station}.csv', "a") as f:
                    f.write(
                        f'pb taitement evenemnt inconnu {cle_ligne}, {buffer}, {file}, {station}\n')
                continue

    return 1,list_data_quotidienne_mesure

def traiter_les_lignes_jour_automate(dictionnaires_voies, data_quotidienne, file, station):
    jour = data_quotidienne[0][1:11]
    list_data_quotidienne_mesure = []
    voies_indicees = dict(
        map(lambda x: (x[1][2], x[1][0]), dictionnaires_voies.items()))

    for line in data_quotidienne:
        cle_ligne = line[0]
        buffer = line.strip('\n')
        match cle_ligne:
            case cle_ligne if (cle_ligne in const.evenements_automates_ana_precision):
                # TODO : traiter k et m
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                for index, item in enumerate(items):
                    if item != '' and 1 <= index <= len(items) - 1:
                        if not (item[0] in dictionnaires_voies.keys()):
                            if index != 1:
                                with open(f'Anomalies_{station}.csv', "a") as f:
                                    f.write(
                                        f'pb taitement evenemnt {cle_ligne} voie non presente en entete, {buffer}, {file}, {station}\n')
                            continue
                        try:
                            (voie, precision, _) = dictionnaires_voies[item[0]]
                            mesure_brute = int(item[1:])
                            mesure = int(item[1:]) / \
                                math.pow(10, abs(int(precision)))
                            if (cle_ligne == '-'): 
                                mesure *= -1
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, mesure, mesure_brute, cle_ligne, 'ana'))
                        except:
                            mesure_brute = item[1:]
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, 0, mesure_brute, cle_ligne, 'ana'))
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {buffer}, {file}, {station}\n')
                            continue

            case cle_ligne if (cle_ligne in const.evenement_automates_changement_echelle):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                for index, item in enumerate(items):
                    if item != '' and 1 <= index <= len(items) - 1:
                        if not (item[0] in dictionnaires_voies.keys()):
                            if index != 1:
                                with open(f'Anomalies_{station}.csv', "a") as f:
                                    f.write(
                                        f'pb taitement evenemnt {cle_ligne} voie non presente en entete, {buffer}, {file}, {station}\n')
                            continue
                        try:
                            (voie, precision, _) = dictionnaires_voies[item[0]]
                            mesure_brute = int(item[1:])
                            mesure = int(item[1:]) / \
                                math.pow(10, abs(int(precision)))
                            if (cle_ligne == '-'):
                                mesure *= -1
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, mesure, mesure_brute, cle_ligne, 'ana'))
                        except:
                            mesure_brute = item[1:]
                            list_data_quotidienne_mesure.append(
                                (horodatage, voie, 0, mesure_brute, cle_ligne, 'ana'))
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {buffer}, {file}, {station}\n')
                            continue

            case cle_ligne if (cle_ligne in  const.evenements_automates_ss_info_rattachee_a_une_voie):
                try:
                    horodatage = jour + " 00h00"
                    nbre_evenements = int(line.strip('\n').split(' ')[-1])
                    if (cle_ligne == 'B' or cle_ligne == 'M'):
                        horodatage = jour + ' ' + line[1:6]
                    list_data_quotidienne_mesure.append(
                        (horodatage, "Pas de voie attachee", 0, nbre_evenements, cle_ligne, 'station'))
                except:
                    with open(f'Anomalies_{station}.csv', "a") as f:
                        f.write(
                            f'pb taitement evenemnt {cle_ligne}, {buffer}, {file}, {station}\n')
                    continue

            case cle_ligne if (cle_ligne in const.evenement_changement_heure):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                list_data_quotidienne_mesure.append(
                    (horodatage, 'Pas de voie attachee', 0, str(items[1:]), cle_ligne, 'station'))
                continue

            case cle_ligne if (cle_ligne in const.evenements_automates_TOR):
                horodatage = jour + ' ' + line[1:6]
                items = line.strip('\n').split(' ')
                numero_tor = int(items[1][1:])
                valeur_voie = int(items[2][1:])
                prefix = 'e' if cle_ligne in ['r', 's'] else 's'
                voie_tor = 'voie_' + prefix + 'tor_' + str(numero_tor)
                list_data_quotidienne_mesure.append(
                    (horodatage, voie_tor, valeur_voie, valeur_voie, cle_ligne, 'tor'))
                continue

            case cle_ligne if (cle_ligne in const.evenements_automates_sten_defauts):
                horodatage = jour + ' ' + line[1:6]
                buffer = line.strip('\n')
                items = buffer.split(' ')
                try:
                    firstWord = items[1]
                    secondWord = items[2]
                    if (firstWord == "a9999"):
                        # TODO : Voies Etat STEN => secondword.startswith("b11") or secondWord.startswith("b10")
                        # Recuperation du num_voie qui est egal au numero de module
                        # Mise en anomalie avec info module
                        if (secondWord.startswith("b21") or secondWord.startswith("b20")):
                            # defaut voie analogique
                            num_voie = int(secondWord[3:])

                            if not (num_voie in voies_indicees.keys()):
                                with open(f'Anomalies_{station}.csv', "a") as f:
                                    f.write(
                                        f'evenement defaut sur voie analogique {cle_ligne}, {buffer}, {file}, {station} portant sur voie non enregistree {num_voie}\n')
                                continue
                            voie_ana = voies_indicees[num_voie]
                            # TODO: chercher la voie ana correspondant au num_voie a partir des dictionnaires
                            code_defaut = 9999 if secondWord[2] == "1" else 8888
                            list_data_quotidienne_mesure.append((horodatage, voie_ana, 0, code_defaut, cle_ligne, 'ana'))
                        else:
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                f'evenement defaut non reconnu non interpretable {cle_ligne}, {buffer}, {file}, {station} \n')
                    elif (firstWord.startswith("a0")):

                        if (secondWord == "b2100" or secondWord == "b2000"):
                            # defaut voie etor
                            numero_tor = int(firstWord[1:])
                            voie_tor = 'voie_etor_' + str(numero_tor)
                            code_defaut = 9999 if secondWord[2] == "1" else 8888
                            list_data_quotidienne_mesure.append((horodatage, voie_tor, 0, code_defaut, cle_ligne, 'tor'))
                        else:
                            with open(f'Anomalies_{station}.csv', "a") as f:
                                f.write(
                                f'evenement defaut non reconnu non interpretable {cle_ligne}, {buffer}, {file}, {station} \n') 
                    else:
                        with open(f'Anomalies_{station}.csv', "a") as f:
                            f.write(
                                f'evenement defaut non reconnu non interpretable {cle_ligne}, {buffer}, {file}, {station} \n')
                except:
                    with open(f'Anomalies_{station}.csv', "a") as f:
                        f.write(
                            f'pb taitement evenemnt {cle_ligne}, {buffer}, {file}, {station}\n')
                continue

            case _:
                with open(f'Anomalies_{station}.csv', "a") as f:
                    f.write(
                        f'pb taitement evenemnt inconnu {cle_ligne}, {buffer}, {file}, {station}\n')
                continue
    return 1,list_data_quotidienne_mesure

def inverser_dictionnaires_voies(dictionnaires_voies):
    return dict(map(lambda x: (x[1][2], x[1][0]), dictionnaires_voies.items()))
def calcul_evenements_etor(references_etor, prefix,cle_ligne, line, file, station):
    references_cartes = list(map(lambda x: x[0], references_etor))
    scale = 16
    num_of_bits = 16
    nums_voie = []
    valeurs_voie = []
    for tor in references_etor:
        carte = tor[0]
        val_tor = tor[1:]
        index_carte = (ord(carte)-97)
        try:
            assert(len(val_tor) == 4)
        except:
            with open(f'Anomalies.txt', "a") as f:
                f.write(
                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {line}, {file}, {station}\n')
            return [], []
        try:
            bin_tor = bin(int(val_tor, scale))[2:].zfill(num_of_bits)
        except:
            with open(f'Anomalies.txt', "a") as f:
                f.write(
                    f'pb taitement evenemnt {cle_ligne} parsing ligne, {line}, {file}, {station}\n')
            return [], []
        for i in range(16):            
            nums_voie.append(f'voie_{prefix}tor_{(16 * index_carte) + i + 1}')
            valeurs_voie.append(bin_tor[15 - i])

    return nums_voie, valeurs_voie



def creer_fichier_station(filename, columns):
    if not os.path.exists(filename):
        myfile = os.open(filename, flags=os.O_RDWR | os.O_CREAT, mode=0o700)
        f = open(filename, mode='w')
        f.write(columns + '\n')
        f.close()
        os.close(myfile)


def parse_int(s):
    try:
        _ = int(s)
        return True
    except ValueError:
        return False
@time_logger
def creer_repertoire_resultat(station):
    # TODO : Recuperer en version
    directory = station
    path = os.getcwd()
    try:
        os.mkdir(path + "/" + directory, mode=0o700)
        os.mkdir(path + "/" + directory + "/Anomalies", mode=0o700)
        os.mkdir(path + "/" + directory + "/Station", mode=0o700)
        os.mkdir(path + "/" + directory + f"/Voies_{station}_ana", mode=0o700)
        os.mkdir(path + "/" + directory + f"/Voies_{station}_tor", mode=0o700)
    except FileExistsError:
        pass

    shutil.copy2(f'Anomalies_{station}.csv', path +
                 "/" + directory + "/Anomalies")
    shutil.copy2(f'resultat_{station}.csv', path +
                 "/" + directory + "/Station")

    path_script = path + "/" + directory + f"/script_{station.lower()}.sql"
    script_file = os.open(path_script, flags=os.O_RDWR | os.O_CREAT, mode=0o700)
    nom_schema = station.lower() if not parse_int(station) else f'"{str(station)}"'
    buffer_query_schema = [f"CREATE SCHEMA IF NOT EXISTS {nom_schema};\n"]


    df = pd.read_csv(f'resultat_{station}_ana.csv', low_memory=False)
    buffer_query_ana =decouper_dataframe_multiple_voies_insertion_sql(station,
                                                    df, path + "\\" + directory + f"\\Voies_{station}_ana", 'ana')
    df = pd.read_csv(f'resultat_{station}_tor.csv', low_memory=False)
    buffer_query_tor = decouper_dataframe_multiple_voies_insertion_sql(station,
                                                    df, path + "\\" + directory + f"\\Voies_{station}_tor", 'tor')
    
    with open(path_script, 'a') as f:
        f.write(''.join(buffer_query_schema))
        f.write(''.join(buffer_query_ana))if buffer_query_ana is not None else None
        f.write(''.join(buffer_query_tor)) if buffer_query_tor is not None else None
        f.close()
        os.close(script_file)
    try:
        os.remove(path + '\\' + f'resultat_{station}.csv')
        os.remove(path + '\\' + f'Anomalies_{station}.csv')
        os.remove(path + '\\' + f'resultat_{station}_tor.csv')
        os.remove(path + '\\' + f'resultat_{station}_ana.csv')
    except:
        print("did not delete result files")
        pass
    
def decouper_dataframe_multiple_voies_insertion_sql(station, df, folder, suffix):
    path_host = f'/home/softia/shut/data/ResultTransfo/{station}/Voies_{station}_ana' if suffix == 'ana' else f'/home/softia/shut/data/ResultTransfo/{station}/Voies_{station}_tor'
    voies = df['voie'].drop_duplicates().tolist()

    buffer_query = []
    try:
        voies.remove('error')
    except:
        pass
    if len(voies) == 0:
        return None
    voies = list(filter(lambda x: str(x).rfind('_') != -1, voies))
    for voie in voies:
        df_voie = df[df['voie'] == voie].copy()
        # TODO creer lambda a partir de horodate et equipement
        # Faire attention NU
        df_voie['horodate_tr'] = df_voie.apply(
            lambda x: transformerTL_TU(x['horodate'],x['TUTL']), axis=1)
        df_voie_tr = df_voie[['horodate_tr',
                            'evenement', 'mesure', 'mesure_brute']]
        df_voie_tr.columns = ['horodate', 'evenement', 'mesure', 'mesure_brute']
        df_voie_tr.to_csv(folder + '\\' + voie + '.csv',
                        index=False, header=False)

        nom_schema = station.lower() if not parse_int(station) else f'"{str(station)}"'

        buffer_query.append(
            f"CREATE TABLE IF NOT EXISTS {nom_schema}.{voie.lower()} (horodate TIMESTAMPTZ, evenement TEXT, mesure DOUBLE PRECISION, mesure_brute TEXT);\n")
        buffer_query.append(
            f"\COPY {nom_schema}.{voie.lower()} FROM '{path_host}/{voie}.csv' DELIMITER ',' CSV ;\n")
    buffer_query.sort()
    return buffer_query

def transformerTL_TU(horodatage, TUTL):
    local = pytz.timezone("Europe/Paris")
    time_m = datetime.datetime.strptime(horodatage, '%d/%m/%Y %Hh%M')    
    local_dt = local.localize(time_m, is_dst=True)
    utc_dt = local_dt.astimezone(pytz.utc)
    return utc_dt.strftime('%Y-%m-%d %H:%M:%S') if TUTL == 'TL' else time_m.strftime('%Y-%m-%d %H:%M:%S')

def transfer_to_remote_machine(station):
    ssh = createSSHClient('192.168.10.77', 22, 'softia', 'Softia321+')
    scp = SCPClient(ssh.get_transport())

    scp.put(rf'C:\Users\Nabil\Desktop\Softia_Work_Repos\SHUTRefonte\shut-refonte\ScriptLectureReleves\{station}',
            '/home/softia/shut/data/ResultTransfo', recursive=True)

    shutil.rmtree(
        rf'C:\Users\Nabil\Desktop\Softia_Work_Repos\SHUTRefonte\shut-refonte\ScriptLectureReleves\{station}')


def createSSHClient(server, port, user, password):
    client = paramiko.SSHClient()
    client.load_system_host_keys()
    client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    client.connect(server, port, user, password)
    return client
def recuperer_derniere_config_etor():
    references_etor = []
    conn = sqlite3.connect('traitement_persistence_tor.db')
    cursor = conn.cursor()
    query = '''
    SELECT * FROM PD_ETOR;
    '''
    cursor.execute(query)
    rows = cursor.fetchall()
    for row in rows:
        references_etor.append(row[1]) if len(row[1]) > 0 else None
    cursor.close()
    conn.close()
    return references_etor

def update_derniere_config_etor(references_etor):
    conn = sqlite3.connect('traitement_persistence_tor.db')
    cursor = conn.cursor()
    for ref in references_etor:
        variable = (ord(ref[0])-97) + 1
        query = f'''
        UPDATE PD_ETOR SET carte_{variable} = '{ref}';
        '''
        print(query)
        cursor.execute(query)
    conn.commit()
    cursor.close()
    conn.close()


def reset_config_etor():
    references_etor = []
    conn = sqlite3.connect('traitement_persistence_tor.db')
    cursor = conn.cursor()
    query = '''
    DELETE FROM PD_ETOR;
    '''
    cursor.execute(query)
    conn.commit()
    query = '''
    INSERT INTO PD_ETOR VALUES ('', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '');
    '''
    cursor.execute(query)
    conn.commit()
    cursor.close()
    conn.close()
    return references_etor

def interpreter_voie_mesure(line, jour, dictionnaires_voies, list_data_quotidienne_mesure, cle_ligne):
    horodatage = jour + ' ' + line[1:6]
    items = line.strip('\n').split(' ')
    for index, item in enumerate(items):
        if item != '' and 1 < index <= len(items) - 1:
            if not (item[0] in dictionnaires_voies.keys()):
                    continue
            try:
                (voie, precision, numero_voie) = dictionnaires_voies[item[0]]
                mesure_brute = int(item[1:])
                mesure = int(item[1:])/math.pow(10, abs(int(precision)))
                list_data_quotidienne_mesure.append(
                    (horodatage, voie, mesure, mesure_brute, cle_ligne))
            except:
                mesure_brute = item[1:]
                list_data_quotidienne_mesure.append(
                    (horodatage, voie, 0, mesure_brute, cle_ligne))
                

def detecter_nombre_jours_dans_hebdo(file, date, data):
    #semaine ne comprenant pas 7 jours
    if len(data) < 7:
        print(file)
        print(date)
        print('fichier avec moins de 7 jours')
    for jour in data:
        #data quotidienne qui ne commence pas par E
        if jour[0][0] != 'E':
            print(file)
            print(date)
            print('fichier data quotidienne qui ne commence pas par E')
        #data horaire croissante 
        for index, _ in enumerate(jour):
            if 0 < index < len(jour) - 1:
                bufferi = (jour[index][1:6])
                bufferi1 = (jour[index+1][1:6])
                if (bufferi > bufferi1):
                    print(file)
                    print(date)
                    print('fichier data horaire non croissante')





def definir_equipement_TUTL_station_selon_date(station, key, index, dates_passage_API_stations, dates_passage_TU_stations):
    return 'Automate', 'TL'


def exec_script_insertion(station):
    command = ['psql', '-h', 'localhost', '-U', 'postgres',
               '-d', 'shut_archive', '-f', f'{const.path_archive}/{station}/script_{station.lower()}.sql']
# Execute the command
    try:
        result = subprocess.run(command, check=True,
                                text=True, capture_output=True)
        print("SQL script executed successfully")
    except subprocess.CalledProcessError as e:
        print(f"Error executing SQL script: {e}")
        pass


def read_station_from_stdin():
    list_stations_a_traiter = os.listdir(
        const.path_archive)
    list_stations_traitees = []
    list_stations = list(set(list_stations_a_traiter) -
                         set(list_stations_traitees))
    list_stations.sort()
    stations = list(
        map(lambda x: definir_nom_station_depuis_archives(x), list_stations))
    stations_cleaned = list(filter(lambda x: x is not None, stations))
    question_user = [
        inquirer.List(
            "User",
            message="Qui es tu ?",
            choices=['Nabil', 'Yann'],
        )]
    answers = inquirer.prompt(question_user, theme=GreenPassion())    
    user = answers['User']
    answers = []
    stations_question = list(
        map(lambda x: x[0], filter(lambda x: x[1] == user, stations_cleaned)))
    question_station = [
        inquirer.List(
            "Station",
            message="Quelle station a traiter ?",
            choices=stations_question,
        ),
    ]
    answers = inquirer.prompt(question_station, theme=GreenPassion())
    station = answers['Station']
    if os.name == 'nt':
        os.system('cls')
    else:
        os.system('clear')
    return station


def detecter_type_equipement(data, file, station):
    events = list(map(lambda x: x[0], data))
    events_nn_dupliques = list(set(events))
    if len(list(set(events_nn_dupliques) & set(const.evenemet_sten))) > len(list(set(events_nn_dupliques) & set(const.evenemets_automates))):
        with open(f'types_equipements.csv', "a") as f:
            f.write(f'{file}, {station}, sten\n')
    elif len(list(set(events_nn_dupliques) & set(const.evenemets_automates))) > len(list(set(events_nn_dupliques) & set(const.evenemet_sten))):
        with open(f'types_equipements.csv', "a") as f:
            f.write(f'{file}, {station}, automate\n')
    else:
        if station in const.stens:
            with open(f'types_equipements.csv', "a") as f:
                f.write(f'{file}, {station}, sten\n')
        else:
            with open(f'types_equipements.csv', "a") as f:
                f.write(f'{file}, {station}, automate\n')

def detecter_equipements_stations():
    list_stations = os.listdir(
        const.path_archive)
    pattern = re.compile(r'.*\S\d{2}$')
    for station in list_stations:
        arr = os.listdir(const.path_archive + '\\' + station)
        files = [const.path_archive + '\\' + station+'\\' + x for x in arr]
        for file in files:
            if pattern.match(file.split('.')[-1]):
                f = open(file, "r")
                _, data = read_file_releve(f)
                detecter_type_equipement(data, file, station)


def detecter_type_equipement_data_quotidienne(data, station):
    events = list(map(lambda x: x[0], data))
    events_nn_dupliques = list(set(events))
    if len(list(set(events_nn_dupliques) & set(const.evenemet_sten))) > len(list(set(events_nn_dupliques) & set(const.evenemets_automates))):
        return 'STEN', 'TL'
    elif len(list(set(events_nn_dupliques) & set(const.evenemets_automates))) > len(list(set(events_nn_dupliques) & set(const.evenemet_sten))):
        return 'Automate', 'TU'
    else:
        if station in const.stens:
            return 'STEN','TL'
        else:
            return 'Automate','TU'

def definir_nom_station_depuis_archives(station_archive):
    with open(f'ListeStations.csv', "r") as f:
        lines = f.readlines()
        for index, line in enumerate(lines):
            if index > 0:
                items = line.strip('\n').split(';')
                station = items[0].strip(' ')
                nom_station = items[1].strip(' ')
                user = items[3].strip(' ')
                if station_archive == station:
                    return nom_station, user


def definir_station_depuis_nom_station(nom_station_a_traiter):
    with open(f'ListeStations.csv', "r") as f:
        lines = f.readlines()
        for index, line in enumerate(lines):
            if index > 0:
                items = line.strip('\n').split(';')
                station = items[0].strip(' ')
                nom_station = items[1].strip(' ')
                if nom_station_a_traiter == nom_station:
                    return station
