using System;

namespace Sertifi.Models
{
    [Serializable]
    public class StudentOutput
    {
        public string YourName { get; set; }
        public string YourEmail { get; set; }
        public int YearWithHighestAttendance { get; set; }
        public int YearWithHighestOverallGpa { get; set; }
        public int[] Top10StudentIdsWithHighestGpa { get; set; }
        public int StudentIdMostInconsistent { get; set; }
    }

}
