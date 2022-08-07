using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingManager.Exceptions;

namespace MeetingManager.Utils
{
    public class Utils
    {
        public static string login()
        {
            string? userName = null;

            while (userName is null || userName.Length == 0)
            {
                Console.Clear();
                Console.WriteLine("Enter your name:");
                userName = Console.ReadLine();
            }

            return userName;
        }

        public static int action(int min, int max, ref bool quit)
        {
            int action = -1;

            while (action == -1)
            {
                try
                {
                    string input = Console.ReadLine();
                    if (input.CompareTo("quit") == 0)
                    {
                        quit = true;
                        return -1;
                    }
                    else
                    {
                        action = Convert.ToInt32(input);
                        if (action < min || action > max)
                        {
                            action = -1;
                            throw (new ActionOutOfRangeException("Please enter a valid action number."));
                        }
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please choose a valid action!");
                }
                catch (ActionOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return action;
        }

        public static int action(int min, int max, ref bool quit, ref bool cancel)
        {
            int action = -1;

            while (action == -1)
            {
                try
                {
                    string input = Console.ReadLine();
                    if (input.CompareTo("quit") == 0)
                    {
                        quit = true;
                        return -1;
                    }
                    else if (input.CompareTo("cancel") == 0)
                    {
                        cancel = true;
                        return -1;
                    }
                    else
                    {
                        action = Convert.ToInt32(input);
                        if (action < min || action > max)
                        {
                            action = -1;
                            throw (new ActionOutOfRangeException("Please enter a valid action number."));
                        }
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please choose a valid action!");
                }
                catch (ActionOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return action;
        }

        public static DateTime readDate(string pattern, ref bool quit)
        {
            DateTime date = new DateTime();
            bool flag = false;
            while (!flag)
            {
                try
                {
                    string input = Console.ReadLine();
                    if (input.CompareTo("quit") == 0)
                    {
                        quit = true;
                        return date;
                    }
                    DateTime.TryParseExact(input, pattern, null, System.Globalization.DateTimeStyles.None, out date);
                    flag = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Incorrect date entered, follow the pattern.");
                }
            }

            return date;
        }
    }
}