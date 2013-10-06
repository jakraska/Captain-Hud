function CaptainHud::onWake(%this)
{
    CaptainHud::RefreshFilters();
    CaptainHud::RefreshPositions();
    // Now refresh our displays
   CaptainHud::PopulateRoster();
   CaptainHud::PopulateLineup();
   CaptainHud::RefreshPlayerInfo();
}
function CaptainHud::RefreshFilters(%this)
{
   %sel = filter.getselected();
   filter.clear();
   filter.add("None", 0);
   filter.add("Offense", 1);
   filter.add("Defense", 2);
   filter.add("Unassigned", 3);
   if(%sel != -1)
      filter.setselected(%sel);
   else
      filter.setselected(0);
   

}
function CaptainHud::Announce(%this)
{
   CreateAssignmentArray();
   CreateAssignmentMsg();
   AnnounceAssignments();
 

}
function CaptainHud::RefreshPositions(%this)
{
   Assignment.Clear();
   Assignment.add("Primary", $CHPOS::Count);
   Assignment.add("Secondary", $CHPOS::Count + 1);
   Assignment.SetSelected($CHPOS::Count);
    for(%i = 0; %i < $CHPOS::Count; %i++)
   {
      Assignment.add($CHPOS::Pos[%i], %i);
   }
   Assignment.Sort(true, 2);


}
function CHIsTeamate(%id)
{
   error("Entering CHIsTeamate() with %ID =" SPC %id);
   //Gotta do it this way because tags will screw up the search :(
   %team = $CHCurTeam;

   %SearchName = strlwr($CHMasterPlayer[%id].name); //convert to lower for comparison
   for(%i = 0; %i < $CHTeamCount[%team]; %i++)
   {
      %CompName = strlwr($CHTeam[%team, %i]);
      if(strstr(%compName, %SearchName) != -1)
      {
         error("--------Returning True");
         error("--------" SPC %Compname SPC "Found in" SPC %searchName);
         return true;     //they are on my team
         
      }
   
   }
         error("--------Returning FALSE");
   //they arn't on the team
   return false;
}
function CHPassFilter(%id)
{
   %filter = filter.getvalue();
   if(%filter $= "None")
      return true;
   else if( $CHMasterPlayer[%id].squad $= %filter)
      return true;
   else
      return false;
}
function CaptainHud::RefreshPlayerInfo(%this)
{
   %sel = Roster_list.getselectedID();
   if(%sel == -1)
   {
      info_name.setvalue("");
      info_squad.setvalue("");
      info_primary.setvalue("");
      info_secondary.setvalue("");
   }
   else
   {
      info_name.setvalue($CHMasterPlayer[%sel].name);
      info_squad.setvalue($CHMasterPlayer[%sel].squad);
      info_primary.setvalue($CHMasterPlayer[%sel].primary);
      info_secondary.setvalue($CHMasterPlayer[%sel].secondary);
   
   }
   //and reset the job to primary
   assignment.setselected($CHPOS::Count);
}
function CaptainHud::PopulateRoster(%this)
{
   Roster_list.clear();
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      if(!CHPassFilter(%i))
         continue;
      if($CHMasterPlayer[%i].selected)
         continue;
      if(Tgl_TOnly.GetValue())  //putting these on separate lines b/c i don't know
         if(!CHIsTeamate(%i))  //the order of prescidence in this language
            continue;
      if(Tgl_Primary.GetValue()) //Primary appears in grey.... use 4 for white tags
         %text = "\c2" @ $CHMasterPlayer[%i].Primary @ ":" SPC "\c0" @ $CHMasterPlayer[%i].Name;
      else
         %text = $CHMasterPlayer[%i].Name;
      Roster_list.addrow(%i, %text);
   
   }
   Roster_list.sort(0);
   //whenever i populate the roster, i should reset the player info
   CaptainHud::RefreshPlayerInfo();

}
function CaptainHud::PopulateLineup(%this)
{
   Lineup_list.clear();
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      if(!$CHMasterPlayer[%i].Selected)
         continue;
      %text = "\c2" @ $CHMasterPlayer[%i].Assignment @ ":" SPC "\c0" @ $CHMasterPlayer[%i].Name;
      Lineup_list.addrow(%i, %text);
   }
   Lineup_list.sort(0);
}
function CaptainHud::AddToLineup(%this)
{
   %Psel = Roster_list.getselectedID();
   if(%Psel == -1)
      return;

   $CHMasterPlayer[%Psel].Selected = true;
   %Asel = Assignment.getselected();
   if(%Asel == $CHPOS::Count)
      %ass =  $CHMasterPlayer[%Psel].Primary;
   else if(%Asel == $CHPOS::Count + 1)
      %ass =  $CHMasterPlayer[%Psel].Secondary;
   else
      %ass = Assignment.getvalue();
   $CHMasterPlayer[%Psel].Assignment = %ass;

// Now refresh our displays
   CaptainHud::PopulateRoster();
   CaptainHud::PopulateLineup();

}
function CaptainHud::ClearLineup(%this)
{
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      $CHMasterPlayer[%i].Selected = false;
   
   }
   // Now refresh our displays
   CaptainHud::PopulateRoster();
   CaptainHud::PopulateLineup();
}
function CaptainHud::RemoveFromLineup(%this)
{
   %sel = Lineup_list.getselectedid();
   if(%sel == -1)
      return;

   $CHMasterPlayer[%sel].Selected = false;
   // Now refresh our displays
   CaptainHud::PopulateRoster();
   CaptainHud::PopulateLineup();
}
