using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Resources;
using System.Reflection;
using ReaderB;
using System.IO.Ports;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace DL950_console
{
    class Program
    {
        // [STAThread]
        private bool fAppClosed; //Respond to close the application in test mode
        private byte fComAdr = 0xff; //Current operation of ComAdr
        private int ferrorcode;
        private byte fBaud;
        private double fdminfre;
        private double fdmaxfre;
        private byte Maskadr;
        private byte MaskLen;
        private byte MaskFlag;
        private int fCmdRet = 30; //Return value of all execution instructions
        private int fOpenComIndex; //Open serial port index number
        private bool fIsInventoryScan;
        private bool fisinventoryscan_6B;
        private byte[] fOperEPC = new byte[36];
        private byte[] fPassWord = new byte[4];
        private byte[] fOperID_6B = new byte[8];
        private int CardNum1 = 0;
        ArrayList list = new ArrayList();
        private bool fTimer_6B_ReadWrite;
        private bool portSet = false;
        private string fInventory_EPC_List; //Store the inquiry list (if the read data has not changed, it will not be refreshed)
        private int frmcomportindex;
        private bool ComOpen = false;

        ////The following variables are required for TCPIP configuration 
        public string fRecvUDPstring = "";
        public string RemostIP = "";
        internal static string filename;
        internal static string applicationLogFilename;
        private static System.IO.StreamWriter logFile;
        private static System.IO.StreamWriter applicationLogFile;
        private static String filePath;
        private static String filenamePrefix, filenamePostfix;
        private static String applicationLogFilePath;
        private static String applicationLogFilenamePrefix, applicationLogFilenamePostfix;
        String antennaID;
        static int ComPort;
        String IP;
        internal static bool isSwitchingLog;
        internal static bool isSwitchingApplicationLog;
        Thread logSwitchTimerThread;
        internal static bool isRunning;
        private LogSwitchTimer logSwitchTimer;
        private static String flag = ".";
        private static int errCount = 0;

        static void Main(string[] args)
        {
            Program pr=new Program();

            pr.initializeMain(args);

            int dotCounter = 0;
            while (true) {
                try
                {
                    if (dotCounter > 30)
                    {
                        Console.Write(flag);
                        dotCounter = 0;
                    }

                    pr.startQuery();
                    
                    dotCounter++;
                    //Thread.Sleep(50);
                }
                catch (Exception e)
                {
                    //close log
                    Debug.WriteLine(e.ToString());
                    Thread.Sleep(300);
                    try {
                        pr.initializeMainRetry(args);
                    }
                    catch (Exception ex) {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private void initializeMain(String[] args)
        {
            Console.WriteLine("SDCDL950_console started.");
            if (args.Length > 0)
                initiate(args);
            else
                initiate();
            Console.WriteLine("Configs initiated.");

            antennaConfig();
            Console.WriteLine("Antenna Configs initiated.");

            if (errCount < 5)
            {
                openNetPort();
            }
            else
            {
                openPort();
            }
            
            readyLogWrite();
        }

        private void initializeMainRetry(String[] args)
        {
            Console.WriteLine("SDCDL950_console restarted.");
            if (args.Length > 0)
                initiate(args);
            else
                initiate();
            Console.WriteLine("Configs re-initiated.");

            antennaConfig();
            Console.WriteLine("Antenna Configs re-initiated.");

            if (errCount < 5)
            {
                openNetPort();
            }
            else
            {
                openPort();
            }
        }

        public void initiate() // old Form1
        {
            antennaID = "0";
            ComPort = 0;
            populateFromArgs(new string[] { "" });
            Console.WriteLine("Populated from ARGS.");
            //  InitializeComponent();
            performSecondaryInitailizations();
            Console.WriteLine("Performed secondary initializations.");
        }

        /**
         * id= AntennaID (String)
         * p = COM Port (Integer)
         * lp= LogPath (String)
         * ap= ApplicationLogPath (String)
         * ip= Antenna's IP to connect to (String)
         */
        public void initiate(String[] args)
        {
            //id=0 p=5 lp=logPath ap=appLogPath
            this.antennaID = args[0];
            ComPort = 0;
            populateFromArgs(args);
            //  InitializeComponent();
            performSecondaryInitailizations(); //logPath , applicationLogPath, ComPort
        }

        ~Program()
        {
            Program.closeLog();
        }

        /**
        * id= AntennaID (String)
        * p = COM Port (Integer)
        * lp= LogPath (String)
        * ap= ApplicationLogPath (String)
        * ip= Antenna's IP to connect to (String)
        */
        private void populateFromArgs(String[] args)
        {
            String[] option;
            for (int i = 0; i < args.Length; ++i)
            {
                option = null;
                if (args[i].Contains("="))
                {
                    option = args[i].Split(new char[] { '=' }, 2);
                    switch (option[0].Trim())
                    {
                        case "id": this.antennaID = option[1].Trim(); break;
                        case "p":
                            portSet = true;
                            if (!Int32.TryParse(option[1].Trim(), out ComPort))
                            {//invalid COM Port number
                                ComPort = 0;
                                portSet = false;
                            }
                            Console.WriteLine("shake Comport : "+ ComPort);
                            break;
                        case "lp":
                            Program.filePath = option[1].Trim();
                            break;
                        case "ap":
                            Program.applicationLogFilePath = option[1].Trim();
                            break;
                        case "ip":
                            this.IP = option[1].Trim();
                            break;
                        default: break;
                    }
                }
            }
            if (this.antennaID == null || this.antennaID.Length == 0) this.antennaID = "0";
            if (Program.filePath == null || Program.filePath.Length == 0) Program.filePath = "C:\\Data\\logs\\";
            if (Program.applicationLogFilePath == null || Program.applicationLogFilePath.Length == 0) Program.applicationLogFilePath = "C:\\Data\\errors\\";
        }

        private void performSecondaryInitailizations()
        {
            ErrorCodeDefinitions.populate();
            //this.tabControl1.DrawItem += TabControl1OnDrawItem;
            //this.Text = this.antennaID + ((this.IP != null && !this.IP.Equals("")) ? (" " + this.IP) : "") + " " + "COM" + (this.ComPort == 0 ? " Auto" : (this.ComPort + "")) + " " + this.Text;

            Program.logFile = null;
            Program.filenamePrefix = "attendance_" + this.antennaID + "_";
            Program.filenamePostfix = ".txt";
            Program.isSwitchingLog = false;

            Program.applicationLogFile = null;
            Program.applicationLogFilenamePrefix = "log_" + this.antennaID + "_";
            Program.applicationLogFilenamePostfix = ".txt";
            Program.isSwitchingApplicationLog = false;

            Program.isRunning = false;
            this.logSwitchTimer = new LogSwitchTimer();
            //this.logSwitchTimerThread = new Thread(new ThreadStart(logSwitchTimer.checkTimeForLogSwitch));
            // Start the thread.
            //this.logSwitchTimerThread.Start();
            //           this.switchLog();
        }
        public static void switchLog()
        {
            if (Program.logFile != null)
                closeLog();
            Program.filename = Program.filePath + Program.filenamePrefix + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + Program.filenamePostfix;
            Console.WriteLine("FileName : "+ Program.filename);
            Program.logFile = new System.IO.StreamWriter(Program.filename, true);
            Program.logFile.AutoFlush = true;
            //if Program.logFile couldn't be opened.. crash.
        }
        private static void closeLog()
        {
            try
            {
                Program.logFile.Flush();
            }
            catch (Exception ex) when (ex is EncoderFallbackException || ex is ObjectDisposedException || ex is IOException)
            {
                //do nothing. Just close it.
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                try
                {
                    Program.logFile.Dispose();
                }
                catch (Exception ex) when (ex is EncoderFallbackException || ex is IOException)
                {
                    //do nothing. Just continue to close it.
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    try
                    {
                        Program.logFile.Close();
                    }
                    catch (Exception ex) when (ex is EncoderFallbackException || ex is IOException || ex is NullReferenceException)
                    {
                        //do nothing. might be a permission error or is already closed.
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public static void switchApplicationLog()
        {
            if (Program.applicationLogFile != null)
                closeApplicationLog();
            Program.applicationLogFilename = Program.applicationLogFilePath + Program.applicationLogFilenamePrefix + DateTime.Now.ToString("yyyyMMddHHmmssffffff") + Program.applicationLogFilenamePostfix;
            Program.applicationLogFile = new System.IO.StreamWriter(Program.applicationLogFilename, true);
            Program.applicationLogFile.AutoFlush = true;
            //if Program.applicationLogFile couldn't be opened.. crash.
        }
        private static void closeApplicationLog()
        {
            try
            {
                Program.applicationLogFile.Flush();
            }
            catch (Exception ex) when (ex is EncoderFallbackException || ex is ObjectDisposedException || ex is IOException)
            {
                //do nothing. Just close it.
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                try
                {
                    Program.applicationLogFile.Dispose();
                }
                catch (Exception ex) when (ex is EncoderFallbackException || ex is IOException)
                {
                    //do nothing. Just continue to close it.
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    try
                    {
                        Program.applicationLogFile.Close();
                    }
                    catch (Exception ex) when (ex is EncoderFallbackException || ex is IOException || ex is NullReferenceException)
                    {
                        //do nothing. might be a permission error or is already closed.
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private string GetReturnCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "Operation Successful";
                case 0x01:
                    return "Return before Inventory finished";
                case 0x02:
                    return "the Inventory-scan-time overflow";
                case 0x03:
                    return "More Data";
                case 0x04:
                    return "Reader module MCU is Full";
                case 0x05:
                    return "Access Password Error";
                case 0x09:
                    return "Destroy Password Error";
                case 0x0a:
                    return "Destroy Password Error Cannot be Zero";
                case 0x0b:
                    return "Tag Not Support the command";
                case 0x0c:
                    return "Use the commmand,Access Password Cannot be Zero";
                case 0x0d:
                    return "Tag is protected,cannot set it again";
                case 0x0e:
                    return "Tag is unprotected,no need to reset it";
                case 0x10:
                    return "There is some locked bytes,write fail";
                case 0x11:
                    return "can not lock it";
                case 0x12:
                    return "is locked,cannot lock it again";
                case 0x13:
                    return "Parameter Save Fail,Can Use Before Power";
                case 0x14:
                    return "Cannot adjust";
                case 0x15:
                    return "Return before Inventory finished";
                case 0x16:
                    return "Inventory-Scan-Time overflow";
                case 0x17:
                    return "More Data";
                case 0x18:
                    return "Reader module MCU is full";
                case 0x19:
                    return "Not Support Command Or AccessPassword Cannot be Zero";
                case 0xFA:
                    return "Get Tag,Poor Communication,Inoperable";
                case 0xFB:
                    return "No Tag Operable";
                case 0xFC:
                    return "Tag Return ErrorCode";
                case 0xFD:
                    return "Command length wrong";
                case 0xFE:
                    return "Illegal command";
                case 0xFF:
                    return "Parameter Error";
                case 0x30:
                    return "Communication error";
                case 0x31:
                    return "CRC checksum error";
                case 0x32:
                    return "Return data length error";
                case 0x33:
                    return "Communication busy";
                case 0x34:
                    return "Busy,command is being executed";
                case 0x35:
                    return "ComPort Opened";
                case 0x36:
                    return "ComPort Closed";
                case 0x37:
                    return "Invalid Handle";
                case 0x38:
                    return "Invalid Port";
                case 0xEE:
                    return "Return command error";
                default:
                    return "";
            }
        }
        private string GetErrorCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "Other error";
                case 0x03:
                    return "Memory out or pc not support";
                case 0x04:
                    return "Memory Locked and unwritable";
                case 0x0b:
                    return "No Power,memory write operation cannot be executed";
                case 0x0f:
                    return "Not Special Error,tag not support special errorcode";
                default:
                    return "";
            }
        }

        private void antennaConfig() // old button13_Click
        {
            byte Ant = 0;
            Ant = Convert.ToByte(Ant | 0x01);
            Ant = Convert.ToByte(Ant | 0x02);
            fCmdRet = StaticClassReaderB.SetAnt(ref fComAdr, Ant, frmcomportindex);
            Console.WriteLine("Antenna config : "+ fCmdRet);
        }

        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }

        private Boolean openPort() //   old OpenPort_Click
        {
            int port = ComPort;
            Console.WriteLine("Attempting open Com : "+port);
            int openresult, i;
            errCount = 0;
            openresult = 30;
            string temp;
            fComAdr = Convert.ToByte("FF", 16); // $FF;
            try
            {
                for (i = 6; i >= 0; i--)
                {
                    fBaud = Convert.ToByte(i);
                    if ((fBaud == 3) || (fBaud == 4))
                        continue;
                    openresult = StaticClassReaderB.OpenComPort(port, ref fComAdr, fBaud, ref frmcomportindex);
                    fOpenComIndex = frmcomportindex;
                    if (openresult == 0x35)
                    {
                        Console.WriteLine("COM port opened : " + fOpenComIndex);
                        ComOpen = true;
                        return ComOpen;
                    }
                    if (openresult == 0)
                    {
                        ComOpen = true;
                        getReaderInfo(); //自动执行读取写卡器信息
                        fBaud = Convert.ToByte(0);  // ComboBox_baud2.SelectedIndex = 0 for AUTO
                        if (fBaud > 2)
                        {
                            fBaud = Convert.ToByte(fBaud + 2);
                        }
                        if ((fCmdRet == 0x35) || (fCmdRet == 0x30))
                        {
                            ComOpen = false;
                            Console.WriteLine("Serial Communication Error or Occupied");
                            StaticClassReaderB.CloseSpecComPort(frmcomportindex);
                            return ComOpen;
                        }
                        break;
                    }

                }
                
            }
            finally
            {
                if ((fOpenComIndex != -1) & (openresult != 0X35) & (openresult != 0X30))
                {
                    ComOpen = true;
                }
                if ((fOpenComIndex == -1) && (openresult == 0x30))
                    Console.WriteLine("Serial Communication Error");
                Console.WriteLine("COM port opened : " + fOpenComIndex);
            }
            return ComOpen;
        }


        private Boolean closePort() {
            int port = ComPort;
            fCmdRet = StaticClassReaderB.CloseSpecComPort(port);
            
            ComOpen = false;
            return !ComOpen;
        }
        private Boolean openNetPort() //    old OpenNetPort_Click
        {
            errCount++;
            Console.WriteLine("TCIP open attempt : "+errCount);
            int port, openresult = 0;
            string IPAddr;
            fComAdr = Convert.ToByte("FF", 16); // $FF;
            port = Convert.ToInt32("6000");
            if (IP==null) {
                IP = "192.168.1.190";
            }
            IPAddr = IP;
            Console.WriteLine("port : "+ port);
            Console.WriteLine("IPAddr : " + IPAddr);
            //Console.WriteLine("fComAdr : " + fComAdr);
            //Console.WriteLine("frmcomportindex : " + frmcomportindex);
            openresult = StaticClassReaderB.OpenNetPort(port, IPAddr, ref fComAdr, ref frmcomportindex);
            fOpenComIndex = frmcomportindex;
            if (openresult == 0)
            {
                ComOpen = true;
                getReaderInfo(); //自动执行读取写卡器信息
            }
            if ((openresult == 0x35) || (openresult == 0x30))
            {
                Console.WriteLine("TCPIP error");
                StaticClassReaderB.CloseNetPort(frmcomportindex);
                ComOpen = false;
                return ComOpen;
            }
            if ((fOpenComIndex != -1) && (openresult != 0X35) && (openresult != 0X30))
            {
                ComOpen = true;
                Console.WriteLine("TCPIP opened");
                errCount = 0;
            }
            if ((fOpenComIndex == -1) && (openresult == 0x30)) { 
                Console.WriteLine("TCPIP Communication Error");
                ComOpen = false;
            }
            Console.WriteLine("COMindex : " + fOpenComIndex + " openResult : " + openresult);
            return ComOpen;

        }

        private void closeNetPort() //    old CloseNetPort_Click
        {
            fCmdRet = StaticClassReaderB.CloseNetPort(frmcomportindex);
            if (fCmdRet == 0)
            {
                fOpenComIndex = -1;
                
            }
        }
    
        private void activateRelay() //  old button20_Click
        {
            byte RelayStatus = 0;
            RelayStatus = Convert.ToByte(RelayStatus | 1);

            RelayStatus = Convert.ToByte(RelayStatus | 2);

            fCmdRet = StaticClassReaderB.SetRelay(ref fComAdr, RelayStatus, frmcomportindex);
            
        }

        private void getReaderInfo() //   old Button3_Click
        {
            byte[] TrType = new byte[2];
            byte[] VersionInfo = new byte[2];
            byte ReaderType = 0;
            byte ScanTime = 0;
            byte dmaxfre = 0;
            byte dminfre = 0;
            byte powerdBm = 0;
            byte FreBand = 0;
            byte Ant = 0;
            fCmdRet = StaticClassReaderB.GetReaderInformation(ref fComAdr, VersionInfo, ref ReaderType, TrType, ref dmaxfre, ref dminfre, ref powerdBm, ref ScanTime, ref Ant, frmcomportindex);
            
        }

        private void readyLogWrite()
        {
            //TODO switchlog here
            Program.switchLog();
            Program.switchApplicationLog();
            Program.isRunning = true;
            this.logSwitchTimerThread = new Thread(new ThreadStart(this.logSwitchTimer.checkTimeForLogSwitch));
            this.logSwitchTimerThread.Start();
           
        }

        private void startQuery() // old Inventory
        {
            int i;
            int CardNum = 0;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[300];
            int CardIndex;
            string temps;
            string s, sEPC;
            bool isonlistview;
            byte AdrTID = 0;
            byte LenTID = 0;
            byte TIDFlag = 0;
            
            AdrTID = 0;
            LenTID = 0;
            TIDFlag = 0;
            
            byte Qvalue = Convert.ToByte(5);
            byte Session = Convert.ToByte(0);
            fCmdRet = StaticClassReaderB.Inventory_G2(ref fComAdr, Qvalue, Session, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, frmcomportindex);
            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))//The end of delegation
            {
                byte[] daw = new byte[Totallen];
                Array.Copy(EPC, daw, Totallen);
                temps = ByteArrayToHexString(daw);
                fInventory_EPC_List = temps;            //storage record
                m = 0;
                if (CardNum == 0)
                {
                    flag = ".";
                    return;
                }
                string lastEPC = "";
                Console.Write(CardNum);
                for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
                {
                    EPClen = daw[m];
                    sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                    lastEPC = sEPC;
                    string RSSI = temps.Substring(m * 2 + 2 + EPClen * 2, 2);
                    m = m + EPClen + 2;
                    if (sEPC.Length != EPClen * 2)
                        return;
                    isonlistview = false;
                    
                    
                    String csvRow = "";
                    s = sEPC;
                    csvRow += s;
                    s = (sEPC.Length / 2).ToString().PadLeft(2, '0');
                    csvRow += "," + s;
                    s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                    csvRow += "," + s;
                    csvRow += "," + this.antennaID;
                    //Console.WriteLine(csvRow);
                    
                    while (Program.isSwitchingLog) Thread.Sleep(200);
                    Program.logFile.WriteLine(csvRow);
                    Console.Write("-");

                }
            }
            else if (fCmdRet == 0x30)
            {
                //Communication error
                while (Program.isSwitchingApplicationLog) Thread.Sleep(200);
                Program.applicationLogFile.WriteLine(this.antennaID + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "," + fCmdRet + "," + ErrorCodeDefinitions.get(fCmdRet));
                Console.WriteLine(this.antennaID + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "," + fCmdRet + "," + ErrorCodeDefinitions.get(fCmdRet));
                closeNetPort();
                closePort();
                Thread.Sleep(300);

                if (errCount < 5)
                {
                    openNetPort();
                }
                else {
                    openPort();
                }
                
                
            }
            else
            {
                while (Program.isSwitchingApplicationLog) Thread.Sleep(200);
                Program.applicationLogFile.WriteLine(this.antennaID + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "," + fCmdRet + "," + ErrorCodeDefinitions.get(fCmdRet));
                Console.WriteLine(this.antennaID + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "," + fCmdRet + "," + ErrorCodeDefinitions.get(fCmdRet));
                if (fCmdRet == 248)
                {
                }
                else {
                    closeNetPort();
                    closePort();
                    Thread.Sleep(300);
                    if (errCount < 5)
                    {
                        openNetPort();
                    }
                    else
                    {
                        openPort();
                    }
                }
                
            }
        }
    }
}
