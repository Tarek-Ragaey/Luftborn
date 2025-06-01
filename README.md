**Project Setup Instructions**

**Backend (Luftborn API)**

Open Luftborn.sln in Visual Studio.

Update the database **connection string in appsettings.json** to point to your desired server.

Open the Package Manager Console and run the following command  **Update-Database** to create the database and seed initial data:


This will create the database with two initial users:

Admin
Email: admin@admin.com
Password: P@ssw0rd123

Writer
Email: writer@writer.com
Password: P@ssw0rd123

Run the application — it will be available at:
http://localhost:5000 abd the APIs will be shown in swagger

**Frontend (Luftborn Client)**

Open a terminal and navigate to the frontend project directory:

cd luftborn-client
Start the frontend application:

npm run dev
The frontend will be available at:
http://localhost:5173

