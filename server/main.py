from mcp.server.fastmcp import FastMCP
from db import Db
from dotenv import load_dotenv
from datetime import date

load_dotenv()

mcp = FastMCP("SQL Server table backuper")

@mcp.tool("backup_tables", "Backup given tables") 
def backup_tables(tables: list[str]) -> bool:
    """Backup given tables"""
    db = Db.Db()
    return db.backup_tables(tables)

@mcp.tool("get_backups", "Get backups newer than") 
def get_backups(newer_than: date) -> str:
    """Get backups"""
    db = Db.Db()
    return db.get_backups(newer_than)

if __name__ == "__main__":
    mcp.run(transport="sse")