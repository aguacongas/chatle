This project is an [Angular 2](https://angular.io) sample wich connect to the [ASP.Net Core](https://docs.asp.net) MVC server.  
The application is hosted in the Home page, other pages are MVC razor pages.  

If you want to launch it on your machine as a development environement, you need 1st to setup external authentication providers by setting app ids and secrets using the [SecretManager](https://docs.asp.net/en/latest/security/app-secrets.html).  
The project use Facebook, Twitter, Google and Microsoft as external authentication providers, You need to provide values for thoses keys :  

    {
      "Authentication:Facebook:AppId": "<your facebook app id>",
      "Authentication:Facebook:AppSecret": "<your facebook app secret>",
      "Authentication:Twitter:ConsumerKey": "<your twitter app id>",
      "Authentication:Twitter:ConsumerSecret": "<your twitter app secret>",
      "Authentication:MicrosoftAccount:ClientId": "<your microsoft app id>",
      "Authentication:MicrosoftAccount:ClientSecret": "<your microsoft app secret>",
      "Authentication:Google:ClientId": "<your google app id>",
      "Authentication:Google:ClientSecret": "<your google app secret>"
    }

In order to create your ids and secret:  
- Twitter: [https://apps.twitter.com/](https://apps.twitter.com/)
- Google: [https://console.developers.google.com](https://console.developers.google.com)
- Microsoft: [https://apps.dev.microsoft.com](https://apps.dev.microsoft.com)
- Facebook: [https://developers.facebook.com/apps/](https://developers.facebook.com/apps/)
    
