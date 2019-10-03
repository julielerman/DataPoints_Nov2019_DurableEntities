# DataPoints_Nov2019_DurableEntities

Taking the working branch and switching to storage emulator (windows app).

**Soution changes:**  
added emulator package to csproj
modified host.json to know i'm using local emulator
modified local settings to use local emulator. You won't see local settings here, but the line is 
  "AzureWebJobsStorage": "UseDevelopmentStorage=true"  
