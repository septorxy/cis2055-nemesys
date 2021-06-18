# cis2055-nemesys
Welcome to our Nemesys Project!

Here are instructions on how to run this code :)

All the database schema is stored on our Azure SQL Server therefore all you need to do is set the connection strings and you are good to go.

Below please find all the necessary connections strings that need to be added. Kindly go on your command prompt and enter these commands:


dotnet user-secrets set SendGridUser "Nemesys Client"
dotnet user-secrets setSendGridKey "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"
dotnet user-secrets set ConnectionStrings:DefaultConnection "Data Source=tcp:memesys-server.database.windows.net,1433;Initial Catalog=NEMESYSDB;User Id=septorand@memesys-server;Password=GattCaruana123!"


For your convenience our web app is also running at https://universitynemesys.azurewebsites.net/ where all the resources can be accessed.

To access all of our feautures you can create an Account with the username "Admin" and a password of your choice and you will have access to all the feautures.

For a User and Investigator you will need to create a new account with a unique username and email and password of your choice.

This has been done this way as you would need email confirmation and emails are sent based on the email address provided so you wouldn't be able to test it otherwise.

