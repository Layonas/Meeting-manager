using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeetingManager.Enums;

namespace MeetingManager.Models
{
    internal class Meeting
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

        public override string? ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}