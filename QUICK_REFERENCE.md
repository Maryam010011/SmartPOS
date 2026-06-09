# SmartPOS+ Quick Reference Card

## 🚀 Running the Application

```bash
cd "c:\Users\Maryam Yaqoob\Pictures\New folder\SmartPOS"
dotnet run
```

**Application URL**: http://localhost:5062

---

## 🔑 Login Credentials

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@smartpos.com | Admin@123 |
| **Manager** | manager@smartpos.com | Admin@123 |
| **Cashier 1** | cashier1@smartpos.com | Admin@123 |
| **Cashier 2** | cashier2@smartpos.com | Admin@123 |

---

## 📱 Application URLs by Role

### Admin Routes
- Dashboard: `/admin/dashboard`
- Users: `/admin/users`
- Products: `/admin/products`
- Categories: `/admin/categories`
- Inventory: (inventory management)
- Customers: `/admin/customers`
- Suppliers: `/admin/suppliers`
- Roles & Permissions: `/admin/roles`
- Sales History: (sales reports)
- Sentiment Analysis: `/admin/sentiment`
- Audit Logs: `/admin/audit-logs`
- Analytics: `/admin/analytics`

### Manager Routes
- Dashboard: `/manager/dashboard`
- Sales Reports: (manager sales view)
- Inventory: (inventory overview)
- Purchase Orders: (PO management)

### Cashier Routes
- Dashboard: `/cashier/dashboard`
- POS: `/cashier/pos`
- Sales History: `/cashier/sales-history`
- Customer Lookup: `/cashier/customer-lookup`

### Customer Routes
- Home: `/customer/home`
- Cart: `/customer/cart`
- Checkout: `/customer/checkout`
- Order History: `/customer/order-history`
- Profile: `/customer/profile`
- Active Promotions: `/customer/promotions`

---

## 🗄️ Database Information

**Server**: localhost  
**Database Name**: SmartPOS_DB  
**Authentication**: Windows Authentication (Trusted Connection)

**Connection String**:
```
Server=localhost;Database=SmartPOS_DB;Trusted_Connection=True;TrustServerCertificate=True;
```

### Database Tables (16 total)
1. Roles
2. Permissions
3. Users
4. Customers
5. AuditLogs
6. Categories
7. Suppliers
8. Products
9. Inventories
10. Sales
11. SaleItems
12. Payments
13. Promotions
14. Reviews
15. PurchaseOrders
16. POLineItems

---

## 🧪 Seeded Data Summary

| Table | Records | Status |
|-------|---------|--------|
| Roles | 4 | ✅ Seeded |
| Permissions | 36 | ✅ Seeded |
| Users | 10 | ✅ Seeded |
| Categories | 9 | ✅ Seeded |
| Products | 18 | ✅ Seeded |
| Inventories | 18 | ✅ Seeded |
| Suppliers | 1 | ✅ Seeded |
| Sales | 10 | ✅ Seeded |
| SaleItems | 19 | ✅ Seeded |
| Payments | 10 | ✅ Seeded |
| AuditLogs | 1 | ✅ Seeded |

---

## 🎨 UI Features (Material Design 3)

- **Modern gradient buttons** with hover effects
- **Professional card system** with shadows
- **Two-tone table headers** with hover states
- **Rounded input fields** with blue focus rings
- **Color-coded badges** (Success/Warning/Danger/Info)
- **Skeleton loaders** for loading states
- **Smooth animations** (200-400ms transitions)
- **Responsive design** (mobile, tablet, desktop)

---

## 🤖 NLP/Sentiment Analysis

**Status**: ✅ Implemented, ⚠️ Requires API Key

**Service**: HuggingFace BERT API  
**Model**: `nlptown/bert-base-multilingual-uncased-sentiment`  
**Current Mode**: Fallback (rating-based sentiment)

### To Activate Full NLP:
1. Get API key from: https://huggingface.co/settings/tokens
2. Add to `appsettings.json`:
   ```json
   "BERTService": {
     "ApiKey": "hf_your_key_here"
   }
   ```
3. Restart application

---

## 📊 Key Features

### ✅ Implemented
- User authentication & authorization (RBAC)
- Product catalog management
- Category hierarchy (nested)
- Real-time inventory tracking
- Point-of-sale (POS) transaction processing
- Customer profiles with loyalty points
- Promotion/discount code system
- Purchase order management
- Payment processing (Cash, Card, Online)
- Customer reviews with sentiment analysis
- Role-based permission editor
- Comprehensive analytics dashboards
- Audit logging (all user actions)
- Modern Material Design 3 UI

### ⚠️ Limitations
- No hardware integration (scanners, printers)
- No real payment gateway (Stripe, PayPal)
- No offline mode
- Single-tenant only
- Single currency
- No mobile app (responsive web only)

---

## 🛠️ Development Commands

### Run Application
```bash
dotnet run
```

### Build Application
```bash
dotnet build
```

### Create Migration
```bash
dotnet ef migrations add MigrationName
```

### Update Database
```bash
dotnet ef database update
```

### Drop Database (CAREFUL!)
```bash
dotnet ef database drop
```

### Publish for Production
```bash
dotnet publish -c Release -o ./publish
```

---

## 📄 Important Files

### Configuration
- `appsettings.json` - App configuration
- `appsettings.Development.json` - Dev settings

### Database
- `Data/AppDbContext.cs` - EF Core context
- `Migrations/` - Database migrations
- `docs/SeedData.sql` - Manual seed script

### Models
- `Models/` - 16 entity models
- `Contracts.cs` - Shared enums

### Services
- `Services/AuthService.cs` - Authentication
- `Services/ProductService.cs` - Product management
- `Services/SaleService.cs` - Sales processing
- `Services/InventoryService.cs` - Inventory
- `Services/CustomerService.cs` - Customers
- `Services/BERTService.cs` - NLP/Sentiment
- `Services/ReviewService.cs` - Reviews
- `Services/UserService.cs` - User management

### UI
- `Components/Pages/Admin/` - Admin pages
- `Components/Pages/Cashier/` - Cashier pages
- `Components/Pages/Customer/` - Customer pages
- `Components/Layout/` - Layout components
- `wwwroot/css/modern-ui-v4.css` - Custom CSS

### Documentation
- `PROJECT_REPORT.md` - Academic report
- `REPORT_COMPLETION_GUIDE.md` - Report instructions
- `UI_REVAMP_COMPLETE.md` - UI documentation
- `NLP_STATUS_REPORT.md` - NLP documentation

---

## 🎯 Testing Credentials for Screenshots

Use these for capturing screenshots:

1. **Login**: admin@smartpos.com / Admin@123
2. **Navigate** to each page
3. **Capture** full window screenshots
4. **Save** as PNG with descriptive names

---

## 📞 Quick Troubleshooting

### Application Won't Start
```bash
# Check if port 5062 is in use
netstat -ano | findstr :5062

# Kill process if needed
taskkill /PID [PID_NUMBER] /F
```

### Database Connection Error
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`

### CSS Not Loading
1. Hard refresh: `Ctrl + Shift + R`
2. Check browser console for errors
3. Verify file exists: `wwwroot/css/modern-ui-v4.css`

### NLP Not Working
1. Check if API key is set in `appsettings.json`
2. Check internet connection to HuggingFace
3. System will fallback to rating-based sentiment

---

## 📚 Resources

- **.NET Documentation**: https://docs.microsoft.com/dotnet
- **Blazor Documentation**: https://docs.microsoft.com/aspnet/core/blazor
- **Entity Framework Core**: https://docs.microsoft.com/ef/core
- **Material Design 3**: https://m3.material.io/
- **HuggingFace BERT**: https://huggingface.co/nlptown/bert-base-multilingual-uncased-sentiment

---

**Last Updated**: June 9, 2026  
**Version**: 1.0  
**Status**: Production Ready ✅
