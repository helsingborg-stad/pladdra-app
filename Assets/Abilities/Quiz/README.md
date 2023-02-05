# AR Sandbox 

For clarity, each manager is assigned to its own GameObject under parent GameObject Managers. 

# Quiz Ability

All quizzes are taken from wordpress. You can specify which url to use in the deeplink or in `QuizAppManager`.

There is currently no way to create a test quiz in Unity. Class `ProjectReference` has a field for a `TextAsset` that will be parsed as a json if it has no url. To assign this you need to create a local representation of a list of project references. 

To change stuff like materials, prefabs and interaction settings: 
1. Make a new settings file from `Assets` > `Create` > `Pladdra` > `Quiz Ability settings`.
2. Assign the settings file to the `QuizManager` in managers in the inspector. 
3. Change the contents in the settings file.

All of the quiz creation happen in quizmanager, each answer has an answercontroller that controls its behavior.

There is a `QuizUserManager` that tracks the user's progress in the quiz. This can be used as a base to create more complex user experiences, such as a leaderboard.

The Quiz Ability does not implement the `UXHandler` system. 