using MeetingManager.Enums;
using MeetingManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeetingManager.Controller
{
    internal class AttendeeController
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
        }
    }
}