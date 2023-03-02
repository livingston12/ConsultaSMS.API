
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebConsultaSMS.DataBase;


namespace ConsultaSMS.Test.Service
{
    public class TestApiContextMemoryData : TestMemoryData, IDisposable
    {
        public ApiContext ApiContext { get; private set; }

        public TestApiContextMemoryData()
        {
            CreateContextData();
        }

        private void CreateContextData()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase((new Guid()).ToString())
            .Options;

            Mock<IWebHostEnvironment> mockWebHost = new Mock<IWebHostEnvironment>();
            mockWebHost.Setup(x => x.EnvironmentName)
            .Returns("InMemory");

            ApiContext = new ApiContext(options, mockWebHost.Object);
            ValidateToInsertValues(ApiContext);
        }

        private void ValidateToInsertValues(ApiContext apiContext)
        {
            if (!ApiContext.Roles.Any())
                ApiContext.Roles.AddRange(rolEntities);

            if (!ApiContext.Users.Any())
                ApiContext.Users.AddRange(userEntities);

            if (!ApiContext.Mediators.Any())
                ApiContext.Mediators.AddRange(mediatorEntities);

            if (!ApiContext.Transactions.Any())
                ApiContext.Transactions.AddRange(transactionEntities);

            if (!ApiContext.GeneralParameters.Any())
                ApiContext.GeneralParameters.AddRange(generalParameterEntities);

            ApiContext.SaveChanges();
        }

        public void Dispose()
        {
            ApiContext.Dispose();
        }
    }
}