# WebTruyenHay - Online Story Reading Platform

WebTruyenHay is a comprehensive web application built with ASP.NET MVC for reading and managing online stories (comics/novels). The platform provides features for both readers and content creators to enjoy and publish stories.

## 🚀 Features

### For Readers
- **Story Browsing**: Browse stories by categories and popularity
- **Reading Experience**: Read stories with image-based chapters
- **Progress Tracking**: Automatic reading progress tracking
- **Favorites System**: Save favorite stories for quick access
- **Rating & Comments**: Rate stories and leave comments
- **Search Functionality**: Basic and advanced search options
- **Notifications**: Email notifications for new chapters of favorite stories
- **Theme Toggle**: Light/dark theme support

### For Authors/Administrators
- **Story Management**: Create, edit, and delete stories
- **Chapter Management**: Add chapters with image content
- **Content Upload**: Image upload functionality for story covers and chapter content
- **User Management**: User profile management

### Premium Features
- **Premium Subscription**: MoMo payment integration for premium access
- **Enhanced Features**: Premium users get additional benefits

## 🛠 Technology Stack

- **Framework**: ASP.NET MVC 5 (.NET Framework 4.7.2)
- **Database**: Entity Framework 6 with SQL Server
- **Frontend**: Bootstrap 3, jQuery, HTML5, CSS3
- **Payment**: MoMo payment gateway integration
- **Email**: Custom mail service for notifications
- **File Upload**: Image upload and management system

## 📦 Dependencies

Key NuGet packages used:
- Entity Framework 6.2.0
- ASP.NET MVC 5.2.7
- Bootstrap 3.4.1
- jQuery 3.4.1
- Newtonsoft.Json 12.0.2

## 🏗 Project Structure
WebTruyenHay/ ├── Controllers/           # MVC Controllers │   ├── HomeController.cs │   ├── truyenController.cs    # Main story controller │   └── UserController.cs      # User authentication ├── Models/               # Entity Framework models │   ├── Truyen.cs         # Story entity │   ├── Chuong.cs         # Chapter entity │   ├── NguoiDung.cs      # User entity │   └── [Other models] ├── Views/                # Razor views │   ├── truyen/           # Story-related views │   └── [Other views] ├── Content/              # Static content │   ├── images/           # Uploaded images │   └── css/              # Stylesheets ├── Scripts/              # JavaScript files └── App_Start/            # Configuration files


## 🎯 Core Entities

### Truyen (Story)
- **IDtruyen**: Unique identifier
- **TieuDe**: Story title
- **MoTa**: Description
- **TacGia**: Author
- **theloai**: Category/Genre
- **imagetruyen**: Cover image
- **viewtruyen**: View count
- **premium**: Premium status

### Chuong (Chapter)
- **IDchuong**: Chapter identifier
- **TieuDe**: Chapter title
- **NoiDung**: Chapter content
- **SoThuTu**: Chapter order
- **TruyenID**: Reference to story

### NguoiDung (User)
- **IDuser**: User identifier
- **HoTen**: Full name
- **Email**: Email address
- **MatKhau**: Password
- **VaiTro**: Role (premium status)

## 🔧 Setup Instructions

1. **Prerequisites**
   - Visual Studio 2019 or later
   - .NET Framework 4.7.2
   - SQL Server (LocalDB or full version)

2. **Database Setup**
   - Update connection string in web.config
   - Run Entity Framework migrations
   - Ensure database contains required tables

3. **Configuration**
   - Configure email settings for notifications
   - Set up MoMo payment credentials
   - Configure file upload directories

4. **Running the Application**
   - Open solution in Visual Studio
   - Build the project
   - Run using IIS Express or local IIS

## 🎮 Key Functionality

### Story Reading Flow
1. Browse stories on main page
2. View story details with chapters list
3. Read chapters with image content
4. Navigate between chapters
5. Track reading progress automatically

### User Management
- Registration and login system
- Profile management with image upload
- Password recovery via email
- Role-based access control

### Content Management
- Create and edit stories
- Upload cover images
- Add chapters with sequential ordering
- Upload chapter images
- Manage story categories

### Payment Integration
- MoMo payment gateway for premium subscriptions
- Transaction tracking
- Premium feature access control

## 🔒 Security Features

- Anti-forgery token validation
- Session-based authentication
- Role-based authorization
- Input validation and sanitization
- Secure file upload handling

## 📱 User Interface

- Responsive design with Bootstrap
- Image-based story reading experience
- AJAX-powered interactions
- Theme switching capability
- Clean and intuitive navigation

## 🚀 Deployment

The application is configured for deployment on IIS with:
- Session state management
- File upload handling
- Database connection management
- Email service configuration

## 📈 Performance Features

- Entity Framework optimization
- Image compression and optimization
- Caching for frequently accessed data
- Pagination for large datasets
- Asynchronous operations for better performance
