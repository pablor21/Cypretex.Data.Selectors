using System;
using System.Collections.Generic;
using System.Linq;
using Cypretex.Data.Selectors.Tests.Database;
using Xunit;

namespace Cypretex.Data.Selectors.Tests
{
    public class DatabaeTests
    {
        TestDbContext context;
        public DatabaeTests()
        {
            //Arrange    
            var factory = new ConnectionFactory();

            //Get the instance of BlogDBContext  
            context = factory.CreateContextForInMemory();
            return;
            var c = new List<User>();
            int documentIndex = 1;
            for (int i = 1; i < 101; i++)
            {
                User user = new User()
                {
                    Id = i,
                    FirstName = $"FirstName {i}",
                    LastName = $"LastName {i}",
                    AnualSalary = i,
                    Phone = i.ToString(),
                    Documents = new List<Document>(),
                    Parent = i > 1 ? context.Users.Where(x => x.Id == i - 1).First() : null
                };
                context.Add(user);
                context.SaveChanges();
                for (var j = 1; j < 10; j++)
                {
                    Document d = new Document()
                    {
                        Name = $"Document {documentIndex}",
                        Id = documentIndex,
                        Owner = user
                    };
                    context.Documents.Add(d);
                    context.SaveChanges();
                    user.Documents.Append(d);

                    documentIndex++;
                }
                context.SaveChanges();
                //Console.WriteLine(user.Documents.Count());
                //user.PrincipalDocument = user.Documents.First();
                //user.Documents = null;
                //context.Users.Add(user);
                //context.SaveChanges();
            }
            context.SaveChanges();
            users = context.Users;
        }

        protected IQueryable<User> users;

        [Fact]
        public void TestDatabase()
        {

        }
    }
}