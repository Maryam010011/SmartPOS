-- ============================================================
-- SmartPOS+ — Full Database Seed Script (Fixed)
-- Database: SmartPOS_DB (localhost SQL Server)
-- Run in SSMS: Open → Execute (F5)
-- Password for all users: Admin@123 (BCrypt hash below)
-- ============================================================

USE SmartPOS_DB;
GO

-- ============================================================
-- ROLES
-- ============================================================
SET IDENTITY_INSERT Roles ON;
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 1)
    INSERT INTO Roles (Id, Name) VALUES (1, N'Admin');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 2)
    INSERT INTO Roles (Id, Name) VALUES (2, N'Manager');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 3)
    INSERT INTO Roles (Id, Name) VALUES (3, N'Cashier');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 4)
    INSERT INTO Roles (Id, Name) VALUES (4, N'Customer');
SET IDENTITY_INSERT Roles OFF;
GO

-- ============================================================
-- PERMISSIONS (9 modules x 4 roles)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE RoleId = 1 AND Module = N'Products')
BEGIN
    INSERT INTO Permissions (RoleId, Module, CanCreate, CanRead, CanUpdate, CanDelete) VALUES
        (1,N'Products',      1,1,1,1),(1,N'Sales',         1,1,1,1),
        (1,N'Inventory',     1,1,1,1),(1,N'Users',          1,1,1,1),
        (1,N'Promotions',    1,1,1,1),(1,N'PurchaseOrders', 1,1,1,1),
        (1,N'Customers',     1,1,1,1),(1,N'Reviews',        1,1,1,1),
        (1,N'Analytics',     0,1,0,0),
        (2,N'Products',      1,1,1,0),(2,N'Sales',          0,1,0,0),
        (2,N'Inventory',     1,1,1,0),(2,N'Users',          0,1,0,0),
        (2,N'Promotions',    1,1,1,1),(2,N'PurchaseOrders', 1,1,1,0),
        (2,N'Customers',     0,1,1,0),(2,N'Reviews',        0,1,0,0),
        (2,N'Analytics',     0,1,0,0),
        (3,N'Products',      0,1,0,0),(3,N'Sales',          1,1,0,0),
        (3,N'Inventory',     0,1,0,0),(3,N'Users',          0,0,0,0),
        (3,N'Promotions',    0,1,0,0),(3,N'PurchaseOrders', 0,0,0,0),
        (3,N'Customers',     0,1,0,0),(3,N'Reviews',        0,1,0,0),
        (3,N'Analytics',     0,0,0,0),
        (4,N'Products',      0,1,0,0),(4,N'Sales',          1,1,0,0),
        (4,N'Inventory',     0,0,0,0),(4,N'Users',          0,0,1,0),
        (4,N'Promotions',    0,1,0,0),(4,N'PurchaseOrders', 0,0,0,0),
        (4,N'Customers',     0,1,1,0),(4,N'Reviews',        1,1,1,0),
        (4,N'Analytics',     0,0,0,0);
END
GO

-- ============================================================
-- USERS (BCrypt of Admin@123)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'admin@smartpos.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Ayesha Tariq',N'admin@smartpos.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',1,1,'2025-01-01 08:00:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'manager@smartpos.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Bilal Ahmed Khan',N'manager@smartpos.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',2,1,'2025-01-02 09:00:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'manager2@smartpos.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Sana Mirza',N'manager2@smartpos.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',2,1,'2025-01-05 09:30:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'cashier1@smartpos.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Usman Raza',N'cashier1@smartpos.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',3,1,'2025-01-10 10:00:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'cashier2@smartpos.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Hina Shahid',N'cashier2@smartpos.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',3,1,'2025-01-12 10:30:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'zara.hussain@email.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Zara Hussain',N'zara.hussain@email.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',4,1,'2025-02-01 11:00:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'ali.nawaz@email.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Ali Nawaz',N'ali.nawaz@email.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',4,1,'2025-02-05 11:30:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'maria.khan@email.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Maria Khan',N'maria.khan@email.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',4,1,'2025-02-10 12:00:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'fahad.malik@email.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Fahad Malik',N'fahad.malik@email.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',4,1,'2025-02-15 09:00:00');

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'nadia.iqbal@email.com')
    INSERT INTO Users (Name,Email,PasswordHash,RoleId,IsActive,CreatedAt) VALUES
    (N'Nadia Iqbal',N'nadia.iqbal@email.com',
     N'$2a$11$K5p2Y3mQvL8nRtXwZuH1eOdJgFqNsPbMcUiAhTlEvY7kW0xGrD9C2',4,1,'2025-02-20 14:00:00');
GO

-- ============================================================
-- CUSTOMERS (no IsActive column in DB)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = N'zara.hussain@email.com')
    INSERT INTO Customers (UserId,Name,Email,Phone,DateOfBirth,Address,LoyaltyPoints,TotalSpent,CreatedAt)
    SELECT Id,N'Zara Hussain',N'zara.hussain@email.com',N'03001234567',
           '1995-04-12',N'House 14, Street 5, F-7/2, Islamabad',
           320,8450.00,'2025-02-01 11:00:00'
    FROM Users WHERE Email=N'zara.hussain@email.com';

IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = N'ali.nawaz@email.com')
    INSERT INTO Customers (UserId,Name,Email,Phone,DateOfBirth,Address,LoyaltyPoints,TotalSpent,CreatedAt)
    SELECT Id,N'Ali Nawaz',N'ali.nawaz@email.com',N'03211112233',
           '1990-08-25',N'Flat 3B, Block 6, PECHS, Karachi',
           185,4600.00,'2025-02-05 11:30:00'
    FROM Users WHERE Email=N'ali.nawaz@email.com';

IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = N'maria.khan@email.com')
    INSERT INTO Customers (UserId,Name,Email,Phone,DateOfBirth,Address,LoyaltyPoints,TotalSpent,CreatedAt)
    SELECT Id,N'Maria Khan',N'maria.khan@email.com',N'03339988776',
           '1998-11-03',N'Plot 22, Johar Town, Lahore',
           540,13500.00,'2025-02-10 12:00:00'
    FROM Users WHERE Email=N'maria.khan@email.com';

IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = N'fahad.malik@email.com')
    INSERT INTO Customers (UserId,Name,Email,Phone,DateOfBirth,Address,LoyaltyPoints,TotalSpent,CreatedAt)
    SELECT Id,N'Fahad Malik',N'fahad.malik@email.com',N'03451234000',
           '1988-03-17',N'House 9, Lane 4, DHA Phase 2, Islamabad',
           90,2250.00,'2025-02-15 09:00:00'
    FROM Users WHERE Email=N'fahad.malik@email.com';

IF NOT EXISTS (SELECT 1 FROM Customers WHERE Email = N'nadia.iqbal@email.com')
    INSERT INTO Customers (UserId,Name,Email,Phone,DateOfBirth,Address,LoyaltyPoints,TotalSpent,CreatedAt)
    SELECT Id,N'Nadia Iqbal',N'nadia.iqbal@email.com',N'03126543210',
           '2000-07-22',N'Room 7, Hostel Block C, Air University, Islamabad',
           60,1500.00,'2025-02-20 14:00:00'
    FROM Users WHERE Email=N'nadia.iqbal@email.com';
GO

-- ============================================================
-- SUPPLIERS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email=N'orders@islamabadbakersupply.pk')
    INSERT INTO Suppliers (Name,ContactPerson,ContactNo,Email,Address,IsActive) VALUES
    (N'Islamabad Baker Supply Co.',N'Tariq Mehmood',N'051-2345678',
     N'orders@islamabadbakersupply.pk',N'Plot 45, I-9 Industrial Area, Islamabad',1);

IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email=N'sales@khi-flour-mills.pk')
    INSERT INTO Suppliers (Name,ContactPerson,ContactNo,Email,Address,IsActive) VALUES
    (N'Karachi Flour Mills Ltd.',N'Rashid Siddiqui',N'021-3456789',
     N'sales@khi-flour-mills.pk',N'Korangi Industrial Area, Karachi',1);

IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email=N'dairy@lahore-fresh.pk')
    INSERT INTO Suppliers (Name,ContactPerson,ContactNo,Email,Address,IsActive) VALUES
    (N'Lahore Fresh Dairy Products',N'Amna Butt',N'042-7654321',
     N'dairy@lahore-fresh.pk',N'Township, Lahore',1);

IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email=N'info@sweetingredients.pk')
    INSERT INTO Suppliers (Name,ContactPerson,ContactNo,Email,Address,IsActive) VALUES
    (N'Sweet Ingredients Pvt. Ltd.',N'Kamran Sheikh',N'051-9988776',
     N'info@sweetingredients.pk',N'G-8 Markaz, Islamabad',1);

IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Email=N'packaging@pakboxes.pk')
    INSERT INTO Suppliers (Name,ContactPerson,ContactNo,Email,Address,IsActive) VALUES
    (N'PakBoxes Packaging Solutions',N'Saima Rao',N'042-1122334',
     N'packaging@pakboxes.pk',N'Gulberg III, Lahore',1);
GO

-- ============================================================
-- CATEGORIES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Bakery')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL) VALUES
    (N'Bakery',NULL,N'All baked goods including breads, cakes, and pastries',
     N'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Beverages')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL) VALUES
    (N'Beverages',NULL,N'Hot and cold drinks available at the café counter',
     N'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=400');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Cakes')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL)
    SELECT N'Cakes',Id,N'Whole cakes for birthdays, weddings, and celebrations',
           N'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400'
    FROM Categories WHERE Name=N'Bakery';

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Breads')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL)
    SELECT N'Breads',Id,N'Fresh baked loaves, rolls, and flatbreads',
           N'https://images.unsplash.com/photo-1549931319-a545dcf3bc73?w=400'
    FROM Categories WHERE Name=N'Bakery';

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Pastries')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL)
    SELECT N'Pastries',Id,N'Croissants, danishes, puffs, and flaky pastries',
           N'https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=400'
    FROM Categories WHERE Name=N'Bakery';

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Cookies')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL)
    SELECT N'Cookies',Id,N'Baked cookies and biscuits in multiple flavours',
           N'https://images.unsplash.com/photo-1499636136210-6f4ee915583e?w=400'
    FROM Categories WHERE Name=N'Bakery';

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Hot Drinks')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL)
    SELECT N'Hot Drinks',Id,N'Tea, coffee, and hot chocolate',
           N'https://images.unsplash.com/photo-1512568400610-62da28bc8a13?w=400'
    FROM Categories WHERE Name=N'Beverages';

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name=N'Cold Drinks')
    INSERT INTO Categories (Name,ParentCategoryId,Description,ImageURL)
    SELECT N'Cold Drinks',Id,N'Iced beverages, juices, and smoothies',
           N'https://images.unsplash.com/photo-1437418747212-8d9709afab22?w=400'
    FROM Categories WHERE Name=N'Beverages';
GO

-- ============================================================
-- PRODUCTS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BRD-001')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Multigrain Loaf',N'BRD-001',N'Freshly baked multigrain loaf with sesame and sunflower seeds. 500g.',
           N'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400',
           280.00,140.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Breads'),
           (SELECT Id FROM Suppliers WHERE Email=N'orders@islamabadbakersupply.pk'),
           '2025-01-15 08:00:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BRD-002')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'White Sandwich Bread',N'BRD-002',N'Classic soft white sandwich bread, sliced. 400g.',
           N'https://images.unsplash.com/photo-1598373182133-52452f7691ef?w=400',
           180.00,85.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Breads'),
           (SELECT Id FROM Suppliers WHERE Email=N'sales@khi-flour-mills.pk'),
           '2025-01-15 08:10:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BRD-003')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Sourdough Artisan Loaf',N'BRD-003',N'Long-fermented sourdough with crispy crust and chewy interior. 600g.',
           N'https://images.unsplash.com/photo-1585478259715-4d3a29e01f3c?w=400',
           450.00,200.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Breads'),
           (SELECT Id FROM Suppliers WHERE Email=N'orders@islamabadbakersupply.pk'),
           '2025-01-15 08:20:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'CKE-001')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Chocolate Fudge Cake (1lb)',N'CKE-001',N'Rich dark chocolate fudge cake with ganache frosting. 1 lb.',
           N'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400',
           1200.00,550.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cakes'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-15 09:00:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'CKE-002')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Red Velvet Cake (1lb)',N'CKE-002',N'Classic red velvet with cream cheese frosting. 1 lb.',
           N'https://images.unsplash.com/photo-1464349095431-e9a21285b5f3?w=400',
           1350.00,620.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cakes'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-15 09:10:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'CKE-003')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Black Forest Cake (2lb)',N'CKE-003',N'German-style black forest cake with cherries and whipped cream. 2 lb.',
           N'https://images.unsplash.com/photo-1606890737304-57a1ca8a5b62?w=400',
           2400.00,1100.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cakes'),
           (SELECT Id FROM Suppliers WHERE Email=N'dairy@lahore-fresh.pk'),
           '2025-01-15 09:20:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'CKE-004')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Pineapple Cake (1lb)',N'CKE-004',N'Light sponge cake with pineapple cream and fresh pineapple topping. 1 lb.',
           N'https://images.unsplash.com/photo-1565661793736-51c32a9cf7ec?w=400',
           1100.00,490.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cakes'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-20 09:00:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'PST-001')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Butter Croissant',N'PST-001',N'Flaky all-butter French croissant, freshly baked each morning.',
           N'https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=400',
           180.00,75.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Pastries'),
           (SELECT Id FROM Suppliers WHERE Email=N'orders@islamabadbakersupply.pk'),
           '2025-01-15 10:00:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'PST-002')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Almond Danish Pastry',N'PST-002',N'Sweet flaky pastry filled with almond cream and topped with flaked almonds.',
           N'https://images.unsplash.com/photo-1509365465985-25d11c17e812?w=400',
           220.00,95.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Pastries'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-15 10:10:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'PST-003')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Chicken Puff Pastry',N'PST-003',N'Savoury puff pastry filled with spiced chicken and onions.',
           N'https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=400',
           250.00,110.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Pastries'),
           (SELECT Id FROM Suppliers WHERE Email=N'orders@islamabadbakersupply.pk'),
           '2025-01-15 10:20:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'COK-001')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Chocolate Chip Cookies (6 pcs)',N'COK-001',N'Classic American-style chocolate chip cookies, 6 pieces per pack.',
           N'https://images.unsplash.com/photo-1499636136210-6f4ee915583e?w=400',
           320.00,140.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cookies'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-15 11:00:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'COK-002')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Oreo Cheesecake Cookies (4 pcs)',N'COK-002',N'Stuffed cookies with oreo-cheesecake filling, 4 pieces per pack.',
           N'https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=400',
           420.00,190.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cookies'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-15 11:10:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'COK-003')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Oatmeal Raisin Cookies (6 pcs)',N'COK-003',N'Healthy oatmeal cookies with raisins and cinnamon, 6 pieces per pack.',
           N'https://images.unsplash.com/photo-1606190538257-9f3a95f4a4a6?w=400',
           280.00,120.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cookies'),
           (SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk'),
           '2025-01-15 11:20:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BEV-001')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Cappuccino',N'BEV-001',N'Italian-style cappuccino with steamed milk foam. Made with Arabica beans.',
           N'https://images.unsplash.com/photo-1541167760496-1628856ab772?w=400',
           350.00,120.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Hot Drinks'),
           NULL,'2025-01-15 12:00:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BEV-002')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Doodh Patti Chai',N'BEV-002',N'Traditional Pakistani milk tea brewed strong with cardamom.',
           N'https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400',
           150.00,40.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Hot Drinks'),
           (SELECT Id FROM Suppliers WHERE Email=N'dairy@lahore-fresh.pk'),
           '2025-01-15 12:10:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BEV-003')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Iced Lemon Tea',N'BEV-003',N'Chilled black tea with fresh lemon and mint leaves.',
           N'https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400',
           200.00,60.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cold Drinks'),
           NULL,'2025-01-15 12:20:00';

IF NOT EXISTS (SELECT 1 FROM Products WHERE SKU=N'BEV-004')
    INSERT INTO Products (Name,SKU,Description,ImageURL,Price,CostPrice,IsActive,CategoryId,SupplierId,CreatedAt)
    SELECT N'Strawberry Smoothie',N'BEV-004',N'Fresh strawberry and yogurt smoothie, no added sugar.',
           N'https://images.unsplash.com/photo-1553530666-ba11a7da3888?w=400',
           380.00,150.00,1,
           (SELECT Id FROM Categories WHERE Name=N'Cold Drinks'),
           (SELECT Id FROM Suppliers WHERE Email=N'dairy@lahore-fresh.pk'),
           '2025-01-15 12:30:00';
GO

-- ============================================================
-- INVENTORIES
-- ============================================================
INSERT INTO Inventories (ProductId,Quantity,ReorderLevel,LastUpdated)
SELECT p.Id,
    CASE p.SKU
        WHEN N'BRD-001' THEN 45  WHEN N'BRD-002' THEN 60  WHEN N'BRD-003' THEN 20
        WHEN N'CKE-001' THEN 8   WHEN N'CKE-002' THEN 6   WHEN N'CKE-003' THEN 4
        WHEN N'CKE-004' THEN 5   WHEN N'PST-001' THEN 80  WHEN N'PST-002' THEN 35
        WHEN N'PST-003' THEN 40  WHEN N'COK-001' THEN 50  WHEN N'COK-002' THEN 30
        WHEN N'COK-003' THEN 45  WHEN N'BEV-001' THEN 100 WHEN N'BEV-002' THEN 100
        WHEN N'BEV-003' THEN 80  WHEN N'BEV-004' THEN 60  ELSE 20
    END,
    CASE p.SKU
        WHEN N'BRD-001' THEN 10  WHEN N'BRD-002' THEN 15  WHEN N'BRD-003' THEN 5
        WHEN N'CKE-001' THEN 3   WHEN N'CKE-002' THEN 3   WHEN N'CKE-003' THEN 2
        WHEN N'CKE-004' THEN 2   WHEN N'PST-001' THEN 20  WHEN N'PST-002' THEN 10
        WHEN N'PST-003' THEN 10  WHEN N'COK-001' THEN 10  WHEN N'COK-002' THEN 8
        WHEN N'COK-003' THEN 10  WHEN N'BEV-001' THEN 20  WHEN N'BEV-002' THEN 20
        WHEN N'BEV-003' THEN 15  WHEN N'BEV-004' THEN 15  ELSE 5
    END,
    GETUTCDATE()
FROM Products p
WHERE NOT EXISTS (SELECT 1 FROM Inventories i WHERE i.ProductId = p.Id);
GO

-- ============================================================
-- PROMOTIONS (no Description or CreatedAt in DB)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code=N'WELCOME15')
    INSERT INTO Promotions (Code,DiscountType,Value,MinOrderValue,MaxUsageLimit,UsageCount,ValidFrom,ValidTo,IsActive)
    VALUES (N'WELCOME15',0,15.00,500.00,200,47,'2025-01-01','2025-12-31',1);

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code=N'FLAT100')
    INSERT INTO Promotions (Code,DiscountType,Value,MinOrderValue,MaxUsageLimit,UsageCount,ValidFrom,ValidTo,IsActive)
    VALUES (N'FLAT100',1,100.00,800.00,500,123,'2025-01-15','2025-06-30',1);

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code=N'EID25')
    INSERT INTO Promotions (Code,DiscountType,Value,MinOrderValue,MaxUsageLimit,UsageCount,ValidFrom,ValidTo,IsActive)
    VALUES (N'EID25',0,25.00,1000.00,100,89,'2025-03-28','2025-04-05',0);

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code=N'WEEKEND10')
    INSERT INTO Promotions (Code,DiscountType,Value,MinOrderValue,MaxUsageLimit,UsageCount,ValidFrom,ValidTo,IsActive)
    VALUES (N'WEEKEND10',0,10.00,300.00,NULL,210,'2025-02-01','2025-12-31',1);

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code=N'LOYALTY200')
    INSERT INTO Promotions (Code,DiscountType,Value,MinOrderValue,MaxUsageLimit,UsageCount,ValidFrom,ValidTo,IsActive)
    VALUES (N'LOYALTY200',1,200.00,1500.00,NULL,55,'2025-01-01','2025-12-31',1);
GO

-- ============================================================
-- SALES + SALEITEMS + PAYMENTS
-- SaleType: 0=Onsite 1=Online | SaleStatus: 0=Completed 1=Voided 2=Refunded
-- PaymentMethod: 0=Cash 1=Card 2=Online | PaymentStatus: 0=Pending 1=Completed 3=Refunded
-- ============================================================
DECLARE @C1 INT=(SELECT Id FROM Users WHERE Email=N'cashier1@smartpos.com');
DECLARE @C2 INT=(SELECT Id FROM Users WHERE Email=N'cashier2@smartpos.com');
DECLARE @Cu1 INT=(SELECT Id FROM Customers WHERE Email=N'zara.hussain@email.com');
DECLARE @Cu2 INT=(SELECT Id FROM Customers WHERE Email=N'ali.nawaz@email.com');
DECLARE @Cu3 INT=(SELECT Id FROM Customers WHERE Email=N'maria.khan@email.com');
DECLARE @Cu4 INT=(SELECT Id FROM Customers WHERE Email=N'fahad.malik@email.com');
DECLARE @Cu5 INT=(SELECT Id FROM Customers WHERE Email=N'nadia.iqbal@email.com');
DECLARE @PFlat INT=(SELECT Id FROM Promotions WHERE Code=N'FLAT100');
DECLARE @PWknd INT=(SELECT Id FROM Promotions WHERE Code=N'WEEKEND10');
DECLARE @PLoyal INT=(SELECT Id FROM Promotions WHERE Code=N'LOYALTY200');
DECLARE @PWelc INT=(SELECT Id FROM Promotions WHERE Code=N'WELCOME15');

DECLARE @Brd1 INT=(SELECT Id FROM Products WHERE SKU=N'BRD-001');
DECLARE @Brd2 INT=(SELECT Id FROM Products WHERE SKU=N'BRD-002');
DECLARE @Brd3 INT=(SELECT Id FROM Products WHERE SKU=N'BRD-003');
DECLARE @Cke1 INT=(SELECT Id FROM Products WHERE SKU=N'CKE-001');
DECLARE @Cke2 INT=(SELECT Id FROM Products WHERE SKU=N'CKE-002');
DECLARE @Cke3 INT=(SELECT Id FROM Products WHERE SKU=N'CKE-003');
DECLARE @Cke4 INT=(SELECT Id FROM Products WHERE SKU=N'CKE-004');
DECLARE @Pst1 INT=(SELECT Id FROM Products WHERE SKU=N'PST-001');
DECLARE @Pst2 INT=(SELECT Id FROM Products WHERE SKU=N'PST-002');
DECLARE @Pst3 INT=(SELECT Id FROM Products WHERE SKU=N'PST-003');
DECLARE @Cok1 INT=(SELECT Id FROM Products WHERE SKU=N'COK-001');
DECLARE @Cok2 INT=(SELECT Id FROM Products WHERE SKU=N'COK-002');
DECLARE @Cok3 INT=(SELECT Id FROM Products WHERE SKU=N'COK-003');
DECLARE @Bev1 INT=(SELECT Id FROM Products WHERE SKU=N'BEV-001');
DECLARE @Bev2 INT=(SELECT Id FROM Products WHERE SKU=N'BEV-002');
DECLARE @Bev3 INT=(SELECT Id FROM Products WHERE SKU=N'BEV-003');
DECLARE @Bev4 INT=(SELECT Id FROM Products WHERE SKU=N'BEV-004');

DECLARE @SID INT;

-- Sale 1: Onsite, Zara — croissants + chai, Cash
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-01 09:15:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu1,@C1,NULL,0,503.55,0.00,63.55,'2025-03-01 09:15:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Pst1,2,180.00,360.00),(@SID,@Bev2,1,150.00,150.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,0,503.55,1,NULL,'2025-03-01 09:16:00');
END

-- Sale 2: Online, Maria — chocolate cake, Card + LOYALTY200
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-02 14:30:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu3,@C1,@PLoyal,1,1287.00,200.00,187.00,'2025-03-02 14:30:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES(@SID,@Cke1,1,1200.00,1200.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,1,1287.00,1,N'ch_3PkR2gLkdIwHu7A80xBq1234','2025-03-02 14:31:00');
END

-- Sale 3: Onsite, guest — bread + cookies, Cash
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-03 10:00:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(NULL,@C2,NULL,0,595.95,0.00,75.95,'2025-03-03 10:00:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Brd2,2,180.00,360.00),(@SID,@Cok1,1,320.00,320.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,0,595.95,1,NULL,'2025-03-03 10:01:00');
END

-- Sale 4: Online, Ali — red velvet + smoothie, Online + WELCOME15
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-05 18:00:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu2,@C1,@PWelc,1,1748.10,262.50,210.60,'2025-03-05 18:00:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Cke2,1,1350.00,1350.00),(@SID,@Bev4,1,380.00,380.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,2,1748.10,1,N'pi_3Pl9WmKsdUwJg8B90yNm5678','2025-03-05 18:02:00');
END

-- Sale 5: Onsite, Fahad — sourdough + cappuccino + almond danish, Card
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-08 08:45:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu4,@C2,NULL,0,1186.75,0.00,149.75,'2025-03-08 08:45:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Brd3,1,450.00,450.00),(@SID,@Bev1,1,350.00,350.00),(@SID,@Pst2,1,220.00,220.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,1,1186.75,1,N'ch_3Qm1ArLxeJxIv6C50zA9012','2025-03-08 08:46:00');
END

-- Sale 6: Onsite, Nadia — cookies + chai, Cash + FLAT100
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-10 16:20:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu5,@C1,@PFlat,0,689.65,100.00,89.65,'2025-03-10 16:20:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Cok2,1,420.00,420.00),(@SID,@Bev2,2,150.00,300.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,0,689.65,1,NULL,'2025-03-10 16:21:00');
END

-- Sale 7: Online, Maria — black forest + pineapple cake, Online + WEEKEND10
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-15 20:00:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu3,@C1,@PWknd,1,3451.50,350.00,501.50,'2025-03-15 20:00:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Cke3,1,2400.00,2400.00),(@SID,@Cke4,1,1100.00,1100.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,2,3451.50,1,N'pi_3Rn2BsMyeKyJw7D60bB3456','2025-03-15 20:01:00');
END

-- Sale 8: Onsite, Zara — multigrain + chicken puff, Cash
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-18 11:30:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu1,@C2,NULL,0,837.65,0.00,107.65,'2025-03-18 11:30:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Brd1,2,280.00,560.00),(@SID,@Pst3,1,250.00,250.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,0,837.65,1,NULL,'2025-03-18 11:31:00');
END

-- Sale 9: Voided — cashier error
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-20 09:05:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(NULL,@C1,NULL,0,350.35,0.00,44.35,'2025-03-20 09:05:00',1);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES(@SID,@Bev1,1,350.00,350.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,0,350.35,3,NULL,NULL);
END

-- Sale 10: Online, Ali — oatmeal cookies + iced tea, Online
IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleDate='2025-03-22 17:45:00')
BEGIN
    INSERT INTO Sales(CustomerId,UserId,PromoId,SaleType,TotalAmount,DiscountAmount,TaxAmount,SaleDate,Status)
    VALUES(@Cu2,@C2,NULL,1,644.65,0.00,84.65,'2025-03-22 17:45:00',0);
    SET @SID=SCOPE_IDENTITY();
    INSERT INTO SaleItems(SaleId,ProductId,Quantity,UnitPrice,LineTotal) VALUES
        (@SID,@Cok3,2,280.00,560.00),(@SID,@Bev3,1,200.00,200.00);
    INSERT INTO Payments(SaleId,Method,Amount,Status,TransactionRef,PaidAt)
    VALUES(@SID,2,644.65,1,N'pi_4So3CtNzfLzKx8E71cC7890','2025-03-22 17:46:00');
END
GO

-- ============================================================
-- PURCHASE ORDERS + PO LINE ITEMS
-- ============================================================
DECLARE @Mgr INT=(SELECT Id FROM Users WHERE Email=N'manager@smartpos.com');
DECLARE @Mgr2 INT=(SELECT Id FROM Users WHERE Email=N'manager2@smartpos.com');
DECLARE @SBaker INT=(SELECT Id FROM Suppliers WHERE Email=N'orders@islamabadbakersupply.pk');
DECLARE @SFlour INT=(SELECT Id FROM Suppliers WHERE Email=N'sales@khi-flour-mills.pk');
DECLARE @SDairy INT=(SELECT Id FROM Suppliers WHERE Email=N'dairy@lahore-fresh.pk');
DECLARE @SSweet INT=(SELECT Id FROM Suppliers WHERE Email=N'info@sweetingredients.pk');
DECLARE @POI INT;

IF NOT EXISTS (SELECT 1 FROM PurchaseOrders WHERE OrderDate='2025-02-01 10:00:00')
BEGIN
    INSERT INTO PurchaseOrders(SupplierId,UserId,Status,TotalCost,OrderDate,ReceivedAt,Notes)
    VALUES(@SBaker,@Mgr,2,18500.00,'2025-02-01 10:00:00','2025-02-04 14:00:00',
           N'Monthly bread and pastry restock. Priority delivery requested.');
    SET @POI=SCOPE_IDENTITY();
    INSERT INTO POLineItems(POID,ProductId,OrderedQty,UnitPrice) VALUES
        (@POI,(SELECT Id FROM Products WHERE SKU=N'BRD-001'),100,140.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'BRD-003'),30,200.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'PST-001'),50,75.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'PST-003'),40,110.00);
END

IF NOT EXISTS (SELECT 1 FROM PurchaseOrders WHERE OrderDate='2025-02-10 09:30:00')
BEGIN
    INSERT INTO PurchaseOrders(SupplierId,UserId,Status,TotalCost,OrderDate,ReceivedAt,Notes)
    VALUES(@SSweet,@Mgr,2,24600.00,'2025-02-10 09:30:00','2025-02-13 11:00:00',
           N'Cookie and cake ingredients restock for February.');
    SET @POI=SCOPE_IDENTITY();
    INSERT INTO POLineItems(POID,ProductId,OrderedQty,UnitPrice) VALUES
        (@POI,(SELECT Id FROM Products WHERE SKU=N'COK-001'),80,140.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'COK-002'),60,190.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'COK-003'),70,120.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'PST-002'),40,95.00);
END

IF NOT EXISTS (SELECT 1 FROM PurchaseOrders WHERE OrderDate='2025-03-25 11:00:00')
BEGIN
    INSERT INTO PurchaseOrders(SupplierId,UserId,Status,TotalCost,OrderDate,ReceivedAt,Notes)
    VALUES(@SDairy,@Mgr2,1,8400.00,'2025-03-25 11:00:00',NULL,
           N'Dairy items running low. Urgent restock for smoothies and cakes.');
    SET @POI=SCOPE_IDENTITY();
    INSERT INTO POLineItems(POID,ProductId,OrderedQty,UnitPrice) VALUES
        (@POI,(SELECT Id FROM Products WHERE SKU=N'CKE-003'),20,1100.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'BEV-004'),50,150.00);
END

IF NOT EXISTS (SELECT 1 FROM PurchaseOrders WHERE OrderDate='2025-04-01 08:00:00')
BEGIN
    INSERT INTO PurchaseOrders(SupplierId,UserId,Status,TotalCost,OrderDate,ReceivedAt,Notes)
    VALUES(@SFlour,@Mgr,0,10200.00,'2025-04-01 08:00:00',NULL,
           N'Draft PO for April flour supplies. Awaiting manager approval.');
    SET @POI=SCOPE_IDENTITY();
    INSERT INTO POLineItems(POID,ProductId,OrderedQty,UnitPrice) VALUES
        (@POI,(SELECT Id FROM Products WHERE SKU=N'BRD-002'),120,85.00),
        (@POI,(SELECT Id FROM Products WHERE SKU=N'CKE-001'),20,550.00);
END
GO

-- ============================================================
-- REVIEWS (with BERT sentiment)
-- ============================================================
DECLARE @RC1 INT=(SELECT Id FROM Customers WHERE Email=N'zara.hussain@email.com');
DECLARE @RC2 INT=(SELECT Id FROM Customers WHERE Email=N'ali.nawaz@email.com');
DECLARE @RC3 INT=(SELECT Id FROM Customers WHERE Email=N'maria.khan@email.com');
DECLARE @RC4 INT=(SELECT Id FROM Customers WHERE Email=N'fahad.malik@email.com');
DECLARE @RC5 INT=(SELECT Id FROM Customers WHERE Email=N'nadia.iqbal@email.com');

IF @RC1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC1 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'PST-001'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC1,(SELECT Id FROM Products WHERE SKU=N'PST-001'),5,
     N'The butter croissant is absolutely amazing! Perfectly flaky and buttery. Will definitely order again.',
     N'Positive',0.97,'2025-03-02 10:00:00');

IF @RC3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC3 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'CKE-001'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC3,(SELECT Id FROM Products WHERE SKU=N'CKE-001'),5,
     N'Ordered for my son''s birthday. The chocolate fudge cake was incredibly moist and rich. Everyone loved it!',
     N'Positive',0.98,'2025-03-03 14:00:00');

IF @RC2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC2 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'CKE-002'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC2,(SELECT Id FROM Products WHERE SKU=N'CKE-002'),4,
     N'Good red velvet cake. The cream cheese frosting was nice but slightly too sweet for my taste.',
     N'Positive',0.72,'2025-03-06 09:00:00');

IF @RC4 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC4 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'BRD-003'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC4,(SELECT Id FROM Products WHERE SKU=N'BRD-003'),5,
     N'Best sourdough in Islamabad. The crust is perfectly crispy and the inside is beautifully chewy.',
     N'Positive',0.96,'2025-03-09 08:30:00');

IF @RC5 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC5 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'COK-002'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC5,(SELECT Id FROM Products WHERE SKU=N'COK-002'),3,
     N'The oreo cheesecake cookies were okay. I expected more cheesecake flavour but not bad.',
     N'Neutral',0.51,'2025-03-11 17:00:00');

IF @RC3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC3 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'CKE-003'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC3,(SELECT Id FROM Products WHERE SKU=N'CKE-003'),5,
     N'The black forest cake was stunning! Beautiful presentation, fresh cherries. Ordered for my anniversary.',
     N'Positive',0.99,'2025-03-16 11:00:00');

IF @RC1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC1 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'BRD-001'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC1,(SELECT Id FROM Products WHERE SKU=N'BRD-001'),4,
     N'The multigrain loaf is healthy and delicious. I wish the slices were a bit thicker but the taste is excellent.',
     N'Positive',0.78,'2025-03-19 12:30:00');

IF @RC2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Reviews WHERE CustomerId=@RC2 AND ProductId=(SELECT Id FROM Products WHERE SKU=N'COK-003'))
    INSERT INTO Reviews(CustomerId,ProductId,Rating,Comment,Sentiment,SentimentScore,CreatedAt) VALUES
    (@RC2,(SELECT Id FROM Products WHERE SKU=N'COK-003'),2,
     N'The oatmeal raisin cookies were dry and crumbly. They did not taste fresh. Disappointed given the price.',
     N'Negative',0.08,'2025-03-23 18:00:00');
GO

-- ============================================================
-- AUDIT LOGS (only columns that exist: UserId,Action,Module,Timestamp,IPAddress,Details)
-- ============================================================
DECLARE @AAdmin INT=(SELECT Id FROM Users WHERE Email=N'admin@smartpos.com');
DECLARE @AMgr   INT=(SELECT Id FROM Users WHERE Email=N'manager@smartpos.com');
DECLARE @ACsh   INT=(SELECT Id FROM Users WHERE Email=N'cashier1@smartpos.com');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@AAdmin AND Action=N'Created User: manager@smartpos.com')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@AAdmin,N'Created User: manager@smartpos.com',N'Users','2025-01-02 09:05:00',
     N'192.168.1.10',N'New manager account created by admin.');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@AAdmin AND Action=N'Created User: cashier1@smartpos.com')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@AAdmin,N'Created User: cashier1@smartpos.com',N'Users','2025-01-10 10:05:00',
     N'192.168.1.10',N'Cashier account created.');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@AMgr AND Action=N'Created Product: Multigrain Loaf')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@AMgr,N'Created Product: Multigrain Loaf',N'Products','2025-01-15 08:05:00',
     N'192.168.1.11',N'New product added to catalog. SKU: BRD-001, Price: 280.');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@AMgr AND Action=N'Created Purchase Order')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@AMgr,N'Created Purchase Order',N'PurchaseOrders','2025-02-01 10:05:00',
     N'192.168.1.11',N'Monthly restock PO raised. Supplier: Islamabad Baker Supply.');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@ACsh AND Action=N'Completed Sale')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@ACsh,N'Completed Sale',N'Sales','2025-03-01 09:16:00',
     N'192.168.1.20',N'Onsite sale completed. Total: 503.55 PKR. Payment: Cash.');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@AMgr AND Action=N'Created Promotion: WEEKEND10')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@AMgr,N'Created Promotion: WEEKEND10',N'Promotions','2025-02-01 00:05:00',
     N'192.168.1.11',N'Weekend 10% discount promotion activated.');

IF NOT EXISTS (SELECT 1 FROM AuditLogs WHERE UserId=@ACsh AND Action=N'Voided Sale')
    INSERT INTO AuditLogs(UserId,Action,Module,Timestamp,IPAddress,Details) VALUES
    (@ACsh,N'Voided Sale',N'Sales','2025-03-20 09:10:00',
     N'192.168.1.20',N'Sale voided due to cashier input error. Customer refunded.');
GO

-- ============================================================
-- VERIFICATION
-- ============================================================
SELECT 'Roles'           AS TableName, COUNT(*) AS Rows FROM Roles          UNION ALL
SELECT 'Permissions',                  COUNT(*)         FROM Permissions     UNION ALL
SELECT 'Users',                        COUNT(*)         FROM Users           UNION ALL
SELECT 'Customers',                    COUNT(*)         FROM Customers       UNION ALL
SELECT 'Suppliers',                    COUNT(*)         FROM Suppliers       UNION ALL
SELECT 'Categories',                   COUNT(*)         FROM Categories      UNION ALL
SELECT 'Products',                     COUNT(*)         FROM Products        UNION ALL
SELECT 'Inventories',                  COUNT(*)         FROM Inventories     UNION ALL
SELECT 'Promotions',                   COUNT(*)         FROM Promotions      UNION ALL
SELECT 'Sales',                        COUNT(*)         FROM Sales           UNION ALL
SELECT 'SaleItems',                    COUNT(*)         FROM SaleItems       UNION ALL
SELECT 'Payments',                     COUNT(*)         FROM Payments        UNION ALL
SELECT 'PurchaseOrders',               COUNT(*)         FROM PurchaseOrders  UNION ALL
SELECT 'POLineItems',                  COUNT(*)         FROM POLineItems     UNION ALL
SELECT 'Reviews',                      COUNT(*)         FROM Reviews         UNION ALL
SELECT 'AuditLogs',                    COUNT(*)         FROM AuditLogs;
