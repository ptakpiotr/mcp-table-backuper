from mcp.server.fastmcp import FastMCP
from db import Db
from dotenv import load_dotenv

load_dotenv()

mcp = FastMCP("SQL Server table backuper")

@mcp.tool() 
def backup_tables(tables: list[str]) -> bool:
    """Backup given tables"""
    print("BACKUP TABLES!!!")
    db = Db()
    return db.backup_tables(tables)

if __name__ == "__main__":
    mcp.run(transport="sse")