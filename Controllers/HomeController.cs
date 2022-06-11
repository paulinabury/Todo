using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Todo.Models;
using Todo.Models.ViewModels;

namespace Todo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);
    }

    internal TodoViewModel GetAllTodos()
    {
        List<TodoItem> todoList = new();

        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            todoList.Add(
                                new TodoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Deadline = reader.GetString(2)
                                });
                        }
                    }
                    else
                    {
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                };
            }
        }
        return new TodoViewModel
        {
            TodoList = todoList
        };
    }

    public RedirectResult Insert(TodoItem todo)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name, deadline) VALUES ('{todo.Name}', '{todo.Deadline}')";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("https://localhost:7197");
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"DELETE from todo WHERE id = '{id}'";
                tableCmd.ExecuteNonQuery();
            }
        }
        return Json(new { });
    }

    //[HttpPost]
    public void Done(int id)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                var doneTodo = GetById(id);
                tableCmd.CommandText = $"INSERT INTO done_todos (name, deadline) VALUES ('{doneTodo.Name}', '{doneTodo.Deadline}')";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                Delete(id);
            }
        }
    }

    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    [HttpGet]
    public TodoItem GetById(int id)
    {
        TodoItem todo = new();

        using (var connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"SELECT * FROM todo WHERE Id='{id}'";
                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);
                        todo.Deadline = reader.GetString(2);
                    }
                    else
                    {
                        return todo;
                    }
                }
            }
        }
        return todo;
    }

    public RedirectResult Update(TodoItem todo)
    {
        using (var connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                tableCmd.CommandText = $"UPDATE todo SET deadline = '{todo.Deadline}' WHERE Id = '{todo.Id}'";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("https://localhost:7197");
    }
}
