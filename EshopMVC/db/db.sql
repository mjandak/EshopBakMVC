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
);

CREATE TABLE [dbo].[ProductCategory]
(
	[product_id] INT NOT NULL , 
    [category_id] INT NOT NULL, 
    PRIMARY KEY ([category_id], [product_id]), 
    CONSTRAINT [FK_ProductCategory_Product] FOREIGN KEY ([product_id]) REFERENCES [Product]([id]), 
    CONSTRAINT [FK_ProductCategory_Category] FOREIGN KEY ([category_id]) REFERENCES [Category]([id])
);

CREATE TABLE [dbo].[ShoppingCart]
(
	[Id]     INT           NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [UserId] NVARCHAR(128) NOT NULL, --key to AspNetUsers
	CONSTRAINT [AK_dbo.ShoppingCart] UNIQUE(UserId) --a user has one cart, max one cart pointing to same user
);

CREATE TABLE [dbo].[CartProduct] (
    [ProductId] INT NOT NULL,
    [CartId]    INT NOT NULL,
    [Quantity]  INT NOT NULL,
    CONSTRAINT [FK_CartProduct_ShoppingCart] FOREIGN KEY ([CartId]) REFERENCES [dbo].[ShoppingCart] ([Id]),
    CONSTRAINT [FK_CartProduct_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([id]), 
    CONSTRAINT [PK_CartProduct] PRIMARY KEY ([CartId], [ProductId]) --one row for a product in same cart
);

CREATE TABLE [dbo].[Order] (
    [Id]         INT            NOT NULL IDENTITY(1,1),
    [UserId]     NVARCHAR (128) NOT NULL, --key to AspNetUsers
    [CreateDate] DATETIME       NOT NULL,
    [State]      NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[OrderProduct] (
    [OrderId]   INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity]  INT DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_OrderProduct] PRIMARY KEY CLUSTERED ([OrderId] ASC, [ProductId] ASC),
    CONSTRAINT [FK_OrderProduct_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]),
    CONSTRAINT [FK_OrderProduct_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([id])
);
