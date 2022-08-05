using MeetingManager.Controller;
using MeetingManager.Models;

bool cancel = false;
bool quit = false;
string? userName = null;

while (userName is null || userName.Length == 0)
{
    Console.Clear();
    Console.WriteLine("Enter your name:");
    userName = Console.ReadLine();
}

Console.Clear();
Console.WriteLine("Choose an action to make: \n");

List<string> actions = new List<string>()
{ "Create a new meeting.", "Delete a meeting.", "Add a person to a meeting.",
    "Remove a person from a meeting.", "List and filter all meetings."};

for (int i = 0; i < actions.Count; i++)
{
    Console.WriteLine((i + 1) + ": " + actions[i]);
}

Console.WriteLine();
int? action = null;

while (action is null)
{
    try
    {
        action = Convert.ToInt32(Console.ReadLine());
    }
    catch (FormatException)
    {
        Console.WriteLine("Please choose a valid action!");
    }
}

switch (action)
{
    case 1:
        MeetingController.createMeeting(ref quit, ref cancel);
        break;

    case 2:
        Console.Clear();
        var deletingMeeting = MeetingController.chooseMeeting();
        if (deletingMeeting is null)
            break;
        MeetingController.deleteMeeting(userName, deletingMeeting);
        break;

    case 3:
        Console.Clear();
        Console.WriteLine("Enter the name of an attendee: ");
        string? name = Console.ReadLine();
        var attendeeMeeting = MeetingController.chooseMeeting();
        if (attendeeMeeting is null)
            break;
        Console.WriteLine($"Write his starting date between {attendeeMeeting.StartDate.ToString("HH:mm")}" +
            $" and {attendeeMeeting.EndDate.ToString("HH:mm")} with format of HH:mm");
        DateTime date;
        DateTime.TryParseExact(Console.ReadLine(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out date);
        MeetingController.addAttendee(name, attendeeMeeting, date);
        break;

    case 4:
        Console.Clear();
        Console.WriteLine("Choose an attendee:");
        var attendee = AttendeeController.chooseAttendee();
        if (attendee is null)
            break;
        Console.WriteLine("Choose a meeting to remove:");
        var removeAttendeeMeeting = AttendeeController.chooseAttendeeMeeting(attendee);
        if (removeAttendeeMeeting is null)
        {
            Console.WriteLine($"{attendee.Name} does not contain any meetings.");
            break;
        }
        AttendeeController.removeMeeting(attendee, removeAttendeeMeeting);
        break;

    case 5:

        break;
}

//while (!quit)
//{
//}