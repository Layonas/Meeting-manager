using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeetingManager.Models
{
    public class Attendee
    {
        public string Name { get; set; }
        public IEnumerable<Meeting> Meetings { get; set; }
        public List<DateTime> AttendTime { get; set; }

        public Attendee()
        {
        }

        public Attendee(string name, IEnumerable<Meeting> meetings, List<DateTime> attendTime)
        {
            Name = name;
            Meetings = meetings;
            AttendTime = attendTime;
        }

        public override string? ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is Attendee attendee &&
                   Name == attendee.Name;
        }
    }
}