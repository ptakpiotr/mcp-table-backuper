import pyodbc
import os
import datetime as dt

class Db:
    def __init__(self):
        self.conn = self.__get_connection()
    
    def __get_connection(self):
        conn_str = os.getenv("MSSQL_CONN_STR")
        
        return pyodbc.connect(conn_str)
    
    def __create_backup_queries(self, tables: list[str]):
        query = ';'.join([f'SELECT * INTO {t}_Backup_{dt.datetime.timestamp()} FROM {t}' for t in tables])
        
        return query
    
    def backup_tables(self, tables: list[str])->bool:
        try:
            cursor = self.conn.cursor()
            query = self.__create_backup_queries(tables)
        
            cursor.execute(query)
            
            return True
        except Exception as e:
            print(e)
            return False