using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingManager.Enums
{
    public class FixedTypes
    {
        public enum Category
        {
            CodeMonkey,
            Hub,
            Short,
            TeamBuilding
        }

        public enum Type
        {
            Live,
            InPerson
        }

        public enum Operation
        {
            Add,
            Delete
        }

        public enum Filter
        {
            Description,
            ResponsiblePerson,
            Category,
            Type,
            Dates,
            Attendees
        }

        public enum AttendeeAction
        {
            AttendeeRemove,
            MeetingRemove
        }
    }
}