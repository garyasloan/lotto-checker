# lotto-checker #

### What is this repository for? ###

This repository contains code for the lotto-checker.com website that allows you to enter and save your lotto picks and displays on which draws your picks won cash prizes.  This site was primarily built to allow prospective employers to view my technical work. 


### Who do I talk to? ###

gary@techie-mail.com

If you want to clone this repo and run this project locally then after you clone, create the file appsettings.Development.json in the Api folder with this content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Default": "Server=YourDBServer;Database=LottoChecker;Trusted_Connection=True;TrustServerCertificate=True;User=YourSqlServerUser;Password=YourSqlServerPassword"
  }
}
```
