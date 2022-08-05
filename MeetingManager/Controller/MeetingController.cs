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

        private static IEnumerable<Meeting>? getMeetings()
        {
            if (!File.Exists(path))
                return Enumerable.Empty<Meeting>();

            using var reader = File.OpenText(path);

            if (reader.ReadToEnd().Length < 3)
                return Enumerable.Empty<Meeting>();

            return JsonSerializer.Deserialize<Meeting[]>(reader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}