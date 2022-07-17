# FocusTimeAccumulator
A program written in c# to track your total time you've been focused on your various windows apps.
By default each individual page title will be saved individually. This is to help track specific file/project times.

### Apps.json and the focus change timer
When built the the program will generate an apps.json file in your current active directory.
this file will update at a minimum of 1 second by default which is configurable in the code.
it saved tracked time only if you change focus to some thing else.

### Shared mode
inside the program apps.json will be a bool for toggling shared mode on a given application.
setting it to true will prevent it from generating any new applications based on their title.

