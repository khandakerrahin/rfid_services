using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DL950_console
{
    class LogSwitchTimer
    {
        int logSwitchTimerInMillis;
        public LogSwitchTimer()
        {
            this.logSwitchTimerInMillis = 60000; //60000
        }
        public LogSwitchTimer(int logSwitchTimerInMinutes)
        {
            this.logSwitchTimerInMillis = logSwitchTimerInMinutes * 60000;
        }
        public void checkTimeForLogSwitch()
        {
            while (Program.isRunning)
            {
                try
                {
                    //Console.WriteLine("LogSwitchTimer is running on another thread.");
                    Thread.Sleep(logSwitchTimerInMillis);
                    if (Program.isRunning)
                    {
                        String lastLogFilename = Program.filename;
                        Program.isSwitchingLog = true;
                        Program.switchLog();
                        Program.isSwitchingLog = false;
                        LogSwitchTimer.removeLastEmptyLogFile(lastLogFilename); //can be done in a separate thread
                        lastLogFilename = Program.applicationLogFilename;
                        Program.isSwitchingApplicationLog = true;
                        Program.switchApplicationLog();
                        Program.isSwitchingApplicationLog = false;
                        LogSwitchTimer.removeLastEmptyLogFile(lastLogFilename); //can be done in a separate thread
                    }
                    //Console.WriteLine("The checkTimeForLogSwitch method called by the worker thread has ended.");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        internal static void removeLastEmptyLogFile(string lastLogFilename)
        {
            FileInfo fileInfo = new FileInfo(lastLogFilename);
            if (fileInfo.Exists)
            {
                if (fileInfo.Length == 0 || fileInfo.Length < 2)
                {
                    //empty file. Delete it.
                    try
                    {
                        File.Delete(lastLogFilename);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
