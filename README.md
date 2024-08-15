# ImageDescriptionApp

**Assignment**

**You are required to use .NET Core to develop this application.**
Due Date: **Week of the 2 – 6 September 2024**

Assignment Two: Utilising Microsoft Cognitive Services
Microsoft Cognitive Services is a collection of machine learning 
algorithms developed by Microsoft to solve problems using Artificial 
Intelligence (AI).

Cognitive Services democratize AI by packaging it into discrete 
components, making it easy for developers to integrate these 
capabilities into their applications. Developers can access these 
algorithms through standard REST calls over the Internet to the 
Cognitive Services APIs.

In this assignment, you will use the Computer Vision AI API to develop 
an application that allows the user to upload a photo and then describe 
what is in that photo in text on the screen

Certainly! Here's a revised README for GitHub that focuses on the actual features present in your application:

# Image Analysis Web Application

## Overview
This web application allows users to upload images, analyze them using Azure Cognitive Services, and view past uploads. It's built using ASP.NET Core MVC and integrates with Azure Blob Storage and Azure Computer Vision API.

## Features

### Image Upload and Analysis
- User-friendly interface with a browse window to select images
- Drag-and-drop functionality for easy image uploading
- Real-time image preview before upload
- Seamless integration with Azure Blob Storage for secure image storage
- Image analysis using Azure Computer Vision API
- Generation of image descriptions and tags

### Results Display
- Clear presentation of uploaded image
- Display of AI-generated image description
- Showing of relevant tags associated with the image

### Past Uploads Management
- Side panel displaying thumbnail history of past uploads
- Ability to re-analyze past uploads with a single click
- Tag-based filtering of past uploads for easy navigation

### Performance Optimization
- Implementation of caching mechanism to improve performance
- Caching of analysis results to reduce API calls and speed up repeat analyses

### User Experience
- Responsive design for various device sizes
- Intuitive user flow from upload to analysis results
- Real-time feedback during upload and analysis processes

### Code Quality
- Utilization of good Object-Oriented Programming (OOP) practices
- Clear separation of concerns in the architecture
- Proper use of dependency injection

## Technical Stack
- ASP.NET Core MVC
- Azure Blob Storage
- Azure Computer Vision API
- JavaScript for client-side interactivity
- Tailwind CSS for styling

Certainly! Here's a detailed Setup and Installation guide for your Image Analysis Web Application:

# Setup and Installation

## Prerequisites
- .NET 6.0 SDK or later
- An Azure account with active subscription
- Visual Studio 2022 or Visual Studio Code

## Azure Services Setup

1. Azure Blob Storage:
   - Create a new Storage Account in Azure Portal
   - In the new storage account, create a container for storing images
   - Note down the Connection String and Container Name

2. Azure Cognitive Services (Computer Vision):
   - Create a Computer Vision resource in Azure Portal
   - Note down the Endpoint and API Key

## Local Development Setup

1. Clone the repository:
   ```
   git clone https://github.com/your-username/image-analysis-app.git
   cd image-analysis-app
   ```

2. Create a `appsettings.json` file in the project root (if not already present) and add the following configuration, replacing the placeholders with your Azure credentials:
   ```json
   {
     "AzureStorage": {
       "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION_STRING",
       "ContainerName": "YOUR_CONTAINER_NAME"
     },
     "AzureCognitiveServices": {
       "Endpoint": "YOUR_COMPUTER_VISION_ENDPOINT",
       "Key": "YOUR_COMPUTER_VISION_API_KEY"
     }
   }
   ```

3. Restore the NuGet packages:
   ```
   dotnet restore
   ```

4. Build the project:
   ```
   dotnet build
   ```

5. Run the application:
   ```
   dotnet run
   ```

## Running with Visual Studio

1. Open the solution file (.sln) in Visual Studio
2. Ensure the configuration file is set up as described in step 2 of Local Development Setup
3. Press F5 or click the "Run" button to start the application

## Running with Visual Studio Code

1. Open the project folder in Visual Studio Code
2. Ensure the configuration file is set up as described in step 2 of Local Development Setup
3. Open a terminal in VS Code
4. Run the following command:
   ```
   dotnet run
   ```

## Verifying the Installation

1. Once the application is running, open a web browser and navigate to `https://localhost:5001` (or the port specified in your console output)
2. You should see the home page with an option to upload images
3. Try uploading an image to ensure connections to Azure services are working correctly

## Troubleshooting

- If you encounter any issues with Azure services, double-check your connection strings and API keys in the `appsettings.json` file
- Ensure your Azure account has the necessary permissions to access Blob Storage and Cognitive Services
- Check your firewall settings if you're having trouble connecting to Azure services

## Additional Notes

- For production deployment, ensure to use secure methods to store your Azure credentials, such as Azure Key Vault
- Regular updates to the .NET SDK and Azure SDKs are recommended to ensure you have the latest features and security patches

This setup guide provides a comprehensive walkthrough for getting your Image Analysis Web Application up and running, covering both the Azure setup and local development configuration. It should help users of your GitHub repository to quickly set up and start using the application.

## Usage
1. Navigate to the home page
2. Upload an image by clicking the upload area or dragging and dropping
3. View the analysis results, including description and tags
4. Browse past uploads in the side panel
5. Use tag filtering to find specific types of images in your upload history




![image](https://github.com/user-attachments/assets/54593f60-fe92-4048-bf7c-2081421bbe1b)
