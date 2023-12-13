using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; } // Store hashed passwords in real-world applications.
    public DateTime RegisteredAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public List<Note> Notes { get; set; } = new List<Note>();
}

public class Note
{
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Choose an option:\n1. Register\n2. Login\n3. Exit");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    User user = LoginUser();
                    if (user != null)
                    {
                        PostLoginMenu(user);
                    }
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    static void RegisterUser()
    {
        Console.WriteLine("Enter username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string password = Console.ReadLine(); 

        var user = new User
        {
            Username = username,
            Password = password,
            RegisteredAt = DateTime.Now
        };

        SaveUser(user);
        Console.WriteLine("Registration successful.");
    }

    static User LoginUser()
    {
        Console.WriteLine("Enter username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string password = Console.ReadLine(); 

        var users = LoadUsers();
        foreach (var user in users)
        {
            if (user.Username == username && user.Password == password)
            {
                Console.WriteLine("Login successful.");
                user.LastLoginAt = DateTime.Now;
                SaveUsers(users);
                return user;
            }
        }

        Console.WriteLine("Login failed.");
        return null;
    }

    static List<User> LoadUsers()
    {
        string filePath = "users.json";
        if (!File.Exists(filePath))
        {
            return new List<User>();
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
    }

    static void SaveUser(User newUser)
    {
        var users = LoadUsers();
        users.Add(newUser);
        SaveUsers(users);
    }

    static void SaveUsers(List<User> users)
    {
        string json = JsonConvert.SerializeObject(users, Formatting.Indented);
        File.WriteAllText("users.json", json);
    }

    static void PostLoginMenu(User currentUser)
    {
        while (true)
        {
            Console.WriteLine("Choose an option:\n1. View Users\n2. Add Note\n3. View My Notes\n4. Exit");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ViewUsers();
                    break;
                case "2":
                    AddNote(currentUser);
                    break;
                case "3":
                    ViewNotes(currentUser);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    static void ViewUsers()
    {
        var users = LoadUsers();
        foreach (var user in users)
        {
            Console.WriteLine($"Username: {user.Username}, Registered: {user.RegisteredAt}, Last Login: {user.LastLoginAt}");
        }
    }

    static void AddNote(User currentUser)
    {
        Console.WriteLine("Enter your note:");
        string noteText = Console.ReadLine();

        var note = new Note
        {
            Text = noteText,
            CreatedAt = DateTime.Now
        };

        var users = LoadUsers();
        var userToUpdate = users.FirstOrDefault(u => u.Username == currentUser.Username);
        if (userToUpdate != null)
        {
            userToUpdate.Notes.Add(note);
            SaveUsers(users); 
            Console.WriteLine("Note added successfully.");
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }

    static void ViewNotes(User currentUser)
    {
        var users = LoadUsers();
        var userWithNotes = users.FirstOrDefault(u => u.Username == currentUser.Username);

        if (userWithNotes != null && userWithNotes.Notes.Count > 0)
        {
            Console.WriteLine("Your notes:");
            foreach (var note in userWithNotes.Notes)
            {
                Console.WriteLine($"Note ({note.CreatedAt}): {note.Text}");
            }
        }
        else
        {
            Console.WriteLine("No notes found.");
        }
    }
}
