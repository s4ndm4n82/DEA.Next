﻿using LogFileCleaner;
using WriteLog;

int deleteStatus = LogFileCleanerClass.StartCleaner();

string logEntry = deleteStatus == 1 ? "Log file deletion ended successfully ...." : "log file deletion unsuccessfull ....";
int logType = deleteStatus != 1 ? 0 : 1;

WriteLogClass.WriteToLog(logType, logEntry, 1);