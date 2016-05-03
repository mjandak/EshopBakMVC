CREATE TABLE [dbo].[Category] (
    [id]        INT          NOT NULL,
    [title]     VARCHAR (50) NOT NULL,
    [parent_id] INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Product]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [title] NVARCHAR(50) NOT NULL, 
    [price] DECIMAL(9, 2) NOT NULL, 
    [small_image_url] NVARCHAR(50) NULL, 
    [large_image_url] NVARCHAR(50) NULL, 
    [description] NTEXT NULL, 
    [producer] NVARCHAR(50) NOT NULL, 
    [special] BIT NOT NULL
)

CREATE TABLE [dbo].[ProductCategory]
(
	[product_id] INT NOT NULL , 
    [category_id] INT NOT NULL, 
    PRIMARY KEY ([category_id], [product_id]), 
    CONSTRAINT [FK_ProductCategory_Product] FOREIGN KEY ([product_id]) REFERENCES [Product]([id]), 
    CONSTRAINT [FK_ProductCategory_Category] FOREIGN KEY ([category_id]) REFERENCES [Category]([id])
)

CREATE TABLE [dbo].[ShoppingCart]
(
	[Id]     INT           NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [UserId] NVARCHAR(128) NOT NULL,
	CONSTRAINT [AK_dbo.ShoppingCart] UNIQUE(UserId) --uživatel má jen jeden košík, nejsou dva košíky ukazující na stejného uživatele
)

CREATE TABLE [dbo].[CartItem] (
    [ProductId] INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Title]     NVARCHAR (100) NOT NULL,
    [Price]     DECIMAL (18)   NOT NULL,
    [Quantity]  INT            NOT NULL,
    [CartId]    INT            NULL,
    CONSTRAINT [FK_CartItem_ShoppingCart] FOREIGN KEY ([CartId]) REFERENCES [dbo].[ShoppingCart] ([Id])
);

