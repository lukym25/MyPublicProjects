# Hledač souborů
# zimní semestr 2024/2025
# Program umožuje rychlejší hledání souboru podle jména nebo data modifikace

import os
import sys
import pickle
import datetime 

#22 znaků
CZECH_LETTERS = ['ě', 'š', 'č', 'ř', 'ž', 'ý', 'á', 'í', 'é', 'ů', 'ú', 'Ě', 'Š', 'Č', 'Ř', 'Ž', 'Ý', 'Á', 'Í', 'É', 'Ů', 'Ú']

#Funkce převádí hodnotu char na int, podle ASCII tabulky
#České znaky jsou přemapované do prvních 32 míst, kde jsou normálně řídící znaky
#Ostatní znaky vrací hodnotu 127
def char_to_index(char: str) -> int:
    ASCII_value = ord(char)

    #české znaky vrací 250 a větší 
    if(ASCII_value < 128):
        return ASCII_value
    
    i = 0
    while(i < len(CZECH_LETTERS)):
        if(CZECH_LETTERS[i] == char):
            return i;
        i += 1

    return 127

###Definice datových struktur###
#definice datové struktury trie (písmenkoví strom)
class node:
    def __init__(self, directory_index: int = -1):
        self.next_nodes = [None] * 128
        #Index do listu directory_list, kde se uchovávají cesty k souborům
        #directory_index -1 znamená, že nikam neukazuje
        self.directory_path_index = directory_index

class trie:
    def __init__(self):
        self.root = node()
    
    #Do stromu vloží dvě slova jméno souboru s příponou a bez přípony 
    #Příklad: soubor krtek.txt vloží se "krtek.txt" a "krtek"
    #Umožuje to vyhledávání souboru i bez přípony a nemusí se vytvářet žádné uzly navíc, 
    #protože v písmenkovém stromě oba soubory sdílí část své cesty "k, r, t, e, k"
    def insert(self, word: str, directory_index: int):
        current_node = self.root
        last_node = self.root
        
        for char in word:
            index = char_to_index(char)

            if(current_node.next_nodes[index] is None):
                if(char == word[-1]):
                    current_node.next_nodes[index] = node(directory_index)
                else:
                    current_node.next_nodes[index] = node()

            #46 je v ascii "."
            if(index == 46):
                current_node.directory_path_index = directory_index

            last_node = current_node
            current_node = current_node.next_nodes[index]

    def find_path_index(self, word: str) -> int:
        current_node = self.root        

        for char in word:
            index = char_to_index(char)

            if(current_node.next_nodes[index] is None):
                return -1
            
            current_node = current_node.next_nodes[index]

        return current_node.directory_path_index

#List reference se používá dvěma způsoby
#Koncové uzly (listy) ve třetí hladině obsahují indexy, které odkazují na cestu k souboru v listu directory_list
#Uzly v nižších hladinách v listu reference mají odkazy na následující uzly
class date_node:
    def __init__(self, value: int = None, directory_index: int = -1):
        self.value = value
        self.references = []
        if(directory_index != -1):
            self.references.append(directory_index)

#date_tree je strom s maximální hladinou 3
#hladina 1 obsahuje uzly s hodntou roků
#hladina 2 obsahuje uzly s hodntou měsíců
#hladina 3 obsahuje uzly s hodntou dní a s indexy do directory_list
class date_tree:
    def __init__(self):
        self.root = date_node()

    def insert(self, date_int: int, file_name: str, directory_index: int):
        year = date_int // 10_000
        month = date_int // 100 - year
        day = date_int - month - year

        year_node = self.jump_to_next_node(self.root, year)
        month_node = self.jump_to_next_node(year_node, month)
        day_node = self.jump_to_next_node(month_node, day)

        day_node.references.append([directory_index, file_name])

    def jump_to_next_node(self, node: date_node, value: int) -> date_node:
        next_node = self.find_next_node(node, value)
        if(next_node != None):
            return next_node

        new_node = date_node(value)
        node.references.append(new_node)
        return new_node

    def find_next_node(self, node: date_node, value: int) -> date_node:
        for next_node in node.references:
            if(value == next_node.value):
                return next_node
        return None

    #vrací list indexů do directory_list <- více souborů sdílí datum úpravy
    def find_path_index(self, date: int) -> list:
        current_node = self.root

        year = date // 10_000 
        month = date // 100 - year
        day = date - month - year

        current_node = self.find_next_node(current_node, year)
        if(current_node == None):
            return None
        current_node = self.find_next_node(current_node, month)
        if(current_node == None):
            return None
        current_node = self.find_next_node(current_node, day)
        if(current_node == None):
            return None

        return current_node.references

###Vyhledávání souborů###
def create_searching_table(path_of_startr_dir: str, search_depth: int) -> object:
    if(not os.path.isdir(path_of_startr_dir)):
        print("Directory is not valid")
        return None

    search_table = find_files(path_of_startr_dir, search_depth)
    save_to_external_file(search_table)
    return search_table

#V hloubce 0 program prohledá soubory zadané složky a hloubka o 1 větší znamená že prohledá i podsložky této složky
def find_files(root_directory: str, search_depth: int) -> object:
    directory_list = []
    file_names_trie = trie()
    modifications_dates_tree = date_tree()

    fileCount = 0

    directories_to_search = []
    directories_to_search.append([root_directory, 0])

    while len(directories_to_search) > 0:
        new_value = directories_to_search.pop()
        new_path = new_value[0]
        cur_depth = new_value[1]
        #někdy se stává, že os.listdir vyhodí error, že nemá přístup do nějaké složky
        try:
            files = os.listdir(new_path)
        except:
            print(f"\rAcces denied: {new_path}")
            continue

        #Pokud je ve složce pouze složka přeskoč (aby se nevytvořil zbytečný zápis v directory_list)
        if (len(files) == 1 and os.path.isdir(os.path.join(new_path, files[0]))):
            add_dir = [os.path.join(new_path, files[0]), cur_depth + 1]
            directories_to_search.append(add_dir)
        else:
            #len() je o 1 větší, tedy directory_list[len()] odkazuje na prvek, který nově přidávám
            directory_table_index = len(directory_list)
            directory_list.append(new_path)
           
            for file in files:
                fileCount += 1
                print(f"\r[{fileCount}]:files found", end="")

                final_path = os.path.join(new_path, file)

                if os.path.isdir(final_path):
                    if(cur_depth < search_depth):
                        add_dir = [final_path, cur_depth + 1]
                        directories_to_search.append(add_dir)
                else:
                    file_names_trie.insert(file, directory_table_index) 

                    time_modified = get_time_modified(final_path)
                    modifications_dates_tree.insert(time_modified, file, directory_table_index)

    #Vytvoření objektu pro vyhledávání
    search_table = {}
    search_table["root_directory"] = root_directory
    search_table["directory_list"] = directory_list
    search_table["file_names_trie"] = file_names_trie
    search_table["modifications_dates_tree"] = modifications_dates_tree

    return search_table

#Převádí se na jiný formát, protože os.path.getatime() vrací timestamp formát a uživatel vyhledává přes formát D/M/Y
def get_time_modified(path: str) -> int:
    time_modified = os.path.getatime(path)
    date = datetime.date.fromtimestamp(time_modified)
    date_in_int = date.year * 10_000 + date.month * 100 + date.day
    return date_in_int

def save_to_external_file(obj: object):
    with open('VyhledávacíData', 'wb') as file:
        pickle.dump(obj, file)
        print("\ndone")

###Main app setup
def save_file_exists() -> bool:
    return os.path.isfile("VyhledávacíData")

def load_saved_file() -> object:
    with open('VyhledávacíData', 'rb') as file:
        return pickle.load(file)

#Rozdělí input na dvě části command a argument
def read_input() -> list:
    command = ""
    argument = ""

    add_to_command = True
    char = sys.stdin.read(1)
    while(char != "\n"):
        if(add_to_command):
            if(char == " "):
                add_to_command = False
            else:
                command += char
        else:
            argument += char
        char = sys.stdin.read(1)

    return [command, argument]

###Main app commands###
def find_by_name(file_name: str, search_table: object):
    if(search_table == None):
        print("Search table not found\nPlease use crtable + path")
        return

    stored_trie = search_table["file_names_trie"]
    index = stored_trie.find_path_index(argument) 
    if(index == -1):
        print("NotFound")
    else:
        directory = search_table["directory_list"][index]
        filePath = os.path.join(directory, argument)
        print(filePath)

def find_by_date(date_in_string: str, search_table: object):
    if(search_table == None):
        print("Search table not found\nPlease use crtable + path")
        return  
    
    date = string_to_date(date_in_string)

    stored_date_tree = search_table["modifications_dates_tree"]
    records = stored_date_tree.find_path_index(date)
    if(records == None):
        print("Not found")
        return

    for record in records:
        directory_index = record[0]
        file_name = record[1]
        directory = search_table["directory_list"][directory_index]

        path = os.path.join(directory, file_name)
        print(path)

#Čte pouze čísla a "/" ostatní vyhazuje
def string_to_date(date_in_string: str) -> int:
    #[year, month, day]
    parts = [0, 0, 0]
    i = 0
    for char in date_in_string:
        if(char == "/" and i < 2):
            i += 1
        elif(is_number(char)):
            parts[i] = parts[i] * 10 + ord(char) - 48

    date_in_int = parts[2] * 10_000 + parts[1] * 100 + parts[0]
    return date_in_int

def is_number(char: str) -> bool:
    ascii_value = ord(char)
    return ((47 < ascii_value) and (58 > ascii_value)) 

def show_root_directory(search_table: object):
    if(search_table == None):
        print("Search table not found\nPlease use crtable + path")
        return
    print(search_table["root_directory"])

def change_depth(argument: str, old_serach_depth) -> int:
    new_depth = string_to_int(argument)
    if(new_depth == -1):
        return old_serach_depth
    print(f"Searching depth changed to: {new_depth}")
    return new_depth

def string_to_int(input_string: str) -> int:
    final_number = 0
    for char in input_string:
        if(is_number(char)):
            new_number = ord(char) - 48
            final_number = final_number * 10 + new_number
        else:
            print("Argument is not a number")
            return -1

    return final_number

def print_help_menu():
    print("\"crtable path\" -> searches in directory and creates file containing information for searching")
    print("\"find filename\" -> searches for file with same name")
    print("\"fdate date\" -> searches for file with same modification date in format D/M/Y")
    print("\"root\" -> shows root directory")
    print("\"depth number\" -> changes seraching depth")
    print("\"help\" -> shows information about available commands")
    print("\"kill\" -> stops application")

###Main APP###
###Setup
print("App Started")
print("Loading Data..", end="")
search_table = None
if(save_file_exists()):
    search_table = load_saved_file()
    print("\rLoading complete")
else:
    print("\nSearch table not found\nPlease use crtable + path")
print("Type help for more information")

#V hloubce 0 program prohledá soubory zadané složky a hloubka o 1 větší znamená že prohledá i podsložky této složky
search_depth = 5

###Main loop
app_running = True
while(app_running):
    print("\nReady for new command")
    new_line = read_input()
    command = new_line[0]
    argument = new_line[1]

    if(command == "crtable"):
        search_table = create_searching_table(argument, search_depth)
    elif(command == "find"):
        find_by_name(argument, search_table)
    elif(command == "fdate"):
        find_by_date(argument, search_table)
    elif(command == "root"):
        show_root_directory(search_table)
    elif(command == "depth"):
        search_depth = change_depth(argument, search_depth)
    elif(command == "help"):
        print_help_menu()
    elif(command == "kill"):
        app_running = False
    else:
        print("Not valid")