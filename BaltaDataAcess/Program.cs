using BaltaDataAcess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Threading;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace BaltaDataAcess
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost,1433;Database=balta;User Id=sa;Password=jmS052703@@;TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                //CreateCategory(connection);
                //CreateManyCategory(connection);
                //DeleteCategory(connection, Guid.Parse("dab32d2f-a45a-446b-8ce7-fb22dd00ae56"));
                //ListCategories(connection);
                //InsertStudents(connection);
                //ListStudents(connection);
                //ExecuteProcedure(connection, Guid.Parse("d8c0602e-268a-405f-a7b5-c58401bd0030"));
                //ExecuteReadProcedureGetCoursesByCategory(connection, Guid.Parse("af3407aa-11ae-4621-a2ef-2028b85507c4"));
                //ExecuteScalar(connection);
                //ReadView(connection);
                //OneToOne(connection);
                //OneToMany(connection);
                QueryMultiple(connection);
            }
             
            static void ListCategories(SqlConnection connection)
            {
                var categories = connection.Query<Category>("SELECT TOP 100 * FROM [Category]");

                foreach (var category in categories)
                {
                    Thread.Sleep(100);
                    Console.WriteLine($" Id: {category.Id}\n Categoria: {category.Title}\n Url: {category.Url}\n Resumo: {category.Summary}\n");
                }
            }

            static void CreateCategory(SqlConnection connection)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Title = "Amazon AWS";
                category.Url = "amazon";
                category.Summary = "Aprenda a desenvolver aplicações com AWS";
                category.Order = 8;
                category.Description = "Categoria destinada a serviços do AWS";
                category.Featured = false;

                var insertSql = @"INSERT INTO [Category]
                                    VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

                var row = connection.Execute(insertSql, new
                {
                    category.Id,
                    category.Title, 
                    category.Url,
                    category.Summary, 
                    category.Order,
                    category.Description,
                    category.Featured
                });

                Console.WriteLine($"{row} categoria criada!");
            }

            static void CreateManyCategory(SqlConnection connection)
            {
                var categories = new[]
                {
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Title = "Mobile",
                        Url = "mobile",
                        Summary = "Aprenda a desenvolver aplicações com Mobile",
                        Order = 8,
                        Description = "Categoria destinada a serviços Mobile",
                        Featured = true
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Title = "Azure",
                        Url = "azure",
                        Summary = "Aprenda a desenvolver aplicações com Azure",
                        Order = 9,
                        Description = "Categoria destinada a serviços do Azure",
                        Featured = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Title = "Google Cloud",
                        Url = "google-cloud",
                        Summary = "Aprenda a desenvolver aplicações com Google Cloud",
                        Order = 10,
                        Description = "Categoria destinada a serviços do Google Cloud",
                        Featured = false
                    }
                };

                var insertSql = @"INSERT INTO [Category]
                        VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

                var rows = connection.Execute(insertSql, categories);

                Console.WriteLine($"{rows} categoria criada!");
            }

            static void DeleteCategory(SqlConnection connection, Guid id)
            {
                var deleteSql = "DELETE FROM [Category] WHERE [Id] = @Id";
                var rows = connection.Execute(deleteSql, new { Id = id });

                if (rows > 0)
                    Console.WriteLine("Categoria removida com sucesso!");
                else
                    Console.WriteLine("Nenhuma categoria encontrada com o Id informado.");
            }

            static void ListStudents(SqlConnection connection)
            {
                var students = connection.Query<Student>("SELECT * FROM [Student]");

                foreach (var student in students)
                {
                    Thread.Sleep(100);
                    Console.WriteLine($" Id: {student.Id}\n Nome: {student.Name}\n Url: {student.Email}\n Data Nascimento: {student.BirthDate}\n");
                }
            }

            static void InsertStudents(SqlConnection connection)
            {
                var students = new[]
                {
                    new Student
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ana Paula",
                        Email = "ana.paula@email.com",
                        Document = "12345678901",
                        Phone = "(11) 91234-5678",
                        BirthDate = new DateTime(1998, 3, 15),
                        CreateDate = DateTime.Now
                    },
                    new Student 
                    {
                        Id = Guid.NewGuid(),
                        Name = "Carlos Eduardo",
                        Email = "carlos.eduardo@email.com",
                        Document = "10987654321",
                        Phone = "(21) 99876-5432",
                        BirthDate = new DateTime(2001, 7, 22),
                        CreateDate = DateTime.Now
                    }
                };

                var insertSql = @"INSERT INTO [Student] (Id, Name, Email, Document, Phone, BirthDate, CreateDate)
                                    VALUES (@Id, @Name, @Email, @Document, @Phone, @BirthDate, @CreateDate)";

                var rows = connection.Execute(insertSql, students);
                Console.WriteLine($"{rows} alunos inseridos!");
            }
             
            static void ExecuteProcedureDeleteStudent(SqlConnection connection, Guid id)
            {
                var procedure = "[spDeleteStudent]";
                var parameters = new { StudentId = id };

                var affectedRows = connection.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
                Console.WriteLine($"{affectedRows} linhas afetadas!");
            }

            static void ExecuteReadProcedureGetCoursesByCategory(SqlConnection connection, Guid id)
            {
                var procedure = "[spGetCoursesByCategory]";
                var parameters = new { CategoryId = id };

                var courses = connection.Query(
                    procedure, 
                    parameters, 
                    commandType: CommandType.StoredProcedure);

                foreach(var course in courses)
                {
                    Console.WriteLine(course.Title);
                }
            }

            static void ExecuteScalar(SqlConnection connection)
            {
                var category = new Category
                {
                    Title = "Amazon AWS",
                    Url = "amazon",
                    Summary = "Aprenda a desenvolver aplicações com AWS",
                    Order = 8,
                    Description = "Categoria destinada a serviços do AWS",
                    Featured = false
                };
 
                var insertSql = @"INSERT INTO [Category]
                                OUTPUT inserted.[Id]
                                VALUES (NEWID(), @Title, @Url, @Summary, @Order, @Description, @Featured)";

                var id = connection.ExecuteScalar(insertSql, category);
                Console.WriteLine($"A categoria criada foi: {id}");
            }

            static void ReadView(SqlConnection connection)
            {
                var sql = "SELECT * FROM [vwCourses];";
                var courses = connection.Query(sql);

                foreach (var course in courses)
                {
                    Thread.Sleep(100);
                    Console.WriteLine($" Id: {course.Id}\n Curso: {course.Title}\n Url: {course.Url}\n Resumo: {course.Summary}\n");
                }
            }

            static void OneToOne(SqlConnection connection)
            {
                var sql = @"SELECT 
                                * 
                            FROM 
                                [CareerItem]
                                INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]
                                INNER JOIN [Career] ON [CareerItem].[CareerId] = [Career].[Id];";

                var items = connection.Query<CareerItem, Course, Career, CareerItem>(
                    sql, 
                    (careerItem, course, career) => 
                    { 
                        careerItem.Course = course;
                        careerItem.Career = career;
                        return careerItem;
                    }, 
                    splitOn: "Id,Id"
                );

                foreach(var item in items)
                {
                    Console.WriteLine($"Carreira: {item.Career.Title} \nCurso: {item.Course.Title}\n");
                }
            }

            static void OneToMany(SqlConnection connection)
            {
                var sql = @"SELECT 
                                [Career].[Id],
                                [Career].[Title],
                                [CareerItem].[CareerId],
                                [CareerItem].[Title]
                            FROM 
                                [Career]
                            INNER JOIN 
                                [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]    
                            ORDER BY
                                [Career].[Title]";

                var careers = new List<Career>();
                var items = connection.Query<Career, CareerItem, Career>(
                    sql,
                    (career, item) =>
                    {
                        var existingCareer = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                        if (existingCareer == null)
                        {
                            existingCareer = career;
                            existingCareer.Items.Add(item);
                            careers.Add(existingCareer);
                        }
                        else
                        {
                            existingCareer.Items.Add(item);
                        }

                        return career;
                    },
                    splitOn: "CareerId"
                );

                foreach (var career in careers)
                {
                    Console.WriteLine($"\nCarreira: {career.Title}\n");
                    foreach (var item in career.Items)
                    {
                        Console.WriteLine($"Items da Carreira: {item.Title}");
                    }
                }
            }

            static void QueryMultiple(SqlConnection connection)
            {
                var sql = "SELECT TOP 10 * FROM [Course]; SELECT TOP 10 * FROM [Career];";

                using (var multiple = connection.QueryMultiple(sql))
                {
                    var course = multiple.Read<Category>();
                    var career = multiple.Read<Career>();

                    foreach (var item in course)
                    {
                        Console.WriteLine($"Courso: {item.Title}");
                    }

                    Console.WriteLine("\n");

                    foreach (var item in career)
                    {
                        Console.WriteLine($"Carreira: {item.Title}");
                    }
                }
            }
        }
    }
}
