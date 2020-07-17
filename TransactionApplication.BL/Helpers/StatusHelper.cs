using System;
using System.Collections.Generic;
using System.Linq;
using TransactionApplication.BL.Constants;

namespace TransactionApplication.BL.Helpers
{
    public static class StatusHelper
    {
        public static List<string> OutputStatuses = new List<string> { Statuses.OutputD, Statuses.OutputR, Statuses.OutputA };

        public static string GetUnifiedStatus(string transactionStatus)
        {
            var status = transactionStatus.ToLower() switch
            {
                Statuses.Approved => Statuses.OutputA,
                Statuses.Rejected => Statuses.OutputR,
                Statuses.Failed => Statuses.OutputR,
                Statuses.Done => Statuses.OutputD,
                Statuses.Finished => Statuses.OutputD,
                _ => null
            };

            return status;
        }

        public static bool IsAppropriateStatus(string status)
        {
            return OutputStatuses.Any(x => x.Equals(status, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
