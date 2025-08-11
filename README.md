# DEA.Next - Download Email Attachments

**DEA (Download Email Attachments)** is a comprehensive .NET 8.0 application suite designed to automatically download email attachments from Microsoft Exchange Online using the Microsoft Graph API and process them through various delivery methods including FTP/SFTP and REST API integration.

## 🏗️ Architecture Overview

DEA.Next consists of four interconnected projects:

- **DEA** - Core console application for automated email processing
- **DEA.UI** - Windows Forms management interface
- **DEACleaner** - Log file maintenance utility  
- **DEAMailer** - Email notification service

## ✨ Key Features

- **Microsoft Graph Integration**: Secure OAuth2 authentication with Exchange Online
- **Multi-Protocol File Transfer**: Support for FTP, FTPS, and SFTP
- **REST API Integration**: Automated file and data submission to web services
- **Database Management**: PostgreSQL backend with Entity Framework Core
- **Customer Configuration**: Multi-tenant support with individual client settings
- **Automated Processing**: Queue-based processing with configurable batch sizes
- **Comprehensive Logging**: Structured logging with Serilog
- **PDF Generation**: Built-in PDF creation capabilities
- **Windows Forms UI**: User-friendly interface for configuration management

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 Runtime
- PostgreSQL Database
- Microsoft Graph API access (Azure AD application registration)
- Visual Studio 2022 or VS Code (for development)

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd DEA.Next
```

2. **Build the solution**
```bash
dotnet build DEA.Next.sln
```

3. **Configure Database**
```bash
# Update database with migrations
dotnet ef database update --project DEA
```

4. **Configure Application Settings**
   - Copy `DEA/Config/_appsettings.json.bkp` to `appsettings.json`
   - Copy `DEA/Config/_CustomerConfig.json.bkp` to `CustomerConfig.json`
   - Update connection strings and Graph API credentials

5. **Run the Application**
```bash
# Console application
dotnet run --project DEA

# UI application (Windows only)
dotnet run --project DEA.UI
```

## 📋 Configuration

### Database Configuration

The application uses PostgreSQL with Entity Framework Core. Connection strings are configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dea_next;Username=postgres;Password=your_password"
  }
}
```

### Customer Configuration

Each customer is configured with:

- **Email Settings**: Exchange Online credentials and folder paths
- **File Delivery**: FTP/SFTP connection details or REST API endpoints
- **Processing Options**: Batch sizes, custom fields, and document encoding
- **Project Integration**: Template keys and project IDs for downstream systems

### Microsoft Graph Setup

1. Register an application in Azure AD
2. Configure API permissions for Mail.Read and Files.ReadWrite
3. Generate client credentials
4. Update configuration files with tenant and application details

## 🔧 Development

### Building

```bash
# Build entire solution
dotnet build DEA.Next.sln

# Build specific project
dotnet build DEA/DEA.Next.csproj
dotnet build DEA.UI/DEA.UI.csproj
```

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project DEA

# Update database
dotnet ef database update --project DEA

# Generate SQL script
dotnet ef migrations script --project DEA
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🏃‍♂️ How It Works

### Processing Flow

1. **Initialization**: Application checks internet connectivity and initializes database
2. **Customer Processing**: Iterates through active customer configurations
3. **Email Processing**: 
   - Connects to Exchange Online via Microsoft Graph
   - Downloads attachments from specified folders
   - Processes files according to customer settings
4. **File Delivery**:
   - **FTP/SFTP**: Uploads files to remote servers
   - **REST API**: Submits files and metadata to web services
5. **Cleanup**: Moves processed emails and cleans temporary files

### Email Folder Structure

The application processes emails from configurable folder hierarchies:
- Main folder (e.g., "Inbox")
- Subfolder 1 (e.g., "ProcessedEmails") 
- Subfolder 2 (e.g., "AttachmentsReady")

### File Processing

- Attachments are downloaded and validated
- PDF generation for text content when required
- Files are batched according to customer settings
- Metadata is extracted and formatted for downstream systems

## 📁 Project Structure

```
DEA.Next/
├── DEA/                          # Core application
│   ├── Data/                     # Entity Framework context and repositories
│   ├── Entities/                 # Database entities
│   ├── Graph/                    # Microsoft Graph integration
│   ├── FTP/                      # File transfer protocols
│   ├── FileOperations/           # File processing and web service integration
│   ├── HelperClasses/            # Utility classes
│   └── Extensions/               # Dependency injection and configuration
├── DEA.UI/                       # Windows Forms application
│   ├── Forms/                    # UI forms
│   └── HelperClasses/            # UI utility classes
├── DEACleaner/                   # Log maintenance utility
├── DEAMailer/                    # Email notification service
└── SQLScript/                    # Database scripts
```

## 🛠️ Technology Stack

- **.NET 8.0**: Core framework
- **Entity Framework Core 9.0**: ORM with PostgreSQL provider
- **Microsoft Graph SDK**: Exchange Online integration
- **FluentFTP**: FTP/FTPS operations
- **SSH.NET**: SFTP operations
- **RestSharp**: REST API client
- **Serilog**: Structured logging
- **iText**: PDF generation
- **Windows Forms**: Desktop UI

## 🔒 Security

- **OAuth2 Authentication**: Secure Graph API access
- **Connection String Security**: User secrets for development
- **File Validation**: Attachment scanning and validation
- **Secure Transfer**: FTPS and SFTP support
- **Audit Logging**: Comprehensive activity logging

## 📊 Monitoring

- **Structured Logging**: JSON-formatted logs with Serilog
- **Process Status Tracking**: Detailed success/failure reporting  
- **Error Handling**: Comprehensive exception management
- **Performance Metrics**: Processing time and throughput tracking

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:

- Check the application logs in the `Logs/` directory
- Review configuration files for correct settings
- Ensure Microsoft Graph permissions are properly configured
- Verify database connectivity and migrations

## 🏢 Company

Developed by **Digital Capture AS**  
Website: [https://digitalcapture.no](https://digitalcapture.no)