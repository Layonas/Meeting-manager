using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingManager.Models;
using MeetingManager.Enums;
using MeetingManager.Utils;
using System.Text.Json;

namespace MeetingManager.Controller
{
    public class MeetingController
    {
        public static string path
        {
            get
            {
                return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "meetings.json");
            }
        }

        public static Meeting createMeeting(ref bool quit, ref bool cancel)
        {
            Console.WriteLine("Write a meeting name:");
            var name = Console.ReadLine();
            cancel = name.CompareTo("cancel") == 0 ? true : false;
            quit = name.CompareTo("quit") == 0 ? true : false;
            if (quit)
                return new Meeting();
            if (cancel)
                return new Meeting();

            Console.WriteLine("Write a responsible person:");
            var rp = Console.ReadLine();
            quit = rp.CompareTo("quit") == 0 ? true : false;
            if (quit)
                return new Meeting();

            Console.WriteLine("Write a meeting description:");
            var description = Console.ReadLine();
            quit = description.CompareTo("quit") == 0 ? true : false;
            if (quit)
                return new Meeting();

            Console.WriteLine("Pick a category:");
            var categories = Enum.GetValues(typeof(FixedTypes.Category))
                .Cast<FixedTypes.Category>();

            for (int i = 0; i < categories.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + categories.ElementAt(i).ToString());
            }

            var categoryIndex = Utils.Utils.action(1, categories.Count(), ref quit);
            if (quit)
                return new Meeting();

            var category = categories.ElementAt(categoryIndex - 1);

            Console.WriteLine("Pick a type:");
            var types = Enum.GetValues(typeof(FixedTypes.Type)).Cast<FixedTypes.Type>();
            for (int i = 0; i < types.Count(); i++)
            {
                Console.WriteLine((i + 1) + ": " + types.ElementAt(i).ToString());
            }

            var typeIndex = Utils.Utils.action(1, types.Count(), ref quit);
            if (quit)
                return new Meeting();

            var type = types.ElementAt(typeIndex - 1);

            string pattern = "yyyy-MM-dd HH:mm";

            Console.WriteLine($"Write a starting date for the meeting with format of {pattern}:");
            DateTime startDate = Utils.Utils.readDate(pattern, ref quit);
            if (quit)
                return new Meeting();

            Console.WriteLine($"Write a ending date for the meeting with format of {pattern}:");
            DateTime endDate = Utils.Utils.readDate(pattern, ref quit);
            if (quit)
                return new Meeting();

            return new Meeting(name, rp, description, category, type, startDate, endDate);
        }

        public static IEnumerable<Meeting> saveMeeting(Meeting meeting)
        {
            var meetings = getMeetings();
            meetings = meetings.Append(meeting);

            return meetings;
        }

        public static IEnumerable<Meeting> removeMeeting(string name, Meeting meeting)
        {
            var meetings = getMeetings();

            if (meeting.ResponsiblePerson.CompareTo(name) != 0)
            {
                Console.WriteLine("You cannot delete a meeting you are not responsible for!");
                return Enumerable.Empty<Meeting>();
            }
            meetings = meetings.Where(v => !v.Equals(meeting));

            return meetings;
        }

        public static void updateMeetings(IEnumerable<Meeting> meetings, FixedTypes.Operation operation)
        {
            if (operation == FixedTypes.Operation.Delete)
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

        public static Meeting? chooseMeeting(ref bool quit, ref bool cancel)
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

            int index = Utils.Utils.action(1, meetings.Count(), ref quit, ref cancel);
            if (quit || cancel)
                return null;

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

        public static IEnumerable<Attendee> addAttendee(string name, Meeting meeting, DateTime start)
        {
            var attendees = AttendeeController.getAttendees();

            Attendee checkAttendee = new Attendee(name, Enumerable.Empty<Meeting>().Append(meeting),
                new List<DateTime>() { start });

            if (attendees.Contains(checkAttendee))
            {
                var attendee = attendees.First(a => a.Name.CompareTo(checkAttendee.Name) == 0);
                if (attendee.Meetings.Contains(meeting))
                {
                    Console.WriteLine($"{name} is already in this meeting.");
                    return Enumerable.Empty<Attendee>();
                }
                else if (attendee.Meetings.Any(m => m.StartDate <= meeting.StartDate && m.EndDate >= meeting.StartDate
                || m.StartDate <= meeting.StartDate && m.EndDate <= meeting.EndDate && m.EndDate >= meeting.StartDate
                || m.StartDate <= meeting.EndDate && m.EndDate >= meeting.EndDate
                || m.StartDate >= meeting.StartDate && m.EndDate <= meeting.EndDate))
                {
                    Console.WriteLine($"{name} has an intersecting meeting.");
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

            return attendees;
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