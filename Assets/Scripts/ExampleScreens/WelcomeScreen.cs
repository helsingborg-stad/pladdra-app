using System;
using System.Text;
using Abilities;
using Abilities.ARRoomAbility.Local;
using Abilities.ARRoomAbility.WP;
using Newtonsoft.Json;
using Screens;
using UnityEngine;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class WelcomeScreen : Screen
    {
         private void Start()
         {
             var uri = AbilityUri.CreateAbilityUri("ar-dialogue-room", new 
             {
                 mode = "edit",
                 endpoint =
                     "https://modul-test.helsingborg.io/helsingborgsrummet/wp-json/wp/v2/ar-dialogue-room/16?acf_format=standard",
                 headers = new 
                 {
                     Authorization = "aGVsc2luZ2JvcmdzcnVtbWV0Ok96enMgSDBZVyBXOWtqIFMxd1cgcU12VCBLN2hZ"
                 }
             });

             var ability = AbilityUri.TryCreateAbility(uri,
                 new WpArDialogueRoomAbilityFactory(),
                 new LocalArDialogueRoomAbilityFactory(Application.temporaryCachePath)
             );
             
             Debug.Log(uri); 
             Debug.Log(ability); 
             GetComponentInParent<ScreenManager>().SetActiveScreen<LoadProjectsScreen>(
                 beforeActivate: screen => screen.Ability = ability);
         }
   }
}