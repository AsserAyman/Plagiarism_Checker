using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DocumentDistance
{
    class DocDistance
    {
        private static char[] separators =
    {
            '!', '@', '#', '$', '%', '^', '&', '*', '_', '(', ')', '-', '+', '=', '~', '`',
            '{', '}', '|', '\\', '[', ']', ':', ';', '<', ',', '>', '.', '?', '/', '\'', '"', ' ', '“', '”'
        };
        public static string[] doc1 = new string[10000000];
        public static string[] doc2 = new string[10000000];
        static Dictionary<string, long> combined;
        public static void CustomReadFile1(string docFilePath, ref long i)
        {

            using (StreamReader sr = File.OpenText(docFilePath))
            {
                string s = String.Empty;
                while (!sr.EndOfStream)
                {
                    //                    Regex.Split(sr.ReadLine(), Regex.)
                    string[] formatted = sr.ReadLine().Split(separators);
                    for (long k = 0; k < formatted.Length; k++)
                    {
                        if (formatted[k] != "\n")
                            doc1[i++] = formatted[k].ToLower();
                    }

                }
            }
        }
        public static void CustomReadFile2(string docFilePath, ref long i)
        {

            //            string[] doc = new string[1000000];
            using (StreamReader sr = File.OpenText(docFilePath))
            {
                while (!sr.EndOfStream)
                {
                    string[] formatted = sr.ReadLine().Split(separators);
                    for (long k = 0; k < formatted.Length; k++)
                        doc2[i++] = formatted[k].ToLower();
                }
            }

        }

        static Object lockObject = new object();
        public static Dictionary<string, long> CustomReadFile(string docFilePath)
        {

            Dictionary<string, long> ret = new Dictionary<string, long>();
            foreach (string line in File.ReadLines(docFilePath))
            {
                //                string[] formatted = Regex.Split(line,"[^a-zA-Z0-9]");
                //                string[] formatted = Regex.Split(line,"[^a-zA-Z0-9]");
                //                Console.WriteLine(line);
                int old = 0;
                int cnt = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    char test = line[i];
                    if (!(test >= 'a' && test <= 'z') && !(test <= 'Z' && test >= 'A') && !(test <= '9' && test >= '0'))
                    {
                        //                        Console.WriteLine("here");
                        string temp = line.Substring(old, cnt);
                        //                        Console.WriteLine(temp);
                        old = i + 1;
                        cnt = 0;
                        temp = temp.ToLower();
                        if (ret.ContainsKey(temp))
                            ret[temp]++;
                        else
                        {
                            if (temp != String.Empty)
                                ret.Add(temp, 1);
                        }
                    }
                    else
                        cnt++;
                }
                string last = line.Substring(old, cnt);
                last = last.ToLower();
                if (ret.ContainsKey(last))
                    ret[last]++;
                else
                {
                    if (last != String.Empty)
                        ret.Add(last, 1);
                }

                //                string[] formatted = line.Split(separators);
                //                for (long k = 0; k < formatted.Length; k++)
                //                {
                //                    string temp = formatted[k].ToLower();
                //                    if (ret.ContainsKey(temp)) 
                //                        ret[temp]++;
                //                    else
                //                    {
                //                        if(temp!= String.Empty)
                //                            ret.Add(temp, 1);
                //                    }
                //
                ////                    lock (lockObject)
                ////                    {
                ////                        if (combined.ContainsKey(temp)) 
                ////                            combined[temp]++;
                ////                        else
                ////                        {
                ////                            if(temp!= String.Empty)
                ////                                combined.Add(temp, 1);
                ////                        }
                ////                    }
                ////                    
                //                }   
            }
            return ret;
        }



        public static double CalculateDistance(string doc1FilePath, string doc2FilePath)
        {   if (doc1FilePath == doc2FilePath)
   
                return 0;
            // TODO comment the following line THEN fill your code here
            //throw new NotImplementedException();

            //            combined = new Dictionary<string, long>();

            Task<Dictionary<string, long>>[] taskArr =
            {
                Task<Dictionary<string,long>>.Factory.StartNew(()=> CustomReadFile(doc1FilePath)),
                Task<Dictionary<string,long>>.Factory.StartNew(()=> CustomReadFile(doc2FilePath))
            };

            long up = 0, down1 = 0, down2 = 0;
            double down, ans;

            Task.WaitAll(taskArr);
            //            return 1.1;
            //            Console.WriteLine(taskArr[0].Result.Count + " : " + taskArr[1].Result.Count);



            //            foreach (KeyValuePair<string,long> VARIABLE in taskArr[0].Result)
            //            {
            //                Console.WriteLine(VARIABLE.Key + " : " + taskArr[0].Result[VARIABLE.Key]);
            //            }
            //            Console.WriteLine("------");
            //            foreach (var VARIABLE in taskArr[1].Result)
            //            {
            //                Console.WriteLine(VARIABLE);
            //            }


            //            foreach (KeyValuePair<string,long> i in combined)
            //            {
            //                long a = taskArr[0].Result.ContainsKey(i.Key) ? taskArr[0].Result[i.Key] : 0;
            //                long b = taskArr[1].Result.ContainsKey(i.Key) ? taskArr[1].Result[i.Key] : 0;
            //                up += (a * b);
            //
            //                down1 += (a * a);
            //                down2 += (b * b);
            //            }
            Task taskA = Task.Run(() =>
            {
                foreach (KeyValuePair<string, long> i in taskArr[0].Result)
                {
                    //                    Console.WriteLine(i);
                    long a = i.Value;
                    long b = taskArr[1].Result.ContainsKey(i.Key) ? taskArr[1].Result[i.Key] : 0;
                    up = up + (a * b);
                    down1 = down1 + (a * a);
                    //                down2 = down2 + (b * b);
                }
            });

            Task taskB = Task.Run(() =>
            {
                foreach (var i in taskArr[1].Result)
                {
                    //                    Console.WriteLine(i);
                    down2 += i.Value * i.Value;
                }
            });
            Task.WaitAll(taskA, taskB);
            //            Console.WriteLine(down1 + " * " + down2);
            down = Math.Sqrt(down1) * Math.Sqrt(down2);
            //            double down = Math.Sqrt(down1 * down2);
            ans = Math.Acos(Math.Min(up / down, 1.0));
            //            Console.WriteLine((double)up + "/" + down);
            //            Console.WriteLine(ans);
            //            Console.WriteLine(up/down);

            return ans * 180 / Math.PI;



            //            Dictionary<string, long> dic1 = CustomReadFile(doc1FilePath);
            //            Dictionary<string, long> dic2 = CustomReadFile(doc2FilePath);
            //            Console.WriteLine(dic1.Count + " : " + dic2.Count);

            //            long f = 0, s = 0;
            //            Task[] taskArr =
            //            {
            //                Task.Factory.StartNew(()=> CustomReadFile1(doc1FilePath,ref f)),
            //                Task.Factory.StartNew(()=> CustomReadFile2(doc2FilePath,ref s))
            //            };
            //            


            //            Console.WriteLine(f + " : " + s);





            //            string[] doc1 = new string[1000000];
            //            string[] doc2 = new string[1000000];
            //            long i = 0;
            //            using (StreamReader sr = File.OpenText(doc1FilePath))
            //            {
            //                while (!sr.EndOfStream)
            //                {
            //                    string [] formatted = sr.ReadLine().Split(separators);
            //                    for(long k = 0; k < formatted.Length; k++)
            //                        doc1[i++] = formatted[k].ToLower();
            //                }
            //            }
            //            long j = 0;
            //            using (StreamReader sr = File.OpenText(doc2FilePath))
            //            {
            //                while (!sr.EndOfStream)
            //                {
            //                    string [] formatted = sr.ReadLine().Split(separators);
            //                    for(long k = 0; k < formatted.Length; k++)
            //                        doc2[j++] = formatted[k].ToLower();
            //                }
            //            }
            //            Console.WriteLine(i + " : "  + j);
            return 1.1;
        }
    }
}