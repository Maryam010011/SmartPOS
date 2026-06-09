# SmartPOS+ — System Design Document

**Project:** SmartPOS+ Bakery Point-of-Sale System  
**Technology:** Blazor Server, .NET 10, EF Core, SQL Server (LocalDB)  
**Author:** Maryam Yaqoob  
**Version:** 1.0  

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Functional Requirements — User Stories](#2-functional-requirements--user-stories)
3. [Non-Functional Requirements](#3-non-functional-requirements)
4. [UML Use Case Diagram](#4-uml-use-case-diagram)
5. [UML Domain Model](#5-uml-domain-model)
6. [UML Class Diagram](#6-uml-class-diagram)
7. [Entity Relationship Diagram (ERD)](#7-entity-relationship-diagram-erd)
8. [Sequence Diagrams](#8-sequence-diagrams)

---

## 1. Project Overview

SmartPOS+ is a web-based Point-of-Sale system built for a bakery. It supports four distinct user roles — Admin, Manager, Cashier, and Customer — each with their own dedicated portal and permissions. The system handles product catalog management, inventory tracking, sales processing (both onsite and online), customer loyalty, promotions, purchase orders, payment processing, product reviews with AI sentiment analysis, and full audit logging.

### Key Modules

| Module | Description |
|--------|-------------|
| Authentication & RBAC | Role-based login, JWT tokens, role-specific dashboards |
| Product Catalog | Products, categories (hierarchical), suppliers |
| Inventory | Stock levels, reorder alerts, purchase orders |
| Sales & POS | Onsite cashier terminal, online customer shop |
| Promotions | Discount codes (percentage or flat), usage limits |
| Payments | Cash, card, online (Stripe) payment processing |
| Reviews | Customer ratings + BERT AI sentiment analysis |
| Audit Logging | Unified log of all user actions across all roles |
| Admin Panel | User management, permissions, system oversight |

---

## 2. Functional Requirements — User Stories

### 2.1 Authentication & User Management

**US-01**  
As a **user**, I want to select my role (Admin, Manager, Cashier, Customer) on the login screen, so that I am directed to the correct portal after signing in.

**US-02**  
As a **user**, I want to log in with my email and password, so that I can securely access my role-specific dashboard.

**US-03**  
As a **new customer**, I want to register an account by providing my name, email, and password, so that I can shop online and track my orders.

**US-04**  
As an **Admin**, I want to create, activate, and deactivate user accounts, so that I can control who has access to the system.

**US-05**  
As an **Admin**, I want to assign roles and module-level permissions (CanCreate, CanRead, CanUpdate, CanDelete) to each role, so that access is controlled at a granular level.

**US-06**  
As a **user**, I want to be redirected to my role-specific dashboard after login (Admin → /admin/dashboard, Manager → /manager/inventory, Cashier → /cashier/pos, Customer → /customer/shop), so that I immediately see the tools relevant to my role.

---

### 2.2 Product Catalog

**US-07**  
As a **Manager**, I want to add new products with a name, SKU, description, price, cost price, category, and supplier, so that they appear in the POS and customer shop.

**US-08**  
As a **Manager**, I want to edit and deactivate products, so that outdated or unavailable items are hidden from the POS and shop without being deleted.

**US-09**  
As a **Manager**, I want to organise products into hierarchical categories (e.g. Bakery > Cakes > Birthday Cakes), so that customers and cashiers can browse products easily.

**US-10**  
As a **Manager**, I want to assign a supplier to a product, so that I know where to reorder it from.

---

### 2.3 Inventory Management

**US-11**  
As a **Manager**, I want to view the current stock level of every product, so that I know what is available for sale.

**US-12**  
As a **Manager**, I want to set a reorder level for each product, so that I receive a low-stock alert when quantity falls to or below that threshold.

**US-13**  
As a **Manager**, I want to create a purchase order for a supplier listing the products and quantities needed, so that I can restock inventory.

**US-14**  
As a **Manager**, I want to update a purchase order status (Draft → Sent → Received → Cancelled), so that I can track the restock lifecycle.

**US-15**  
As a **Manager**, I want inventory quantities to update automatically when a purchase order is marked as Received, so that stock levels stay accurate.

---

### 2.4 Sales & POS Terminal

**US-16**  
As a **Cashier**, I want to search for products by name or SKU and add them to a cart, so that I can build a sale quickly at the counter.

**US-17**  
As a **Cashier**, I want to apply a promotion code to a sale, so that the discount is calculated and deducted from the total automatically.

**US-18**  
As a **Cashier**, I want to process payment (cash, card, or online) and complete the sale, so that a receipt is generated and inventory is decremented.

**US-19**  
As a **Cashier**, I want to void a completed sale, so that incorrect transactions can be reversed.

**US-20**  
As a **Customer**, I want to browse the online shop, add products to a cart, and place an order, so that I can buy bakery items without visiting the store.

**US-21**  
As a **Customer**, I want to apply a promotion code at checkout, so that I receive the applicable discount on my order.

**US-22**  
As a **system**, I want to calculate 17% GST on every sale automatically, so that tax is always correctly applied.

---

### 2.5 Promotions

**US-23**  
As a **Manager**, I want to create a promotion with a unique code, discount type (percentage or flat), value, minimum order value, usage limit, and validity dates, so that customers can redeem discounts at checkout.

**US-24**  
As a **Manager**, I want to activate and deactivate promotions, so that I can control which offers are currently available.

**US-25**  
As a **system**, I want to increment the usage count of a promotion each time it is successfully applied, so that usage limits are enforced.

---

### 2.6 Payments

**US-26**  
As a **Cashier**, I want to record a cash payment against a sale, so that the transaction is marked as completed.

**US-27**  
As a **Customer**, I want to pay online using a card (via Stripe), so that I can complete my purchase securely without cash.

**US-28**  
As a **system**, I want to store the Stripe transaction reference against a payment, so that card payments can be traced and refunded if needed.

**US-29**  
As a **Manager**, I want to process a refund on a completed sale, so that the customer is reimbursed and the sale status is updated to Refunded.

---

### 2.7 Customer Loyalty

**US-30**  
As a **Customer**, I want to earn loyalty points on every completed purchase, so that I am rewarded for repeat business.

**US-31**  
As a **Customer**, I want to view my total loyalty points and total amount spent, so that I can track my rewards.

---

### 2.8 Reviews & Sentiment Analysis

**US-32**  
As a **Customer**, I want to leave a star rating (1–5) and written review for a product I have purchased, so that I can share my feedback.

**US-33**  
As a **system**, I want to send the review text to a BERT AI service and store the sentiment result (Positive / Neutral / Negative) and confidence score, so that the business can analyse customer feedback at scale.

**US-34**  
As a **Manager**, I want to view the sentiment breakdown of reviews for each product, so that I can identify which products are well-received and which need attention.

---

### 2.9 Audit Logging

**US-35**  
As an **Admin**, I want every significant user action (create, update, delete, void, login) to be recorded in the audit log with the user, module, action, timestamp, and IP address, so that I have a full activity trail for compliance and security.

**US-36**  
As an **Admin**, I want to filter and search the audit log by user, module, date range, and action, so that I can investigate specific events quickly.

---

## 3. Non-Functional Requirements

### 3.1 Security

**NFR-01** — All passwords must be hashed using BCrypt before storage. Plain-text passwords must never be persisted.

**NFR-02** — Authentication must use JWT tokens stored in browser LocalStorage. Tokens must expire after a configurable period.

**NFR-03** — All pages must enforce role-based access control using `[Authorize(Roles = "...")]`. Unauthorised access attempts must redirect to the login page.

**NFR-04** — The connection string must never be hardcoded in source files. It must be read from `appsettings.json` via dependency injection.

**NFR-05** — All database queries must use parameterised EF Core LINQ queries to prevent SQL injection.

### 3.2 Performance

**NFR-06** — The POS terminal product search must return results within 500ms for a catalogue of up to 10,000 products.

**NFR-07** — The customer shop page must load within 2 seconds on a standard broadband connection.

**NFR-08** — Database queries must use indexed columns (Email, RoleId, SKU, SaleId, ProductId, UserId) to avoid full table scans.

### 3.3 Reliability & Data Integrity

**NFR-09** — All decimal monetary values must be stored with precision (10, 2) to prevent rounding errors.

**NFR-10** — The Review Rating column must enforce a CHECK constraint (`Rating >= 1 AND Rating <= 5`) at the database level.

**NFR-11** — Unique constraints must be enforced on User.Email, Customer.Email, Product.SKU, Promotion.Code, Supplier.Email, Inventory.ProductId, and Payment.SaleId.

**NFR-12** — Cascade delete must only be applied where logically correct (Sale → SaleItems, PurchaseOrder → POLineItems). Restrict must be used where history must be preserved (Product → SaleItems).

### 3.4 Usability

**NFR-13** — Each role must have a distinct layout and navigation menu so that users only see the features relevant to their role.

**NFR-14** — All forms must display inline validation messages using Blazor `DataAnnotationsValidator`.

**NFR-15** — The system must display a loading indicator during async operations (login, checkout, product search).

### 3.5 Maintainability

**NFR-16** — All EF Core model configuration must use Fluent API in `AppDbContext.OnModelCreating`. Data annotations on model classes must not be used for database configuration.

**NFR-17** — Shared enum types (`DiscountType`, `SaleType`, `SaleStatus`, `POStatus`, `PaymentMethod`, `PaymentStatus`) must be defined in a single `Contracts.cs` file under `SmartPOS.Shared.Enums`.

**NFR-18** — The application must follow the partial class pattern for all EF Core model classes to allow scaffolding without overwriting custom logic.

### 3.6 Scalability

**NFR-19** — The category system must support unlimited nesting depth via the self-referencing `ParentCategoryId` foreign key.

**NFR-20** — The audit log must be designed to handle high write volume without impacting read performance on other tables (separate table, indexed on UserId and Timestamp).

---

---

## 4. UML Use Case Diagram

```plantuml
@startuml SmartPOS_UseCaseDiagram

skinparam actorStyle awesome
skinparam packageStyle rectangle
skinparam usecase {
  BackgroundColor #FFF8F0
  BorderColor #CC6600
  ArrowColor #CC6600
}
skinparam actor {
  BackgroundColor #FFE4B5
  BorderColor #CC6600
}

title SmartPOS+ — UML Use Case Diagram

' ─────────────────────────────────────────
' ACTORS
' ─────────────────────────────────────────
actor Admin      as A #FFD700
actor Manager    as M #87CEEB
actor Cashier    as C #90EE90
actor Customer   as CU #FFB6C1
actor "BERT AI\nService" as BERT #D3D3D3
actor "Stripe\nPayment" as STRIPE #D3D3D3

' ─────────────────────────────────────────
' SYSTEM BOUNDARY
' ─────────────────────────────────────────
rectangle "SmartPOS+ System" {

  ' ── AUTHENTICATION ──────────────────────
  package "Authentication" {
    usecase "Select Role"            as UC01
    usecase "Login"                  as UC02
    usecase "Register Account"       as UC03
    usecase "Logout"                 as UC04
  }

  ' ── USER & PERMISSION MANAGEMENT ────────
  package "User & Permission Management" {
    usecase "Manage Users\n(Create/Activate/Deactivate)" as UC05
    usecase "Assign Role Permissions"                    as UC06
    usecase "View Audit Log"                             as UC07
  }

  ' ── PRODUCT CATALOG ─────────────────────
  package "Product Catalog" {
    usecase "Add / Edit Product"         as UC08
    usecase "Deactivate Product"         as UC09
    usecase "Manage Categories"          as UC10
    usecase "Manage Suppliers"           as UC11
  }

  ' ── INVENTORY ───────────────────────────
  package "Inventory Management" {
    usecase "View Stock Levels"          as UC12
    usecase "Set Reorder Level"          as UC13
    usecase "Create Purchase Order"      as UC14
    usecase "Update PO Status"           as UC15
    usecase "Receive Stock"              as UC16
  }

  ' ── SALES & POS ─────────────────────────
  package "Sales & POS" {
    usecase "Search Products"            as UC17
    usecase "Add Item to Cart"           as UC18
    usecase "Apply Promotion Code"       as UC19
    usecase "Process Payment"            as UC20
    usecase "Complete Sale"              as UC21
    usecase "Void Sale"                  as UC22
    usecase "Browse Online Shop"         as UC23
    usecase "Place Online Order"         as UC24
  }

  ' ── PROMOTIONS ──────────────────────────
  package "Promotions" {
    usecase "Create / Edit Promotion"    as UC25
    usecase "Activate / Deactivate Promo" as UC26
  }

  ' ── PAYMENTS ────────────────────────────
  package "Payments" {
    usecase "Pay by Cash"                as UC27
    usecase "Pay by Card (Stripe)"       as UC28
    usecase "Process Refund"             as UC29
  }

  ' ── REVIEWS ─────────────────────────────
  package "Reviews & Sentiment" {
    usecase "Submit Product Review"      as UC30
    usecase "Analyse Sentiment (BERT)"   as UC31
    usecase "View Sentiment Report"      as UC32
  }

  ' ── LOYALTY ─────────────────────────────
  package "Customer Loyalty" {
    usecase "Earn Loyalty Points"        as UC33
    usecase "View Loyalty Balance"       as UC34
  }

}

' ─────────────────────────────────────────
' ACTOR → USE CASE ASSOCIATIONS
' ─────────────────────────────────────────

' All actors share authentication
A  --> UC01
M  --> UC01
C  --> UC01
CU --> UC01

A  --> UC02
M  --> UC02
C  --> UC02
CU --> UC02

CU --> UC03
A  --> UC04
M  --> UC04
C  --> UC04
CU --> UC04

' Admin
A --> UC05
A --> UC06
A --> UC07
A --> UC08
A --> UC09
A --> UC10
A --> UC11
A --> UC12
A --> UC17
A --> UC22
A --> UC25
A --> UC26
A --> UC29

' Manager
M --> UC08
M --> UC09
M --> UC10
M --> UC11
M --> UC12
M --> UC13
M --> UC14
M --> UC15
M --> UC16
M --> UC25
M --> UC26
M --> UC29
M --> UC32

' Cashier
C --> UC17
C --> UC18
C --> UC19
C --> UC20
C --> UC21
C --> UC22
C --> UC27

' Customer
CU --> UC23
CU --> UC24
CU --> UC19
CU --> UC28
CU --> UC30
CU --> UC33
CU --> UC34

' External systems
STRIPE --> UC28
BERT   --> UC31

' ─────────────────────────────────────────
' INCLUDE / EXTEND RELATIONSHIPS
' ─────────────────────────────────────────
UC21 .> UC20 : <<include>>
UC21 .> UC33 : <<include>>
UC24 .> UC20 : <<include>>
UC24 .> UC33 : <<include>>
UC20 .> UC27 : <<extend>>
UC20 .> UC28 : <<extend>>
UC19 .> UC21 : <<extend>>
UC30 .> UC31 : <<include>>
UC16 .> UC12 : <<include>>

@enduml
```

### Use Case Summary Table

| Use Case | Actor(s) | Description |
|----------|----------|-------------|
| UC01 Select Role | All | Choose Admin/Manager/Cashier/Customer before login |
| UC02 Login | All | Authenticate with email + password |
| UC03 Register | Customer | Self-register as a new customer |
| UC04 Logout | All | End session and clear JWT token |
| UC05 Manage Users | Admin | Create, activate, deactivate user accounts |
| UC06 Assign Permissions | Admin | Set per-role, per-module CRUD rights |
| UC07 View Audit Log | Admin | Search and filter all system activity |
| UC08 Add/Edit Product | Admin, Manager | Manage product catalog entries |
| UC09 Deactivate Product | Admin, Manager | Hide product from POS and shop |
| UC10 Manage Categories | Admin, Manager | Create hierarchical product categories |
| UC11 Manage Suppliers | Admin, Manager | Maintain supplier contact records |
| UC12 View Stock Levels | Admin, Manager | See current inventory quantities |
| UC13 Set Reorder Level | Manager | Configure low-stock alert threshold |
| UC14 Create Purchase Order | Manager | Raise a PO to restock from supplier |
| UC15 Update PO Status | Manager | Move PO through Draft→Sent→Received→Cancelled |
| UC16 Receive Stock | Manager | Mark PO received and update inventory |
| UC17 Search Products | Admin, Cashier | Find products by name or SKU |
| UC18 Add Item to Cart | Cashier | Build a sale at the POS terminal |
| UC19 Apply Promo Code | Cashier, Customer | Validate and apply a discount code |
| UC20 Process Payment | Cashier, Customer | Collect payment for a sale |
| UC21 Complete Sale | Cashier | Finalise sale, generate receipt |
| UC22 Void Sale | Admin, Cashier | Reverse a completed transaction |
| UC23 Browse Online Shop | Customer | View active products in the shop |
| UC24 Place Online Order | Customer | Submit an online sale |
| UC25 Create/Edit Promotion | Admin, Manager | Define discount codes and rules |
| UC26 Activate/Deactivate Promo | Admin, Manager | Toggle promotion availability |
| UC27 Pay by Cash | Cashier | Record a cash payment |
| UC28 Pay by Card | Customer | Process Stripe card payment |
| UC29 Process Refund | Admin, Manager | Refund a completed sale |
| UC30 Submit Review | Customer | Rate and review a product |
| UC31 Analyse Sentiment | BERT AI | Auto-classify review text |
| UC32 View Sentiment Report | Manager | See AI sentiment breakdown per product |
| UC33 Earn Loyalty Points | Customer | Accumulate points on purchase |
| UC34 View Loyalty Balance | Customer | Check points and total spent |

---

---

## 5. UML Domain Model

The Domain Model shows the key business concepts and their relationships using plain language — no implementation details, no data types, no methods. It captures **what the business cares about**.

```plantuml
@startuml SmartPOS_DomainModel

skinparam classBackgroundColor #FFF8F0
skinparam classBorderColor #CC6600
skinparam arrowColor #CC6600
skinparam classHeaderBackgroundColor #FFE4B5
skinparam packageStyle rectangle
skinparam linetype ortho

title SmartPOS+ — UML Domain Model

' ─────────────────────────────────────────
' DOMAIN CONCEPTS
' ─────────────────────────────────────────

class "User" as User {
  A person who can log into the system.
  Has exactly one Role.
}

class "Role" as Role {
  Defines what a User is allowed to do.
  Values: Admin, Manager, Cashier, Customer.
}

class "Permission" as Permission {
  Grants or denies CRUD access
  to a specific module for a Role.
}

class "Customer" as Customer {
  Extended profile for a User
  with the Customer role.
  Tracks loyalty points and spending.
}

class "AuditLog" as AuditLog {
  Records every significant action
  taken by any User in the system.
}

class "Product" as Product {
  A bakery item available for sale.
  Has a price, cost price, and SKU.
}

class "Category" as Category {
  Groups products into a hierarchy.
  A category can have sub-categories.
}

class "Supplier" as Supplier {
  A company that supplies products
  to the bakery.
}

class "Inventory" as Inventory {
  Tracks the current stock level
  of a single Product.
  Triggers alerts at reorder level.
}

class "PurchaseOrder" as PO {
  A formal request to a Supplier
  to restock one or more Products.
}

class "POLineItem" as POLine {
  One product line within
  a Purchase Order.
}

class "Sale" as Sale {
  A completed transaction —
  either onsite (POS) or online (Shop).
}

class "SaleItem" as SaleItem {
  One product line within a Sale.
  Captures price at time of sale.
}

class "Promotion" as Promotion {
  A discount code that can be
  applied at checkout.
  Can be percentage or flat amount.
}

class "Payment" as Payment {
  The payment record for a Sale.
  Supports Cash, Card, or Online.
}

class "Review" as Review {
  A customer's star rating and
  written comment for a Product.
  Enriched with AI sentiment.
}

' ─────────────────────────────────────────
' RELATIONSHIPS
' ─────────────────────────────────────────

' User & Role
Role        "1" -- "0..*" User        : is assigned to >
Role        "1" -- "0..*" Permission  : has >
User        "1" -- "0..1" Customer    : extends to >
User        "1" -- "0..*" AuditLog    : generates >

' Product hierarchy
Category    "0..1" -- "0..*" Category  : parent of >
Category    "1"    -- "0..*" Product   : contains >
Supplier    "0..1" -- "0..*" Product   : supplies >
Product     "1"    -- "1"   Inventory  : tracked by >

' Purchase Orders
Supplier    "1"    -- "0..*" PO        : receives >
User        "1"    -- "0..*" PO        : raised by >
PO          "1"    -- "1..*" POLine    : contains >
Product     "1"    -- "0..*" POLine    : ordered in >

' Sales
User        "1"    -- "0..*" Sale      : processed by >
Customer    "0..1" -- "0..*" Sale      : placed by >
Promotion   "0..1" -- "0..*" Sale      : applied to >
Sale        "1"    -- "1..*" SaleItem  : contains >
Product     "1"    -- "0..*" SaleItem  : sold as >
Sale        "1"    -- "1"   Payment   : paid via >

' Reviews
Customer    "1"    -- "0..*" Review    : writes >
Product     "1"    -- "0..*" Review    : reviewed in >

' ─────────────────────────────────────────
' DOMAIN NOTES
' ─────────────────────────────────────────

note top of Sale
  A Sale can be Onsite (Cashier at POS)
  or Online (Customer via Shop).
  Status: Completed | Voided | Refunded
end note

note top of Promotion
  DiscountType: Percentage or Flat.
  Enforces MinOrderValue and MaxUsageLimit.
  Has a ValidFrom / ValidTo date range.
end note

note right of Review
  Rating: 1–5 stars.
  Sentiment populated by BERT AI:
  Positive | Neutral | Negative
  with a confidence score (0.0–1.0).
end note

note right of Inventory
  Quantity falls when a Sale is completed.
  Quantity rises when a PO is Received.
  Alert fires when Quantity <= ReorderLevel.
end note

note bottom of Payment
  Method: Cash | Card | Online
  Status: Pending | Completed | Failed | Refunded
  TransactionRef stores Stripe charge ID.
end note

@enduml
```

### Domain Concept Descriptions

| Concept | Business Meaning |
|---------|-----------------|
| **User** | Anyone who logs into SmartPOS+. Every user has exactly one role. |
| **Role** | Defines the type of user — Admin, Manager, Cashier, or Customer. |
| **Permission** | A per-role, per-module access rule (CanCreate, CanRead, CanUpdate, CanDelete). |
| **Customer** | An extended profile for users with the Customer role. Stores loyalty points, total spent, phone, address, and date of birth. |
| **AuditLog** | A tamper-evident record of every significant action in the system — who did what, when, from where. |
| **Product** | A bakery item with a selling price, cost price, SKU, and category. Can be active or inactive. |
| **Category** | A named group for products. Categories can be nested (e.g. Bakery > Cakes > Birthday Cakes). |
| **Supplier** | A company that provides products to the bakery. Linked to products and purchase orders. |
| **Inventory** | One stock record per product. Tracks current quantity and the reorder threshold. |
| **PurchaseOrder** | A formal restock request sent to a supplier. Goes through Draft → Sent → Received → Cancelled. |
| **POLineItem** | A single product line within a purchase order, with quantity and agreed unit price. |
| **Sale** | A transaction — either onsite at the POS terminal or online via the customer shop. |
| **SaleItem** | A single product line within a sale. Stores the price at the time of sale (not the current price). |
| **Promotion** | A discount code with rules: type (percentage/flat), value, minimum order, usage limit, and validity dates. |
| **Payment** | The payment record attached to a sale. One payment per sale. Supports cash, card, and online. |
| **Review** | A customer's rating and comment for a product, enriched with BERT AI sentiment analysis. |

### Key Domain Rules

1. Every **User** has exactly one **Role**.
2. Every **Product** has exactly one **Inventory** record.
3. Every **Sale** has exactly one **Payment** record.
4. A **Customer** profile exists only for users with the Customer role.
5. A **Sale** can exist without a **Customer** (guest checkout at POS).
6. A **SaleItem** stores the price at the time of sale — changing a product's price later does not affect historical sales.
7. A **Category** can be a parent of other categories (unlimited depth).
8. A **Promotion** can be applied to many sales but tracks its total usage count.
9. Deleting a **Supplier** sets `SupplierId` to null on linked products — products are not deleted.
10. Deleting a **Category** is blocked if it has linked products (Restrict).

---

---

## 6. UML Class Diagram

The Class Diagram shows the actual C# model classes with their properties, data types, and navigation relationships — directly reflecting the EF Core models in `SmartPOS/Models/`.

```plantuml
@startuml SmartPOS_ClassDiagram

skinparam classBackgroundColor #FFF8F0
skinparam classBorderColor #CC6600
skinparam arrowColor #555555
skinparam classHeaderBackgroundColor #FFE4B5
skinparam linetype ortho
skinparam nodesep 60
skinparam ranksep 80

title SmartPOS+ — UML Class Diagram

' ─────────────────────────────────────────
' ENUMS
' ─────────────────────────────────────────

enum DiscountType <<SmartPOS.Shared.Enums>> {
  Percentage = 0
  Flat = 1
}

enum SaleType <<SmartPOS.Shared.Enums>> {
  Onsite = 0
  Online = 1
}

enum SaleStatus <<SmartPOS.Shared.Enums>> {
  Completed = 0
  Voided = 1
  Refunded = 2
}

enum POStatus <<SmartPOS.Shared.Enums>> {
  Draft = 0
  Sent = 1
  Received = 2
  Cancelled = 3
}

enum PaymentMethod <<SmartPOS.Shared.Enums>> {
  Cash = 0
  Card = 1
  Online = 2
}

enum PaymentStatus <<SmartPOS.Shared.Enums>> {
  Pending = 0
  Completed = 1
  Failed = 2
  Refunded = 3
}

' ─────────────────────────────────────────
' CLASSES — USER & ACCESS CONTROL
' ─────────────────────────────────────────

class Role <<SmartPOS.Models>> {
  + Id : int
  + Name : string
  --
  + Users : ICollection<User>
  + Permissions : ICollection<Permission>
}

class Permission <<SmartPOS.Models>> {
  + Id : int
  + RoleId : int
  + Module : string
  + CanCreate : bool
  + CanRead : bool
  + CanUpdate : bool
  + CanDelete : bool
  --
  + Role : Role
}

class User <<SmartPOS.Models>> {
  + Id : int
  + Name : string
  + Email : string
  + PasswordHash : string
  + RoleId : int
  + IsActive : bool
  + CreatedAt : DateTime
  --
  + Role : Role
  + Customer : Customer?
  + Sales : ICollection<Sale>
  + PurchaseOrders : ICollection<PurchaseOrder>
  + AuditLogs : ICollection<AuditLog>
}

class Customer <<SmartPOS.Models>> {
  + Id : int
  + UserId : int
  + Name : string
  + Email : string
  + Phone : string?
  + DateOfBirth : DateOnly?
  + Address : string?
  + LoyaltyPoints : int
  + TotalSpent : decimal
  + CreatedAt : DateTime
  --
  + User : User
  + Sales : ICollection<Sale>
  + Reviews : ICollection<Review>
}

class AuditLog <<SmartPOS.Models>> {
  + Id : int
  + UserId : int
  + Action : string
  + Module : string
  + Timestamp : DateTime
  + IPAddress : string?
  + Details : string?
  --
  + User : User
}

' ─────────────────────────────────────────
' CLASSES — PRODUCT CATALOG
' ─────────────────────────────────────────

class Category <<SmartPOS.Models>> {
  + Id : int
  + Name : string
  + ParentCategoryId : int?
  + Description : string?
  + ImageURL : string?
  --
  + ParentCategory : Category?
  + SubCategories : ICollection<Category>
  + Products : ICollection<Product>
}

class Supplier <<SmartPOS.Models>> {
  + Id : int
  + Name : string
  + ContactPerson : string?
  + ContactNo : string?
  + Email : string?
  + Address : string?
  + IsActive : bool
  --
  + Products : ICollection<Product>
  + PurchaseOrders : ICollection<PurchaseOrder>
}

class Product <<SmartPOS.Models>> {
  + Id : int
  + Name : string
  + SKU : string
  + Description : string?
  + ImageURL : string?
  + Price : decimal
  + CostPrice : decimal
  + IsActive : bool
  + CategoryId : int
  + SupplierId : int?
  + CreatedAt : DateTime
  --
  + Category : Category
  + Supplier : Supplier?
  + Inventory : Inventory?
  + SaleItems : ICollection<SaleItem>
  + POLineItems : ICollection<POLineItem>
  + Reviews : ICollection<Review>
}

' ─────────────────────────────────────────
' CLASSES — INVENTORY & PURCHASING
' ─────────────────────────────────────────

class Inventory <<SmartPOS.Models>> {
  + Id : int
  + ProductId : int
  + Quantity : int
  + ReorderLevel : int
  + LastUpdated : DateTime
  --
  + Product : Product
}

class PurchaseOrder <<SmartPOS.Models>> {
  + Id : int
  + SupplierId : int
  + UserId : int
  + Status : POStatus
  + TotalCost : decimal
  + OrderDate : DateTime
  + ReceivedAt : DateTime?
  + Notes : string?
  --
  + Supplier : Supplier
  + User : User
  + LineItems : ICollection<POLineItem>
}

class POLineItem <<SmartPOS.Models>> {
  + Id : int
  + POID : int
  + ProductId : int
  + OrderedQty : int
  + UnitPrice : decimal
  --
  + PurchaseOrder : PurchaseOrder
  + Product : Product
}

' ─────────────────────────────────────────
' CLASSES — SALES
' ─────────────────────────────────────────

class Promotion <<SmartPOS.Models>> {
  + Id : int
  + Code : string
  + DiscountType : DiscountType
  + Value : decimal
  + MinOrderValue : decimal
  + MaxUsageLimit : int?
  + UsageCount : int
  + ValidFrom : DateOnly
  + ValidTo : DateOnly
  + IsActive : bool
  --
  + Sales : ICollection<Sale>
}

class Sale <<SmartPOS.Models>> {
  + Id : int
  + CustomerId : int?
  + UserId : int
  + PromoId : int?
  + SaleType : SaleType
  + TotalAmount : decimal
  + DiscountAmount : decimal
  + TaxAmount : decimal
  + SaleDate : DateTime
  + Status : SaleStatus
  --
  + Customer : Customer?
  + User : User
  + Promotion : Promotion?
  + SaleItems : ICollection<SaleItem>
  + Payment : Payment?
}

class SaleItem <<SmartPOS.Models>> {
  + Id : int
  + SaleId : int
  + ProductId : int
  + Quantity : int
  + UnitPrice : decimal
  + LineTotal : decimal
  --
  + Sale : Sale
  + Product : Product
}

class Payment <<SmartPOS.Models>> {
  + Id : int
  + SaleId : int
  + Method : PaymentMethod
  + Amount : decimal
  + Status : PaymentStatus
  + TransactionRef : string?
  + PaidAt : DateTime?
  --
  + Sale : Sale
}

class Review <<SmartPOS.Models>> {
  + Id : int
  + CustomerId : int
  + ProductId : int
  + Rating : int
  + Comment : string?
  + Sentiment : string?
  + SentimentScore : double?
  + CreatedAt : DateTime
  --
  + Customer : Customer
  + Product : Product
}

' ─────────────────────────────────────────
' RELATIONSHIPS
' ─────────────────────────────────────────

' Role ↔ Permission ↔ User
Role        "1" *-- "0..*" Permission  : has
Role        "1" *-- "0..*" User        : assigned to

' User ↔ Customer (1:1)
User        "1" *-- "0..1" Customer    : extends

' User ↔ AuditLog
User        "1" *-- "0..*" AuditLog   : generates

' Category self-ref
Category    "0..1" o-- "0..*" Category : parent of

' Category ↔ Product
Category    "1" *-- "0..*" Product    : contains

' Supplier ↔ Product
Supplier    "0..1" o-- "0..*" Product  : supplies

' Product ↔ Inventory (1:1)
Product     "1" *-- "1"    Inventory  : tracked by

' Supplier ↔ PurchaseOrder
Supplier    "1" *-- "0..*" PurchaseOrder : receives

' User ↔ PurchaseOrder
User        "1" *-- "0..*" PurchaseOrder : raises

' PurchaseOrder ↔ POLineItem
PurchaseOrder "1" *-- "1..*" POLineItem : contains

' Product ↔ POLineItem
Product     "1" o-- "0..*" POLineItem : ordered in

' Customer ↔ Sale
Customer    "0..1" o-- "0..*" Sale    : places

' User ↔ Sale
User        "1" *-- "0..*" Sale       : processes

' Promotion ↔ Sale
Promotion   "0..1" o-- "0..*" Sale    : applied to

' Sale ↔ SaleItem
Sale        "1" *-- "1..*" SaleItem   : contains

' Product ↔ SaleItem
Product     "1" o-- "0..*" SaleItem   : sold as

' Sale ↔ Payment (1:1)
Sale        "1" *-- "1"    Payment    : paid via

' Customer ↔ Review
Customer    "1" *-- "0..*" Review     : writes

' Product ↔ Review
Product     "1" *-- "0..*" Review     : reviewed in

' Enum usage
Promotion   ..> DiscountType
Sale        ..> SaleType
Sale        ..> SaleStatus
PurchaseOrder ..> POStatus
Payment     ..> PaymentMethod
Payment     ..> PaymentStatus

@enduml
```

### Class Diagram — Relationship Legend

| Symbol | Meaning |
|--------|---------|
| `*--` (filled diamond) | Composition — child cannot exist without parent |
| `o--` (open diamond) | Aggregation — child can exist independently |
| `..>` (dashed arrow) | Dependency — class uses an enum type |
| `"1"` / `"0..*"` | Multiplicity on each end of the relationship |

### Composition vs Aggregation Decisions

| Relationship | Type | Reason |
|---|---|---|
| Role → Permission | Composition | Permissions have no meaning without a Role |
| Role → User | Composition | Users must have a Role |
| User → Customer | Composition | Customer profile is part of the User |
| User → AuditLog | Composition | Logs belong to the User who created them |
| Category → Product | Composition | Products must belong to a Category |
| Product → Inventory | Composition | Inventory record is part of the Product |
| PurchaseOrder → POLineItem | Composition | Line items have no meaning without the PO |
| Sale → SaleItem | Composition | Line items have no meaning without the Sale |
| Sale → Payment | Composition | Payment is part of the Sale |
| Customer → Review | Composition | Reviews belong to the Customer |
| Product → Review | Composition | Reviews belong to the Product |
| Supplier → Product | Aggregation | Products survive if Supplier is deleted (SetNull) |
| Supplier → PurchaseOrder | Aggregation | POs reference Supplier but are independent records |
| Promotion → Sale | Aggregation | Sales survive if Promotion is removed |
| Product → SaleItem | Aggregation | Historical sale items survive if Product is deactivated |
| Product → POLineItem | Aggregation | PO history survives independently |

---

---

## 7. Entity Relationship Diagram (ERD)

The ERD shows the actual database tables, columns, data types, primary keys, foreign keys, unique constraints, and delete behaviors — directly reflecting the `FullDatabaseSetup` migration.

```plantuml
@startuml SmartPOS_ERD

skinparam linetype ortho
skinparam nodesep 50
skinparam ranksep 70
skinparam classBackgroundColor #FFFDF5
skinparam classBorderColor #8B4513
skinparam classHeaderBackgroundColor #F4A460
skinparam arrowColor #8B4513

title SmartPOS+ — Entity Relationship Diagram (ERD)

' ─────────────────────────────────────────
' TABLE: Roles
' ─────────────────────────────────────────
entity "Roles" as Roles {
  * Id : INT <<PK, IDENTITY>>
  --
  * Name : NVARCHAR(50) NOT NULL
}

' ─────────────────────────────────────────
' TABLE: Permissions
' ─────────────────────────────────────────
entity "Permissions" as Permissions {
  * Id : INT <<PK, IDENTITY>>
  --
  * RoleId : INT <<FK → Roles.Id>> <<IDX>>
  * Module : NVARCHAR(100) NOT NULL
  * CanCreate : BIT DEFAULT 0
  * CanRead : BIT DEFAULT 0
  * CanUpdate : BIT DEFAULT 0
  * CanDelete : BIT DEFAULT 0
}

' ─────────────────────────────────────────
' TABLE: Users
' ─────────────────────────────────────────
entity "Users" as Users {
  * Id : INT <<PK, IDENTITY>>
  --
  * Name : NVARCHAR(100) NOT NULL
  * Email : NVARCHAR(150) NOT NULL <<UQ>>
  * PasswordHash : NVARCHAR(255) NOT NULL
  * RoleId : INT <<FK → Roles.Id>> <<IDX>>
  * IsActive : BIT DEFAULT 1
  * CreatedAt : DATETIME2 DEFAULT getutcdate()
}

' ─────────────────────────────────────────
' TABLE: Customers
' ─────────────────────────────────────────
entity "Customers" as Customers {
  * Id : INT <<PK, IDENTITY>>
  --
  * UserId : INT <<FK → Users.Id>> <<UQ>>
  * Name : NVARCHAR(100) NOT NULL
  * Email : NVARCHAR(150) NOT NULL <<UQ>>
  Phone : NVARCHAR(20) NULL
  DateOfBirth : DATE NULL
  Address : TEXT NULL
  * LoyaltyPoints : INT DEFAULT 0
  * TotalSpent : DECIMAL(10,2) DEFAULT 0.00
  * CreatedAt : DATETIME2 DEFAULT getutcdate()
}

' ─────────────────────────────────────────
' TABLE: AuditLogs
' ─────────────────────────────────────────
entity "AuditLogs" as AuditLogs {
  * Id : INT <<PK, IDENTITY>>
  --
  * UserId : INT <<FK → Users.Id>> <<IDX>>
  * Action : NVARCHAR(255) NOT NULL
  * Module : NVARCHAR(100) NOT NULL
  * Timestamp : DATETIME2 DEFAULT getutcdate()
  IPAddress : NVARCHAR(45) NULL
  Details : TEXT NULL
}

' ─────────────────────────────────────────
' TABLE: Categories
' ─────────────────────────────────────────
entity "Categories" as Categories {
  * Id : INT <<PK, IDENTITY>>
  --
  * Name : NVARCHAR(100) NOT NULL
  ParentCategoryId : INT <<FK → Categories.Id, NO ACTION>> NULL
  Description : TEXT NULL
  ImageURL : NVARCHAR(255) NULL
}

' ─────────────────────────────────────────
' TABLE: Suppliers
' ─────────────────────────────────────────
entity "Suppliers" as Suppliers {
  * Id : INT <<PK, IDENTITY>>
  --
  * Name : NVARCHAR(100) NOT NULL
  ContactPerson : NVARCHAR(100) NULL
  ContactNo : NVARCHAR(20) NULL
  Email : NVARCHAR(150) NULL <<UQ>>
  Address : TEXT NULL
  * IsActive : BIT DEFAULT 1
}

' ─────────────────────────────────────────
' TABLE: Products
' ─────────────────────────────────────────
entity "Products" as Products {
  * Id : INT <<PK, IDENTITY>>
  --
  * Name : NVARCHAR(150) NOT NULL
  * SKU : NVARCHAR(50) NOT NULL <<UQ>>
  Description : TEXT NULL
  ImageURL : NVARCHAR(255) NULL
  * Price : DECIMAL(10,2) NOT NULL
  * CostPrice : DECIMAL(10,2) NOT NULL
  * IsActive : BIT DEFAULT 1
  * CategoryId : INT <<FK → Categories.Id, RESTRICT>>
  SupplierId : INT <<FK → Suppliers.Id, SET NULL>> NULL
  * CreatedAt : DATETIME2 DEFAULT getutcdate()
}

' ─────────────────────────────────────────
' TABLE: Inventories
' ─────────────────────────────────────────
entity "Inventories" as Inventories {
  * Id : INT <<PK, IDENTITY>>
  --
  * ProductId : INT <<FK → Products.Id>> <<UQ>>
  * Quantity : INT DEFAULT 0
  * ReorderLevel : INT NOT NULL
  * LastUpdated : DATETIME2 DEFAULT getutcdate()
}

' ─────────────────────────────────────────
' TABLE: Promotions
' ─────────────────────────────────────────
entity "Promotions" as Promotions {
  * Id : INT <<PK, IDENTITY>>
  --
  * Code : NVARCHAR(50) NOT NULL <<UQ>>
  * DiscountType : INT NOT NULL
  * Value : DECIMAL(10,2) NOT NULL
  * MinOrderValue : DECIMAL(10,2) DEFAULT 0.00
  MaxUsageLimit : INT NULL
  * UsageCount : INT DEFAULT 0
  * ValidFrom : DATE NOT NULL
  * ValidTo : DATE NOT NULL
  * IsActive : BIT DEFAULT 1
}

' ─────────────────────────────────────────
' TABLE: PurchaseOrders
' ─────────────────────────────────────────
entity "PurchaseOrders" as PurchaseOrders {
  * Id : INT <<PK, IDENTITY>>
  --
  * SupplierId : INT <<FK → Suppliers.Id>>
  * UserId : INT <<FK → Users.Id>>
  * Status : INT DEFAULT 0
  * TotalCost : DECIMAL(10,2) NOT NULL
  * OrderDate : DATETIME2 DEFAULT getutcdate()
  ReceivedAt : DATETIME2 NULL
  Notes : TEXT NULL
}

' ─────────────────────────────────────────
' TABLE: POLineItems
' ─────────────────────────────────────────
entity "POLineItems" as POLineItems {
  * Id : INT <<PK, IDENTITY>>
  --
  * POID : INT <<FK → PurchaseOrders.Id, CASCADE>> <<IDX>>
  * ProductId : INT <<FK → Products.Id>>
  * OrderedQty : INT NOT NULL
  * UnitPrice : DECIMAL(10,2) NOT NULL
}

' ─────────────────────────────────────────
' TABLE: Sales
' ─────────────────────────────────────────
entity "Sales" as Sales {
  * Id : INT <<PK, IDENTITY>>
  --
  CustomerId : INT <<FK → Customers.Id>> NULL
  * UserId : INT <<FK → Users.Id>>
  PromoId : INT <<FK → Promotions.Id>> NULL
  * SaleType : INT NOT NULL
  * TotalAmount : DECIMAL(10,2) NOT NULL
  * DiscountAmount : DECIMAL(10,2) DEFAULT 0.00
  * TaxAmount : DECIMAL(10,2) NOT NULL
  * SaleDate : DATETIME2 DEFAULT getutcdate()
  * Status : INT DEFAULT 0
}

' ─────────────────────────────────────────
' TABLE: SaleItems
' ─────────────────────────────────────────
entity "SaleItems" as SaleItems {
  * Id : INT <<PK, IDENTITY>>
  --
  * SaleId : INT <<FK → Sales.Id, CASCADE>> <<IDX>>
  * ProductId : INT <<FK → Products.Id, RESTRICT>>
  * Quantity : INT NOT NULL
  * UnitPrice : DECIMAL(10,2) NOT NULL
  * LineTotal : DECIMAL(10,2) NOT NULL
}

' ─────────────────────────────────────────
' TABLE: Reviews
' ─────────────────────────────────────────
entity "Reviews" as Reviews {
  * Id : INT <<PK, IDENTITY>>
  --
  * CustomerId : INT <<FK → Customers.Id>>
  * ProductId : INT <<FK → Products.Id>>
  * Rating : INT NOT NULL <<CHECK: 1-5>>
  Comment : TEXT NULL
  Sentiment : NVARCHAR(20) NULL
  SentimentScore : FLOAT NULL
  * CreatedAt : DATETIME2 DEFAULT getutcdate()
}

' ─────────────────────────────────────────
' TABLE: Payments
' ─────────────────────────────────────────
entity "Payments" as Payments {
  * Id : INT <<PK, IDENTITY>>
  --
  * SaleId : INT <<FK → Sales.Id>> <<UQ>>
  * Method : INT NOT NULL
  * Amount : DECIMAL(10,2) NOT NULL
  * Status : INT DEFAULT 0
  TransactionRef : NVARCHAR(255) NULL
  PaidAt : DATETIME2 NULL
}

' ─────────────────────────────────────────
' FOREIGN KEY RELATIONSHIPS
' ─────────────────────────────────────────

' Access Control
Roles        ||--o{ Permissions     : "RoleId"
Roles        ||--o{ Users           : "RoleId"

' User extensions
Users        ||--|| Customers       : "UserId (1:1)"
Users        ||--o{ AuditLogs       : "UserId"
Users        ||--o{ Sales           : "UserId"
Users        ||--o{ PurchaseOrders  : "UserId"

' Product catalog
Categories   |o--o{ Categories      : "ParentCategoryId (self-ref)"
Categories   ||--o{ Products        : "CategoryId (RESTRICT)"
Suppliers    |o--o{ Products        : "SupplierId (SET NULL)"
Suppliers    ||--o{ PurchaseOrders  : "SupplierId"

' Inventory
Products     ||--|| Inventories     : "ProductId (1:1)"

' Purchase Orders
PurchaseOrders ||--o{ POLineItems   : "POID (CASCADE)"
Products       ||--o{ POLineItems   : "ProductId"

' Sales
Customers    |o--o{ Sales           : "CustomerId (nullable)"
Promotions   |o--o{ Sales           : "PromoId (nullable)"
Sales        ||--o{ SaleItems       : "SaleId (CASCADE)"
Products     ||--o{ SaleItems       : "ProductId (RESTRICT)"
Sales        ||--|| Payments        : "SaleId (1:1)"

' Reviews
Customers    ||--o{ Reviews         : "CustomerId"
Products     ||--o{ Reviews         : "ProductId"

@enduml
```

### ERD — Constraints & Delete Behaviors Summary

| Table | Column | Constraint | Delete Behavior |
|-------|--------|-----------|----------------|
| Users | Email | UNIQUE | — |
| Customers | UserId | UNIQUE (1:1 with Users) | Cascade |
| Customers | Email | UNIQUE | — |
| Products | SKU | UNIQUE | — |
| Products | CategoryId | NOT NULL | **Restrict** — cannot delete Category with Products |
| Products | SupplierId | NULLABLE | **SetNull** — Supplier deleted → SupplierId = NULL |
| Inventories | ProductId | UNIQUE (1:1 with Products) | Cascade |
| Promotions | Code | UNIQUE | — |
| Suppliers | Email | UNIQUE (nullable) | — |
| Categories | ParentCategoryId | NULLABLE (self-ref) | **NoAction** — prevents cascade cycles |
| Sales | CustomerId | NULLABLE | No action (guest checkout) |
| Sales | PromoId | NULLABLE | No action |
| SaleItems | SaleId | INDEX | **Cascade** — delete Sale removes its items |
| SaleItems | ProductId | NOT NULL | **Restrict** — cannot delete Product with sale history |
| PurchaseOrders | POID | INDEX | **Cascade** — delete PO removes its line items |
| Payments | SaleId | UNIQUE (1:1 with Sales) | Cascade |
| Reviews | Rating | CHECK (1–5) | — |

### ERD — Crow's Foot Notation Key

| Symbol | Meaning |
|--------|---------|
| `\|\|` | Exactly one (mandatory) |
| `\|o` | Zero or one (optional) |
| `o{` | Zero or many |
| `\|{` | One or many |

---

---

## 8. Sequence Diagrams

Four critical system flows are documented below.

---

### 8.1 Sequence Diagram — User Login & Role-Based Redirect

This diagram shows what happens from the moment a user selects their role and submits credentials to when they land on their dashboard.

```plantuml
@startuml SD_Login

skinparam sequenceArrowThickness 2
skinparam sequenceParticipantBackgroundColor #FFF8F0
skinparam sequenceParticipantBorderColor #CC6600
skinparam sequenceLifeLineBorderColor #CC6600
skinparam sequenceMessageAlign center
skinparam noteBorderColor #CC6600
skinparam noteBackgroundColor #FFF0D0

title SmartPOS+ — Sequence Diagram: User Login & Role-Based Redirect

actor       "User"              as U
participant "Login.razor"       as LR
participant "AuthService"       as AS
participant "AppDbContext"      as DB
participant "JwtService"        as JWT
participant "LocalStorage"      as LS
participant "AuthStateProvider" as ASP
participant "NavigationManager" as NM

== Step 1: Role Selection ==

U  -> LR  : Open /login
LR -> U   : Display role selection grid\n(Admin | Manager | Cashier | Customer)
U  -> LR  : Click role button (e.g. "Cashier")
LR -> LR  : selectedRole = "Cashier"\nReset loginModel

== Step 2: Credential Entry ==

U  -> LR  : Enter email + password
U  -> LR  : Click "Sign In as Cashier"
LR -> LR  : Validate form (DataAnnotationsValidator)

alt Validation fails
  LR -> U : Show inline validation errors
else Validation passes

  == Step 3: Authentication ==

  LR -> AS  : LoginAsync(email, password)
  AS -> DB  : Users.Include(Role)\n.FirstOrDefault(u => u.Email == email)

  alt User not found
    DB --> AS : null
    AS --> LR : { Token: null, User: null }
    LR -> U   : Show "Invalid email or password."
  else User found
    DB --> AS : User entity (with Role)
    AS -> AS  : BCrypt.Verify(password, user.PasswordHash)

    alt Password mismatch
      AS --> LR : { Token: null, User: null }
      LR -> U   : Show "Invalid email or password."
    else Password correct
      AS -> AS  : Check user.IsActive == true

      alt Account inactive
        AS --> LR : { Token: null, User: null }
        LR -> U   : Show "Account is deactivated."
      else Account active
        AS -> JWT : GenerateToken(user)
        JWT --> AS : JWT string (contains userId, email, role)
        AS --> LR  : { Token: "eyJ...", User: userDto }

        == Step 4: Role Enforcement ==

        LR -> LR : Compare user.Role.Name vs selectedRole
        alt Role mismatch (e.g. user is Admin, selected Cashier)
          LR -> U : Show "Access Denied. These credentials\nare for an Admin, not a Cashier."
        else Role matches
          == Step 5: Session Setup ==

          LR -> LS  : SetItemAsync("authToken", token)
          LR -> ASP : MarkUserAsAuthenticated(token)
          ASP -> ASP : Parse JWT claims\nSet ClaimsPrincipal

          == Step 6: Role-Based Redirect ==

          LR -> NM  : NavigateTo(role-specific route)
          note right of NM
            Admin    → /admin/dashboard
            Manager  → /manager/inventory
            Cashier  → /cashier/pos
            Customer → /customer/shop
          end note
          NM -> U   : Load role-specific page
        end
      end
    end
  end
end

@enduml
```

---

### 8.2 Sequence Diagram — Cashier POS Sale (Onsite)

This diagram shows a cashier processing a sale at the POS terminal — from product search through payment to inventory update.

```plantuml
@startuml SD_POSSale

skinparam sequenceArrowThickness 2
skinparam sequenceParticipantBackgroundColor #FFF8F0
skinparam sequenceParticipantBorderColor #CC6600
skinparam sequenceLifeLineBorderColor #CC6600
skinparam sequenceMessageAlign center
skinparam noteBorderColor #CC6600
skinparam noteBackgroundColor #FFF0D0

title SmartPOS+ — Sequence Diagram: Cashier POS Sale (Onsite)

actor       "Cashier"           as C
participant "Pos.razor"         as POS
participant "SaleService"       as SS
participant "PromoService"      as PS
participant "AppDbContext"      as DB
participant "InventoryService"  as IS
participant "AuditService"      as AU

== Step 1: Product Search ==

C   -> POS : Enter product name or SKU
POS -> DB  : Products.Where(IsActive)\n.Where(name/SKU match)
DB  --> POS : List<Product>
POS -> C   : Display matching products

== Step 2: Build Cart ==

loop For each product
  C   -> POS : Click "Add to Cart" + enter quantity
  POS -> POS : Add SaleItem to cart\nLineTotal = Qty × UnitPrice
end

POS -> POS : SubTotal = sum of LineTotals

== Step 3: Apply Promotion (optional) ==

opt Customer has promo code
  C   -> POS : Enter promo code
  POS -> PS  : ValidatePromo(code, subTotal)
  PS  -> DB  : Promotions.FirstOrDefault(p =>\n  p.Code == code &&\n  p.IsActive &&\n  p.ValidFrom <= today &&\n  p.ValidTo >= today &&\n  (p.MaxUsageLimit == null ||\n   p.UsageCount < p.MaxUsageLimit) &&\n  subTotal >= p.MinOrderValue)

  alt Promo invalid / expired / limit reached
    DB  --> PS  : null
    PS  --> POS : PromoResult { Valid: false, Message: "..." }
    POS -> C    : Show error message
  else Promo valid
    DB  --> PS  : Promotion entity
    PS  -> PS   : Calculate discount\n(Percentage: subTotal × value/100)\n(Flat: value)
    PS  --> POS : PromoResult { Valid: true, Discount: amount }
    POS -> POS  : DiscountAmount = result.Discount
    POS -> C    : Show discount applied
  end
end

== Step 4: Calculate Totals ==

POS -> POS : TaxAmount = (SubTotal - Discount) × 0.17
POS -> POS : TotalAmount = SubTotal - Discount + Tax
POS -> C   : Display order summary

== Step 5: Process Payment ==

C   -> POS : Select payment method (Cash)
C   -> POS : Click "Complete Sale"
POS -> SS  : CreateSale(cart, userId, promoId?, method)

SS  -> DB  : BEGIN TRANSACTION
SS  -> DB  : INSERT INTO Sales\n(CustomerId, UserId, PromoId,\nSaleType=Onsite, TotalAmount,\nDiscountAmount, TaxAmount,\nSaleDate, Status=Completed)
DB  --> SS : Sale.Id

loop For each SaleItem
  SS -> DB : INSERT INTO SaleItems\n(SaleId, ProductId, Qty, UnitPrice, LineTotal)
end

SS  -> DB  : INSERT INTO Payments\n(SaleId, Method=Cash,\nAmount, Status=Completed, PaidAt=now)

== Step 6: Update Inventory ==

loop For each SaleItem
  SS -> IS : DecrementStock(productId, qty)
  IS -> DB : UPDATE Inventories\nSET Quantity -= qty,\nLastUpdated = now\nWHERE ProductId = id
  IS -> IS : Check if Quantity <= ReorderLevel
  opt Low stock
    IS -> C : Show low-stock alert for product
  end
end

== Step 7: Update Promo Usage ==

opt Promo was applied
  SS -> DB : UPDATE Promotions\nSET UsageCount += 1\nWHERE Id = promoId
end

SS  -> DB  : COMMIT TRANSACTION
DB  --> SS : Success

== Step 8: Audit Log ==

SS  -> AU  : LogAction(userId, "Completed Sale",\n"Sales", saleId)
AU  -> DB  : INSERT INTO AuditLogs

SS  --> POS : Sale completed, saleId
POS -> C   : Show receipt / success message
POS -> POS : Clear cart

@enduml
```

---

### 8.3 Sequence Diagram — Customer Online Order

This diagram shows a customer placing an order through the online shop with card payment via Stripe.

```plantuml
@startuml SD_OnlineOrder

skinparam sequenceArrowThickness 2
skinparam sequenceParticipantBackgroundColor #FFF8F0
skinparam sequenceParticipantBorderColor #CC6600
skinparam sequenceLifeLineBorderColor #CC6600
skinparam sequenceMessageAlign center
skinparam noteBorderColor #CC6600
skinparam noteBackgroundColor #FFF0D0

title SmartPOS+ — Sequence Diagram: Customer Online Order

actor       "Customer"          as CU
participant "Shop.razor"        as SH
participant "SaleService"       as SS
participant "PromoService"      as PS
participant "StripeService"     as STR
participant "AppDbContext"      as DB
participant "InventoryService"  as IS
participant "LoyaltyService"    as LS
participant "AuditService"      as AU

== Step 1: Browse Shop ==

CU  -> SH  : Open /customer/shop
SH  -> DB  : Products.Where(IsActive)\n.Include(Category)\n.Include(Inventory)
DB  --> SH : List<Product> (with stock info)
SH  -> CU  : Display product grid

== Step 2: Add to Cart ==

loop For each product
  CU  -> SH  : Click "Add to Cart" + quantity
  SH  -> SH  : Validate stock available
  SH  -> SH  : Add to cart, update totals
end

== Step 3: Apply Promo (optional) ==

opt Customer enters promo code
  CU  -> SH  : Enter promo code + click Apply
  SH  -> PS  : ValidatePromo(code, subTotal)
  PS  -> DB  : Query Promotions (same validation as POS)
  alt Invalid
    PS  --> SH : { Valid: false }
    SH  -> CU  : Show error
  else Valid
    PS  --> SH : { Valid: true, Discount: amount }
    SH  -> SH  : Apply discount to totals
    SH  -> CU  : Show discount applied
  end
end

== Step 4: Checkout Summary ==

SH  -> SH  : TaxAmount = (SubTotal - Discount) × 0.17
SH  -> SH  : TotalAmount = SubTotal - Discount + Tax
SH  -> CU  : Show order summary + total

== Step 5: Card Payment via Stripe ==

CU  -> SH  : Click "Pay by Card"
SH  -> STR : CreatePaymentIntent(amount, currency="PKR")
STR --> SH : { clientSecret, paymentIntentId }
SH  -> CU  : Show Stripe card input form

CU  -> STR : Enter card details + confirm payment
STR -> STR : Process card charge
alt Payment failed
  STR --> SH : { status: "failed", error: "..." }
  SH  -> CU  : Show payment failure message
else Payment succeeded
  STR --> SH : { status: "succeeded",\nchargeId: "ch_xxx" }

  == Step 6: Create Sale Record ==

  SH  -> SS  : CreateSale(cart, customerId, userId,\npromoId?, method=Online,\ntransactionRef="ch_xxx")

  SS  -> DB  : BEGIN TRANSACTION
  SS  -> DB  : INSERT INTO Sales\n(CustomerId, UserId, PromoId,\nSaleType=Online, TotalAmount,\nDiscountAmount, TaxAmount,\nSaleDate, Status=Completed)
  DB  --> SS : Sale.Id

  loop For each SaleItem
    SS -> DB : INSERT INTO SaleItems
  end

  SS  -> DB  : INSERT INTO Payments\n(SaleId, Method=Online,\nAmount, Status=Completed,\nTransactionRef="ch_xxx", PaidAt=now)

  == Step 7: Update Inventory ==

  loop For each SaleItem
    SS -> IS : DecrementStock(productId, qty)
    IS -> DB : UPDATE Inventories SET Quantity -= qty
  end

  == Step 8: Update Loyalty Points ==

  SS  -> LS  : AwardPoints(customerId, totalAmount)
  LS  -> LS  : points = floor(totalAmount / 10)
  LS  -> DB  : UPDATE Customers\nSET LoyaltyPoints += points,\nTotalSpent += totalAmount\nWHERE Id = customerId

  == Step 9: Update Promo Usage ==

  opt Promo applied
    SS -> DB : UPDATE Promotions SET UsageCount += 1
  end

  SS  -> DB  : COMMIT TRANSACTION

  == Step 10: Audit Log ==

  SS  -> AU  : LogAction(userId, "Online Order Placed",\n"Sales", saleId)
  AU  -> DB  : INSERT INTO AuditLogs

  SS  --> SH : Sale completed, saleId
  SH  -> CU  : Show order confirmation + receipt
end

@enduml
```

---

### 8.4 Sequence Diagram — Manager Purchase Order & Stock Receive

This diagram shows a manager creating a purchase order and then receiving the stock to update inventory.

```plantuml
@startuml SD_PurchaseOrder

skinparam sequenceArrowThickness 2
skinparam sequenceParticipantBackgroundColor #FFF8F0
skinparam sequenceParticipantBorderColor #CC6600
skinparam sequenceLifeLineBorderColor #CC6600
skinparam sequenceMessageAlign center
skinparam noteBorderColor #CC6600
skinparam noteBackgroundColor #FFF0D0

title SmartPOS+ — Sequence Diagram: Purchase Order & Stock Receive

actor       "Manager"           as M
participant "Inventory.razor"   as INV
participant "POService"         as POS
participant "AppDbContext"      as DB
participant "InventoryService"  as IS
participant "AuditService"      as AU

== Step 1: View Low Stock Alert ==

M   -> INV : Open /manager/inventory
INV -> DB  : Inventories.Include(Product)\n.Where(i => i.Quantity <= i.ReorderLevel)
DB  --> INV : List<Inventory> (low stock items)
INV -> M   : Highlight low-stock products in red

== Step 2: Create Purchase Order ==

M   -> INV : Click "Create Purchase Order"
INV -> DB  : Suppliers.Where(IsActive).ToList()
DB  --> INV : List<Supplier>
INV -> M   : Show PO creation form

M   -> INV : Select supplier
M   -> INV : Add line items (product + qty + unit price)
M   -> INV : Add optional notes
M   -> INV : Click "Save as Draft"

INV -> POS : CreatePurchaseOrder(supplierId, userId,\nlineItems, notes)
POS -> POS : TotalCost = sum(qty × unitPrice)
POS -> DB  : INSERT INTO PurchaseOrders\n(SupplierId, UserId, Status=Draft,\nTotalCost, OrderDate, Notes)
DB  --> POS : PurchaseOrder.Id

loop For each line item
  POS -> DB : INSERT INTO POLineItems\n(POID, ProductId, OrderedQty, UnitPrice)
end

POS -> AU  : LogAction(userId, "Created PO #id",\n"PurchaseOrders")
AU  -> DB  : INSERT INTO AuditLogs
POS --> INV : PO created (id, Status=Draft)
INV -> M   : Show PO summary

== Step 3: Send Purchase Order ==

M   -> INV : Click "Send to Supplier"
INV -> POS : UpdatePOStatus(poId, Status=Sent)
POS -> DB  : UPDATE PurchaseOrders\nSET Status = Sent\nWHERE Id = poId
POS -> AU  : LogAction(userId, "Sent PO #id", "PurchaseOrders")
AU  -> DB  : INSERT INTO AuditLogs
POS --> INV : Status updated
INV -> M   : Show "PO sent to supplier"

note over M, INV
  Time passes — supplier delivers goods
end note

== Step 4: Receive Stock ==

M   -> INV : Open PO list, find PO in "Sent" status
M   -> INV : Click "Mark as Received"
INV -> POS : ReceivePurchaseOrder(poId, userId)

POS -> DB  : SELECT POLineItems WHERE POID = poId
DB  --> POS : List<POLineItem>

POS -> DB  : BEGIN TRANSACTION

POS -> DB  : UPDATE PurchaseOrders\nSET Status = Received,\nReceivedAt = now\nWHERE Id = poId

loop For each POLineItem
  POS -> IS  : IncrementStock(productId, orderedQty)
  IS  -> DB  : UPDATE Inventories\nSET Quantity += orderedQty,\nLastUpdated = now\nWHERE ProductId = productId
  IS  --> POS : Updated quantity
end

POS -> DB  : COMMIT TRANSACTION

== Step 5: Audit & Confirmation ==

POS -> AU  : LogAction(userId,\n"Received PO #id — stock updated",\n"PurchaseOrders")
AU  -> DB  : INSERT INTO AuditLogs

POS --> INV : Stock updated successfully
INV -> DB  : Reload Inventories (low stock check)
DB  --> INV : Updated stock levels
INV -> M   : Show updated inventory\n(low-stock alerts cleared for received items)

@enduml
```

---

### 8.5 Sequence Diagram — Customer Submits Review with BERT Sentiment

```plantuml
@startuml SD_Review

skinparam sequenceArrowThickness 2
skinparam sequenceParticipantBackgroundColor #FFF8F0
skinparam sequenceParticipantBorderColor #CC6600
skinparam sequenceLifeLineBorderColor #CC6600
skinparam sequenceMessageAlign center
skinparam noteBorderColor #CC6600
skinparam noteBackgroundColor #FFF0D0

title SmartPOS+ — Sequence Diagram: Customer Review + BERT Sentiment

actor       "Customer"          as CU
participant "Shop.razor"        as SH
participant "ReviewService"     as RS
participant "BertApiClient"     as BERT
participant "AppDbContext"      as DB
participant "AuditService"      as AU

== Step 1: Open Review Form ==

CU  -> SH   : Click "Leave a Review" on product
SH  -> DB   : Verify customer has purchased this product\n(SaleItems JOIN Sales WHERE CustomerId = id)
alt Customer has not purchased product
  DB  --> SH  : No matching sale
  SH  -> CU   : Show "You can only review products you have purchased."
else Customer has purchased
  DB  --> SH  : Purchase confirmed
  SH  -> CU   : Show review form (rating 1–5 + comment box)

  == Step 2: Submit Review ==

  CU  -> SH   : Select rating (e.g. 4 stars)
  CU  -> SH   : Type comment text
  CU  -> SH   : Click "Submit Review"
  SH  -> SH   : Validate rating (1–5) and comment not empty

  SH  -> RS   : SubmitReview(customerId, productId,\nrating, comment)

  == Step 3: Save Review (Pending Sentiment) ==

  RS  -> DB   : INSERT INTO Reviews\n(CustomerId, ProductId, Rating,\nComment, Sentiment=null,\nSentimentScore=null, CreatedAt=now)
  DB  --> RS  : Review.Id

  RS  --> SH  : Review saved (id)
  SH  -> CU   : Show "Review submitted! Analysing sentiment..."

  == Step 4: BERT Sentiment Analysis (async) ==

  RS  -> BERT : POST /analyse\n{ text: comment }
  note right of BERT
    BERT model classifies text as:
    Positive  → score = 1.0
    Neutral   → score = 0.5
    Negative  → score = 0.0
  end note

  alt BERT service unavailable
    BERT --> RS : HTTP 503 / timeout
    RS   -> RS  : Log warning, leave sentiment null
    RS   -> AU  : LogAction(customerId,\n"Review submitted (sentiment pending)",\n"Reviews")
  else BERT responds
    BERT --> RS : { sentiment: "Positive", score: 0.92 }

    == Step 5: Update Review with Sentiment ==

    RS  -> DB   : UPDATE Reviews\nSET Sentiment = "Positive",\nSentimentScore = 0.92\nWHERE Id = reviewId
    DB  --> RS  : Updated

    RS  -> AU   : LogAction(customerId,\n"Review submitted — Positive",\n"Reviews")
    AU  -> DB   : INSERT INTO AuditLogs

    RS  --> SH  : Sentiment result
    SH  -> CU   : Update review display\nwith sentiment badge (😊 Positive)
  end
end

@enduml
```

---

### Sequence Diagram Summary

| Diagram | Actors | Key Steps |
|---------|--------|-----------|
| **8.1 Login** | User, Login.razor, AuthService, JWT, LocalStorage, AuthStateProvider | Role select → credential validation → BCrypt verify → JWT issue → role-match check → redirect |
| **8.2 POS Sale** | Cashier, Pos.razor, SaleService, PromoService, DB, InventoryService | Product search → cart build → promo apply → tax calc → payment → inventory decrement → audit |
| **8.3 Online Order** | Customer, Shop.razor, SaleService, Stripe, DB, LoyaltyService | Browse → cart → promo → Stripe charge → sale record → inventory → loyalty points → audit |
| **8.4 Purchase Order** | Manager, Inventory.razor, POService, DB, InventoryService | Low-stock view → PO create → send → receive → stock increment → audit |
| **8.5 Review + BERT** | Customer, Shop.razor, ReviewService, BERT API, DB | Purchase verify → review save → BERT async call → sentiment update → audit |

---
