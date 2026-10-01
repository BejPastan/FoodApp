using FoodApp.Services;
using FoodApp.Repositories;
using FoodApp.Utilities;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;
using DbUp;

var builder = WebApplication.CreateBuilder(args);

Dapper.SqlMapper.AddTypeHandler(new SQLDateOnlyTypeHandler());
SecretController.LoadSecrets(ref builder);

#region DB migration
EnsureDatabase.For.SqlDatabase(SecretController.GetDatabaseCredentials());

var upgrader = DeployChanges.To
    .SqlDatabase(SecretController.GetDatabaseCredentials())
    .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
    .WithTransactionPerScript()
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Database migration successful!");
Console.ResetColor();
#endregion

// connecting to database
DBConnector.SetConnectionString(SecretController.GetDatabaseCredentials());
Authentication.Initialize(SecretController.GetAuthSecretKey());//3 months
DBConnector.Open();

Swagger.AddSwaggerDocumentation(ref builder);

//adding cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://localhost:4173", "http://localhost:4173", "https://food-app-front-vercel.vercel.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add global exception filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

#region adding repositories
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IFoodTypeRepository, FoodTypeRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IStepRepository, StepRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IRecipeRepo, RecipeRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IRecipeTagRepository, RecipeTagRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMealRepository, MealRepository>();
builder.Services.AddScoped<IRecipeMealRepository, RecipeMealRepository>();
builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
builder.Services.AddScoped<IKitchenRepository, KitchenRepository>();
builder.Services.AddScoped<IKitchenRoleRepository, KitchenRoleRepository>();
builder.Services.AddScoped<IKitchenUsersRepository, KitchenUsersRepository>();
builder.Services.AddScoped<IKitchenMealRepository, KitchenMealRepository>();
#endregion

#region services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKitchenUsersService, KitchenUserService>();
builder.Services.AddScoped<IKitchenService, KitchenService>();
builder.Services.AddScoped<IKitchenRoleService, KitchenRoleService>();

builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IStepService, StepService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IRecipeMealService, RecipeMealService>();
builder.Services.AddScoped<IRecipeTagService, RecipeTagService>();
builder.Services.AddScoped<IKitchenMealService, KitchenMealService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IFoodTypeService, FoodTypeService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
#endregion

var app = builder.Build();
Swagger.UseSwaggerDocumentation(ref app);
app.UseRouting();
app.UseCors("AllowAll");
app.MapControllers();
app.UseDeveloperExceptionPage();
app.Run();
