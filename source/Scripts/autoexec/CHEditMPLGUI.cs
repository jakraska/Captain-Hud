
function CHEditMPL::onWake(%this)
{
  error("CHEditMPL::onWake(%this)");
  //Display the current list
  Name.SetValue("");
  %this.RefreshMPL();
  %this.RefreshPositions();
  Squad.Clear();
  Squad.add("Unassigned", 0);
  Squad.add("Offense",1);
  Squad.add("Defense",2);
}
function CHEditMPL::Done(%this)
{

   Canvas.Popdialog(CHEditMPL);
   CaptainHud::onWake();
}
function CHEditMPL::RefreshMPL(%this)
{
   error("CHEditMPL::RefreshMPL(%this)");
   EditMPL_List.clear();
   for(%i = 0; %i < $CHMasterPlayerCount; %i++)
   {
      EditMPL_List.addRow( %i, $CHMasterPlayer[%i].Name);
   }
   EditMPL_List.Sort(0);
}
function CHEditMPL::RefreshPositions(%this)
{
   error("CHEditMPL::RefreshPositions(%this)");
   Secondary.Clear();
   Primary.Clear();
   for(%i = 0; %i < $CHPOS::Count; %i++)
   {
      Secondary.add($CHPOS::Pos[%i], %i);
      Primary.add($CHPOS::Pos[%i], %i);
   }
   Secondary.Sort(true);
   Primary.Sort(true);
}
function CHEditMPL::AddPlayer(%this)
{
   error("CHEditMPL::AddPlayer(%this)");
   %name = Name.getvalue();
   %squad = Squad.GetValue();
   %primary = Primary.GetValue();
   %secondary = Secondary.GetValue();
   
   
   if(%name $= "" || %squad $= "" || %primary $="" || %secondary $="")
   {
      MessageBoxOK( "ERROR", "Not all fields have been entered");
      return;
   }
   if(!AddPlayerToMPL(%name, %primary, %secondary, %squad))
      return;
   CHEditMPL::RefreshMPL();
   
   Name.SetValue("");
   Squad.SetSelected(-1);
   Primary.SetSelected(-1);
   Secondary.SetSelected(-1);
}
function CHEditMPL::RemoveSelectedPlayer(%this)
{
   error("CHEditMPL::RemoveSelectedPlayer(%this)");
   if(EditMPL_List.getselectedID() == -1)
      return;
      
   RemovePlayerFromMPL(EditMPL_List.getValue());
   CHEditMPL::RefreshMPL();
   

}
function CHEditMPL::DisplayPlayer(%this)
{
   error("CHEditMPL::DisplayPlayer(%this)");
   %id = EditMPL_List.getselectedID();
   error("--->ID =" SPC %id);
   Name.SetValue($CHMasterPlayer[%id].Name);
   error("--->Name =" SPC $CHMasterPlayer[%id].Name);
   Squad.SetSelected(Squad.findtext($CHMasterPlayer[%id].Squad));
   error("--->SquadID =" SPC Squad.findtext($CHMasterPlayer[%id].Squad));
   Primary.SetSelected(Primary.findtext($CHMasterPlayer[%id].Primary));
   Secondary.SetSelected(Secondary.findtext($CHMasterPlayer[%id].Secondary));
}
function CHEditMPL::UpdatePlayer(%this)
{
   error("CHEditMPL::UpdatePlayer(%this)");
   %name = Name.getvalue();
   %squad = Squad.GetValue();
   %primary = Primary.GetValue();
   %secondary = Secondary.GetValue();


   if(%name $= "" || %squad $= "" || %primary $="" || %secondary $="")
   {
      MessageBoxOK( "ERROR", "Not all fields have been entered");
      return;
   }
   if(!UpdatePlayer(%name, %primary, %secondary, %squad))
      return;

   // Do i want it to clear the data on successfull update?... yah, what the hell
   EditMPL_List.SetSelectedRow(-1);
   Name.SetValue("");
   Squad.SetSelected(-1);
   Primary.SetSelected(-1);
   Secondary.SetSelected(-1);
}
