# TT-SS

# Disaster Prediction and Alert System API

## Scenario

You are tasked with building a Disaster Prediction and Alert System API for a government agency. This API will predict potential disaster risks (such as floods, earthquakes, and wildfires) for specified regions and send alerts to affected communities. The system integrates with external APIs to gather real-time environmental data and uses a simple scoring algorithm to assess risk levels.

## Requirements

- Build the API: Implement each endpoint with the specified functionality.
- External API Integration: Set up integration with external environmental data sources for real-time data.
- Caching with Redis: Use Redis to cache environmental data and risk calculations to minimize redundant external API calls.
- Azure Deployment: Deploy the solution on Azure and provide a live demo.
- Error Handling: Manage scenarios like:
  - Failed external API calls.
  - Missing data from external sources.
  - Regions with no available data.
- Messaging API Integration: Implement alert-sending functionality via a messaging API to notify people in high-risk regions.
- Logging: Track API usage and alerts for auditing purposes.

## Stacks

**Backend:** .Net Core

**Database:** Postgres, Redis

**Server:** Azure

**Logs**: Serilog integrate with Azure

**Others:** Notion, Docker, Github

## External APIs

- Weather: https://openweathermap.org/
- Messaging API: https://sendgrid.com/

## Submission

Start Date: 15/10/24

End Date: NA

Github Repository: https://github.com/Tanachuns/dpa-api

Presentation Video: NA

## Todos

- [x] Init project
- [x] Setup CI/CD
- [x] Setup Database
- [ ] Setup Logs Integrated with Azure
- [ ] Setup redis
- [ ] [POST] /api/regions
- [ ] [POST] /api/alert-settings
- [ ] [GET] /api/disaster-risks
- [ ] [POST] /api/alerts/send
- [ ] [GET] /api/alerts

## Knowledge Reference

- [Deploy Node.js ด้วย Azure App Service Plan, Azure Container Registry และ GitHub Action | by Ponggun | T. T. Software Solution | Medium](https://medium.com/t-t-software-solution/deploy-node-js-%E0%B8%94%E0%B9%89%E0%B8%A7%E0%B8%A2-azure-app-service-plan-azure-container-registry-%E0%B9%81%E0%B8%A5%E0%B8%B0-github-action-460998dd805f)
- [A Comprehensive Guide to Configuring Logging with Serilog and Azure App Insights in .NET | by Shazni Shiraz | Ascentic Technology | Medium](https://medium.com/ascentic-technology/a-comprehensive-guide-to-configuring-logging-with-serilog-and-azure-app-insights-in-net-f6e4bda69e76)
- [[C#] .netCore + redis caching | by Macus.y | Medium](https://rugby4.medium.com/c-netcore-redis-caching-e3c1c5c95957)
- [Tutorial: Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux&pivots=dotnet-8-0)
- [Cache Strategies | Medium](https://medium.com/@mmoshikoo/cache-strategies-996e91c80303)
