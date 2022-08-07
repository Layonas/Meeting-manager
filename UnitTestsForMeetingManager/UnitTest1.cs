using System.Linq;
using Xunit;
using System;
using MeetingManager.Models;
using MeetingManager.Enums;
using MeetingManager.Controller;

namespace UnitTestsForMeetingManager
{
    public class UnitTest1
    {
        private static DateTime start = new DateTime(2020, 1, 1, 13, 0, 0), end = new DateTime(2020, 1, 1, 14, 0, 0);

        private static Meeting meeting = new Meeting("test", "test", "test",
            FixedTypes.Category.TeamBuilding, FixedTypes.Type.Live, start, end);

        [Fact]
        public void MeetingCreationSuccessful()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"test
test
test
4
1
2020-01-01 13:00
2020-01-01 14:00");

            Console.SetIn(input);

            bool quit = false, cancel = false;

            var m = MeetingController.createMeeting(ref quit, ref cancel);

            Assert.Equal(meeting, m);
        }

        [Fact]
        public void AttendeeAddedToTheMeeting()
        {
            var attendees = MeetingController.addAttendee(meeting.ResponsiblePerson, meeting, meeting.StartDate);
            Attendee attendee = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());

            Assert.Contains(attendee, attendees);
        }

        [Fact]
        public void MeetingHasBeenSaved()
        {
            var meetings = MeetingController.saveMeeting(meeting);
            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void MeetingHasBeenRemoved()
        {
            var meetings = MeetingController.removeMeeting("test", meeting);

            Assert.DoesNotContain(meeting, meetings);
        }

        [Fact]
        public void CorrectChoiceOnAttendeesMeetings()
        {
            Attendee attendee = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());
            attendee.Meetings = attendee.Meetings.Append(meeting);
            bool quit = false;

            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"1");
            Console.SetIn(input);

            var m = AttendeeController.chooseAttendeeMeeting(attendee, ref quit);

            Assert.Equal(1, attendee.Meetings.Count());
        }

        [Fact]
        public void CannotRemoveMeetingBecauseResponsible()
        {
            Attendee attendee = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());
            var output = new StringWriter();
            Console.SetOut(output);

            attendee.Meetings = attendee.Meetings.Append(meeting);
            attendee.AttendTime.Add(meeting.StartDate);

            AttendeeController.removeMeeting(attendee, meeting, FixedTypes.AttendeeAction.AttendeeRemove);
            string expected = @"Cannot remove the attendee from the meeting he is responsible!
";
            Assert.Equal(expected, output.ToString());
        }

        [Fact]
        public void RemovedAMeetingFromAttendee()
        {
            Attendee attendee = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());
            meeting.ResponsiblePerson = "test2";
            attendee.Meetings = attendee.Meetings.Append(meeting);
            attendee.AttendTime.Add(meeting.StartDate);

            attendee = AttendeeController.removeMeeting(attendee, meeting, FixedTypes.AttendeeAction.AttendeeRemove);

            Assert.DoesNotContain(meeting, attendee.Meetings);
            meeting.ResponsiblePerson = "test";
        }

        [Fact]
        public void ContainsFilteredMeetingByDescription()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"test");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Description, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void DoesNotContainFilteredMeetingByDescription()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"test");

            Console.SetIn(input);

            bool quit = false;
            meetings.First(m => m.Name == "test").Description = "";
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Description, ref quit);

            Assert.DoesNotContain(meeting, meetings);
        }

        [Fact]
        public void ContainsFilteredMeetingsByCategory()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"4");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Category, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void DoesNotContainFilteredMeetingsByCategory()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"3");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Category, ref quit);

            Assert.DoesNotContain(meeting, meetings);
        }

        [Fact]
        public void ContainsFilteredMeetingsByType()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"1");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Type, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void DoesNotContainFilteredMeetingsByType()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"2");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Type, ref quit);

            Assert.DoesNotContain(meeting, meetings);
        }

        [Fact]
        public void ContainsFilteredMeetingsByStartingDate()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            //2020, 1, 1, 13, 0, 0
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"2020-01-01

");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Dates, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void DoesNotContainFilteredMeetingsByStartingDate()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            //2020, 1, 1, 13, 0, 0
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"2021-01-01

");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Dates, ref quit);

            Assert.DoesNotContain(meeting, meetings);
        }

        [Fact]
        public void ContainsFilteredMeetingsByEndingDate()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            //2020, 1, 1, 13, 0, 0
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"
2021-01-01");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Dates, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void DoesNotContainFilteredMeetingsByEndingDate()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            //2020, 1, 1, 13, 0, 0
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@"
1999-01-01");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>(), FixedTypes.Filter.Dates, ref quit);

            Assert.DoesNotContain(meeting, meetings);
        }

        [Fact]
        public void ContainsFilteredMeetingsByAttendeeCount()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var attendee1 = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());

            attendee1.Meetings = attendee1.Meetings.Append(meeting);

            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@">0");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>().Append(attendee1), FixedTypes.Filter.Attendees, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void ContainsFilteredMeetingsByAttendeeCount2()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var attendee1 = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());

            attendee1.Meetings = attendee1.Meetings.Append(meeting);

            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@">0?<2");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>().Append(attendee1), FixedTypes.Filter.Attendees, ref quit);

            Assert.Contains(meeting, meetings);
        }

        [Fact]
        public void DoesNotContainFilteredMeetingsByAttendeeCount()
        {
            IEnumerable<Meeting> meetings = Enumerable.Empty<Meeting>().Append(meeting);
            var attendee1 = new Attendee("test", Enumerable.Empty<Meeting>(), new List<DateTime>());

            attendee1.Meetings = attendee1.Meetings.Append(meeting);

            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader(@">1");

            Console.SetIn(input);

            bool quit = false;
            meetings = FilteringController.applyFiltering(meetings, Enumerable.Empty<Attendee>().Append(attendee1), FixedTypes.Filter.Attendees, ref quit);

            Assert.DoesNotContain(meeting, meetings);
        }
    }
}