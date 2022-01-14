using System;
using System.Linq;
using System.Collections.Generic;

using static System.Console;

using RBush;
using PointClass;

using static ParseUtilities.StringExtensions;
using static ParseUtilities.Extensions;
using static ParseUtilities.Extensions.ParseStatus;

namespace Functions
{
    public class Extentions
    {
        static class Constants
        {
            public const int maxEntries = 2;
        }

        public static void Emulator(ref Dictionary<string, RBush<Point>> TreeSet)
        {
            WriteLine("Proceed with looking through command menu.\nType: 'menu;'");

            string cmdStr = string.Empty;
            List<string> cmdWordsList = new();

            while(true)
            {
                cmdWordsList.Clear();

                getCommandStr(out cmdStr);

                ParseStatus checkStatus = CheckSyntax(cmdStr);

                if(checkStatus.HasFlag(FAIL))
                {
                    WriteLine("Failed to execute because of incorrect syntax.");
                    continue;
                }
                else if(checkStatus.HasFlag(UNKNOWN))
                {
                    WriteLine($"No command as entered was found.");
                    continue;
                }


                cmdStr = cmdStr.Remove(cmdStr.IndexOf(';'));
                splitCommandToWordsList(ref cmdStr, ref cmdWordsList);
                
                switch(cmdWordsList[0].ToUpper())
                {
                    case "MENU":
                    {
                        string menuTemplate =     "- CREATE tree_name                      -\n"
                                                + "- INSERT tree_name [x,y]                -\n"
                                                + "- PRINT_TREE tree_name                  -\n"
                                                + "- CONTAINS tree_name [x,y]              -\n"
                                                + "- SEARCH tree_name [WHERE query]        -\n"
                                                + "-        query:=                        -\n"
                                                + "-               CONTAINS [x,y]          -\n"
                                                + "-               INTERSECTS [x,y]        -\n"
                                                + "-               LEFT_OF x               -\n"
                                                + "- LIST_TREES                            -\n"
                                                + "- - - - - - - - - - - - - - - - - - - - -\n"
                                                + "- MENU                                  -\n"
                                                + "- CLEAR                                 -\n"
                                                + "- QUIT                                  -\n";
                        Write(menuTemplate);
                        break;
                    }

                    case "CREATE":
                    {
                        string treeName = cmdWordsList[1];
                        if (!TreeSet.ContainsKey(treeName))
                        {
                            RBush<Point> tree = new (maxEntries: Constants.maxEntries);
                            TreeSet.Add(treeName, tree);
                            WriteLine($"Tree \"{treeName}\" added to entire collection.");
                        }
                        else
                        {
                            WriteLine($"Tree \"{treeName}\" exists. Choose other name.");
                        }
                        break;
                    }

                    case "INSERT":
                    {
                        string treeName = cmdWordsList[1];
                        string section = cmdWordsList[2];
                        
                        if (TreeSet.ContainsKey(treeName))
                        {
                            (int min, int max) = section.ParseSection();
                            if(min >= max) 
                            {
                                WriteLine($"Incorrect section's limits where min >= max.");
                                break;
                            }
                            else if(TreeSet[treeName].Contains(new Envelope(min, max)))
                            {
                                WriteLine($"Section [{min},{max}] already exists in the collection \"{treeName}\".");
                                break;
                            }
                            TreeSet[treeName].Insert(new Point(min, max));
                            WriteLine($"Section [{min},{max}] added into the collection \"{treeName}\".");
                        }
                        else
                        {
                            WriteLine($"Collection \"{treeName}\" does not exist.");
                        }
                        break;
                    }

                    case "PRINT_TREE":
                    {
                        string treeName = cmdWordsList[1];
                        if (TreeSet.ContainsKey(treeName))
                        {
                            WriteLine($"Printing tree for \"{treeName}\" collection: ");
                            WriteLine(TreeSet[treeName].ToString());
                        }
                        else
                        {
                            WriteLine($"Collection \"{treeName}\" does not exist.");
                        }
                        break;
                    }
                    
                    case "CONTAINS":
                    {
                        string treeName = cmdWordsList[1];
                        string section = cmdWordsList[2];
                        
                        if (TreeSet.ContainsKey(treeName))
                        {
                            
                            (int min, int max) = section.ParseSection();
                            if(min >= max) 
                            {
                                WriteLine($"Incorrect section's limits where min >= max.");
                                break;
                            }
                            else if(TreeSet[treeName].Contains(new Envelope(min, max)))
                            {
                                WriteLine($"Section [{min},{max}] is contained in the collection \"{treeName}\".");
                                break;
                            }
                            else
                            {
                                WriteLine($"Section [{min},{max}] is not contained in the collection \"{treeName}\".");
                            }
                        }
                        else
                        {
                            WriteLine($"Collection \"{treeName}\" does not exist.");
                        }
                        break;
                    }
                    
                    case "SEARCH":
                    {
                        string treeName = cmdWordsList[1];
                        if(TreeSet.ContainsKey(treeName))
                        {
                            if(checkStatus.HasFlag(SEARCH_NORMAL))
                            {
                                var results = TreeSet[treeName].Search();
                                WriteLine($"Contents of {treeName}");
                                foreach(var result in results)
                                {
                                    Write(result.ToString() + "  ");
                                }
                                if(results.Count() == 0)
                                {
                                    WriteLine("Collection is empty.");
                                }
                                else
                                {
                                    WriteLine();
                                }
                            }
                            else if(checkStatus.HasFlag(SEARCH_CONTAINS) || checkStatus.HasFlag(SEARCH_INTERSECTS))
                            {
                                string section = cmdWordsList[4];
                                (int min, int max) = section.ParseSection();

                                if(max > min)
                                {
                                    IReadOnlyList<ISpatialData> results;
                                    if(checkStatus.HasFlag(SEARCH_CONTAINS))
                                    {
                                        results = TreeSet[treeName].SubsetOf(new Envelope(min, max));
                                    }
                                    else
                                    {
                                        results = TreeSet[treeName].Search(new Envelope(min, max));
                                    }
                                    WriteLine($"Search result for section [{min},{max}] in {treeName}");
                                    foreach(var result in results)
                                    {
                                        Write(result.ToString() + "  ");
                                    }
                                    if(results.Count() == 0)
                                    {
                                        WriteLine("Nothing found.");
                                    }
                                    else
                                    {
                                        WriteLine();
                                    }
                                }
                                else
                                {
                                    WriteLine("Incorrect section limits. Eg.: [x,y] where x > y.");
                                }
                            }
                            else if(checkStatus.HasFlag(SEARCH_LEFT_OF))
                            {
                                int limit = int.Parse(cmdWordsList[4]);
                                if(limit >= 1)
                                {
                                    var results = TreeSet[treeName].SubsetOf(new Envelope(0, limit));

                                    WriteLine($"Search result for section [0,{limit}] in {treeName}");
                                    foreach(var result in results)
                                    {
                                        Write(result.ToString() + "  ");
                                    }
                                    if(results.Count() == 0)
                                    {
                                        WriteLine("Nothing found.");
                                    }
                                    else
                                    {
                                        WriteLine();
                                    }
                                }
                                else
                                {
                                    WriteLine("Upper limit must be a positive integer.");
                                }
                            }
                            else throw new ApplicationException("SEARCH command did not get a correct flag.");
                        }
                        else
                        {
                            WriteLine($"Collection \"{treeName}\" does not exist.");
                        }
                        break;
                    }

                    case "LIST_TREES":
                    {
                        var tNames = TreeSet.Keys.ToArray();

                        foreach(string tName in tNames)
                            Write($"{tName}\n");
                        
                        if(tNames.Count() == 0)
                            WriteLine("No collections exist so far.");
                        else
                            WriteLine();

                        break;
                    }

                    case "CLEAR":
                    {
                        Clear();
                        break;
                    }

                    case "QUIT":
                    {
                        return;
                    }
                }
            }
        }

    }
}
