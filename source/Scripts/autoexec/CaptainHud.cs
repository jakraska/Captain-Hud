


$CHDISPLAYED = false;
$CHVisible = false;


function ToggleCH(%val)
{
   if(%val)
   {
      if($CHVisible)
         HideCH();
      else
         ShowCH();
      
      $CHVisible = !$CHVisible;
   }
}
function ShowCH()
{
   if(!$CHDISPLAYED)
   {
      //Load all associated GUI files
      exec("gui/CHEditMPL.gui");
      exec("gui/CHEditPos.gui");
      exec("gui/CaptainHud.gui");
      exec("gui/CHEditAnnounce.gui");
      $CHDISPLAYED = true;
      CHLoadPrefs();
   }
   
   canvas.Pushdialog(CaptainHud);

}
function CHLoadPrefs()
{
   exec("prefs/CaptainHud/Positions.cs");
   if(!exec("prefs/CaptainHud/CHPrefs.cs"))
   {
      //if there is no prefs file, make one
      $CHPrefs::MaxMessageLen = 120;
      $CHPrefs::SPAM_MESSAGE_THRESHOLD = 4;
      $CHPrefs::SPAM_PROTECTION_PERIOD = 10000;
      CHSavePrefs();
      return;
   }
   else
      LoadMPL($CHPrefs::LastMPL);

}
//need to find a sooner time to clear the info... before i connect
//addMessageCallback( 'MsgLoadInfo', ClearCHTeams );
//addMessageCallback( 'MsgMissionDropInfo', ClearCHTeams );
addMessageCallback( 'MsgGameOver', ClearCHTeams );
addMessageCallback('MsgClientJoinTeam', HChandleClientJoinTeam);
addMessageCallback('MsgClientDrop', HChandleClientDrop);


function ClearCHTeams()
{
   error(">>>>>>>>>>>>>>>>>>>>CLEARING TEAMS<<<<<<<<<<<<<<<<<");
   $CHCurTeam = 0;
   for(%i = 0; %i < 3; %i++)
      $CHTeamCount[%i] = 0;
}
function DumpCHTeams()
{
   error("-----------------------------------------");
   error("              Dump Start                 ");
   error("-----------------------------------------");
   error("Your Team:");
   error("   " @ $CHCurTeam);
   
   for(%i = 0; %i < 3; %i++)
   {
      error("Team" SPC %i);
      for(%j = 0; %j < $CHTeamCount[%i]; %j++)
      {
         error("   " @ $CHTeam[%i, %j]);
      }
   
   }
   error("-----------------------------------------");
   error("                Dump End                 ");
   error("-----------------------------------------");
}
function HChandleClientDrop( %msgType, %msgString, %clientName, %clientId )
{
   error("Using my ClientDrop Callback");
   for(%i = 0; %i < 3; %i++)
   {
         for(%j = 0; %j < $CHTeamCount[%i]; %j++)
         {
            if(detag(%clientName) $= $CHTeam[%i, %j])
            {
               %found = true;
               break;
            }
         }


      if(%found)
         break;

   }
   if(%found)
   {
      for(%k = %j; %k < $CHTeamCount[%i]; %k++)
      {
         $CHTeam[%i, %k] = $CHTeam[%i, %k+1];
      }
      $CHTeamCount[%i]--;
      CHJoinRefresh();
   }
}
function HChandleClientJoinTeam(%msgType, %msgString, %clientName, %teamName, %clientId, %teamId )
{
   // this is a crappily programmed function, but i don't care for the moment
   if($CHTeamCount[%teamId] $= "")
      $CHTeamCount[%teamId] = 0;
   error("Using my JoinTeam Callback");

   if( %clientId == getManagerId())
   {
      $CHCurTeam = %teamId;
      CHJoinRefresh();
      return;
   }
   $CHTeam[%teamId, $CHTeamCount[%teamId]] = detag(%clientName);
   $CHTeamCount[%teamId]++;
   %found = false;
   for(%i = 0; %i < 3; %i++)
   {
      if(%i != %teamId)
      {
         for(%j = 0; %j < $CHTeamCount[%i]; %j++)
         {
            if(detag(%clientName) $= $CHTeam[%i, %j])
            {
               %found = true;
               break;
            }
         }
      
      }
      if(%found)
         break;
   
   }
   if(%found)
   {
      for(%k = %j; %k < $CHTeamCount[%i]; %k++)
      {
         $CHTeam[%i, %k] = $CHTeam[%i, %k+1];
      }
      $CHTeamCount[%i]--;
      CHJoinRefresh();
   }

}
function CHJoinRefresh()
{
    if(!CaptainHud.isVisible())
       return;
    if(!Tgl_TOnly.getvalue())
       return;

    %sel = Roster_list.getselectedID();
    CaptainHud::PopulateRoster();
    Roster_list.setselectedbyID(%sel);

}
package CaptainHud {
//function handleClientJoinTeam(%msgType, %msgString, %clientName, %teamName, %clientId, %teamId )
//{
//  Parent::handleClientJoinTeam(%msgType, %msgString, %clientName, %teamName, %clientId, %teamId );
///  error("Using My Join Team Callback");
//  error("------>ClientName:" SPC %clientName);
//  error("------>DetTag: " SPC deTag(%clientName));
//  if( %clientId == getManagerId())
//  {
//     $CHCurTeam = %teamId;
//     return;
//  }
function OptionsDlg::onWake(%this)
{
  $RemapName[$RemapCount] = "Captain Hud";
  $RemapCmd[$RemapCount] = "ToggleCH";
  $RemapCount++;

  parent::onWake(%this);
}
function connect( %address, $JoinGamePassword, %playerName, %playerRaceGender, %playerSkin, %playerVoice, %playerVoicePitch )
{
       error(">>>>>>>>>>>>>USING MY Connect!");
       ClearCHTeams();
       parent::connect( %address, $JoinGamePassword, %playerName, %playerRaceGender, %playerSkin, %playerVoice, %playerVoicePitch );

}
//}
//function handleClientDrop( %msgType, %msgString, %clientName, %clientId )
//{

//}
};
deactivatepackage(CaptainHud);
activatepackage(CaptainHud);
function HideCH()
{
   Canvas.Popdialog(CaptainHud);
}
function RebuildCH()
{
   exec("scripts/autoexec/CaptainHud.cs");
   exec("scripts/autoexec/CHEditMPLGUI.cs");
   exec("scripts/autoexec/CHEditPosGUI.cs");
   exec("scripts/autoexec/CaptainHudGUI.cs");
   exec("scripts/autoexec/CHEditAnnounceGUI.cs");
   exec("gui/CaptainHud.gui");
   exec("gui/CHEditMPL.gui");
   exec("gui/CHEditPos.gui");
   exec("gui/CHEditAnnounce.gui");
}
function RefreshMPL()
{
   for(%i = 0; %i < $CH::MasterPlayerCount; %i++)
   {
      $CHMasterPlayer[%i] = new ScriptObject(){
                               Class      = CHMP;
                               Name       = $CH::Name[%i];
                               Primary    = $CH::Primary[%i];
                               Secondary  = $CH::Secondary[%i];
                               Squad      = $CH::Squad[%i];
                               Selected   = false;
                               Assignment = "";
                               };
   
   }
   $CHMasterPlayerCount = $CH::MasterPlayerCount;
}
function RequestLoadMPL()
{
 ShellGetLoadFilename( "Load Players", "prefs/CaptainHud/*.MPL", "", "LoadMPL" );
}
function LoadMPL(%filename)
{
   %file = "prefs/CaptainHud/" @ %filename @ ".MPL";
   exec(%file);
   $CHActiveMPL = %filename;
   $CHPrefs::LastMPL = %filename;
   CHSavePrefs();
   RefreshMPL();
   CHEditMPL::RefreshMPL();
}
function CHSavePrefs()
{
    %file = "prefs/CaptainHud/CHPrefs.cs";
    export( "$CHPrefs::*", %file, false );
}
function NewMPL()
{
  // if($CHChanged)
  //    MessageBoxOKCancel( "Warning", "Warning", %callback, %cancelCallback )
  $CHActiveMPL = "";
  $CHMasterPlayerCount = 0;
  CHEditMPL::RefreshMPL();
}
function UpdatePlayer(%name, %primary, %secondary, %squad)
{
   %found = false;
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      if(%name $= $CHMasterPlayer[%i].Name)
      {
         %found = true;
         break;
      }
   }
   if(!%found)
   {
      MessageBoxOK( "ERROR", %name SPC "does not exist to be updated.");
      return false;
   }

   $CHMasterPlayer[%i].Primary = %primary;
   $CHMasterPlayer[%i].Secondary = %Secondary;
   $CHMasterPlayer[%i].Squad = %Squad;


   return true;

}
function AddPlayerToMPL(%name, %primary, %secondary, %squad)
{
   error("Entering AddPlayerToMPL(" SPC %name SPC %Primary SPC %Secondary SPC %Squad SPC ")");
   //make sure the player doesn't already exist
   %found = false;
   if($CHMasterPlayerCount $= "")
      $CHMasterPlayerCount = 0;
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      if(%name $= $CHMasterPlayer[%i].Name)
      {
         %found = true;
         break;
      }
   }
   if(%found)
   {
      MessageBoxOK( "ERROR", %name SPC "is already part of the player list.");
      return false;
   }
   if($CHMasterPlayerCount $= "")
      $CHMasterPlayerCount = 0;
   error("MP Count before creation is" SPC $CHMasterPlayerCount);
   $CHMasterPlayer[$CHMasterPlayerCount] = new ScriptObject(){
                                              Class      = CHMP;
                                              Name       = %name;
                                              Primary    = %primary;
                                              Secondary  = %secondary;
                                              Squad      = %squad;
                                              Selected   = false;
                                              Assignment = "";
                                              };
   $CHMasterPlayerCount++;
   error("MP Count After creation is" SPC $CHMasterPlayerCount);
   
   return true;
}
function RemovePlayerFromMPL(%name)
{
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      if(%name $= $CHMasterPlayer[%i].Name)
      {
         %found = true;
         break;
      }
   }
   if(!%found)
   {
      //This should never happen... but i'll put it here just incase
      MessageBoxOK( "ERROR", %name SPC "was not found.");
      return false;
   }
   //reorganize the array
   for(%j = %i; %j < $CHMasterPlayerCount; %j++)
   {
      $CHMasterPlayer[%j] = $CHMasterPlayer[%j + 1];
   }
   $CHMasterPlayerCount--;
}
function RequestSaveMPL()
{
    ShellGetSaveFilename( "Save Players", "prefs/CaptainHud/*.MPL", "", "SaveMPL", $CHActiveMPL );
}
function SaveMPL(%filename)
{
   //verify it's a valid file name
   if ( strcspn( %filename, "\\/?*\"\'<>|" ) < strlen( %filename ) )
   {
      MessageBoxOK( "SAVE FAILED", "Filenames may not contain any of the following characters:" NL "\\ / ? * < > \" \' |",
            "RequestSaveMPL();" );
      return;
   }

   //first lets clear out the extra values of $CH::* so they don't get written
   // when i don't need them
   if($CH::MasterPlayerCount $= "")
      $CH::MasterPlayerCount = 0;

   for(%i = 0; %i < $CH::MasterPlayerCount; %i++)
   {
      $CH::Name[%i] = "";
      $CH::Primary[%i] = "";
      $CH::Secondary[%i] = "";
      $CH::Squad[%i] = "";
   }
   $CH::MasterPlayerCount = 0;

   //ok, we are clear... prepare the data to be written....
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      $CH::Name[%i] = $CHMasterPlayer[%i].Name;
      $CH::Primary[%i] = $CHMasterPlayer[%i].Primary;
      $CH::Secondary[%i] = $CHMasterPlayer[%i].Secondary;
      $CH::Squad[%i] = $CHMasterPlayer[%i].Squad;
   }
   $CH::MasterPlayerCount = $CHMasterPlayerCount;
   %file = "prefs/CaptainHud/" @ %filename @ ".MPL";
   $CHActiveMPL = %filename;

   export( "$CH::*", %file, false );

   $CHPrefs::LastMPL = %filename;
   CHSavePrefs();
}
function CreateAssignmentArray()
{
    //error("CREATING ASSIGNMENT ARRAY");
    //first clear out old array or initialize it
    for(%i = 0; %i < $CHPOS::Count; %i++)
    {
       $AssignmentCount[$CHPOS::Pos[%i]] = 0;
    
    }
    //now create our new array
    
    for(%i = 0; %i < $CHMasterPlayerCount; %i++)
    {
       if(!$CHMasterPlayer[%i].Selected)
          continue;

       %ass = $CHMasterPlayer[%i].Assignment;
       $Assignment[%ass, $AssignmentCount[%ass]] = $CHMasterPlayer[%i].Name;
       $AssignmentCount[%ass]++;
       //error("Adding" SPC $CHMasterPlayer[%i].Name SPC "To Position" SPC %ass);
    }
       // error("ASSIGNMENT ARRAY Complete");
}
function CreateAssignmentMsg()
{
  error("Creating Assignment Messages");
  $AssignmentMsgCount = 0;

  //Take first pass at array
  for(%i = 0; %i < $CHPOS::Count; %i++)
  {
     %pos = $CHPOS::Pos[%i];
     if($AssignmentCount[%pos] <= 0)
        continue;
     %msg = "\c5" @ %pos @ ":\c2";
     for(%j = 0; %j < $AssignmentCount[%pos]; %j++)
     {
        if( strlen(%msg @ " " @ $Assignment[%pos, %j]) >= $CHPrefs::MaxMessageLen)
        {
           error("Adding message because of length:" SPC %msg);
           $AssignmentMsg[$AssignmentMsgCount] = %msg;
           $AssignmentMsgCount++;
           %msg = %pos @ ":";
        }
        error("------>CurMsg:" SPC %msg);
        %msg = %msg SPC $Assignment[%pos, %j] @ ",";
        error("------>Adding to CurMsg( %ass =" SPC %pos SPC "%j =" SPC %j SPC ":" SPC $Assignment[%ass, %j]);
     }
     //get rid of that extra comma on the end...
     %msg = getsubstr(%msg, 0, strlen(%msg) - 1);
     $AssignmentMsg[$AssignmentMsgCount] = %msg;
     $AssignmentMsgCount++;
     error("Adding message:" SPC %msg);
  }

  error("Assignment Messages Complete");
}
function AnnounceAssignments()
{

   //calculate the delay between messages based on spam prevention
   if($CHPrefs::SPAM_MESSAGE_THRESHOLD <= 1)
      $CHPrefs::SPAM_MESSAGE_THRESHOLD = 2; //avoid Divide by zero
   %delay = $CHPrefs::SPAM_PROTECTION_PERIOD / ($CHPrefs::SPAM_MESSAGE_THRESHOLD - 1) + 200;
   %delay = mceil(%delay);
   %time = 0;
   for(%i = 0; %i < $AssignmentMsgCount; %i++)
   {
      schedule(%time,0,"AnnounceAssignment", $AssignmentMsg[%i]);
      %time += %delay;
   
   }


}
function AnnounceAssignment(%msg)
{
 commandToServer('teamMessageSent', %msg);

}
