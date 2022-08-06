using MeetingManager.Controller;
using MeetingManager.Enums;
using MeetingManager.Models;
using MeetingManager.Utils;
using System.Text.RegularExpressions;

bool cancel = false;
bool quit = false;
List<string> actions = new List<string>()
{ "Create a new meeting.", "Delete a meeting.", "Add a person to a meeting.",
    "Remove a person from a meeting.", "List and filter all meetings."};

var userName = Utils.login();
Console.Clear();

while (!quit)
{
    cancel = false;
    Console.WriteLine("You can quit at any time.");
    Console.WriteLine("You can cancel an action when you start the action.\n");
    Console.WriteLine("Choose an action to make: \n");

    for (int i = 0; i < actions.Count; i++)
    {
        Console.WriteLine((i + 1) + ": " + actions[i]);
    }

    Console.WriteLine();

    var action = Utils.action(1, actions.Count, ref quit);
    if (!quit && !cancel)
    {
        switch (action)
        {
            case 1:
                Console.Clear();
                Meeting meeting = MeetingController.createMeeting(ref quit, ref cancel);
                if (quit || cancel)
                    break;
                var attendees = MeetingController.addAttendee(meeting.ResponsiblePerson, meeting, meeting.StartDate);
                AttendeeController.updateAttendees(attendees, FixedTypes.Operation.Add);

                var meetings = MeetingController.saveMeeting(meeting);
                MeetingController.updateMeetings(meetings, FixedTypes.Operation.Add);

                break;

            case 2:
                Console.Clear();
                var deletingMeeting = MeetingController.chooseMeeting(ref quit, ref cancel);
                if (deletingMeeting is null || quit || cancel)
                    break;
                meetings = MeetingController.removeMeeting(userName, deletingMeeting);
                MeetingController.updateMeetings(meetings, FixedTypes.Operation.Delete);
                break;

            case 3:
                Console.Clear();
                Console.WriteLine("Enter the name of an attendee: ");
                string? name = Console.ReadLine();
                if (name.CompareTo("quit") == 0)
                {
                    quit = true;
                    break;
                }
                if (name.CompareTo("cancel") == 0)
                {
                    cancel = true;
                    break;
                }
                var attendeeMeeting = MeetingController.chooseMeeting(ref quit, ref cancel);
                if (attendeeMeeting is null)
                    break;
                Console.WriteLine($"Write his starting date between {attendeeMeeting.StartDate.ToString("HH:mm")}" +
                    $" and {attendeeMeeting.EndDate.ToString("HH:mm")} with format of HH:mm");
                DateTime date = Utils.readDate("HH:mm", ref quit);
                date = new DateTime(attendeeMeeting.StartDate.Year, attendeeMeeting.StartDate.Month,
                    attendeeMeeting.StartDate.Day, date.Hour, date.Minute, 0);
                attendees = MeetingController.addAttendee(name, attendeeMeeting, date);
                AttendeeController.updateAttendees(attendees, FixedTypes.Operation.Add);
                break;

            case 4:
                Console.Clear();
                Console.WriteLine("Choose an attendee:");
                var attendee = AttendeeController.chooseAttendee(ref quit, ref cancel);
                if (attendee is null)
                    break;
                Console.WriteLine("Choose a meeting to remove:");
                var removeAttendeeMeeting = AttendeeController.chooseAttendeeMeeting(attendee, ref quit);
                if (quit)
                    break;
                if (removeAttendeeMeeting is null)
                {
                    Console.WriteLine($"{attendee.Name} does not contain any meetings.");
                    break;
                }
                attendee = AttendeeController.removeMeeting(attendee, removeAttendeeMeeting);
                attendees = AttendeeController.updateAttendee(attendee);
                AttendeeController.updateAttendees(attendees, FixedTypes.Operation.Delete);

                break;

            case 5:
                Console.Clear();
                Console.WriteLine("Apply a filter? yes/no");
                var yesNO = Console.ReadLine();
                if (yesNO.CompareTo("yes") != 0)
                {
                    if (yesNO.CompareTo("cancel") == 0)
                    {
                        cancel = true;
                        break;
                    }
                    else if (yesNO.CompareTo("quit") == 0)
                    {
                        quit = true;
                        break;
                    }
                    MeetingController.ListMeeting(MeetingController.getMeetings());
                    break;
                }

                var filter = FilteringController.chooseFilter(ref quit, ref cancel);

                if (quit || cancel)
                    break;
                meetings = MeetingController.getMeetings();
                attendees = AttendeeController.getAttendees();
                var output = FilteringController.applyFiltering(meetings, attendees, filter, ref quit);
                if (quit)
                    break;

                MeetingController.ListMeeting(output);

                break;
        }
    }

    if (cancel)
    {
        Console.WriteLine("Your action has been canceled.");
    }

    if (quit)
        Console.WriteLine("\nYou have quit the program.");
    else
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}