# HealthCheckerService
Basic dotnet core worker app that can be installed as a Windows service to check health of remote endpoint.

## Deployment
- Compile to self contained app using dotnet
- Depoloy with PowerShell or other tool that creatse service to start immediately and on startup

## References 
- [dotnet core app as win service](https://solrevdev.com/2020/01/31/event-viewer-logs-with-net-core-workers-as-windows-services.html)

## Logging avaible in Windows Event Viewer 
![Event Viwer](events.png)

## Logging from Console
![Drag Racing](loggerconsole.png)
