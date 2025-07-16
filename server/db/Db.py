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
        query = ';'.join([f'SELECT * INTO {t}_Backup_{dt.datetime.now().timestamp().as_integer_ratio()[0]} FROM {t}' for t in tables])
        return query
    
    def __get_backups_query(self):
        query = f"SELECT * FROM sys.tables WHERE [name] LIKE '%Backup%' AND [create_date] > ?"
        return query
    
    def get_backups(self, date: dt.date)->str:
        try:
            cursor = self.conn.cursor()
            query = self.__get_backups_query()
        
            cursor.execute(query, (date))
            rows = cursor.fetchall()
            
            result = "\n".join(" | ".join(str(col) for col in row) for row in rows)

            return result
        except Exception as e:
            print(e)
            return ""
    
    def backup_tables(self, tables: list[str])->bool:
        try:
            cursor = self.conn.cursor()
            query = self.__create_backup_queries(tables)
        
            cursor.execute(query)
            cursor.commit()
            
            return True
        except Exception as e:
            print(e)
            return False