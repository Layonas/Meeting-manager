using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingManager.Models;
using MeetingManager.Enums;
using System.Text.Json;

namespace MeetingManager.Controller
{
    internal class MeetingController
    {
        public static string path
        {
            get
            {
                return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "meetings.json");
            }
        }

        public static void createMeeting(ref bool quit, ref bool cancel)
        {
            Console.Clear();

            Console.WriteLine("Write a meeting name:");
            var name = Console.ReadLine();

            Console.WriteLine("Write a responsible person:");
            var rp = Console.ReadLine();

            Console.WriteLine("Write a meeting description:");
            var description = Console.ReadLine();

            Console.WriteLine("Pick a category:");
            var categories = Enum.GetValues(typeof(FixedTypes.Category))
                .Cast<FixedTypes.Category>();

            for (int i = 0; i < categories.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + categories.ElementAt(i).ToString());
            }

            var categoryIndex = Convert.ToInt32(Console.ReadLine());

            var category = categories.ElementAt(categoryIndex - 1);

            Console.WriteLine("Pick a type:");
            var types = Enum.GetValues(typeof(FixedTypes.Type)).Cast<FixedTypes.Type>();
            for (int i = 0; i < types.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + types.ElementAt(i).ToString());
            }

            var typeIndex = Convert.ToInt32(Console.ReadLine());

            var type = types.ElementAt(typeIndex - 1);

            DateTime startDate;
            string pattern = "yyyy-MM-dd HH:mm";

            Console.WriteLine($"Write a starting date for the meeting with format of {pattern}:");
            DateTime.TryParseExact(Console.ReadLine(), pattern, null, System.Globalization.DateTimeStyles.None, out startDate);

            DateTime endDate;
            Console.WriteLine($"Write a ending date for the meeting with format of {pattern}:");
            DateTime.TryParseExact(Console.ReadLine(), pattern, null, System.Globalization.DateTimeStyles.None, out endDate);

            Meeting meeting = new Meeting(name, rp, description, category, type, startDate, endDate);

            saveMeeting(meeting);
        }

        private static void saveMeeting(Meeting meeting)
        {
            var meetings = getMeetings();
            meetings = meetings.Append(meeting);

            using (var writer = File.OpenWrite(path))
            {
                JsonSerializer.Serialize<IEnumerable<Meeting>>(new Utf8JsonWriter(writer, new JsonWriterOptions
                {
                    Indented = true,
                    SkipValidation = true
                }),
                meetings);
            }
        }

        public static void deleteMeeting(string name, Meeting meeting)
        {
            var meetings = getMeetings();

            if (meeting.ResponsiblePerson.CompareTo(name) != 0)
            {
                Console.WriteLine("You cannot delete a meeting you are not responsible for!");
                return;
            }
            meetings = meetings.Where(v => !v.Equals(meeting));

            File.WriteAllText(path, "");

            using (var writer = File.OpenWrite(path))
            {
                JsonSerializer.Serialize<IEnumerable<Meeting>>(new Utf8JsonWriter(writer, new JsonWriterOptions
                {
                    Indented = true,
                    SkipValidation = true
                }),
                meetings);
            }
        }

        public static Meeting? chooseMeeting()
        {
            var meetings = getMeetings();

            if (meetings?.Count() == 0)
            {
                Console.WriteLine("I'm sorry, there are no meetings currently.");
                return null;
            }

            Console.WriteLine("Choose a meeting");

            for (int i = 0; i < meetings?.Count(); i++)
            {
                Console.WriteLine((i + 1) + $":\n{meetings.ElementAt(i)}");
            }

            int index = Convert.ToInt32(Console.ReadLine());

            return meetings?.ElementAt(index - 1);
        }

        public static IEnumerable<Meeting>? getMeetings()
        {
            if (!File.Exists(path))
                return Enumerable.Empty<Meeting>();

            using var reader = File.OpenText(path);
            var json = reader.ReadToEnd();

            if (json.Length < 3)
                return Enumerable.Empty<Meeting>();

            return JsonSerializer.Deserialize<IEnumerable<Meeting>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public static void addAttendee(string name, Meeting meeting, DateTime start)
        {
            var attendees = AttendeeController.getAttendees();

            Attendee checkAttendee = new Attendee(name, Enumerable.Empty<Meeting>().Append(meeting),
                new List<DateTime>() { start });

            if (attendees.Contains(checkAttendee))
            {
                var attendee = attendees.First(a => a.Name.CompareTo(checkAttendee.Name) == 0);
                if (attendee.Meetings.Contains(meeting))
                {
                    Console.WriteLine($"{name} is already in this meeting");
                    return;
                }
                else
                {
                    attendee.Meetings = attendee.Meetings.Append(meeting);
                    //attendee.AttendTime = attendee.AttendTime.Append(start);
                    attendee.AttendTime.Add(start);
                }
            }
            else
            {
                attendees = attendees.Append(checkAttendee);
            }

            AttendeeController.updateAttendees(attendees, FixedTypes.Operation.Add);
        }

        public static void ListMeeting(IEnumerable<Meeting> meetings)
        {
            if (meetings is null || meetings.Count() == 0)
                Console.WriteLine("There are no meetings.");

            for (int i = 0; i < meetings.Count(); i++)
                Console.WriteLine((i + 1) + $":\n{meetings.ElementAt(i)}");
        }
    }
}