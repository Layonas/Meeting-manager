using MeetingManager.Enums;
using MeetingManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeetingManager.Controller
{
    internal class FilteringController
    {
        private static List<string> filterOptions = new List<string>() { "description", "responsible person",
            "category", "type", "dates", "number of attendees"};

        public static FixedTypes.Filter chooseFilter()
        {
            var filters = Enum.GetValues(typeof(FixedTypes.Filter)).Cast<FixedTypes.Filter>();

            for (int i = 0; i < filters.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + filters.ElementAt(i).ToString());
            }

            var filterIndex = Convert.ToInt32(Console.ReadLine());

            return filters.ElementAt(filterIndex - 1);
        }

        public static IEnumerable<Meeting> applyFiltering(FixedTypes.Filter filter)
        {
            var filteredMeetings = Enumerable.Empty<Meeting>();
            switch (filter)
            {
                case FixedTypes.Filter.Description:
                    Console.WriteLine("Write description to filter by:");
                    var description = Console.ReadLine();
                    filteredMeetings = filterByDescription(description);
                    break;

                case FixedTypes.Filter.ResponsiblePerson:
                    filteredMeetings = filterByResposiblePerson();
                    break;

                case FixedTypes.Filter.Category:
                    filteredMeetings = filterByCategory();
                    break;

                case FixedTypes.Filter.Type:
                    filteredMeetings = filterByType();
                    break;

                case FixedTypes.Filter.Dates:
                    filteredMeetings = filterByDates();
                    break;

                case FixedTypes.Filter.Attendees:
                    filteredMeetings = filterByAttendees();
                    break;
            }

            return filteredMeetings;
        }

        private static IEnumerable<Meeting> filterByDescription(string input)
        {
            var meetings = MeetingController.getMeetings();

            meetings = meetings.Where(m => m.Description.ToLower().Contains(input.ToLower()));

            return meetings;
        }

        private static IEnumerable<Meeting> filterByResposiblePerson()
        {
            var meetings = MeetingController.getMeetings();

            Console.WriteLine("Choose the person to filter by:");
            var person = AttendeeController.chooseAttendee();

            meetings = meetings.Where(m => m.ResponsiblePerson.CompareTo(person.Name) == 0);

            return meetings;
        }

        private static IEnumerable<Meeting> filterByCategory()
        {
            var categories = Enum.GetValues(typeof(FixedTypes.Category)).Cast<FixedTypes.Category>();

            Console.WriteLine("Choose a category to filter:");

            for (int i = 0; i < categories.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + categories.ElementAt(i).ToString());
            }

            var categoryIndex = Convert.ToInt32(Console.ReadLine());

            return MeetingController.getMeetings().Where(m => m.Category == categories.ElementAt(categoryIndex - 1));
        }

        private static IEnumerable<Meeting> filterByType()
        {
            var types = Enum.GetValues(typeof(FixedTypes.Type)).Cast<FixedTypes.Type>();

            Console.WriteLine("Choose a type to filter:");

            for (int i = 0; i < types.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + types.ElementAt(i).ToString());
            }

            var categoryIndex = Convert.ToInt32(Console.ReadLine());

            return MeetingController.getMeetings().Where(m => m.Type == types.ElementAt(categoryIndex - 1));
        }

        private static IEnumerable<Meeting> filterByDates()
        {
            var pattern = "yyyy-MM-dd";
            Console.WriteLine($"Date format should be: {pattern}");
            Console.WriteLine("Enter starting date: ");
            var startDateString = Console.ReadLine();
            Console.WriteLine("Enter ending date: ");
            var endDateString = Console.ReadLine();
            DateTime startDate, endDate;
            bool start = false, end = false;

            if (startDateString is null || startDateString.Length != 0)
                start = true;
            if (endDateString is null || endDateString.Length != 0)
                end = true;

            var meetings = MeetingController.getMeetings();

            if (start && end)
            {
                DateTime.TryParseExact(startDateString, pattern, null, System.Globalization.DateTimeStyles.None, out startDate);
                DateTime.TryParseExact(endDateString, pattern, null, System.Globalization.DateTimeStyles.None, out endDate);

                meetings = meetings.Where(m => m.StartDate.CompareTo(startDate) >= 0 && m.EndDate.CompareTo(endDate) <= 0);
            }
            else if (start)
            {
                DateTime.TryParseExact(startDateString, pattern, null, System.Globalization.DateTimeStyles.None, out startDate);
                DateTime.TryParseExact(endDateString, pattern, null, System.Globalization.DateTimeStyles.None, out endDate);

                meetings = meetings.Where(m => m.StartDate.CompareTo(startDate) >= 0);
            }
            else if (end)
            {
                DateTime.TryParseExact(startDateString, pattern, null, System.Globalization.DateTimeStyles.None, out startDate);
                DateTime.TryParseExact(endDateString, pattern, null, System.Globalization.DateTimeStyles.None, out endDate);

                meetings = meetings.Where(m => m.EndDate.CompareTo(endDate) <= 0);
            }

            return meetings;
        }

        private static IEnumerable<Meeting> filterByAttendees()
        {
            string test = ">3?<7";
            Console.WriteLine("Enter expression to filter by attendee count:");
            Console.WriteLine("Usage [<>num?<num]");
            Console.WriteLine("Example: >3?<7 - meaning more than 3 but less than 7");
            Console.WriteLine("Example: <4 - meaning less than 4\n");

            var input = Console.ReadLine();

            Regex pattern = new Regex(@"^(?<left>[<>])(?<leftNumber>\d+)(\?(?<right><)(?<rightNumber>\d+)$)?");

            Match match = pattern.Match(input);
            string left = match.Groups["left"].Value;
            string leftNumberString = match.Groups["leftNumber"].Value;
            string right = match.Groups["right"].Value;
            string rightNumberString = match.Groups["rightNumber"].Value;

            if (!match.Success)
            {
                Console.WriteLine("Incorrect format.");
                return Enumerable.Empty<Meeting>();
            }

            var meetings = MeetingController.getMeetings();
            var attendees = AttendeeController.getAttendees();
            Dictionary<Meeting, int> pairs = new Dictionary<Meeting, int>();

            foreach (var m in meetings)
                pairs[m] = 0;

            foreach (var a in attendees)
            {
                foreach (var m in a.Meetings)
                {
                    foreach (var meeting in meetings)
                        if (meeting.Equals(m))
                            pairs[meeting] = pairs[meeting] + 1;
                }
            }

            var filtered = Enumerable.Empty<Meeting>();

            if (left.Length > 0 && right.Length > 0)
            {
                if (left.CompareTo("<") == 0)
                {
                    Console.WriteLine("Incorrect format.");
                    return Enumerable.Empty<Meeting>();
                }

                int leftNumber = Convert.ToInt32(leftNumberString);
                int rightNumber = Convert.ToInt32(rightNumberString);

                foreach (var entry in pairs)
                    if (entry.Value > leftNumber && entry.Value < rightNumber)
                        filtered = filtered.Append(entry.Key);
            }
            else if (left.Length > 0)
            {
                if (left.CompareTo(">") == 0)
                {
                    int leftNumber = Convert.ToInt32(leftNumberString);

                    foreach (var entry in pairs)
                        if (entry.Value > leftNumber)
                        {
                            filtered = filtered.Append(entry.Key);
                        }
                }
                else
                {
                    int leftNumber = Convert.ToInt32(leftNumberString);

                    Console.WriteLine("here");
                    foreach (var entry in pairs)
                        if (entry.Value < leftNumber)
                            filtered = filtered.Append(entry.Key);
                }
            }

            return filtered;
        }
    }
}