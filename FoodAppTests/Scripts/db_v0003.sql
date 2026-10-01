ALTER TABLE refresh_token DROP CONSTRAINT IF EXISTS FK_refresh_token_users;
GO
ALTER TABLE refresh_token ADD CONSTRAINT FK_refresh_token_users FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE;
GO

CREATE TABLE Kitchens(
	id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL PRIMARY KEY,
	name varchar(50) NOT NULL,
	ownerId UNIQUEIDENTIFIER NOT NULL,
	updated_at datetime2(7) DEFAULT SYSUTCDATETIME() NOT NULL,

	CONSTRAINT FK_kitchens_owner FOREIGN KEY (ownerId) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE kitchen_role(
	id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL PRIMARY KEY,
	name VARCHAR(20) NOT NULL,
	updated_at datetime2(7) DEFAULT SYSUTCDATETIME() NOT NULL,
);

CREATE TABLE kitchens_users(
	id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL PRIMARY KEY,
	userId UNIQUEIDENTIFIER NOT NULL,
	kitchenId UNIQUEIDENTIFIER NOT NULL,
	roleId UNIQUEIDENTIFIER NOT NULL,
	updated_at datetime2(7) DEFAULT SYSUTCDATETIME() NOT NULL,

	CONSTRAINT FK_kichens_users_user FOREIGN KEY (userid) REFERENCES users(id),
	CONSTRAINT FK_kichens_users_kitchen FOREIGN KEY (kitchenId) REFERENCES Kitchens(id) ON DELETE CASCADE
);


INSERT INTO Kitchens(name, ownerId) SELECT u.name, u.id FROM users u;
INSERT INTO Kitchen_role(name) VALUES('owner'), ('inspector'),( 'editor'), ('admin');
INSERT INTO kitchens_users(userId, kitchenId, roleId) SELECT k.ownerId, k.id, r.id FROM Kitchens k CROSS JOIN Kitchen_role r WHERE r.name='owner';

ALTER TABLE user_meals DROP CONSTRAINT FK_user_user_meals;
ALTER TABLE user_meals ADD kitchenId UNIQUEIDENTIFIER NULL;
GO;
UPDATE user_meals SET kitchenId = (SELECT k.id FROM Kitchens k WHERE k.ownerId = user_meals.userid);
GO;
ALTER TABLE user_meals ADD CONSTRAINT FK_kitchen_kitchens_meal FOREIGN KEY (kitchenId) REFERENCES Kitchens(id) ON DELETE CASCADE;
GO;
ALTER TABLE user_meals ALTER COLUMN kitchenId UNIQUEIDENTIFIER NOT NULL;
GO;
ALTER TABLE user_meals DROP COLUMN userId;