using System;
using System.Collections;
using System.Collections.Generic;

using RBush;
using PointClass;

using static Functions.Extentions;


namespace ConsoleApp
{
    class Program
    {
        static void AddSampleData(ref Dictionary<string, RBush<Point>> treeSet)
        {
            List<Point> dataList = new List<Point>
            {
                new Point(1,3),     new Point(10,20),
                new Point(12,19),   new Point(14,16),
                new Point(10,30),   new Point(11,40),
                new Point(9,50),    new Point(19,34),
                new Point(68,69),   new Point(65,70),
                
                new Point(3,15),    new Point(8,16),
                new Point(27,53),   new Point(3,7),
                new Point(1,99),    new Point(90,92),
                new Point(14,15),   new Point(42,54),
            /*        
                new Point(70,81),   new Point(9,85),
                new Point(123,529), new Point(1,700),
                new Point(33,75),   new Point(67,96),
                new Point(31,43),   new Point(11,22),
                new Point(49,50),   new Point(51,82),

                new Point(45,85),   new Point(94,98),
                new Point(63,79),   new Point(83,90),
                new Point(14,75),   new Point(84,96),
                new Point(323,432), new Point(17,289),
                new Point(112,500), new Point(53,62)
            */
            };

            RBush<Point> tree = new (maxEntries: 2);
            tree.BulkLoad(dataList);
            treeSet.Add("test", tree);
        }

        static void Main(string[] args)
        {
            try
            {
                Dictionary<string, RBush<Point>> TreeSet = new();

                AddSampleData(ref TreeSet);       //Uncomment this line to add test collection to the set

                Emulator(ref TreeSet);
            }
            catch(Exception e)
            {
                int line = (new System.Diagnostics.StackTrace(e, true)).GetFrame(0).GetFileLineNumber();
                string fileName = (new System.Diagnostics.StackTrace(e, true)).GetFrame(0).GetFileName();

                Console.WriteLine($"Exception occured!\n{e.GetType()} in file {fileName} at line {line}:\n{e.Message}");
            }
        }
    }
}
