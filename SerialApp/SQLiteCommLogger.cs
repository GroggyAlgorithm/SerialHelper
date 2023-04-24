using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;



public class SQLiteCommLogger
{
    private string logFolder = (Directory.GetCurrentDirectory() + "/Logs");
    private string logName = "/IO_LOG.db";

    ///Path to log file document
    private string logFilePath;//(Directory.GetCurrentDirectory() + "/Logs/IO_LOG.db");

    private string logFileConnectionString =>  "Data Source=" + this.logFilePath +";Version=3;New=True;Compress=True;";

    private string rxTableName = "RX_DATA";
    private string txTableName = "TX_DATA";

    public List<string> RxData => ReadDbData(rxTableName,logFileConnectionString);
    public List<string> TxData => ReadDbData(txTableName,logFileConnectionString);


    public void CreateNewDatabase()
    {
        string selectedFile = null;

        while (string.IsNullOrEmpty(selectedFile))
        {
            using (SaveFileDialog diag = new SaveFileDialog())
            {
                diag.RestoreDirectory = true;
                diag.InitialDirectory = logFolder;
                diag.Filter = "Database|*.db";
                diag.Title = "Create Log File";
                diag.CheckPathExists = true;
                diag.AddExtension = true;
                DialogResult result = diag.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.OK)
                {
                    try
                    {
                        
                        if(File.Exists(diag.FileName) == false)
                        {
                            var fs = File.Create(diag.FileName);

                            fs.Dispose();
                            fs.Close();

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
                                    
                                }
                            }
                            
                        }

                        selectedFile = diag.FileName;

                        logFilePath = selectedFile;
                        
                    }
                    catch
                    {
                        MessageBox.Show("Could not create file", selectedFile);
                        selectedFile = null;
                    }

                }
            }
        }


        

        

        
    }


    public void SelectDatabase()
    {
        string selectedFile = null;

        while (string.IsNullOrEmpty(selectedFile))
        {


            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.RestoreDirectory = true;
                diag.InitialDirectory = logFolder;
                diag.Filter = "Database|*.db";
                diag.Title = "Select Log File";
                diag.CheckPathExists = true;
                diag.AddExtension = true;
                DialogResult result = diag.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.OK)
                {
                    try
                    {
                        selectedFile = diag.FileName;
                    }
                    catch
                    {
                        MessageBox.Show("Could not open file", selectedFile);
                        selectedFile = null;
                    }

                }
            }
        }


        
    }


    public void ReadDatabase(out List<string> rxDbData, out List<string> txDbData)
    {
        rxDbData = new List<string>();
        txDbData = new List<string>();
        string selectedFile = "";
        using (OpenFileDialog diag = new OpenFileDialog())
        {
            diag.RestoreDirectory = true;
            diag.InitialDirectory = logFolder;
            diag.Filter = "Database|*.db";
            diag.Title = "Open Log File";
            diag.CheckPathExists = true;
            DialogResult result = diag.ShowDialog();
             if(result == DialogResult.Cancel)
            {
                return;
            }
            else if(result == DialogResult.OK)
            {
                try
                {
                    selectedFile = diag.FileName;                    
                    rxDbData = ReadDbData(rxTableName,"Data Source=" + selectedFile +";Version=3;New=True;Compress=True;");
                    txDbData = ReadDbData(txTableName,"Data Source=" + selectedFile +";Version=3;New=True;Compress=True;");
                }
                catch
                {
                    MessageBox.Show("Could not open file", selectedFile);
                }
                
            }
        }
    }


    public void ExportDatabase()
    {
        string exportedFilePath = (logFolder + "/IO_LOG.db");

        
        using(SaveFileDialog saveBrowser = new SaveFileDialog())
        {
            saveBrowser.FileName = "IO_LOG.db";
            saveBrowser.InitialDirectory = logFolder;
            saveBrowser.Filter = "Database|*.db";
            saveBrowser.Title = "Save Log File";
            saveBrowser.CheckPathExists = true;
            DialogResult result = saveBrowser.ShowDialog();
            
            if(result == DialogResult.Cancel)
            {
                return;
            }
            else if(result == DialogResult.OK)
            {
                if(string.IsNullOrWhiteSpace(saveBrowser.FileName) || string.IsNullOrEmpty(saveBrowser.FileName))
                {
                    saveBrowser.FileName = "IO_LOG.db";
                }
                
                if(saveBrowser.FileName.EndsWith(".db") == false)
                {
                    saveBrowser.FileName += ".db";
                }

                exportedFilePath = saveBrowser.FileName;

                
                try
                {
                    
                    File.Copy(logFilePath, exportedFilePath);
                }
                catch
                {
                    MessageBox.Show("Save Failed", exportedFilePath);
                }
                
            }

            
// if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            // {
            //     string[] files = Directory.GetFiles(folderBrowser.SelectedPath);

            //     System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            // }
        }
        





    }



    public void ExportDbData()
    {
        string exportedFilePath = (logFolder + "/IO_LOG.txt");
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

        
        using(SaveFileDialog saveBrowser = new SaveFileDialog())
        {
            saveBrowser.FileName = "IO_LOG.txt";
            saveBrowser.InitialDirectory = logFolder;
            saveBrowser.Filter = "Text File|*.txt";
            saveBrowser.Title = "Save Log File";
            saveBrowser.CheckPathExists = true;
            DialogResult result = saveBrowser.ShowDialog();

            if(result == DialogResult.Cancel)
            {
                return;
            }
            else if(result == DialogResult.OK)
            {
                if(string.IsNullOrWhiteSpace(saveBrowser.FileName) || string.IsNullOrEmpty(saveBrowser.FileName))
                {
                    saveBrowser.FileName = "IO_LOG.txt";
                }
                
                if(saveBrowser.FileName.EndsWith(".txt") == false)
                {
                    saveBrowser.FileName += ".txt";
                }

                exportedFilePath = saveBrowser.FileName;

                
                try
                {
                    if(File.Exists(exportedFilePath) == false)
                    {
                        var fs = File.Create(exportedFilePath);
                        fs.Dispose();
                        fs.Close();


                    }

                    File.WriteAllLines(exportedFilePath, exportData);
                }
                catch
                {
                    MessageBox.Show("Save Failed", exportedFilePath);
                }
                
            }

            

        }
        





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
        
        if(string.IsNullOrWhiteSpace(logFolder) || string.IsNullOrEmpty(logFolder))
        {
            while (string.IsNullOrWhiteSpace(logFolder) || string.IsNullOrEmpty(logFolder))
            {
                MessageBox.Show("Please select log folder path", "IO_LOG.db Path not set",MessageBoxButtons.OK,MessageBoxIcon.Information);
                
                using (var folderBrowser = new FolderBrowserDialog())
                {
                    folderBrowser.RootFolder = System.Environment.SpecialFolder.Recent;
                    folderBrowser.Description = "Select database save location";
                    folderBrowser.UseDescriptionForTitle = true;

                    DialogResult result = folderBrowser.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && Directory.Exists(folderBrowser.SelectedPath))
                    {
                        logFolder = folderBrowser.SelectedPath;
                    }
                }
            }
        }

        logFilePath = (logFolder + logName);

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


    public SQLiteCommLogger(string logPath, string logFileName = "/IO_LOG.db")
    {
        
        logFolder = logPath;
        logName = logFileName;

        
        if (string.IsNullOrWhiteSpace(logFileName) || string.IsNullOrEmpty(logFileName))
        {
            logName = "/IO_LOG.db";
        }

        if(string.IsNullOrWhiteSpace(logFolder) || string.IsNullOrEmpty(logFolder))
        {
            while (string.IsNullOrWhiteSpace(logFolder) || string.IsNullOrEmpty(logFolder))
            {
                
                using (var folderBrowser = new FolderBrowserDialog())
                {
                    folderBrowser.RootFolder = System.Environment.SpecialFolder.Recent;
                    folderBrowser.Description = "Select database save location";
                    folderBrowser.UseDescriptionForTitle = true;

                    DialogResult result = folderBrowser.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && Directory.Exists(folderBrowser.SelectedPath))
                    {
                        logFolder = folderBrowser.SelectedPath;
                    }
                }
            
            
            
            }
        }

        logFilePath = (logFolder + logName);

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

