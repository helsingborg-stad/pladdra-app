using System;
using System.Linq;
using System.Net;
using System.Text;
using Abilities;
using Abilities.ARRoomAbility;
using Newtonsoft.Json;
using Screens;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class PladdraConfiguration
    {
        
    }
    
    
    public class WelcomeScreen : Screen
    {
         private void Start()
         {
             var uri = CreateAbilityUri("ar-dialogue-room", new 
             {
                 endpoint =
                     "https://modul-test.helsingborg.io/helsingborgsrummet/wp-json/wp/v2/ar-dialogue-room/16?acf_format=standard",
                 headers = new 
                 {
                     Authorization = "aGVsc2luZ2JvcmdzcnVtbWV0Ok96enMgSDBZVyBXOWtqIFMxd1cgcU12VCBLN2hZ"
                 }
             });



             var ability = new CreateAbilityFromUri().TryCreateAbility(uri,
                 new WpArDialogueRoomAbilityFactory()
                 // new LocalArDialogRoomAbility()
             );
             
             Debug.Log(uri); 
             Debug.Log(ability); 
             GetComponentInParent<ScreenManager>().SetActiveScreen<LoadProjectsScreen>(
                 beforeActivate: screen => screen.Ability = ability);
         }

         private Uri CreateAbilityUri(string ability, object abilityConfiguration)
         {
             return new UriBuilder()
             {
                 Scheme = "pladdra",
                 Host = ability,
                 Path = Convert.ToBase64String(
                     Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(abilityConfiguration)))
             }.Uri;
         }
   }
}