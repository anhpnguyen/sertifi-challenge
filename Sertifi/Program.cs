using Newtonsoft.Json;
using Sertifi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Sertifi
{
    class Program
    {
        /// <summary>
        /// Anh Mike Nguyen's coding challenge for Sertifi
        /// </summary>
        /// <param name="args"></param>

        private const string uri = "http://apitest.sertifi.net/api";

        static void Main(string[] args)
        {
            var students = GetStudents();
            if (students.Length == 0) Console.WriteLine("No student retrieved.");
            else
            {
                StudentOutput output = new StudentOutput();
                output.YourEmail = "anhpnguyen.an@gmail.com";
                output.YourName = "Anh Mike Nguyen";
                output.YearWithHighestAttendance = GetHighestAttendance(students);
                output.YearWithHighestOverallGpa = GetYearWithHighestGPA(students);
                output.Top10StudentIdsWithHighestGpa = GetTopStudents(students);
                output.StudentIdMostInconsistent = GetIDWithLargestDiff(students);
                PrettyPrint(output);
                var result = Put(output);
                if (result) Console.WriteLine("POST successful!");
                else Console.WriteLine("POST failed :(");
            }         
            Console.ReadLine();
        }

        private static void PrettyPrint(StudentOutput output)
        {
            Console.WriteLine("The year which saw the highest attendance: " + output.YearWithHighestAttendance);
            Console.WriteLine("The year with highest overall GPA: " + output.YearWithHighestOverallGpa);
            Console.WriteLine("Top 10 students with highest overall GPA");
            foreach(var id in output.Top10StudentIdsWithHighestGpa)
            {
                Console.WriteLine(id);
            }
            Console.WriteLine("Student with the largest difference between their minimum and maximum GPA: " + output.StudentIdMostInconsistent);
        }

        /// <summary>
        /// PUT request to submit the result
        /// </summary>
        /// <param name="output"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        static bool Put(StudentOutput output)
        {
            var json = JsonConvert.SerializeObject(output);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri + "/StudentAggregate");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    return true;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }   
        }

        /// <summary>
        /// GET request to get the students
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static Student[] GetStudents()
        {
            Student[] students;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + "/Students");
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stringResponse = reader.ReadToEnd();
                        students = JsonConvert.DeserializeObject<Student[]>(stringResponse);
                        return students;
                    }
                    return null;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Increment count of each year in range(start year, end year) then return the kvp with the highest count
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        static int GetHighestAttendance(Student[] students)
        {
            var yearCount = new Dictionary<int, int>();
            foreach(var s in students)
            {
                if(s.StartYear == s.EndYear)
                {
                    if (yearCount.ContainsKey(s.StartYear)) yearCount[s.StartYear]++;
                    else yearCount.Add(s.StartYear, 1);
                }
                else
                {
                    for (int i = s.StartYear; i <= s.EndYear; i++)
                    {
                        if (yearCount.ContainsKey(i)) yearCount[i]++;
                        else yearCount.Add(i, 1);
                    }
                }        
            }
            var sortedPair = (from pair in yearCount orderby pair.Value descending, pair.Key descending select pair).FirstOrDefault();
            return sortedPair.Key;
        }

        /// <summary>
        /// Get year with highest overall GPA
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        static int GetYearWithHighestGPA(Student[] students)
        {
            var year = (from student in students orderby student.OverallGPA descending select student.EndYear).FirstOrDefault();
            
            return year;
        }

        /// <summary>
        /// Calculate year based on index from gpa array
        /// </summary>
        /// <param name="student"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        private static int GetYearFromGPA(Student student, float high)
        {
            int offset = Array.IndexOf(student.GPARecord, high);
            int year = student.StartYear + offset;
            return year;
        }

        /// <summary>
        /// Returns top 10 students with highest overall GPA
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        static int[] GetTopStudents(Student[] students)
        {
            var topStudents = (from student in students orderby student.OverallGPA descending select student.Id).Take(10).ToArray();
            return topStudents;
        }

        /// <summary>
        /// Get student with largest GPA different by subtracting max gpa from min gpa. Return student id.
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        static int GetIDWithLargestDiff(Student[] students)
        {
            int currId = 0;
            float currMax = 0;
            foreach(var student in students)
            {
                float diff = student.GPARecord.Max() - student.GPARecord.Min();
                if(diff > currMax)
                {
                    currMax = diff;
                    currId = student.Id;
                }
            }
            return currId;
        }
    }
}
