<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

<!-- PROJECT LOGO -->
<br />
<p align="center">
<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
      </ul>
      <ul>
        <li><a href="#setup">Setup & Running</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project
Project Owl is a proof of concept AI powered application that aims to demonstrate how the Expert AI Natural Language API can be used for auditing and gaining insight from customers by analyzing audio recorded calls at call centers. Project Owl employs the Sentiment Analysis and Taxonomy features of the NLP API to analyse and prioritize customer service complaints and also provides insight on the customer emotional state respectively. 

The inspiration for this project is to experiment how a call center issue management system can be automated for customer serving financial institutions such as banks and fin-tech companies. 

You can view the demo [here](TBD)

You can try the live application here [Live site](TBD)

### Built With
* [Azure Functions](https://azure.microsoft.com/services/functions/)
* [Azure Speech Service](https://azure.microsoft.com/services/cognitive-services/speech-services/)
* [Azure SignalR Service](https://azure.microsoft.com/services/signalr-service/)
* [Expert AI](https://www.expert.ai/)
* [Azure Storage](https://docs.microsoft.com/azure/storage/)



<!-- GETTING STARTED -->
## Getting Started

To run the application locally, please follow the steps below.

### Prerequisites

* [Visual Studio 2019](https://visualstudio.microsoft.com/)
* [Expert AI Developer account](https://developer.expert.ai/)
* [Azure Account](https://azure.microsoft.com/)


### Setup

1. Clone the repo 
2. Create a developer account at [Expert AI](https://developer.expert.ai/)
3. Create a free Azure Account at [Azure](https://azure.microsoft.com/) or use an existing one. 
4. Create an Speech Service resource in Azure portal.
5. Create a SignalR Service in Azure portal.
6. Create a SQL Database in Azure portal and run sql script(or to use local in memory database see below).
7. Create a Storage Account V2 in Azure portal(or use local Azure storage using Azure Storage Emulator) 
8. Update `appsettings.json`
  ```sh
  {
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true" //Replace with Storage account connection string or keep to use emulator,
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "AuthUrl": "Add expert ai authentication url",
        "AccountId": "Add authentication account id",
        "AccountPassword": "Add authentication account password",
        "NlpApiUrl": "Add expert ai NLP API url",
        "AzureSpeechServiceKey": "Add Azure Speech Service Key",
        "AzureSpeechServiceRegion": "Add Azure Speech Service Region",
        "DatabaseConnectionString": "Add SQL Connection string",
        "AzureSignalRConnectionString": "Add SignalR Service Connection String",
        "Env": "Local" //Local to use in-memory database or Prod to use SQL Db
    },
    "Host": {
        "LocalHttpPort": 7071,
        "CORS": "*"
    }
}

```

### Running from Visual Studio

1. From Visual Studio, press `F5`
 
<!-- USAGE EXAMPLES -->
## Usage

Please refer to the [demo](TBD)

