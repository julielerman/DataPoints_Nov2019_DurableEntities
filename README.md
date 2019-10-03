# DataPoints_Nov2019_DurableEntities

Taking the working branch and switching to storage emulator (windows app).

**Solution changes:**  
1. added emulator package to csproj
2. modified host.json to know i'm using local emulator
3. modified local settings to use local emulator. You won't see local settings here, but the line is 
 ` "AzureWebJobsStorage": "UseDevelopmentStorage=true" 
