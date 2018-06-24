using System.Linq;

namespace Sertifi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public float[] GPARecord { get; set; }
        public float OverallGPA {
            get
            {
                return GPARecord.Max();
            }
        }
    }
  
}
