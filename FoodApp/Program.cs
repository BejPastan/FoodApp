using FoodApp.Services;
using FoodApp.Utilities;

var builder = WebApplication.CreateBuilder(args);

// connecting to database

DBConnector.SetConnectionString(SecretController.GetDatabaseCredentials());
Authentication.Initialize(SecretController.GetAuthSecretKey(), 20);
DBConnector.Open();

Swagger.AddSwaggerDocumentation(ref builder);

#region adding repositories
builder.Services.AddScoped<FoodApp.Repositories.IFoodRepository, FoodApp.Repositories.FoodRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IFoodTypeRepository, FoodApp.Repositories.FoodTypeRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IUnitRepository, FoodApp.Repositories.UnitRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IStepRepository, FoodApp.Repositories.StepRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IIngredientRepository, FoodApp.Repositories.IngredientRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IRecipeRepository, FoodApp.Repositories.RecipeRepository>();
builder.Services.AddScoped<FoodApp.Repositories.ITagRepository, FoodApp.Repositories.TagRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IRecipeTagRepository, FoodApp.Repositories.RecipeTagRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IUserRepository, FoodApp.Repositories.UserRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IMealRepository, FoodApp.Repositories.MealRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IRecipeMealRepository, FoodApp.Repositories.RecipeMealRepository>();
builder.Services.AddScoped<FoodApp.Repositories.IUserMealRepository, FoodApp.Repositories.UserMealRepository>();
#endregion

// services
builder.Services.AddScoped<FoodApp.Services.IMealService, FoodApp.Services.MealService>();
builder.Services.AddScoped<FoodApp.Services.IStepService, FoodApp.Services.StepService>();
builder.Services.AddScoped<FoodApp.Services.IUnitService, FoodApp.Services.UnitService>();
builder.Services.AddScoped<FoodApp.Services.IRecipeMealService, FoodApp.Services.RecipeMealService>();
builder.Services.AddScoped<FoodApp.Services.IRecipeTagService, FoodApp.Services.RecipeTagService>();
builder.Services.AddScoped<FoodApp.Services.IUserMealService, FoodApp.Services.UserMealService>();
builder.Services.AddScoped<FoodApp.Services.IFoodService, FoodApp.Services.FoodService>();
builder.Services.AddScoped<FoodApp.Services.IFoodTypeService, FoodApp.Services.FoodTypeService>();
builder.Services.AddScoped<FoodApp.Services.ITagService, FoodApp.Services.TagService>();
builder.Services.AddScoped<FoodApp.Services.IIngredientService, FoodApp.Services.IngredientService>();
builder.Services.AddScoped<FoodApp.Services.IRecipeService, FoodApp.Services.RecipeService>();

var app = builder.Build();

Swagger.UseSwaggerDocumentation(ref app);
app.MapControllers();
app.Run();
