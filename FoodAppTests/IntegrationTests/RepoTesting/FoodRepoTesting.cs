using FoodApp.Utilities;


namespace FoodAppTests.IntegrationTests.RepoTesting
{

    public class FoodRepoTesting
    {
        FoodRepoTesting(DBMockup dBMockup)
        {
            DBConnector.SetConnectionString(dBMockup._container.GetConnectionString());
            
        }
    }
}
