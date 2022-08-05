using MeetingManager.Controller;

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

        break;

    case 3:

        break;

    case 4:

        break;

    case 5:

        break;

    case 6:

        break;
}

//while (!quit)
//{
//}