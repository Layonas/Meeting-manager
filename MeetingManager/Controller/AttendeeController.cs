using MeetingManager.Enums;
using MeetingManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeetingManager.Utils;

namespace MeetingManager.Controller
{
    public class AttendeeController
    {
        public static string path
        {
            get
            {
                return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "attendees.json");
            }
        }

        public static IEnumerable<Attendee>? getAttendees()
        {
            if (!File.Exists(path))
                return Enumerable.Empty<Attendee>();

            var reader = File.OpenText(path);

            var json = reader.ReadToEnd();

            reader.Close();

            if (json.Length < 3)
                return Enumerable.Empty<Attendee>();

            return JsonSerializer.Deserialize<IEnumerable<Attendee>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public static void updateAttendees(IEnumerable<Attendee> attendees, FixedTypes.Operation operation)
        {
            if (operation.Equals(FixedTypes.Operation.Delete))
                File.WriteAllText(path, "");

            var writer = File.OpenWrite(path);

            JsonSerializer.Serialize<IEnumerable<Attendee>>(new Utf8JsonWriter(writer, new JsonWriterOptions
            {
                Indented = true,
                SkipValidation = true
            }), attendees);

            writer.Close();
        }

        public static Attendee? chooseAttendee(ref bool quit, ref bool cancel)
        {
            var attendees = getAttendees();

            if (attendees.Count() == 0)
                return null;

            for (int i = 0; i < attendees.Count(); i++)
            {
                Console.WriteLine((i + 1) + $": {attendees.ElementAt(i).Name}");
            }

            int index = Utils.Utils.action(1, attendees.Count(), ref quit, ref cancel);
            if (quit || cancel)
                return null;

            return attendees.ElementAt(index - 1);
        }

        public static Meeting? chooseAttendeeMeeting(Attendee attendee, ref bool quit)
        {
            if (attendee.Meetings.Count() == 0)
                return null;

            for (int i = 0; i < attendee.Meetings.Count(); i++)
            {
                Console.WriteLine((i + 1) + $":\n{attendee.Meetings.ElementAt(i)}");
            }

            int index = Utils.Utils.action(1, attendee.Meetings.Count(), ref quit);

            if (quit)
                return null;

            return attendee.Meetings.ElementAt(index - 1);
        }

        public static Attendee removeMeeting(Attendee attendee, Meeting meeting, FixedTypes.AttendeeAction action)
        {
            if (meeting.ResponsiblePerson.CompareTo(attendee.Name) == 0 && action == FixedTypes.AttendeeAction.AttendeeRemove)
            {
                Console.WriteLine($"Cannot remove the attendee from the meeting he is responsible!");
                return new Attendee();
            }
            int index = -1;
            for (int i = 0; i < attendee.Meetings.Count(); i++)
                index = attendee.Meetings.ElementAt(i).Equals(meeting) ? i : -1;

            attendee.Meetings = attendee.Meetings.Where(m => !m.Equals(meeting));
            attendee.AttendTime.RemoveAt(index);

            return attendee;
        }

        public static IEnumerable<Attendee> updateAttendee(Attendee attendee)
        {
            var attendees = getAttendees();

            attendees.First(a => a.Equals(attendee)).Meetings = attendee.Meetings;
            attendees.First(a => a.Equals(attendee)).AttendTime = attendee.AttendTime;

            return attendees;
        }

        public static Attendee getAttendee(string userName)
        {
            return getAttendees().First(u => u.Name.CompareTo(userName) == 0);
        }
    }
}