

************ Important ***********
Please go throught PROJECT SETUP.docx file ,attached in email and with project as well in project folder
*****************************************


*************Swagger URL************
https://localhost:7206/swagger/index.html


########## DATABASE SETUP #########

(1)Open your SQL Server (or preferred DBMS).
(2)Locate the SQL file included in the project under the Database folder (e.g., Database/AdFormScript.sql).
(3)Execute the SQL script to create the necessary tables (ExchangeRates, CurrencyConversionHistories, etc.) and seed initial data.
(4)Update your appsettings.json connection string with your database details


############ JWT Authentication #########

(1) Default credentials for testing:
Username: admin
Password: admin


(2) Use the login endpoint "/api/auth/login" to get the JWT token:
POST /api/auth/login
Body:
{
  "username": "admin",
  "password": "admin"
}

Copy the returned JWT token and past


##########################################



