# SportMania 🏆

SportMania is a subscription-based sports service platform built with ASP.NET Core that enables users to purchase subscription plans and receive redemption keys for access to sports services.

## 🎯 What This Application Can Do

### Core Features

1. **Subscription Plan Management**
   - Browse available subscription plans with detailed information
   - View plan pricing, duration, and features
   - Display plan images and descriptions
   - Admin functionality to create, update, and delete plans

2. **Payment Processing**
   - Integrated with ToyyibPay payment gateway (Malaysian payment system)
   - Secure online payment handling
   - Support for multiple plan categories (Daily, Weekly, Monthly, Seasonal)
   - Automatic price conversion (RM to cents)
   - Real-time payment status updates

3. **Redemption Key System**
   - Automatic generation of unique 16-character alphanumeric redemption codes
   - Keys delivered via email upon successful payment
   - Key redemption tracking
   - Prevention of duplicate key usage

4. **Transaction Management**
   - Complete transaction history tracking
   - Real-time payment status monitoring (Pending, Success, Failed)
   - Customer details recording (email, phone number, Discord username)
   - Secure transaction callback handling

5. **Customer Management**
   - Store customer information for repeat purchases
   - Link customers to their transactions and keys
   - Customer identification via email, phone, and Discord username
   - Discord username field available for community integration

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core (.NET 10.0)
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Payment Gateway**: ToyyibPay API
- **Frontend**: Razor Pages/MVC Views
- **Architecture**: Repository Pattern + Service Layer

## 📋 Prerequisites

- .NET 10.0 SDK or later
- PostgreSQL database
- ToyyibPay account (for payment processing)

## 🚀 Getting Started

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/HafiyKharu/SportMania.git
   cd SportMania
   ```

2. Configure database connection:
   - Update `appsettings.json` with your PostgreSQL connection string:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=sportmania;Username=your_user;Password=your_password"
     }
     ```

3. Configure ToyyibPay:
   - Add your ToyyibPay credentials to user secrets or configuration
   - Update the payment handler with your API keys
   - Note: Email delivery is handled by ToyyibPay's system

4. Run database migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. Navigate to `https://localhost:5001` (or the configured URL)

## 📁 Project Structure

```
SportMania/
├── Controllers/         # MVC Controllers
│   ├── HomeController.cs
│   ├── PlanController.cs
│   ├── TransactionController.cs
│   └── PaymentController.cs
├── Models/             # Domain Models
│   ├── Customer.cs
│   ├── Plan.cs
│   ├── PlanDetails.cs
│   ├── Transaction.cs
│   └── Key.cs
├── Services/           # Business Logic Layer
│   ├── TransactionService.cs
│   └── KeyService.cs
├── Repository/         # Data Access Layer
│   ├── CustomerRepository.cs
│   ├── PlanRepository.cs
│   ├── TransactionRepository.cs
│   ├── KeyRepository.cs
│   └── PlanDetailsRepository.cs
├── Handlers/           # External API Handlers
│   └── ToyyibPayHandler.cs
├── Data/              # Database Context
├── Views/             # Razor Views
└── wwwroot/           # Static Files
```

## 💳 Payment Flow

1. User selects a subscription plan
2. User enters email, phone number, and optionally Discord username
3. Transaction is created with "Pending" status
4. Unique redemption key is generated
5. User is redirected to ToyyibPay payment gateway
6. Upon payment completion, ToyyibPay sends callback
7. Transaction status is updated (Success/Failed)
8. Redemption key is emailed to customer via ToyyibPay (on success)

## 🔑 Key Features Details

### Plan Categories
- **Daily Plans**: Short-term access
- **Weekly Plans**: Week-long subscriptions
- **Monthly Plans**: One-month access
- **Seasonal Plans**: Extended period subscriptions

### Security Features
- ASP.NET Core Identity for user management
- Secure payment callback verification
- Database-level soft deletes
- Transaction integrity checks

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📧 Contact

For questions or support, please contact the repository owner.
