using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;



public class SQLiteCommLogger
{
    private string logFolder => (Directory.GetCurrentDirectory() + "/Logs");
     ///Path to log file document
    private string logFilePath => (Directory.GetCurrentDirectory() + "/Logs/IO_LOG.db");
    private string exportedFilePath => (Directory.GetCurrentDirectory() + "/Logs/IO_LOG.txt");
    private string logFileConnectionString =>  "Data Source=" + this.logFilePath +";Version=3;New=True;Compress=True;";

    private string rxTableName = "RX_DATA";
    private string txTableName = "TX_DATA";

    public List<string> RxData => ReadDbData(rxTableName,logFileConnectionString);
    public List<string> TxData => ReadDbData(txTableName,logFileConnectionString);


    public void ExportDbData()
    {
        List<string> exportData = new List<string>();
        exportData.Add("TABLE PID: DATE: DATA---------------------------------");
        exportData.Add(" ");
        exportData.Add("RX DATA-----------------------------------------------");
        exportData.Add(" ");
        exportData.AddRange(RxData);
        exportData.Add(" ");
        exportData.Add("------------------------------------------------------");
        exportData.Add(" ");
        exportData.Add("TX DATA-----------------------------------------------");
        exportData.Add(" ");
        exportData.AddRange(TxData);
        exportData.Add(" ");
        exportData.Add("------------------------------------------------------");
        
        if(Directory.Exists(logFolder) == false)
        {
            Directory.CreateDirectory(logFolder);
        }

        if(File.Exists(exportedFilePath) == false)
        {
            File.Create(exportedFilePath);
        }
        
        
        File.WriteAllLines(exportedFilePath, exportData);




    }

    private List<string> ReadDbData(string tableName, string connectionString)
    {
        SQLiteConnection conn = new SQLiteConnection();
        List<string> dbReturn = new List<string>();

        var data = DatabaseConnector.ExecSqlite("SELECT * FROM " + tableName, connectionString);

        foreach(var dataRow in data)
        {
            try
            {
                var pid = dataRow["PID"].ToString();
                var dateTime = dataRow["TIME_STAMP"].ToString();
                var newData = dataRow["DATA"].ToString();

                dbReturn.Add(tableName + " " + pid + ": " + dateTime + ": " + newData);
                
            }
            catch
            {
                continue;
            }
            
        }

        return dbReturn;
    }



    public void WriteToRxData(string newData)
    {

        var dataCount = RxData.Count;
        var currentTime = System.DateTime.Now.ToString();
        var insertString = string.Format("INSERT INTO {0}({1}, {2}, {3}) VALUES ({4}, '{5}', '{6}');",rxTableName,"PID","TIME_STAMP","DATA",dataCount+1,currentTime,newData);
        DatabaseConnector.ExecSqlite( insertString,logFileConnectionString);
    }


    public void WriteToTxData(string newData)
    {
        var dataCount = TxData.Count;
        var currentTime = System.DateTime.Now.ToString();
        var insertString = string.Format("INSERT INTO {0}({1}, {2}, {3}) VALUES ({4}, '{5}', '{6}');",txTableName,"PID","TIME_STAMP","DATA",dataCount+1,currentTime,newData);
        DatabaseConnector.ExecSqlite( insertString,logFileConnectionString);
    }


    public bool TryClearRxTable()
    {
        bool status = false;

        try
        {
            DatabaseConnector.ExecSqlite("DROP TABLE " + rxTableName, logFileConnectionString);
            DatabaseConnector.ExecSqlite("CREATE TABLE " + rxTableName + "(PID INT, TIME_STAMP VARCHAR(25), DATA VARCHAR(100))",logFileConnectionString);
            status = true;
        }
        catch
        {
            status = false;
        }


        return status;
    }



    public bool TryClearTxTable()
    {
        bool status = false;

        try
        {
            DatabaseConnector.ExecSqlite("DROP TABLE " + txTableName, logFileConnectionString);
            DatabaseConnector.ExecSqlite("CREATE TABLE " + txTableName + "(PID INT, TIME_STAMP VARCHAR(25), DATA VARCHAR(100))",logFileConnectionString);
            status = true;
        }
        catch
        {
            status = false;
        }


        return status;
    }

    

    public void TryDeleteLogs()
    {
        SQLiteConnection conn = new SQLiteConnection();
        try
        {
            DatabaseConnector.ExecSqlite("DROP TABLE " + rxTableName,logFileConnectionString);
            DatabaseConnector.ExecSqlite("DROP TABLE " + txTableName,logFileConnectionString);

            DatabaseConnector.ExecSqlite("CREATE TABLE " + rxTableName + "(PID INT, TIME_STAMP VARCHAR(25), DATA VARCHAR(100))",logFileConnectionString);
            DatabaseConnector.ExecSqlite("CREATE TABLE " + txTableName + "(PID INT, TIME_STAMP VARCHAR(25), DATA VARCHAR(100))",logFileConnectionString);
        }
        catch
        {
            return;
        }
        
    }
    

    public SQLiteCommLogger()
    {
        if(Directory.Exists(logFolder) == false)
        {
            Directory.CreateDirectory(logFolder);
        }

        if(File.Exists(logFilePath) == false)
        {
            var fs = File.Create(logFilePath);
        }

        SQLiteConnection conn = new SQLiteConnection();
        if(DatabaseConnector.GetDBConnection(ref conn,logFileConnectionString))
        {
            try
            {
                DatabaseConnector.ExecSqlite("CREATE TABLE " + rxTableName + "(PID INT, TIME_STAMP VARCHAR(25), DATA VARCHAR(100))",logFileConnectionString);
                DatabaseConnector.ExecSqlite("CREATE TABLE " + txTableName + "(PID INT, TIME_STAMP VARCHAR(25), DATA VARCHAR(100))",logFileConnectionString);
            }
            catch
            {
                return;
            }
        }
        

        
        
    }


    

}

