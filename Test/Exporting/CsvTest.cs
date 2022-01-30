using MoneyCheckWebApp.ExportingServices.Csv;
using Xunit;

namespace Test.Exporting
{
    public class CsvTest
    {
        [Fact]
        public void ConvertingSingleObjectTest()
        {
            const string expectedCsv = "Tom,0,True";
            var user = new User()
            {
                Name = "Tom",
                Id = 0,
                IsAdmin = true
            };

            var csvConverter = new CsvConverter<User>();
            
            Assert.Equal(expectedCsv, csvConverter.Convert(user));
        }
        
        [Fact]
        public void TestObjectsConvertation()
        {
            const string expectedCsv = "Tom,0,True\nKate,1,False\nDenis,2,True";
            var users = new[] {  
                new User()
                {
                    Name = "Tom",
                    Id = 0,
                    IsAdmin = true
                },
                new User()
                {
                    Name = "Kate",
                    Id = 1,
                    IsAdmin = false
                }, 
                new User()
                {
                    Name = "Denis",
                    Id = 2,
                    IsAdmin = true
                }
            };
            
            var csvConverter = new CsvConverter<User>();
            
            Assert.Equal(expectedCsv, csvConverter.ConvertEnumerable(users));
        }
    }

    public class User 
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsAdmin { get; set; }
    }
}