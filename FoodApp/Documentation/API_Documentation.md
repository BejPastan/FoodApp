# FoodApp API Documentation

This document provides comprehensive documentation for all API endpoints in the FoodApp backend system. The API is built using ASP.NET Core and provides endpoints for managing food items, recipes, meals, users, and related entities.

## Table of Contents

1. [Authentication](#authentication)
2. [Food Management](#food-management)
3. [Food Type Management](#food-type-management)
4. [Ingredient Management](#ingredient-management)
5. [Meal Management](#meal-management)
6. [Recipe Management](#recipe-management)
7. [Recipe-Meal Relationships](#recipe-meal-relationships)
8. [Recipe-Tag Relationships](#recipe-tag-relationships)
9. [Step Management](#step-management)
10. [Tag Management](#tag-management)
11. [Unit Management](#unit-management)
12. [User Management](#user-management)
13. [Current User](#current-user)
14. [User-Meal Tracking](#user-meal-tracking)

## Authentication

### Authentication Headers

All authenticated endpoints require a valid JWT token in the Authorization header. The token must be included in the following format:

```
Authorization: Bearer <your-auth-token>
```

**Important Notes:**
- The token must be a valid JWT token obtained from the login endpoint
- The token never expired
- If the token is missing or invalid the API will return a 401 Unauthorized response
- The token should be included in the header for all subsequent requests after login

### User Signup
- **Endpoint**: `POST /api/users/signup`
- **Required Parameters**: 
  - `email` (query parameter) - User's email address
  - `password` (query parameter) - User's password
  - `name` (query parameter) - User's display name
- **Description**: Creates a new user account and returns an authentication token.
- **Success Response Structure**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### User Login
- **Endpoint**: `POST /api/users/login`
- **Required Parameters**:
  - `email` (query parameter) - User's email address
  - `password` (query parameter) - User's password
- **Description**: Authenticates a user and returns a JWT token for subsequent requests.
- **Success Response Structure**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Example Authentication Flow

1. **Signup Request:**
```
POST /api/users/signup?email=user@example.com&password=securepassword&name=JohnDoe
```

2. **Login Request:**
```
POST /api/users/login?email=user@example.com&password=securepassword
```

3. **Authenticated Request:**
```
GET /auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Food Management

### Search Foods
- **Endpoint**: `GET /api/food`
- **Optional Parameters**:
  - `typeId` (query parameter) - Filter by food type ID
  - `name` (query parameter) - Filter by food name (partial match)
  - `page` (query parameter, default: 1) - Page number for pagination
  - `perPage` (query parameter, default: 10) - Number of items per page
- **Description**: Searches for foods based on type and/or name criteria with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "Apple",
    "foodTypeId": 1,
    "foodType": {
      "id": 1,
      "name": "Fruit"
    }
  }
]
```

### Get Food by ID
- **Endpoint**: `GET /api/food/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Food ID
- **Description**: Retrieves a specific food item by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Apple",
  "foodTypeId": 1,
  "foodType": {
    "id": 1,
    "name": "Fruit"
  }
}
```

### Create Food
- **Endpoint**: `POST /api/food`
- **Required Parameters**:
  - Request body with `Food` object containing:
    - `foodTypeId` OR `foodType.name` (required) - Food type reference
    - Other food properties as needed
- **Description**: Creates a new food item in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Apple",
  "foodTypeId": 1,
  "foodType": {
    "id": 1,
    "name": "Fruit"
  }
}
```

### Update Food
- **Endpoint**: `PATCH /api/food/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Food ID to update
  - Request body with `Food` object containing fields to update
- **Description**: Updates specific fields of an existing food item.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Updated Apple Name",
  "foodTypeId": 1,
  "foodType": {
    "id": 1,
    "name": "Fruit"
  }
}
```

### Delete Food
- **Endpoint**: `DELETE /api/food/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Food ID to delete
- **Description**: Removes a food item from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Food Type Management

### Get Food Types
- **Endpoint**: `GET /api/food_type`
- **Optional Parameters**:
  - `name` (query parameter) - Filter by food type name (partial match)
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves all food types, optionally filtered by name with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "Fruit"
  }
]
```

### Get Food Type by ID
- **Endpoint**: `GET /api/food_type/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Food type ID
- **Description**: Retrieves a specific food type by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Fruit"
}
```

### Create Food Type
- **Endpoint**: `POST /api/food_type`
- **Required Parameters**:
  - Request body with `FoodType` object containing:
    - `name` (required) - Food type name
- **Description**: Creates a new food type in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Fruit"
}
```

### Update Food Type
- **Endpoint**: `PATCH /api/food_type/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Food type ID to update
  - Request body with `FoodType` object containing fields to update
- **Description**: Updates specific fields of an existing food type.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Updated Fruit Name"
}
```

### Delete Food Type
- **Endpoint**: `DELETE /api/food_type/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Food type ID to delete
- **Description**: Removes a food type from the database.
- **Success Response Structure**:
```json
{
  "success": true
}
```

## Ingredient Management

### Get Ingredients
- **Endpoint**: `GET /api/ingredients`
- **Optional Parameters**:
  - `recipeId` (query parameter) - Filter by recipe ID
  - `foodId` (query parameter) - Filter by food ID
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves ingredients, optionally filtered by recipe or food with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "foodId": 1,
    "unitId": 1,
    "unitAmount": 2.5,
    "recipeId": 1,
    "food": {
      "id": 1,
      "name": "Apple",
      "foodTypeId": 1,
      "foodType": {
        "id": 1,
        "name": "Fruit"
      }
    },
    "unit": {
      "id": 1,
      "name": "cup",
      "volumeEquivalent": 240.0
    }
  }
]
```

### Get Ingredient by ID
- **Endpoint**: `GET /api/ingredients/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Ingredient ID
- **Description**: Retrieves a specific ingredient by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "foodId": 1,
  "unitId": 1,
  "unitAmount": 2.5,
  "recipeId": 1,
  "food": {
    "id": 1,
    "name": "Apple",
    "foodTypeId": 1,
    "foodType": {
      "id": 1,
      "name": "Fruit"
    }
  },
  "unit": {
    "id": 1,
    "name": "cup",
    "volumeEquivalent": 240.0
  }
}
```

### Create Ingredient
- **Endpoint**: `POST /api/ingredients`
- **Required Parameters**:
  - Request body with `Ingredient` object containing all required fields
- **Description**: Creates a new ingredient in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "foodId": 1,
  "unitId": 1,
  "unitAmount": 2.5,
  "recipeId": 1,
  "food": {
    "id": 1,
    "name": "Apple",
    "foodTypeId": 1,
    "foodType": {
      "id": 1,
      "name": "Fruit"
    }
  },
  "unit": {
    "id": 1,
    "name": "cup",
    "volumeEquivalent": 240.0
  }
}
```

### Update Ingredient
- **Endpoint**: `PATCH /api/ingredients/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Ingredient ID to update
  - Request body with `Ingredient` object containing fields to update
- **Description**: Updates specific fields of an existing ingredient.
- **Success Response Structure**:
```json
{
  "id": 1,
  "foodId": 1,
  "unitId": 1,
  "unitAmount": 3.0,
  "recipeId": 1,
  "food": {
    "id": 1,
    "name": "Apple",
    "foodTypeId": 1,
    "foodType": {
      "id": 1,
      "name": "Fruit"
    }
  },
  "unit": {
    "id": 1,
    "name": "cup",
    "volumeEquivalent": 240.0
  }
}
```

### Delete Ingredient
- **Endpoint**: `DELETE /api/ingredients/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Ingredient ID to delete
- **Description**: Removes an ingredient from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Meal Management

### Get Meals
- **Endpoint**: `GET /api/meals`
- **Optional Parameters**:
  - `name` (query parameter) - Filter by meal name (partial match)
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves all meals, optionally filtered by name with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "Breakfast"
  }
]
```

### Get Meal by ID
- **Endpoint**: `GET /api/meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Meal ID
- **Description**: Retrieves a specific meal by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Breakfast"
}
```

### Create Meal
- **Endpoint**: `POST /api/meals`
- **Required Parameters**:
  - Request body with `Meal` object containing all required fields
- **Description**: Creates a new meal in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Breakfast"
}
```

### Update Meal
- **Endpoint**: `PATCH /api/meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Meal ID to update
  - `name` (query parameter, optional) - New meal name
- **Description**: Updates the name of an existing meal.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Updated Breakfast Name"
}
```

### Delete Meal
- **Endpoint**: `DELETE /api/meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Meal ID to delete
- **Description**: Removes a meal from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Recipe Management

### Get Recipes
- **Endpoint**: `GET /api/recipes`
- **Optional Parameters**:
  - `name` (query parameter) - Filter by recipe name (partial match)
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves all recipes, optionally filtered by name with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "Apple Pie",
    "ingredients": [
      {
        "id": 1,
        "foodId": 1,
        "unitId": 1,
        "unitAmount": 2.5,
        "recipeId": 1,
        "food": {
          "id": 1,
          "name": "Apple",
          "foodTypeId": 1,
          "foodType": {
            "id": 1,
            "name": "Fruit"
          }
        },
        "unit": {
          "id": 1,
          "name": "cup",
          "volumeEquivalent": 240.0
        }
      }
    ],
    "steps": [
      {
        "id": 1,
        "recipeId": 1,
        "instruction": "Mix ingredients",
        "stepNumber": 1
      }
    ],
    "meals": [
      {
        "id": 1,
        "name": "Dessert"
      }
    ]
  }
]
```

### Get Recipe by ID
- **Endpoint**: `GET /api/recipes/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Recipe ID
- **Description**: Retrieves a specific recipe by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Apple Pie",
  "ingredients": [
    {
      "id": 1,
      "foodId": 1,
      "unitId": 1,
      "unitAmount": 2.5,
      "recipeId": 1,
      "food": {
        "id": 1,
        "name": "Apple",
        "foodTypeId": 1,
        "foodType": {
          "id": 1,
          "name": "Fruit"
        }
      },
      "unit": {
        "id": 1,
        "name": "cup",
        "volumeEquivalent": 240.0
      }
    }
  ],
  "steps": [
    {
      "id": 1,
      "recipeId": 1,
      "instruction": "Mix ingredients",
      "stepNumber": 1
    }
  ],
  "meals": [
    {
      "id": 1,
      "name": "Dessert"
    }
  ]
}
```

### Create Recipe
- **Endpoint**: `POST /api/recipes`
- **Required Parameters**:
  - Request body with `Recipe` object containing all required fields
- **Description**: Creates a new recipe in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Apple Pie",
  "ingredients": [],
  "steps": [],
  "meals": []
}
```

### Update Recipe
- **Endpoint**: `PATCH /api/recipes/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Recipe ID to update
  - Request body with `Recipe` object containing fields to update
- **Description**: Updates specific fields of an existing recipe.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Updated Apple Pie Name",
  "ingredients": [],
  "steps": [],
  "meals": []
}
```

### Delete Recipe
- **Endpoint**: `DELETE /api/recipes/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Recipe ID to delete
- **Description**: Removes a recipe from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

### Get Recipe Choices
- **Endpoint**: `GET /api/recipes/choices`
- **Required Parameters**:
  - `mealId` (query parameter) - Meal ID to get recipes for
  - `userId` (query parameter) - User ID for filtering
  - `excludeWeeks` (query parameter) - Number of weeks to exclude from choices
- **Description**: Gets a selection of recipes for a specific meal, excluding recently used recipes for the user.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "Apple Pie",
    "ingredients": [],
    "steps": [],
    "meals": []
  }
]
```

## Recipe-Meal Relationships

### Get Recipe Meals
- **Endpoint**: `GET /api/recipe_meals`
- **Optional Parameters**:
  - `recipeId` (query parameter) - Filter by recipe ID
  - `mealId` (query parameter) - Filter by meal ID
- **Description**: Retrieves recipe-meal relationships, optionally filtered by recipe or meal.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "recipeId": 1,
    "mealId": 1
  }
]
```

### Get Recipe Meal by ID
- **Endpoint**: `GET /api/recipe_meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Recipe meal relationship ID
- **Description**: Retrieves a specific recipe-meal relationship by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "recipeId": 1,
  "mealId": 1
}
```

### Create Recipe Meal
- **Endpoint**: `POST /api/recipe_meals`
- **Required Parameters**:
  - Request body with `RecipeMeal` object containing all required fields
- **Description**: Creates a new recipe-meal relationship in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "recipeId": 1,
  "mealId": 1
}
```

### Delete Recipe Meal
- **Endpoint**: `DELETE /api/recipe_meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Recipe meal relationship ID to delete
- **Description**: Removes a recipe-meal relationship from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Recipe-Tag Relationships

### Get Recipe Tags
- **Endpoint**: `GET /api/recipe_tags`
- **Optional Parameters**:
  - `recipeId` (query parameter) - Filter by recipe ID
  - `tagId` (query parameter) - Filter by tag ID
- **Description**: Retrieves recipe-tag relationships, optionally filtered by recipe or tag.
- **Success Response Structure**:
```json
[
  {
    "recipeId": 1,
    "tagId": 1
  }
]
```

### Create Recipe Tag
- **Endpoint**: `POST /api/recipe_tags`
- **Required Parameters**:
  - Request body with `RecipeTag` object containing all required fields
- **Description**: Creates a new recipe-tag relationship in the database.
- **Success Response Structure**:
```json
{
  "recipeId": 1,
  "tagId": 1
}
```

### Delete Recipe Tag
- **Endpoint**: `DELETE /api/recipe_tags`
- **Required Parameters**:
  - `recipeId` (query parameter) - Recipe ID
  - `tagId` (query parameter) - Tag ID
- **Description**: Removes a recipe-tag relationship from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Step Management

### Get Steps
- **Endpoint**: `GET /api/steps`
- **Optional Parameters**:
  - `recipeId` (query parameter) - Filter by recipe ID
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves steps, optionally filtered by recipe with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "recipeId": 1,
    "instruction": "Mix ingredients",
    "stepNumber": 1
  }
]
```

### Get Step by ID
- **Endpoint**: `GET /api/steps/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Step ID
- **Description**: Retrieves a specific step by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "recipeId": 1,
  "instruction": "Mix ingredients",
  "stepNumber": 1
}
```

### Create Step
- **Endpoint**: `POST /api/steps`
- **Required Parameters**:
  - Request body with `Step` object containing all required fields
- **Description**: Creates a new step in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "recipeId": 1,
  "instruction": "Mix ingredients",
  "stepNumber": 1
}
```

### Update Step
- **Endpoint**: `PATCH /api/steps/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Step ID to update
  - Request body with `Step` object containing fields to update
- **Description**: Updates specific fields of an existing step.
- **Success Response Structure**:
```json
{
  "id": 1,
  "recipeId": 1,
  "instruction": "Updated instruction",
  "stepNumber": 2
}
```

### Delete Step
- **Endpoint**: `DELETE /api/steps/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Step ID to delete
- **Description**: Removes a step from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Tag Management

### Get Tags
- **Endpoint**: `GET /api/tags`
- **Optional Parameters**:
  - `name` (query parameter) - Filter by tag name (partial match)
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves all tags, optionally filtered by name with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "Dessert"
  }
]
```

### Get Tag by ID
- **Endpoint**: `GET /api/tags/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Tag ID
- **Description**: Retrieves a specific tag by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Dessert"
}
```

### Create Tag
- **Endpoint**: `POST /api/tags`
- **Required Parameters**:
  - Request body with `Tag` object containing all required fields
- **Description**: Creates a new tag in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Dessert"
}
```

### Update Tag
- **Endpoint**: `PATCH /api/tags/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Tag ID to update
  - Request body with `Tag` object containing fields to update
- **Description**: Updates specific fields of an existing tag.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Updated Dessert Name"
}
```

### Delete Tag
- **Endpoint**: `DELETE /api/tags/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Tag ID to delete
- **Description**: Removes a tag from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Unit Management

### Get Units
- **Endpoint**: `GET /api/units`
- **Optional Parameters**:
  - `name` (query parameter) - Filter by unit name (partial match)
  - `page` (query parameter, default: 25) - Page number for pagination
  - `perPage` (query parameter, default: 25) - Number of items per page
- **Description**: Retrieves all units, optionally filtered by name with pagination support.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "name": "cup",
    "volumeEquivalent": 240.0
  }
]
```

### Get Unit by ID
- **Endpoint**: `GET /api/units/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Unit ID
- **Description**: Retrieves a specific unit by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "cup",
  "volumeEquivalent": 240.0
}
```

### Create Unit
- **Endpoint**: `POST /api/units`
- **Required Parameters**:
  - Request body with `Unit` object containing all required fields
- **Description**: Creates a new unit in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "cup",
  "volumeEquivalent": 240.0
}
```

### Update Unit
- **Endpoint**: `PATCH /api/units/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Unit ID to update
  - Request body with `Unit` object containing fields to update
- **Description**: Updates specific fields of an existing unit.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "Updated Cup Name",
  "volumeEquivalent": 250.0
}
```

### Delete Unit
- **Endpoint**: `DELETE /api/units/{id}`
- **Required Parameters**:
  - `id` (path parameter) - Unit ID to delete
- **Description**: Removes a unit from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Current User

### Get Current User Profile
- **Endpoint**: `GET /auth/me`
- **Required Headers**:
  - `Authorization: Bearer <your-jwt-token>` - Valid JWT token from login
- **Description**: Retrieves the current authenticated user's profile information. The response includes user details but excludes the password for security.
- **Success Response Structure**:
```json
{
  "id": 1,
  "name": "JohnDoe",
  "email": "user@example.com",
  "last_login": "2023-01-01T12:00:00"
}
```
- **Error Response Examples**:
```json
{
  "error": "Invalid authorization header or token"
}
```

## User Management

### User Signup
- **Endpoint**: `POST /api/users/signup`
- **Required Parameters**:
  - `email` (query parameter) - User's email address
  - `password` (query parameter) - User's password
  - `name` (query parameter) - User's display name
- **Description**: Creates a new user account and returns an authentication token.
- **Success Response Structure**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### User Login
- **Endpoint**: `POST /api/users/login`
- **Required Parameters**:
  - `email` (query parameter) - User's email address
  - `password` (query parameter) - User's password
- **Description**: Authenticates a user and returns a JWT token for subsequent requests.
- **Success Response Structure**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## User-Meal Tracking

### Get User Meals
- **Endpoint**: `GET /api/user_meals`
- **Required Parameters**:
  - `userId` (query parameter) - User ID
- **Optional Parameters**:
  - `startDate` (query parameter) - Start date for filtering meals
  - `endDate` (query parameter) - End date for filtering meals
- **Description**: Retrieves meals associated with a specific user, optionally filtered by date range.
- **Success Response Structure**:
```json
[
  {
    "id": 1,
    "userId": 1,
    "recipeId": 1,
    "mealId": 1,
    "mealDate": "2023-01-01T00:00:00"
  }
]
```

### Get User Meal by ID
- **Endpoint**: `GET /api/user_meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - User meal tracking ID
- **Description**: Retrieves a specific user meal tracking record by its ID.
- **Success Response Structure**:
```json
{
  "id": 1,
  "userId": 1,
  "recipeId": 1,
  "mealId": 1,
  "mealDate": "2023-01-01T00:00:00"
}
```

### Create User Meal
- **Endpoint**: `POST /api/user_meals`
- **Required Parameters**:
  - Request body with `UserMeal` object containing all required fields
- **Description**: Creates a new user meal tracking record in the database.
- **Success Response Structure**:
```json
{
  "id": 1,
  "userId": 1,
  "recipeId": 1,
  "mealId": 1,
  "mealDate": "2023-01-01T00:00:00"
}
```

### Delete User Meal
- **Endpoint**: `DELETE /api/user_meals/{id}`
- **Required Parameters**:
  - `id` (path parameter) - User meal tracking ID to delete
- **Description**: Removes a user meal tracking record from the database.
- **Success Response Structure**:
```json
{
  "deleted": true
}
```

## Pagination

Most GET endpoints support pagination to handle large datasets efficiently. When pagination is supported, the following query parameters are available:

### Pagination Parameters

- `page` (query parameter) - Page number (default: 1, minimum: 1)
- `perPage` (query parameter) - Number of items per page (default: varies by endpoint, minimum: 1, maximum: 100)

### Pagination Examples

```bash
# Get first page with 10 items per page
GET /api/food?page=1&perPage=10

# Get third page with 25 items per page
GET /api/recipes?page=3&perPage=25

# Get all food types with default pagination (page 25, 25 items per page)
GET /api/food_type
```

### Pagination Best Practices

- Use appropriate `perPage` values based on your needs (smaller values for mobile, larger for desktop)
- Implement client-side caching to reduce API calls
- Handle pagination errors gracefully (invalid page numbers, negative values)
- Consider using cursor-based pagination for real-time applications

## Error Handling

The API returns standard HTTP status codes:
- `200` - Success
- `201` - Created
- `400` - Bad Request (invalid parameters)
- `401` - Unauthorized (missing or invalid token)
- `404` - Not Found
- `500` - Internal Server Error

Error responses include a JSON object with an `error` field containing a descriptive message.

```json
{
  "error": "Invalid page number. Page must be greater than 0."
}
```