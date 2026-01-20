# SportMania Bot

## Description

SportMania Bot is a comprehensive solution for managing subscriptions and digital content access through Discord. It integrates with the ToyyibPay payment gateway to handle transactions and automatically assigns Discord roles to users based on their active subscription plans. The bot is managed via slash commands within your Discord server.

## Features

-   **Discord Slash Commands:** Easy-to-use commands for managing the bot.
-   **Plan & Role Management:** Map subscription plans to specific Discord roles.
-   **License Key Generation:** Admins can generate license keys for different plans.
-   **Key Redemption:** Users can redeem license keys to gain access.
-   **Payment Integration:** Seamlessly handles payments using ToyyibPay.
-   **Automated Role Assignment:** Automatically grants and (in the future) revokes roles based on subscription status.

## Setup and Configuration

1.  **Prerequisites:**
    *   .NET 10 SDK
    *   PostgreSQL Database
    *   A Discord Bot application with a token.

2.  **Configuration:**
    Update the `appsettings.json` file with your specific settings.

    ```json
    {
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=sportmania;Username=postgres;Password=your_password"
    },
    "Logging": {
        "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ToyyibPay": {
        "UserSecretKey": "ABCD",
        "CategoryCodes": {
        "Seasonal": "ABCD",
        "Daily": "ABCD",
        "Monthly": "ABCD",
        "Weekly": "ABCD"
        }
    },
    "Discord": {
        "BotToken": "ABCD.ABCD.ABCD"
    },
    "DiscordBot": {
            "Enabled": true // false 
    },
    "GuildsID": 
    {
        "DefaultGuildId": 1234
    }
    }
    ```

3.  **Run the Application:**
    Use the .NET CLI to run the application. The bot will automatically connect to Discord if `DiscordBot:Enabled` is set to `true`.
    ```bash
    dotnet run
    ```

## Discord Commands

The following slash commands are available for managing the bot.

---

### `/setuproles`

Maps a subscription plan to a Discord role. Users who purchase or redeem a key for this plan will be granted the specified role.

-   **Syntax:** `/setuproles plan:<Plan Name> role:<@Role>`
-   **Permissions:** Administrator only.

-   **Examples:**
    -   `/setuproles plan:Season Pass role:@Season Pass Holder`
    -   `/setuproles plan:Monthly Plan role:@Subscriber`
    -   `/setuproles plan:Weekly Pass role:@Weekly User`

---

### `/viewmappings`

Displays all current plan-to-role mappings for the server.

-   **Syntax:** `/viewmappings`
-   **Permissions:** Administrator only.

---

### `/removemapping`

Removes an existing plan-to-role mapping.

-   **Syntax:** `/removemapping plan:<Plan Name>`
-   **Permissions:** Administrator only.

-   **Example:**
    -   `/removemapping plan:Weekly Pass`

---

### `/generate`

Generates one or more license keys for a specific plan.

-   **Syntax:** `/generate plan:<Plan Name> amount:[Number]`
-   **Permissions:** Administrator only.

-   **Examples:**
    -   `/generate plan:Monthly Plan amount:10` (Generates 10 keys)
    -   `/generate plan:Season Pass` (Generates 1 key)

---

### `/redeem`

Allows a user to redeem a license key to gain access and the associated role.

-   **Syntax:** `/redeem key:<License Key>`
-   **Permissions:** All users.

-   **Example:**
    -   `/redeem key:ABCD-EFGH-IJKL-MNOP-QRST`