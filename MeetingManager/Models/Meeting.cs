using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeetingManager.Enums;

namespace MeetingManager.Models
{
    public class Meeting
    {
        public string Name { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Description { get; set; }
        public FixedTypes.Category Category { get; set; }
        public FixedTypes.Type Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Meeting()
        {
        }

        public Meeting(string name, string responsiblePerson, string description,
            FixedTypes.Category category, FixedTypes.Type type, DateTime startDate, DateTime endDate)
        {
            Name = name;
            ResponsiblePerson = responsiblePerson;
            Description = description;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string? serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public override string? ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Meeting name: {Name}, responsible person for the meeting: {ResponsiblePerson}");
            builder.AppendLine($"Meeting description: {Description}");
            builder.AppendLine($"Category: {Category}, Type: {Type}");
            builder.AppendLine($"Meeting starts at: {StartDate.ToString("yyyy-MM-dd HH:mm")} and ends at {EndDate.ToString("yyyy-MM-dd HH:mm")}");
            return builder.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is Meeting meeting &&
                   Name == meeting.Name &&
                   ResponsiblePerson == meeting.ResponsiblePerson &&
                   Description == meeting.Description &&
                   Category == meeting.Category &&
                   Type == meeting.Type &&
                   StartDate == meeting.StartDate &&
                   EndDate == meeting.EndDate;
        }
    }
}