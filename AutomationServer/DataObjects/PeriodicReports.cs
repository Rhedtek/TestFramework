using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AutomationTestServer.DatabaseObjects;

namespace AutomationTestServer.DataObjects
{
    internal class PeriodicReportEntry
    { 
        internal PeriodicReportData Data { get; set; }
        internal DateTime NextAttempt { get; set; }
    }

    public class PeriodicReports
    {
        private Timer mTimer;
        private const int mTimeoutMs = 1 * 60 * 1000;
        private List<PeriodicReportEntry> mEntries = new List<PeriodicReportEntry>();
        private TestServer mTestServer;

        public PeriodicReports()
        {
            // nothing
        }

        public void Start(TestServer testServer)
        {
            Log("Starting PeriodicReports.");

            mTestServer = testServer;

            Init();

            // One timer to periodically check if new reports need to be sent.
            mTimer = new Timer(TimeoutCallback, null, mTimeoutMs, mTimeoutMs);
        }

        private void Init()
        {
            var results = PeriodicReportData.Select();
            foreach (var result in results)
            {
                var entry = mEntries.FirstOrDefault(e => e.Data.PeriodicReportID == result.PeriodicReportID);
                if (entry == null)
                {
                    if (result.PeriodicReportStatus == 1)
                    {
                        Log("Adding PeriodReport ID=" + result.PeriodicReportID + " Recipients=" + result.Recipients);

                        entry = new PeriodicReportEntry()
                        {
                            Data = result,
                            NextAttempt = CalculateNextTimestamp(result.ScheduleDay, result.ScheduleHour, result.ScheduleMinute),
                        };
                        mEntries.Add(entry);
                    }
                }
                else
                {
                    if (entry.Data.PeriodicReportStatus != result.PeriodicReportStatus && result.PeriodicReportStatus == 0)
                    {
                        Log("Removing PeriodReport ID=" + result.PeriodicReportID);
                        mEntries.Remove(entry);
                    }
                }
            }
        }

        private void TimeoutCallback(object state)
        {
            Init();

            foreach (var entry in mEntries)
            {
                if (entry.NextAttempt < DateTime.Now)
                {
                    SendReport(entry.Data);
                    entry.NextAttempt = CalculateNextTimestamp(entry.Data.ScheduleDay, entry.Data.ScheduleHour, entry.Data.ScheduleMinute);
                }
            }
        }

        // Sun = 0, .., Sat = 6
        private DateTime CalculateNextTimestamp(DayOfWeek day, int hour, int minute)
        {
            DateTime now = DateTime.Now;
            var dayOfWeek = now.DayOfWeek;
            int dayDiff = day - dayOfWeek;
            DateTime next = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            next = next.AddDays(dayDiff);
            if (next < now)
            {
                next = next.AddDays(7);
            }
            Log("CalculateNextTimestamp: Now=" + now + " WeekDay=" + day + " Hour=" + hour + " Min=" + minute + " Next=" + next);

            return next;
        }

        private bool SendReport(PeriodicReportData data)
        {
            string outputFilename = "Report.txt";
            Log("Sending report: ID=" + data.PeriodicReportID);

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "sqlcmd.exe";
                process.StartInfo.Arguments = "-S" + Environment.MachineName + "\\sqlexpress -U testuser -P testapp_987 -d testautomation -i " + data.ScriptPath + " -o " + outputFilename;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();

                EmailHelper.SendEmail(data.Recipients, data.EmailHeader, data.EmailBody, outputFilename);
            }
            catch (Exception ex)
            {
                Log("SendReport exception: " + ex);
            }

            return true;
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);

            if (mTestServer != null)
            {
                mTestServer.Log(msg);
            }
        }
    }
}
