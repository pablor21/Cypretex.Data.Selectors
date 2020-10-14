# NETCORE DYNAMIC FIELD SELECTORS

[![NuGet](https://img.shields.io/nuget/v/Cypretex.Data.Selectors?style=flat)](https://www.nuget.org/packages/Cypretex.Data.Selectors/)

Select a subset of fields of results on a graphql-like fashion.


## Install

```
PM> Install-Package Cypretex.Data.Selectors
```

## Usage

For the examples we will use the following model clases:

```
public class User
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public List<Car> Cars { get; set; } = new List<Car>();
    public User? Parent { get; set; }
    public DateTime BirthDate { get; set; }
}

public class Car
{
    public int? Id { get; set; }
    public string Make { get; set; }
    public int Year { get; set; }
    public User Owner { get; set; }
}
```

Now we can create a list of users with cars

```
var col = new List<User>();
int carIndex = 1;
for (int i = 1; i < 100; i++)
{
    User u = new User()
    {
        Id = i,
        Name = "User " + i,
        Parent = i > 2 ? col.First() : null,
        BirthDate = DateTime.UtcNow.AddDays(-i),
    };

    for (var j = 1; j < 11; j++)
    {
        carIndex++;
        u.Cars.Add(new Car
        {
            Id = carIndex,
            Make = "Make " + carIndex,
            Year = carIndex,
            Owner = u,
        }); ;
    }

    col.Add(u);
}
```

Create a selector and apply it:

```
var selector = Selector.Properties("Id,FirstName");
var result = selector.Parse<User>(users);
//now the result contains only the Id and FirstName
```