using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xfinium.Pdf;
using Xfinium.Pdf.Content;
using System.IO;
using NUREMarks.Models;

namespace NUREMarks.Services
{

    public class PDFParser
    {
        public List<StudentData> Students = new List<StudentData>();
        public List<Group> Groups = new List<Group>();
        private Group testGroup = new Group();
        private StringBuilder text = new StringBuilder();

        public PDFParser(string PDFPath)
        {
            GetDataFromPDF(PDFPath);
        }

        private void GetDataFromPDF(string path)
        {
            text = new StringBuilder();

            using (var stream = File.Open(path, FileMode.Open))
            {
                PdfFixedDocument doc = new PdfFixedDocument(stream);
                PdfContentExtractionContext ctx = new PdfContentExtractionContext();
                for (int i = 0; i < doc.Pages.Count; i++)
                {
                    PdfContentExtractor ce = new PdfContentExtractor(doc.Pages[i]);
                    text.Append(ce.ExtractText(ctx));
                }
            }

            testGroup.DepShort = ConvertPath(path);
            List<string> dataStudents = FormatParsedText(text.ToString().Split('\n').ToList());
            FillStudentList(dataStudents);
        }

        private List<string> FormatParsedText(List<string> parsedText)
        {
            for (int i = 0; i < parsedText.Count; i++)
            {
                parsedText[i].Replace('`', '\'');
            }

            string[] temp;

            parsedText.RemoveAll(p => p.Length < 5);

            if (parsedText[0].Contains("Рейтинг"))
                parsedText.Insert(0, "Факультет   ");


            testGroup.Course =  Int32.Parse(parsedText[2].Split(' ')[1]);

            if (parsedText[3].Contains("другого рівня"))
                testGroup.Course += 4;


            testGroup.FacultyFull = parsedText[0].Substring(10, parsedText[0].Length - 12);
            testGroup.Department = parsedText[5].Substring(1, parsedText[5].Length - 4);
            testGroup.FacultyShort = GetFacultyAcronym(testGroup.FacultyFull);

            parsedText.RemoveRange(0, 10);
            

            for (int i = 0; i < parsedText.Count; i++)
            {
                int index = CheckInfo(parsedText[i]);

                if (index > 2)
                {
                    temp = parsedText[i].Split(' ');
                    Array.Copy(temp, index, temp, 0, temp.Length - index);
                    Array.Resize(ref temp, temp.Length - index);
                    parsedText.Insert(i + 1, String.Join(" ", temp.Select(p => p.ToString()).ToArray()));

                    temp = parsedText[i].Split(' ');
                    Array.Resize(ref temp, index);
                    parsedText[i] = String.Join(" ", temp.Select(p => p.ToString()).ToArray()) + " \r";
                }
                else if (index == 0)
                {
                    parsedText[i - 1] = parsedText[i - 1].Substring(0, parsedText[i - 1].Length - 2);
                    parsedText[i - 1] += " академ. відпустка \r";
                    parsedText.RemoveRange(i, 2);
                }
                else if (index == 1)
                {
                    List<string> arr = parsedText[i].Split(' ').ToList();
                    arr.Remove(arr.Last());

                    arr.Insert(arr.Count, parsedText[i+1].Substring(0, parsedText[i+1].Length));
                    parsedText[i] = String.Join(" ", arr.Select(p => p.ToString()).ToArray());
                    parsedText.RemoveRange(i+1, 1);
                }
                else if (index == 2)
                {
                    parsedText[i] = parsedText[i - 1].Replace("\r", " ") + parsedText[i];
                    parsedText.RemoveAt(i-- - 1);
                }
            }
            parsedText.RemoveAll(p => p.Length < 5);

            return parsedText;
        }

        private int CheckInfo(string s)
        {
            string[] temp = s.Split(' ');

            if (temp.Length > 15)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] != "")
                        continue;
                    else
                    {
                        for (int j = i + 5; j < temp.Length; j++)
                        {
                            if (temp[j] != "" && temp[j] != "після" &&
                                temp[j] != "терміну" && temp[j] != "контракт")
                                return j;
                        }
                    }
                }
            }

            if (temp.Length < 5 && temp.ToList().IndexOf("академ.") != -1)
                return 0;

            if (temp[0] == "не")
                return 2;

            if (temp.Length < 8)
                return 1;    

            return -1;
        }

        private void FillStudentList(List<string> dataStudents)
        {
            string name, group, info;
            double rating;
            int index;
            List<string> dataStudent;

            for (int i = 0; i < dataStudents.Count; i++)
            {
                dataStudent = dataStudents[i].Split(' ').ToList();
                index = dataStudent.IndexOf("") == -1 ? 3 : dataStudent.IndexOf("");

                dataStudent.RemoveAll(p => p == "");

                name = String.Join(" ", dataStudent.Where(p => Array.IndexOf(dataStudent.ToArray(), p) < index).
                    Select(p => p.ToString()).ToArray()).Replace("`", "'");

                int apos = name.IndexOf('\'');

                if (apos != -1)
                {
                    name = name.Insert(apos + 1, name[apos + 1].ToString().ToLower());
                    name = name.Remove(apos + 2, 1);
                }

                if (dataStudent[index] == "не")
                {
                    rating = 0;
                    index++;
                }
                else
                   rating = Double.Parse(dataStudent[index].Replace('.', ','));


                group = dataStudent[++index];

                if (testGroup.DepShort.Contains("РТу"))
                    group = group.Insert(2, "у");

                if (Groups.Select(g => g.Name).ToList().IndexOf(group) == -1)
                    Groups.Add(new Group
                    {
                        Name = group,
                        Course = testGroup.Course,
                        Department = testGroup.Department,
                        DepShort = testGroup.DepShort,
                        FacultyFull = testGroup.FacultyFull,
                        FacultyShort = testGroup.FacultyShort
                    });

                info = String.Join(" ", dataStudent.Where(p => (Array.IndexOf(dataStudent.ToArray(), p) > index &&
                    Array.IndexOf(dataStudent.ToArray(), p) < dataStudent.Count - 1)).
                    Select(p => p.ToString()).ToArray());

                Students.Add(new StudentData
                {
                    Name = name,
                    Group = group,
                    Rating = rating,
                    Info = info
                });
            }
            
        }

        private string ConvertPath(string path)
        {
            string fileName = path.Split('\\').Last();
            string result = fileName.Substring(0, fileName.Length - 4);

            if (result.IndexOfAny(new char[] { '0', '1', '2', '3', '4',
                '5', '6', '7', '8', '9' }) != -1)
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        private string GetFacultyAcronym(string faculty)
        {
            string acronym = "";
            string[] temp = faculty.Split(' ');

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].Length > 3)
                    acronym += temp[i][0];
            }

            return acronym.ToUpper();
        }
    }
}